using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ShipClassInterface.Dialogs;
using ShipClassInterface.Models.BlockCreator;
using ShipClassInterface.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace ShipClassInterface.ViewModels
{
    public partial class BlockCreatorViewModel : ObservableObject
    {
        private readonly XmlService _xmlService;

        [ObservableProperty]
        private BlockDefinitions _blockDefinitions = new();

        private CubeBlockDefinition? _previousSelectedBlock;
        
        [ObservableProperty]
        private CubeBlockDefinition? _selectedBlock;

        [ObservableProperty]
        private string _currentFilePath = string.Empty;

        [ObservableProperty]
        private bool _hasUnsavedChanges;

        [ObservableProperty]
        private Models.BlockCreator.Component? _selectedComponent;

        [ObservableProperty]
        private MountPoint? _selectedMountPoint;

        [ObservableProperty]
        private BuildProgressModel? _selectedBuildProgressModel;

        [ObservableProperty]
        private ScreenArea? _selectedScreenArea;

        [ObservableProperty]
        private UpgradeModuleInfo? _selectedUpgrade;

        [ObservableProperty]
        private bool _isBlockCategoryMode;

        [ObservableProperty]
        private string _currentFileType = "CubeBlock";

        [ObservableProperty]
        private string? _selectedItemId;

        public BlockCreatorViewModel()
        {
            _xmlService = new XmlService();
        }

        [RelayCommand]
        private void NewFile()
        {
            if (HasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save them?", 
                    "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile();
                    if (HasUnsavedChanges) // Save was cancelled
                    {
                        return;
                    }
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            BlockDefinitions = new BlockDefinitions();
            CurrentFilePath = string.Empty;
            HasUnsavedChanges = false;
            SelectedBlock = null;
        }

        [RelayCommand]
        private void ClearAll()
        {
            if (BlockDefinitions.CubeBlocks.Count > 0)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to clear all {BlockDefinitions.CubeBlocks.Count} loaded blocks? This action cannot be undone.", 
                    "Clear All Blocks", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    BlockDefinitions = new BlockDefinitions();
                    CurrentFilePath = string.Empty;
                    HasUnsavedChanges = false;
                    SelectedBlock = null;
                    
                    ToastNotificationService.Instance.ShowInfo(
                        "Blocks Cleared",
                        "All loaded blocks have been cleared."
                    );
                }
            }
            else
            {
                ToastNotificationService.Instance.ShowInfo(
                    "No Blocks",
                    "There are no blocks to clear."
                );
            }
        }

        [RelayCommand]
        private void LoadFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "SBC files (*.sbc)|*.sbc|All files (*.*)|*.*",
                Title = "Open Block Definition File(s)",
                Multiselect = true // Enable multiple file selection
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var loadedFiles = new List<string>();
                var failedFiles = new List<string>();

                foreach (var fileName in openFileDialog.FileNames)
                {
                    try
                    {
                        LoadSbcFile(fileName);
                        loadedFiles.Add(Path.GetFileName(fileName));
                    }
                    catch (Exception ex)
                    {
                        failedFiles.Add($"{Path.GetFileName(fileName)}: {ex.Message}");
                    }
                }

                // Show toast notifications for results
                if (loadedFiles.Any())
                {
                    var fileList = string.Join(", ", loadedFiles);
                    ToastNotificationService.Instance.ShowSuccess(
                        "Files Loaded",
                        loadedFiles.Count == 1 
                            ? $"Successfully loaded: {fileList}"
                            : $"Successfully loaded {loadedFiles.Count} files: {fileList}"
                    );
                }

                if (failedFiles.Any())
                {
                    var errorList = string.Join("\n", failedFiles);
                    ToastNotificationService.Instance.ShowError(
                        "Load Error",
                        failedFiles.Count == 1
                            ? failedFiles[0]
                            : $"Failed to load {failedFiles.Count} files"
                    );
                }
            }
        }

        [RelayCommand]
        private void LoadTemplate()
        {
            var dialog = new TemplateSelectionDialog();
            
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    LoadSbcFile(dialog.SelectedTemplatePath);
                    CurrentFilePath = string.Empty;
                    HasUnsavedChanges = true;
                    
                    var fileName = Path.GetFileName(dialog.SelectedTemplatePath);
                    ToastNotificationService.Instance.ShowSuccess(
                        "Template Loaded",
                        $"Template '{fileName}' loaded successfully. Make your changes and save to a new file."
                    );
                }
                catch (Exception ex)
                {
                    ToastNotificationService.Instance.ShowError(
                        "Load Error",
                        $"Error loading template: {ex.Message}"
                    );
                }
            }
        }

        private void LoadSbcFile(string filePath)
        {
            try
            {
                // Use XML DOM approach to handle xsi:type properly
                LoadSbcFileAlternative(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load SBC file: {ex.Message}", ex);
            }
        }

        private void LoadSbcFileAlternative(string filePath)
        {
            try
            {
                // Read and parse XML manually to handle xsi:type variations
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);
                
                // If loading multiple files, preserve existing blocks
                var definitions = BlockDefinitions.CubeBlocks.Count > 0 
                    ? BlockDefinitions 
                    : new BlockDefinitions();
                
                // Track how many blocks are loaded from this file
                var blocksAddedFromThisFile = 0;
                
                // Try to parse CubeBlocks first
                var cubeBlocksNode = xmlDoc.SelectSingleNode("//CubeBlocks");
                if (cubeBlocksNode != null)
                {
                    foreach (XmlNode definitionNode in cubeBlocksNode.ChildNodes)
                    {
                        if (definitionNode.NodeType == XmlNodeType.Element && definitionNode.Name == "Definition")
                        {
                            var block = ParseDefinitionNode(definitionNode);
                            if (block != null)
                            {
                                block.SourceFilePath = filePath;
                                definitions.CubeBlocks.Add(block);
                                blocksAddedFromThisFile++;
                            }
                        }
                    }
                }
                
                // If no CubeBlocks found, check for CategoryClasses
                var categoryClassesNode = xmlDoc.SelectSingleNode("//CategoryClasses");
                if (categoryClassesNode != null)
                {
                    IsBlockCategoryMode = true;
                    CurrentFileType = "BlockCategory";
                    
                    // Parse all Category elements within CategoryClasses
                    var categoryNodes = categoryClassesNode.SelectNodes("Category");
                    if (categoryNodes != null)
                    {
                        foreach (XmlNode categoryNode in categoryNodes)
                        {
                            var categoryBlock = CreateCategoryAsBlock(categoryNode);
                            if (categoryBlock != null)
                            {
                                categoryBlock.SourceFilePath = filePath;
                                definitions.CubeBlocks.Add(categoryBlock);
                                blocksAddedFromThisFile++;
                            }
                        }
                    }
                }
                else
                {
                    IsBlockCategoryMode = false;
                    CurrentFileType = "CubeBlock";
                }
                
                // No automatic default block creation - user must manually add blocks if needed
                
                // Only update BlockDefinitions if it's a new instance or different
                if (BlockDefinitions != definitions)
                {
                    BlockDefinitions = definitions;
                }
                
                // Select the last loaded block if any were added from this file
                if (blocksAddedFromThisFile > 0)
                {
                    SelectedBlock = definitions.CubeBlocks.LastOrDefault();
                }
                else if (definitions.CubeBlocks.Count > 0)
                {
                    // If no blocks were added but we have existing blocks, keep current selection or select first
                    SelectedBlock ??= definitions.CubeBlocks.FirstOrDefault();
                }
                
                // Update file path - for multiple files, show "Multiple Files"
                if (string.IsNullOrEmpty(CurrentFilePath))
                {
                    CurrentFilePath = filePath;
                }
                else if (CurrentFilePath != filePath && !CurrentFilePath.Contains("Multiple Files"))
                {
                    CurrentFilePath = "Multiple Files";
                }
                
                // Mark as not having unsaved changes after loading
                // Use dispatcher to ensure this happens after all bindings update
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    HasUnsavedChanges = false;
                }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse SBC file: {ex.Message}", ex);
            }
        }
        
        private CubeBlockDefinition? CreateCategoryAsBlock(XmlNode categoryNode)
        {
            try
            {
                var block = new CubeBlockDefinition();
                
                // Extract xsi:type if present
                var xsiTypeAttr = categoryNode.Attributes?["type", "http://www.w3.org/2001/XMLSchema-instance"];
                if (xsiTypeAttr != null)
                {
                    block.XsiType = xsiTypeAttr.Value;
                }
                
                // Parse Id
                var idNode = categoryNode.SelectSingleNode("Id");
                if (idNode != null)
                {
                    var typeIdNode = idNode.SelectSingleNode("TypeId");
                    var subtypeIdNode = idNode.SelectSingleNode("SubtypeId");
                    if (typeIdNode != null) block.Id.TypeId = typeIdNode.InnerText;
                    if (subtypeIdNode != null) block.Id.SubtypeId = subtypeIdNode.InnerText;
                }
                
                // Parse DisplayName
                SetStringProperty(categoryNode, "DisplayName", value => block.DisplayName = value);
                
                // Parse Name
                SetStringProperty(categoryNode, "Name", value => block.Name = value);
                
                // Parse SearchBlocks
                var searchBlocksNode = categoryNode.SelectSingleNode("SearchBlocks");
                if (searchBlocksNode != null && bool.TryParse(searchBlocksNode.InnerText, out var searchBlocks))
                {
                    block.SearchBlocks = searchBlocks;
                }
                
                // Parse IsToolCategory
                var isToolCategoryNode = categoryNode.SelectSingleNode("IsToolCategory");
                if (isToolCategoryNode != null && bool.TryParse(isToolCategoryNode.InnerText, out var isToolCategory))
                {
                    block.IsToolCategory = isToolCategory;
                }
                
                // Parse ItemIds
                var itemIdsNode = categoryNode.SelectSingleNode("ItemIds");
                if (itemIdsNode != null)
                {
                    var stringNodes = itemIdsNode.SelectNodes("string");
                    if (stringNodes != null)
                    {
                        foreach (XmlNode stringNode in stringNodes)
                        {
                            if (!string.IsNullOrEmpty(stringNode.InnerText))
                            {
                                block.ItemIds.Add(stringNode.InnerText);
                            }
                        }
                    }
                }
                
                // Set a note that this is a category definition
                block.Description = "Category Definition - Use this template for creating block categories in the G menu.";
                
                return block;
            }
            catch
            {
                return null;
            }
        }

        private CubeBlockDefinition? ParseDefinitionNode(XmlNode definitionNode)
        {
            try
            {
                // Use manual parsing instead of XML serialization to avoid errors
                var block = new CubeBlockDefinition();
                
                // Extract xsi:type if present
                var xsiTypeAttr = definitionNode.Attributes?["type", "http://www.w3.org/2001/XMLSchema-instance"];
                if (xsiTypeAttr != null)
                {
                    block.XsiType = xsiTypeAttr.Value;
                }
                
                // Parse all properties manually
                ParseBasicProperties(definitionNode, block);
                return block;
            }
            catch (Exception ex)
            {
                // Log the error for debugging but return a basic block
                System.Diagnostics.Debug.WriteLine($"Error parsing definition node: {ex.Message}");
                return null;
            }
        }

        private void ParseBasicProperties(XmlNode definitionNode, CubeBlockDefinition block)
        {
            // Parse Id
            var idNode = definitionNode.SelectSingleNode("Id");
            if (idNode != null)
            {
                var typeIdNode = idNode.SelectSingleNode("TypeId");
                var subtypeIdNode = idNode.SelectSingleNode("SubtypeId");
                if (typeIdNode != null) block.Id.TypeId = typeIdNode.InnerText;
                if (subtypeIdNode != null) block.Id.SubtypeId = subtypeIdNode.InnerText;
            }
            
            // Parse simple string properties
            SetStringProperty(definitionNode, "DisplayName", value => block.DisplayName = value);
            SetStringProperty(definitionNode, "Description", value => block.Description = value);
            SetStringProperty(definitionNode, "Icon", value => block.Icon = value);
            SetStringProperty(definitionNode, "CubeSize", value => block.CubeSize = value);
            SetStringProperty(definitionNode, "BlockTopology", value => block.BlockTopology = value);
            SetStringProperty(definitionNode, "Model", value => block.Model = value);
            SetStringProperty(definitionNode, "BlockPairName", value => block.BlockPairName = value);
            SetStringProperty(definitionNode, "MirroringX", value => block.MirroringX = value);
            SetStringProperty(definitionNode, "MirroringY", value => block.MirroringY = value);
            SetStringProperty(definitionNode, "MirroringZ", value => block.MirroringZ = value);
            SetStringProperty(definitionNode, "EdgeType", value => block.EdgeType = value);
            SetStringProperty(definitionNode, "EmissiveColorPreset", value => block.EmissiveColorPreset = value);
            SetStringProperty(definitionNode, "DamageEffectName", value => block.DamageEffectName = value);
            SetStringProperty(definitionNode, "DamagedSound", value => block.DamagedSound = value);
            SetStringProperty(definitionNode, "PrimarySound", value => block.PrimarySound = value);
            SetStringProperty(definitionNode, "ActionSound", value => block.ActionSound = value);
            SetStringProperty(definitionNode, "DestroyEffect", value => block.DestroyEffect = value);
            SetStringProperty(definitionNode, "DestroySound", value => block.DestroySound = value);
            
            // Parse integer properties
            SetIntProperty(definitionNode, "BuildTimeSeconds", value => block.BuildTimeSeconds = value);
            SetIntProperty(definitionNode, "PCU", value => block.PCU = value);
            
            // Parse nullable boolean properties
            SetBooleanProperty(definitionNode, "IsAirTight", value => block.IsAirTight = value);
            SetBooleanProperty(definitionNode, "UseNeighbourOxygenRooms", value => block.UseNeighbourOxygenRooms = value);
            SetBooleanProperty(definitionNode, "UsesDeformation", value => block.UsesDeformation = value);
            SetBooleanProperty(definitionNode, "SilenceableByShipSoundSystem", value => block.SilenceableByShipSoundSystem = value);
            SetBooleanProperty(definitionNode, "UseModelIntersection", value => block.UseModelIntersection = value);
            SetBooleanProperty(definitionNode, "GuiVisible", value => block.GuiVisible = value);
            SetBooleanProperty(definitionNode, "VisibleInSurvival", value => block.VisibleInSurvival = value);
            SetBooleanProperty(definitionNode, "CompoundEnabled", value => block.CompoundEnabled = value);
            SetBooleanProperty(definitionNode, "CreateFracturedPieces", value => block.CreateFracturedPieces = value);
            SetBooleanProperty(definitionNode, "RandomRotation", value => block.RandomRotation = value);
            SetBooleanProperty(definitionNode, "Public", value => block.Public = value);
            SetBooleanProperty(definitionNode, "AvailableInSurvival", value => block.AvailableInSurvival = value);
            SetBooleanProperty(definitionNode, "PlaceDecals", value => block.PlaceDecals = value);
            
            // Parse double properties
            SetDoubleProperty(definitionNode, "GeneralDamageMultiplier", value => block.GeneralDamageMultiplier = value);
            SetDoubleProperty(definitionNode, "DeformationRatio", value => block.DeformationRatio = value);
            
            // Parse additional integer properties
            SetNullableIntProperty(definitionNode, "DamageThreshold", value => block.DamageThreshold = value);
            SetNullableIntProperty(definitionNode, "Points", value => block.Points = value);
            SetNullableIntProperty(definitionNode, "DamageEffectId", value => block.DamageEffectId = value);
            
            // Parse additional string properties
            SetStringProperty(definitionNode, "NavigationDefinition", value => block.NavigationDefinition = value);
            SetStringProperty(definitionNode, "PhysicalMaterial", value => block.PhysicalMaterial = value);
            SetStringProperty(definitionNode, "MirroringBlock", value => block.MirroringBlock = value);
            SetStringProperty(definitionNode, "MultiBlock", value => block.MultiBlock = value);
            SetStringProperty(definitionNode, "BlockVariants", value => block.BlockVariants = value);
            SetStringProperty(definitionNode, "BuildType", value => block.BuildType = value);
            SetStringProperty(definitionNode, "BuildMaterial", value => block.BuildMaterial = value);
            SetStringProperty(definitionNode, "GeneratedBlockType", value => block.GeneratedBlockType = value);
            SetStringProperty(definitionNode, "CompoundMaterial", value => block.CompoundMaterial = value);
            SetStringProperty(definitionNode, "ShadowCastingMode", value => block.ShadowCastingMode = value);
            
            // Parse Size
            var sizeNode = definitionNode.SelectSingleNode("Size");
            if (sizeNode?.Attributes != null)
            {
                if (int.TryParse(sizeNode.Attributes["x"]?.Value, out var x)) block.Size.X = x;
                if (int.TryParse(sizeNode.Attributes["y"]?.Value, out var y)) block.Size.Y = y;
                if (int.TryParse(sizeNode.Attributes["z"]?.Value, out var z)) block.Size.Z = z;
            }
            
            // Parse ModelOffset
            var modelOffsetNode = definitionNode.SelectSingleNode("ModelOffset");
            if (modelOffsetNode?.Attributes != null)
            {
                if (double.TryParse(modelOffsetNode.Attributes["x"]?.Value, out var x)) block.ModelOffset.X = x;
                if (double.TryParse(modelOffsetNode.Attributes["y"]?.Value, out var y)) block.ModelOffset.Y = y;
                if (double.TryParse(modelOffsetNode.Attributes["z"]?.Value, out var z)) block.ModelOffset.Z = z;
            }
            
            // Parse Center
            var centerNode = definitionNode.SelectSingleNode("Center");
            if (centerNode?.Attributes != null)
            {
                block.Center = new Vector3();
                if (double.TryParse(centerNode.Attributes["x"]?.Value, out var x)) block.Center.X = x;
                if (double.TryParse(centerNode.Attributes["y"]?.Value, out var y)) block.Center.Y = y;
                if (double.TryParse(centerNode.Attributes["z"]?.Value, out var z)) block.Center.Z = z;
            }
            
            // Parse Components
            ParseComponents(definitionNode, block);
            
            // Parse CriticalComponent
            var criticalComponentNode = definitionNode.SelectSingleNode("CriticalComponent");
            if (criticalComponentNode?.Attributes != null)
            {
                block.CriticalComponent.Subtype = criticalComponentNode.Attributes["Subtype"]?.Value ?? "";
                if (int.TryParse(criticalComponentNode.Attributes["Index"]?.Value, out var index))
                    block.CriticalComponent.Index = index;
            }
            
            // Parse MountPoints
            ParseMountPoints(definitionNode, block);
            
            // Parse BuildProgressModels
            ParseBuildProgressModels(definitionNode, block);
            
            // Parse ScreenAreas
            ParseScreenAreas(definitionNode, block);
            
            // Parse TieredUpdateTimes
            ParseTieredUpdateTimes(definitionNode, block);
            
            // Check if this is an UpgradeModule and parse specific properties
            if (block.XsiType.Contains("UpgradeModule"))
            {
                ParseUpgradeModuleProperties(definitionNode, block);
            }
        }
        
        private void ParseComponents(XmlNode definitionNode, CubeBlockDefinition block)
        {
            var componentsNode = definitionNode.SelectSingleNode("Components");
            if (componentsNode != null)
            {
                var componentNodes = componentsNode.SelectNodes("Component");
                if (componentNodes != null)
                {
                    foreach (XmlNode componentNode in componentNodes)
                    {
                        if (componentNode?.Attributes != null)
                        {
                            var component = new Models.BlockCreator.Component
                            {
                                Subtype = componentNode.Attributes["Subtype"]?.Value ?? "",
                            };
                            if (int.TryParse(componentNode.Attributes["Count"]?.Value, out var count))
                                component.Count = count;
                            block.Components.Add(component);
                        }
                    }
                }
            }
        }
        
        private void ParseMountPoints(XmlNode definitionNode, CubeBlockDefinition block)
        {
            var mountPointsNode = definitionNode.SelectSingleNode("MountPoints");
            if (mountPointsNode != null)
            {
                foreach (XmlNode mountPointNode in mountPointsNode.SelectNodes("MountPoint"))
                {
                    if (mountPointNode?.Attributes != null)
                    {
                        var mountPoint = new MountPoint
                        {
                            Side = mountPointNode.Attributes["Side"]?.Value ?? ""
                        };
                        if (double.TryParse(mountPointNode.Attributes["StartX"]?.Value, out var startX)) mountPoint.StartX = startX;
                        if (double.TryParse(mountPointNode.Attributes["StartY"]?.Value, out var startY)) mountPoint.StartY = startY;
                        if (double.TryParse(mountPointNode.Attributes["EndX"]?.Value, out var endX)) mountPoint.EndX = endX;
                        if (double.TryParse(mountPointNode.Attributes["EndY"]?.Value, out var endY)) mountPoint.EndY = endY;
                        block.MountPoints.Add(mountPoint);
                    }
                }
            }
        }
        
        private void ParseBuildProgressModels(XmlNode definitionNode, CubeBlockDefinition block)
        {
            var buildProgressModelsNode = definitionNode.SelectSingleNode("BuildProgressModels");
            if (buildProgressModelsNode != null)
            {
                foreach (XmlNode modelNode in buildProgressModelsNode.SelectNodes("Model"))
                {
                    if (modelNode?.Attributes != null)
                    {
                        var model = new BuildProgressModel
                        {
                            File = modelNode.Attributes["File"]?.Value ?? ""
                        };
                        if (double.TryParse(modelNode.Attributes["BuildPercentUpperBound"]?.Value, out var percent))
                            model.BuildPercentUpperBound = percent;
                        block.BuildProgressModels.Add(model);
                    }
                }
            }
        }
        
        private void ParseScreenAreas(XmlNode definitionNode, CubeBlockDefinition block)
        {
            var screenAreasNode = definitionNode.SelectSingleNode("ScreenAreas");
            if (screenAreasNode != null)
            {
                foreach (XmlNode screenAreaNode in screenAreasNode.SelectNodes("ScreenArea"))
                {
                    if (screenAreaNode?.Attributes != null)
                    {
                        var screenArea = new ScreenArea
                        {
                            Name = screenAreaNode.Attributes["Name"]?.Value ?? "",
                            DisplayName = screenAreaNode.Attributes["DisplayName"]?.Value ?? ""
                        };
                        if (int.TryParse(screenAreaNode.Attributes["TextureResolution"]?.Value, out var resolution))
                            screenArea.TextureResolution = resolution;
                        if (int.TryParse(screenAreaNode.Attributes["ScreenWidth"]?.Value, out var width))
                            screenArea.ScreenWidth = width;
                        if (int.TryParse(screenAreaNode.Attributes["ScreenHeight"]?.Value, out var height))
                            screenArea.ScreenHeight = height;
                        block.ScreenAreas.Add(screenArea);
                    }
                }
            }
        }
        
        private void ParseTieredUpdateTimes(XmlNode definitionNode, CubeBlockDefinition block)
        {
            var tieredUpdateTimesNode = definitionNode.SelectSingleNode("TieredUpdateTimes");
            if (tieredUpdateTimesNode != null)
            {
                foreach (XmlNode timeNode in tieredUpdateTimesNode.SelectNodes("unsignedInt"))
                {
                    if (int.TryParse(timeNode?.InnerText, out var time))
                    {
                        block.TieredUpdateTimes.Add(time);
                    }
                }
            }
        }
        
        private void ParseUpgradeModuleProperties(XmlNode definitionNode, CubeBlockDefinition block)
        {
            var upgradesNode = definitionNode.SelectSingleNode("Upgrades");
            if (upgradesNode != null)
            {
                var upgradeInfoNodes = upgradesNode.SelectNodes("MyUpgradeModuleInfo");
                if (upgradeInfoNodes != null)
                {
                    foreach (XmlNode upgradeInfoNode in upgradeInfoNodes)
                    {
                        var upgradeInfo = new UpgradeModuleInfo();
                        
                        var upgradeTypeNode = upgradeInfoNode.SelectSingleNode("UpgradeType");
                        if (upgradeTypeNode != null) upgradeInfo.UpgradeType = upgradeTypeNode.InnerText;
                        
                        var modifierNode = upgradeInfoNode.SelectSingleNode("Modifier");
                        if (modifierNode != null && double.TryParse(modifierNode.InnerText, out var modifier))
                            upgradeInfo.Modifier = modifier;
                        
                        var modifierTypeNode = upgradeInfoNode.SelectSingleNode("ModifierType");
                        if (modifierTypeNode != null) upgradeInfo.ModifierType = modifierTypeNode.InnerText;
                        
                        block.Upgrades.Add(upgradeInfo);
                    }
                }
            }
        }

        private void SetStringProperty(XmlNode parent, string elementName, Action<string> setter)
        {
            var node = parent.SelectSingleNode(elementName);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
            {
                setter(node.InnerText);
            }
        }

        private void SetIntProperty(XmlNode parent, string elementName, Action<int> setter)
        {
            var node = parent.SelectSingleNode(elementName);
            if (node != null && int.TryParse(node.InnerText, out var value))
            {
                setter(value);
            }
        }

        private void SetBooleanProperty(XmlNode parent, string elementName, Action<bool?> setter)
        {
            var node = parent.SelectSingleNode(elementName);
            if (node != null && bool.TryParse(node.InnerText, out var value))
            {
                setter(value);
            }
        }

        private void SetDoubleProperty(XmlNode parent, string elementName, Action<double?> setter)
        {
            var node = parent.SelectSingleNode(elementName);
            if (node != null && double.TryParse(node.InnerText, out var value))
            {
                setter(value);
            }
        }

        private void SetNullableIntProperty(XmlNode parent, string elementName, Action<int?> setter)
        {
            var node = parent.SelectSingleNode(elementName);
            if (node != null && int.TryParse(node.InnerText, out var value))
            {
                setter(value);
            }
        }

        [RelayCommand]
        private void SaveFile()
        {
            if (CurrentFilePath == "Multiple Files")
            {
                SaveAllFiles();
            }
            else if (string.IsNullOrEmpty(CurrentFilePath))
            {
                SaveAsFile();
            }
            else
            {
                SaveToFile(CurrentFilePath);
            }
        }

        [RelayCommand]
        private void SaveAsFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "SBC files (*.sbc)|*.sbc|All files (*.*)|*.*",
                Title = "Save Block Definition File",
                DefaultExt = ".sbc"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToFile(saveFileDialog.FileName);
            }
        }

        private void SaveAllFiles()
        {
            try
            {
                // Group blocks by their source file path
                var blocksByFile = BlockDefinitions.CubeBlocks
                    .Where(b => !string.IsNullOrEmpty(b.SourceFilePath))
                    .GroupBy(b => b.SourceFilePath)
                    .ToList();

                if (!blocksByFile.Any())
                {
                    ToastNotificationService.Instance.ShowWarning(
                        "Save Warning",
                        "No blocks have source files. Use 'Save As' to save to a new file."
                    );
                    return;
                }

                var savedFiles = new List<string>();
                var failedFiles = new List<string>();

                foreach (var fileGroup in blocksByFile)
                {
                    try
                    {
                        var filePath = fileGroup.Key!;
                        
                        // Create a temporary BlockDefinitions with just this file's blocks
                        var tempDefinitions = new BlockDefinitions();
                        foreach (var block in fileGroup)
                        {
                            tempDefinitions.CubeBlocks.Add(block);
                        }

                        // Save using the same logic as SaveToFile but with filtered blocks
                        SaveBlocksToFile(tempDefinitions, filePath);
                        savedFiles.Add(Path.GetFileName(filePath));
                    }
                    catch (Exception ex)
                    {
                        failedFiles.Add($"{Path.GetFileName(fileGroup.Key!)}: {ex.Message}");
                    }
                }

                // Show results
                if (savedFiles.Any())
                {
                    var fileList = string.Join(", ", savedFiles);
                    ToastNotificationService.Instance.ShowSuccess(
                        "Save Complete",
                        savedFiles.Count == 1 
                            ? $"Successfully saved: {fileList}"
                            : $"Successfully saved {savedFiles.Count} files: {fileList}"
                    );
                }

                if (failedFiles.Any())
                {
                    var errorList = string.Join("\n", failedFiles);
                    ToastNotificationService.Instance.ShowError(
                        "Save Error",
                        failedFiles.Count == 1
                            ? failedFiles[0]
                            : $"Failed to save {failedFiles.Count} files"
                    );
                }

                if (savedFiles.Any())
                {
                    HasUnsavedChanges = false;
                }
            }
            catch (Exception ex)
            {
                ToastNotificationService.Instance.ShowError(
                    "Save Error",
                    $"Error saving files: {ex.Message}"
                );
            }
        }

        private void SaveToFile(string filePath)
        {
            try
            {
                SaveBlocksToFile(BlockDefinitions, filePath);
                CurrentFilePath = filePath;
                HasUnsavedChanges = false;
                ToastNotificationService.Instance.ShowSuccess(
                    "Save Complete",
                    $"File saved successfully to {Path.GetFileName(filePath)}"
                );
            }
            catch (Exception ex)
            {
                ToastNotificationService.Instance.ShowError(
                    "Save Error",
                    $"Error saving file: {ex.Message}"
                );
            }
        }

        private void SaveBlocksToFile(BlockDefinitions definitions, string filePath)
        {
            try
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t",
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = false
                };

                using var writer = XmlWriter.Create(filePath, settings);
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");

                if (IsBlockCategoryMode)
                {
                    // Save as CategoryClasses format
                    SaveCategoryClassesFormat(writer, namespaces, definitions);
                }
                else
                {
                    // Save as standard CubeBlocks format using manual XML writing to avoid serialization issues
                    SaveCubeBlocksFormat(writer, namespaces, definitions);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save file: {ex.Message}", ex);
            }
        }

        private void SaveCategoryClassesFormat(XmlWriter writer, XmlSerializerNamespaces namespaces, BlockDefinitions definitions)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Definitions");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
            
            writer.WriteStartElement("CategoryClasses");
            
            foreach (var block in definitions.CubeBlocks)
            {
                writer.WriteStartElement("Category");
                writer.WriteAttributeString("type", "http://www.w3.org/2001/XMLSchema-instance", block.XsiType);
                
                // Write Id
                writer.WriteStartElement("Id");
                writer.WriteElementString("TypeId", block.Id.TypeId);
                writer.WriteElementString("SubtypeId", block.Id.SubtypeId);
                writer.WriteEndElement(); // Id
                
                // Write basic properties
                if (!string.IsNullOrEmpty(block.DisplayName))
                    writer.WriteElementString("DisplayName", block.DisplayName);
                    
                if (!string.IsNullOrEmpty(block.Name))
                    writer.WriteElementString("Name", block.Name);
                    
                if (block.SearchBlocks.HasValue)
                    writer.WriteElementString("SearchBlocks", block.SearchBlocks.Value.ToString().ToLower());
                    
                if (block.IsToolCategory.HasValue)
                    writer.WriteElementString("IsToolCategory", block.IsToolCategory.Value.ToString().ToLower());
                
                // Write ItemIds
                if (block.ItemIds.Count > 0)
                {
                    writer.WriteStartElement("ItemIds");
                    foreach (var itemId in block.ItemIds)
                    {
                        writer.WriteElementString("string", itemId);
                    }
                    writer.WriteEndElement(); // ItemIds
                }
                
                writer.WriteEndElement(); // Category
            }
            
            writer.WriteEndElement(); // CategoryClasses
            writer.WriteEndElement(); // Definitions
            writer.WriteEndDocument();
        }

        private void SaveCubeBlocksFormat(XmlWriter writer, XmlSerializerNamespaces namespaces, BlockDefinitions definitions)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Definitions");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
            
            writer.WriteStartElement("CubeBlocks");
            
            foreach (var block in definitions.CubeBlocks)
            {
                writer.WriteStartElement("Definition");
                writer.WriteAttributeString("type", "http://www.w3.org/2001/XMLSchema-instance", block.XsiType);
                
                // Write Id
                writer.WriteStartElement("Id");
                writer.WriteElementString("TypeId", block.Id.TypeId);
                writer.WriteElementString("SubtypeId", block.Id.SubtypeId);
                writer.WriteEndElement(); // Id
                
                // Write basic properties
                WriteNonEmptyElement(writer, "DisplayName", block.DisplayName);
                WriteNonEmptyElement(writer, "Description", block.Description);
                WriteNonEmptyElement(writer, "Icon", block.Icon);
                WriteNonEmptyElement(writer, "CubeSize", block.CubeSize);
                WriteNonEmptyElement(writer, "BlockTopology", block.BlockTopology);
                
                // Always write Size element
                writer.WriteStartElement("Size");
                writer.WriteAttributeString("x", block.Size.X.ToString());
                writer.WriteAttributeString("y", block.Size.Y.ToString());
                writer.WriteAttributeString("z", block.Size.Z.ToString());
                writer.WriteEndElement();
                
                // Write ModelOffset if not default (0,0,0)
                if (block.ModelOffset.X != 0 || block.ModelOffset.Y != 0 || block.ModelOffset.Z != 0)
                {
                    writer.WriteStartElement("ModelOffset");
                    writer.WriteAttributeString("x", block.ModelOffset.X.ToString());
                    writer.WriteAttributeString("y", block.ModelOffset.Y.ToString());
                    writer.WriteAttributeString("z", block.ModelOffset.Z.ToString());
                    writer.WriteEndElement();
                }
                
                WriteNonEmptyElement(writer, "Model", block.Model);
                
                // Write Components
                if (block.Components.Count > 0)
                {
                    writer.WriteStartElement("Components");
                    foreach (var component in block.Components)
                    {
                        writer.WriteStartElement("Component");
                        writer.WriteAttributeString("Subtype", component.Subtype);
                        writer.WriteAttributeString("Count", component.Count.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement(); // Components
                }
                
                // Write CriticalComponent if it has values
                if (!string.IsNullOrEmpty(block.CriticalComponent.Subtype))
                {
                    writer.WriteStartElement("CriticalComponent");
                    writer.WriteAttributeString("Subtype", block.CriticalComponent.Subtype);
                    writer.WriteAttributeString("Index", block.CriticalComponent.Index.ToString());
                    writer.WriteEndElement();
                }
                
                // Write MountPoints
                if (block.MountPoints.Count > 0)
                {
                    writer.WriteStartElement("MountPoints");
                    foreach (var mountPoint in block.MountPoints)
                    {
                        writer.WriteStartElement("MountPoint");
                        writer.WriteAttributeString("Side", mountPoint.Side);
                        writer.WriteAttributeString("StartX", mountPoint.StartX.ToString());
                        writer.WriteAttributeString("StartY", mountPoint.StartY.ToString());
                        writer.WriteAttributeString("EndX", mountPoint.EndX.ToString());
                        writer.WriteAttributeString("EndY", mountPoint.EndY.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement(); // MountPoints
                }
                
                // Write other properties
                WriteNonEmptyElement(writer, "BlockPairName", block.BlockPairName);
                WriteNonEmptyElement(writer, "MirroringX", block.MirroringX);
                WriteNonEmptyElement(writer, "MirroringY", block.MirroringY);
                WriteNonEmptyElement(writer, "MirroringZ", block.MirroringZ);
                WriteNonEmptyElement(writer, "EdgeType", block.EdgeType);
                
                if (block.BuildTimeSeconds > 0)
                    writer.WriteElementString("BuildTimeSeconds", block.BuildTimeSeconds.ToString());
                    
                WriteNonEmptyElement(writer, "DamageEffectName", block.DamageEffectName);
                WriteNonEmptyElement(writer, "DamagedSound", block.DamagedSound);
                WriteNonEmptyElement(writer, "PrimarySound", block.PrimarySound);
                WriteNonEmptyElement(writer, "ActionSound", block.ActionSound);
                WriteNonEmptyElement(writer, "EmissiveColorPreset", block.EmissiveColorPreset);
                WriteNonEmptyElement(writer, "DestroyEffect", block.DestroyEffect);
                WriteNonEmptyElement(writer, "DestroySound", block.DestroySound);
                
                if (block.PCU > 0)
                    writer.WriteElementString("PCU", block.PCU.ToString());
                
                // Write BuildProgressModels
                if (block.BuildProgressModels.Count > 0)
                {
                    writer.WriteStartElement("BuildProgressModels");
                    foreach (var model in block.BuildProgressModels)
                    {
                        writer.WriteStartElement("Model");
                        writer.WriteAttributeString("BuildPercentUpperBound", model.BuildPercentUpperBound.ToString());
                        writer.WriteAttributeString("File", model.File);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement(); // BuildProgressModels
                }
                
                // Write ScreenAreas
                if (block.ScreenAreas.Count > 0)
                {
                    writer.WriteStartElement("ScreenAreas");
                    foreach (var screenArea in block.ScreenAreas)
                    {
                        writer.WriteStartElement("ScreenArea");
                        writer.WriteAttributeString("Name", screenArea.Name);
                        writer.WriteAttributeString("DisplayName", screenArea.DisplayName);
                        writer.WriteAttributeString("TextureResolution", screenArea.TextureResolution.ToString());
                        writer.WriteAttributeString("ScreenWidth", screenArea.ScreenWidth.ToString());
                        writer.WriteAttributeString("ScreenHeight", screenArea.ScreenHeight.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement(); // ScreenAreas
                }
                
                // Write TieredUpdateTimes
                if (block.TieredUpdateTimes.Count > 0)
                {
                    writer.WriteStartElement("TieredUpdateTimes");
                    foreach (var time in block.TieredUpdateTimes)
                    {
                        writer.WriteElementString("unsignedInt", time.ToString());
                    }
                    writer.WriteEndElement(); // TieredUpdateTimes
                }
                
                // Write Upgrades if this is an upgrade module
                if (block.Upgrades.Count > 0)
                {
                    writer.WriteStartElement("Upgrades");
                    foreach (var upgrade in block.Upgrades)
                    {
                        writer.WriteStartElement("MyUpgradeModuleInfo");
                        writer.WriteElementString("UpgradeType", upgrade.UpgradeType);
                        writer.WriteElementString("Modifier", upgrade.Modifier.ToString());
                        writer.WriteElementString("ModifierType", upgrade.ModifierType);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement(); // Upgrades
                }
                
                // Write nullable boolean properties only if they have values
                if (block.IsAirTight.HasValue)
                    writer.WriteElementString("IsAirTight", block.IsAirTight.Value.ToString().ToLower());
                if (block.UseNeighbourOxygenRooms.HasValue)
                    writer.WriteElementString("UseNeighbourOxygenRooms", block.UseNeighbourOxygenRooms.Value.ToString().ToLower());
                if (block.GuiVisible.HasValue)
                    writer.WriteElementString("GuiVisible", block.GuiVisible.Value.ToString().ToLower());
                if (block.Public.HasValue)
                    writer.WriteElementString("Public", block.Public.Value.ToString().ToLower());
                
                // Write other optional properties
                if (block.GeneralDamageMultiplier.HasValue)
                    writer.WriteElementString("GeneralDamageMultiplier", block.GeneralDamageMultiplier.Value.ToString());
                if (block.Points.HasValue)
                    writer.WriteElementString("Points", block.Points.Value.ToString());
                
                WriteNonEmptyElement(writer, "PhysicalMaterial", block.PhysicalMaterial);
                
                writer.WriteEndElement(); // Definition
            }
            
            writer.WriteEndElement(); // CubeBlocks
            writer.WriteEndElement(); // Definitions
            writer.WriteEndDocument();
        }

        private void WriteNonEmptyElement(XmlWriter writer, string elementName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteElementString(elementName, value);
            }
        }

        [RelayCommand]
        private void AddBlock()
        {
            var newBlock = CreateDefaultBlock();
            // Don't set SourceFilePath for manually created blocks - they need to be saved to a new location
            newBlock.SourceFilePath = null;
            BlockDefinitions.CubeBlocks.Add(newBlock);
            SelectedBlock = newBlock;
            HasUnsavedChanges = true;
        }

        [RelayCommand]
        private void RemoveBlock()
        {
            if (SelectedBlock != null && BlockDefinitions.CubeBlocks.Contains(SelectedBlock))
            {
                var result = MessageBox.Show($"Are you sure you want to remove '{SelectedBlock.DisplayName}'?", 
                    "Confirm Remove", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    BlockDefinitions.CubeBlocks.Remove(SelectedBlock);
                    SelectedBlock = BlockDefinitions.CubeBlocks.FirstOrDefault();
                    HasUnsavedChanges = true;
                }
            }
        }

        [RelayCommand]
        private void DuplicateBlock()
        {
            if (SelectedBlock != null)
            {
                var duplicatedBlock = SelectedBlock.Clone();
                duplicatedBlock.Id.SubtypeId += "_Copy";
                duplicatedBlock.DisplayName += " (Copy)";
                // Duplicated blocks inherit the source file path from the original
                // This way they'll be saved back to the same file as the original
                BlockDefinitions.CubeBlocks.Add(duplicatedBlock);
                SelectedBlock = duplicatedBlock;
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void AddComponent()
        {
            if (SelectedBlock != null)
            {
                var component = new Models.BlockCreator.Component { Subtype = "SteelPlate", Count = 1 };
                SelectedBlock.Components.Add(component);
                SelectedComponent = component;
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void RemoveComponent()
        {
            if (SelectedBlock != null && SelectedComponent != null)
            {
                SelectedBlock.Components.Remove(SelectedComponent);
                SelectedComponent = SelectedBlock.Components.FirstOrDefault();
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void AddMountPoint()
        {
            if (SelectedBlock != null)
            {
                var mountPoint = new MountPoint 
                { 
                    Side = "Front", 
                    StartX = 0.0, 
                    StartY = 0.0, 
                    EndX = 1.0, 
                    EndY = 1.0 
                };
                SelectedBlock.MountPoints.Add(mountPoint);
                SelectedMountPoint = mountPoint;
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void RemoveMountPoint()
        {
            if (SelectedBlock != null && SelectedMountPoint != null)
            {
                SelectedBlock.MountPoints.Remove(SelectedMountPoint);
                SelectedMountPoint = SelectedBlock.MountPoints.FirstOrDefault();
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void AddBuildProgressModel()
        {
            if (SelectedBlock != null)
            {
                var model = new BuildProgressModel 
                { 
                    BuildPercentUpperBound = 0.33, 
                    File = "Models\\Model_BS1.mwm" 
                };
                SelectedBlock.BuildProgressModels.Add(model);
                SelectedBuildProgressModel = model;
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void RemoveBuildProgressModel()
        {
            if (SelectedBlock != null && SelectedBuildProgressModel != null)
            {
                SelectedBlock.BuildProgressModels.Remove(SelectedBuildProgressModel);
                SelectedBuildProgressModel = SelectedBlock.BuildProgressModels.FirstOrDefault();
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void AddScreenArea()
        {
            if (SelectedBlock != null)
            {
                var screenArea = new ScreenArea 
                { 
                    Name = "Screen_01", 
                    DisplayName = "Screen 1",
                    TextureResolution = 512,
                    ScreenWidth = 5,
                    ScreenHeight = 3
                };
                SelectedBlock.ScreenAreas.Add(screenArea);
                SelectedScreenArea = screenArea;
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void RemoveScreenArea()
        {
            if (SelectedBlock != null && SelectedScreenArea != null)
            {
                SelectedBlock.ScreenAreas.Remove(SelectedScreenArea);
                SelectedScreenArea = SelectedBlock.ScreenAreas.FirstOrDefault();
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void AddTieredUpdateTime()
        {
            if (SelectedBlock != null)
            {
                SelectedBlock.TieredUpdateTimes.Add(60);
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void RemoveTieredUpdateTime(int time)
        {
            if (SelectedBlock != null && SelectedBlock.TieredUpdateTimes.Contains(time))
            {
                SelectedBlock.TieredUpdateTimes.Remove(time);
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void AddUpgrade()
        {
            if (SelectedBlock != null)
            {
                var upgrade = new UpgradeModuleInfo 
                { 
                    UpgradeType = "Productivity", 
                    Modifier = 0.1, 
                    ModifierType = "Additive" 
                };
                SelectedBlock.Upgrades.Add(upgrade);
                SelectedUpgrade = upgrade;
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void RemoveUpgrade()
        {
            if (SelectedBlock != null && SelectedUpgrade != null)
            {
                SelectedBlock.Upgrades.Remove(SelectedUpgrade);
                SelectedUpgrade = SelectedBlock.Upgrades.FirstOrDefault();
                HasUnsavedChanges = true;
            }
        }

        private void InitializeNewBlock()
        {
            if (BlockDefinitions.CubeBlocks.Count == 0)
            {
                var defaultBlock = CreateDefaultBlock();
                BlockDefinitions.CubeBlocks.Add(defaultBlock);
                SelectedBlock = defaultBlock;
            }
        }

        private CubeBlockDefinition CreateDefaultBlock()
        {
            if (IsBlockCategoryMode)
            {
                // Create a default category
                var block = new CubeBlockDefinition
                {
                    XsiType = "MyObjectBuilder_GuiBlockCategoryDefinition",
                    Id = new BlockId 
                    { 
                        TypeId = "GuiBlockCategoryDefinition", 
                        SubtypeId = "" 
                    },
                    DisplayName = "New Category",
                    Name = "Section0_Position1_NewCategory",
                    Description = "Category Definition - Use this template for creating block categories in the G menu.",
                    SearchBlocks = true,
                    IsToolCategory = false
                };

                // Add a sample item
                block.ItemIds.Add("CubeBlock/SampleBlock");

                return block;
            }
            else
            {
                // Create a default cube block
                var block = new CubeBlockDefinition
                {
                    Id = new BlockId 
                    { 
                        TypeId = "TerminalBlock", 
                        SubtypeId = "NewBlock" 
                    },
                    DisplayName = "New Block",
                    Description = "Block description",
                    Icon = "Textures\\Icon.dds",
                    CubeSize = "Large",
                    BlockTopology = "TriangleMesh",
                    Size = new BlockSize { X = 1, Y = 1, Z = 1 },
                    ModelOffset = new ModelOffset { X = 0, Y = 0, Z = 0 },
                    Model = "Models\\Model.mwm",
                    BlockPairName = "BlockPair",
                    EdgeType = "Light",
                    BuildTimeSeconds = 10,
                    EmissiveColorPreset = "Default",
                    PCU = 10
                };

                block.Components.Add(new Models.BlockCreator.Component { Subtype = "SteelPlate", Count = 10 });
                block.CriticalComponent = new CriticalComponent { Subtype = "SteelPlate", Index = 0 };
                
                block.TieredUpdateTimes.Add(60);
                block.TieredUpdateTimes.Add(120);
                block.TieredUpdateTimes.Add(240);

                return block;
            }
        }

        [RelayCommand]
        private void AddItemId()
        {
            if (SelectedBlock != null)
            {
                var newItemId = "CubeBlock/NewBlockSubtype";
                SelectedBlock.ItemIds.Add(newItemId);
                SelectedItemId = newItemId;
                HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private void RemoveItemId()
        {
            if (SelectedBlock != null && SelectedItemId != null && SelectedBlock.ItemIds.Contains(SelectedItemId))
            {
                SelectedBlock.ItemIds.Remove(SelectedItemId);
                SelectedItemId = SelectedBlock.ItemIds.FirstOrDefault();
                HasUnsavedChanges = true;
            }
        }

        partial void OnSelectedBlockChanged(CubeBlockDefinition? value)
        {
            // Unsubscribe from old block's property changes
            if (_previousSelectedBlock != null)
            {
                _previousSelectedBlock.PropertyChanged -= OnBlockPropertyChanged;
                _previousSelectedBlock.Id.PropertyChanged -= OnBlockPropertyChanged;
                _previousSelectedBlock.Size.PropertyChanged -= OnBlockPropertyChanged;
                _previousSelectedBlock.ModelOffset.PropertyChanged -= OnBlockPropertyChanged;
            }
            
            if (value != null)
            {
                SelectedComponent = value.Components.FirstOrDefault();
                SelectedMountPoint = value.MountPoints.FirstOrDefault();
                SelectedBuildProgressModel = value.BuildProgressModels.FirstOrDefault();
                SelectedScreenArea = value.ScreenAreas.FirstOrDefault();
                SelectedUpgrade = value.Upgrades.FirstOrDefault();
                SelectedItemId = value.ItemIds.FirstOrDefault();
                
                // Subscribe to property changes to track modifications
                value.PropertyChanged += OnBlockPropertyChanged;
                value.Id.PropertyChanged += OnBlockPropertyChanged;
                value.Size.PropertyChanged += OnBlockPropertyChanged;
                value.ModelOffset.PropertyChanged += OnBlockPropertyChanged;
                
                // Subscribe to collection changes to track modifications
                value.Components.CollectionChanged += (s, e) => HasUnsavedChanges = true;
                value.MountPoints.CollectionChanged += (s, e) => HasUnsavedChanges = true;
                value.BuildProgressModels.CollectionChanged += (s, e) => HasUnsavedChanges = true;
                value.ScreenAreas.CollectionChanged += (s, e) => HasUnsavedChanges = true;
                value.Upgrades.CollectionChanged += (s, e) => HasUnsavedChanges = true;
                value.TieredUpdateTimes.CollectionChanged += (s, e) => HasUnsavedChanges = true;
                value.ItemIds.CollectionChanged += (s, e) => HasUnsavedChanges = true;
            }
            
            _previousSelectedBlock = value;
        }
        
        private void OnBlockPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            HasUnsavedChanges = true;
        }
        
        // Add a method to mark changes when UI updates properties
        [RelayCommand]
        private void MarkAsChanged()
        {
            HasUnsavedChanges = true;
        }
    }
}