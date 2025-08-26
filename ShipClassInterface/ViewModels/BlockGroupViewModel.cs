using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ShipClassInterface.Models;
using ShipClassInterface.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace ShipClassInterface.ViewModels
{
    public partial class BlockGroupViewModel : ObservableObject
    {
        private readonly XmlService xmlService = new();
        private readonly BlockGroupService blockGroupService = BlockGroupService.Instance;
        private readonly ToastNotificationService toastService = ToastNotificationService.Instance;

        [ObservableProperty]
        private ObservableCollection<BlockGroup> blockGroups = new();

        [ObservableProperty]
        private BlockGroup? selectedBlockGroup;

        [ObservableProperty]
        private string currentFilePath = string.Empty;

        [RelayCommand]
        private void NewConfiguration()
        {
            // Create a new blank block group configuration
            var newBlockGroup = new BlockGroup
            {
                Name = "New Block Group",
                BlockTypes = new ObservableCollection<BlockType>()
            };

            BlockGroups.Clear();
            BlockGroups.Add(newBlockGroup);
            SelectedBlockGroup = newBlockGroup;
            CurrentFilePath = string.Empty;
            
            toastService.ShowSuccess("New Configuration", 
                "Created new block group configuration. Add block types and save when ready!");
        }

        [RelayCommand]
        private void LoadConfiguration()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Load Block Groups Configuration"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var groups = xmlService.LoadBlockGroups(dialog.FileName);
                    if (groups != null)
                    {
                        BlockGroups = groups.BlockGroups;
                        OnPropertyChanged(nameof(BlockGroups));
                        SelectedBlockGroup = BlockGroups.FirstOrDefault();
                        CurrentFilePath = dialog.FileName;
                        
                        // Update shared service
                        blockGroupService.LoadBlockGroups(BlockGroups, CurrentFilePath);
                    }
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Load Failed", 
                        $"Error loading configuration: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private void SaveConfiguration()
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
            {
                SaveAsConfiguration();
                return;
            }

            try
            {
                // Ensure all null/empty SubtypeId values are set to "any" before saving
                EnsureSubtypeIdValues();
                
                var collection = new BlockGroupCollection { BlockGroups = BlockGroups };
                xmlService.SaveBlockGroups(collection, CurrentFilePath);
                
                // Update shared service
                blockGroupService.LoadBlockGroups(BlockGroups, CurrentFilePath);
                
                toastService.ShowSuccess("Saved", 
                    "Block groups configuration saved successfully!");
            }
            catch (Exception ex)
            {
                toastService.ShowError("Save Failed", 
                    $"Error saving configuration: {ex.Message}");
            }
        }

        [RelayCommand]
        private void SaveAsConfiguration()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Save Block Groups Configuration",
                FileName = "ShipCoreConfig_Groups.xml"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Ensure all null/empty SubtypeId values are set to "any" before saving
                    EnsureSubtypeIdValues();
                    
                    var collection = new BlockGroupCollection { BlockGroups = BlockGroups };
                    xmlService.SaveBlockGroups(collection, dialog.FileName);
                    CurrentFilePath = dialog.FileName;
                    
                    // Update shared service
                    blockGroupService.LoadBlockGroups(BlockGroups, CurrentFilePath);
                    
                    toastService.ShowSuccess("Saved", 
                        "Block groups configuration saved successfully!");
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Save Failed", 
                        $"Error saving configuration: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private void AddBlockGroup()
        {
            var newGroup = new BlockGroup
            {
                Name = "New Block Group"
            };
            BlockGroups.Add(newGroup);
            SelectedBlockGroup = newGroup;
        }

        [RelayCommand]
        private void RemoveBlockGroup()
        {
            if (SelectedBlockGroup != null)
            {
                BlockGroups.Remove(SelectedBlockGroup);
                SelectedBlockGroup = BlockGroups.FirstOrDefault();
            }
        }

        [RelayCommand]
        private void AddBlockType()
        {
            if (SelectedBlockGroup != null)
            {
                SelectedBlockGroup.BlockTypes.Add(new BlockType
                {
                    TypeId = "CubeBlock",
                    SubtypeId = "any",
                    CountWeight = 1.0f
                });
            }
        }

        [RelayCommand]
        private void RemoveBlockType(BlockType? blockType)
        {
            if (SelectedBlockGroup != null && blockType != null)
            {
                SelectedBlockGroup.BlockTypes.Remove(blockType);
            }
        }

        private void EnsureSubtypeIdValues()
        {
            int updatedCount = 0;
            
            foreach (var blockGroup in BlockGroups)
            {
                foreach (var blockType in blockGroup.BlockTypes)
                {
                    if (string.IsNullOrWhiteSpace(blockType.SubtypeId))
                    {
                        blockType.SubtypeId = "any";
                        updatedCount++;
                    }
                }
            }
            
            if (updatedCount > 0)
            {
                toastService.ShowInfo("SubtypeId Updated", 
                    $"Updated {updatedCount} empty SubtypeId value(s) to 'any' before saving.");
            }
        }
    }
}