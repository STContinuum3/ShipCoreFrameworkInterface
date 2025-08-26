using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ShipClassInterface.Dialogs;
using ShipClassInterface.Models;
using ShipClassInterface.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace ShipClassInterface.ViewModels
{
    public partial class ShipCoreViewModel : ObservableObject
    {
        private readonly XmlService xmlService = new();
        private readonly BlockGroupService blockGroupService = BlockGroupService.Instance;
        private readonly ToastNotificationService toastService = ToastNotificationService.Instance;
        private readonly ShipCoreRepositoryService shipCoreRepository = ShipCoreRepositoryService.Instance;
        private string? originalSubtypeId;

        [ObservableProperty]
        private ObservableCollection<ShipCore> shipCores = new();

        [ObservableProperty]
        private ShipCore? selectedShipCore;

        partial void OnSelectedShipCoreChanged(ShipCore? value)
        {
            if (value != null)
            {
                originalSubtypeId = value.SubtypeId;
                // Subscribe to property changes to detect SubtypeId changes
                value.PropertyChanged += OnShipCorePropertyChanged;
                
                // Update the current file path to match the selected ship core
                CurrentFilePath = value.SourceFilePath ?? string.Empty;
            }
            else
            {
                // No ship core selected
                CurrentFilePath = string.Empty;
            }
        }

        private void OnShipCorePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShipCore.SubtypeId) && sender is ShipCore shipCore)
            {
                HandleSubtypeIdChange(shipCore);
            }
        }

        private void HandleSubtypeIdChange(ShipCore shipCore)
        {
            if (!string.IsNullOrEmpty(CurrentFilePath) && 
                !string.IsNullOrEmpty(originalSubtypeId) && 
                originalSubtypeId != shipCore.SubtypeId)
            {
                try
                {
                    var directory = Path.GetDirectoryName(CurrentFilePath);
                    var newFileName = $"ShipCoreConfig_{shipCore.SubtypeId}.xml";
                    var newFilePath = Path.Combine(directory!, newFileName);

                    if (File.Exists(CurrentFilePath))
                    {
                        File.Move(CurrentFilePath, newFilePath);
                        CurrentFilePath = newFilePath;
                        shipCore.SourceFilePath = newFilePath;  // Update the ship core's source file path
                        originalSubtypeId = shipCore.SubtypeId;
                        
                        toastService.ShowSuccess("File Renamed", 
                            $"Configuration file renamed to: {newFileName}");
                    }
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Rename Failed", 
                        $"Could not rename file: {ex.Message}");
                }
            }
        }

        [ObservableProperty]
        private string currentFilePath = string.Empty;

        public ObservableCollection<string> AvailableBlockGroups => blockGroupService.BlockGroupNames;

        public ShipCoreViewModel()
        {
            // Listen to block group service changes
            blockGroupService.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(BlockGroupService.BlockGroupNames) || 
                    e.PropertyName == nameof(BlockGroupService.AvailableBlockGroups))
                {
                    OnPropertyChanged(nameof(AvailableBlockGroups));
                }
            };
        }

        [RelayCommand]
        private void NewConfiguration()
        {
            // Generate a unique SubtypeId to avoid conflicts
            var baseId = "New_Ship_Core";
            var subtypeId = baseId;
            var counter = 1;
            
            while (ShipCores.Any(sc => sc.SubtypeId == subtypeId))
            {
                subtypeId = $"{baseId}_{counter}";
                counter++;
            }
            
            // Create a new blank ship core configuration
            var newShipCore = new ShipCore
            {
                SubtypeId = subtypeId,
                UniqueName = "New Ship Class",
                ForceBroadCast = false,
                ForceBroadCastRange = 0,
                LargeGridStatic = true,
                LargeGridMobile = true,
                MaxBlocks = 10000,
                MaxMass = -1,
                MaxPCU = -1,
                MaxPerFaction = -1,
                MaxPerPlayer = -1,
                MinBlocks = -1,
                MinPlayers = -1,
                Modifiers = new ShipModifiers
                {
                    AssemblerSpeed = 1.0f,
                    DrillHarvestMultiplier = 1.0f,
                    GyroEfficiency = 1.0f,
                    GyroForce = 1.0f,
                    PowerProducersOutput = 1.0f,
                    RefineEfficiency = 1.0f,
                    RefineSpeed = 1.0f,
                    ThrusterEfficiency = 1.0f,
                    ThrusterForce = 1.0f,
                    MaxSpeed = 0.5f,
                    MaxBoost = 1.0f,
                    BoostDuration = 0,
                    BoostCoolDown = 0
                },
                PassiveDefenseModifiers = new DefenseModifiers
                {
                    Bullet = 1.0f,
                    Energy = 1.0f,
                    Kinetic = 1.0f,
                    Duration = 0,
                    Cooldown = 0,
                    Rocket = 1.0f,
                    Explosion = 1.0f,
                    Environment = 1.0f
                },
                SpeedBoostEnabled = false,
                EnableActiveDefenseModifiers = false,
                ActiveDefenseModifiers = new DefenseModifiers
                {
                    Bullet = 1.0f,
                    Energy = 1.0f,
                    Kinetic = 1.0f,
                    Duration = 0,
                    Cooldown = 0,
                    Rocket = 1.0f,
                    Explosion = 1.0f,
                    Environment = 1.0f
                },
                EnableReloadModifier = false,
                ReloadModifier = 1.0f,
                BlockLimits = new ObservableCollection<BlockLimit>()
            };

            // Add the new ship core without clearing existing ones
            ShipCores.Add(newShipCore);
            SelectedShipCore = newShipCore;
            CurrentFilePath = string.Empty;
            
            // Add to repository
            shipCoreRepository.AddShipCore(newShipCore);
            
            toastService.ShowSuccess("New Configuration", 
                "Created new ship core configuration. Remember to save when ready!");
        }

        [RelayCommand]
        private void LoadConfiguration()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Load Ship Core Configuration",
                Multiselect = true  // Allow selecting multiple files
            };

            if (dialog.ShowDialog() == true)
            {
                bool firstFile = true;
                foreach (var fileName in dialog.FileNames)
                {
                    try
                    {
                        var shipCore = xmlService.LoadShipCore(fileName);
                        if (shipCore != null)
                        {
                            // Store the source file path with the ship core
                            shipCore.SourceFilePath = fileName;
                            
                            // Check if this ship core is already loaded (by SubtypeId)
                            var existing = ShipCores.FirstOrDefault(sc => sc.SubtypeId == shipCore.SubtypeId);
                            if (existing != null)
                            {
                                // Replace existing with newly loaded version
                                var index = ShipCores.IndexOf(existing);
                                ShipCores[index] = shipCore;
                                if (SelectedShipCore == existing)
                                {
                                    SelectedShipCore = shipCore;
                                }
                            }
                            else
                            {
                                // Add new ship core
                                ShipCores.Add(shipCore);
                            }
                            
                            // Select the first loaded ship core
                            if (firstFile)
                            {
                                SelectedShipCore = shipCore;
                                CurrentFilePath = fileName;
                                firstFile = false;
                            }
                            
                            // Add to repository
                            shipCoreRepository.AddShipCore(shipCore);
                        }
                    }
                    catch (Exception ex)
                    {
                        toastService.ShowError("Load Failed", 
                            $"Error loading {Path.GetFileName(fileName)}: {ex.Message}");
                    }
                }
            }
        }

        [RelayCommand]
        private void SaveConfiguration()
        {
            if (SelectedShipCore == null)
            {
                toastService.ShowWarning("No Selection", 
                    "Please select a ship core to save.");
                return;
            }

            // Validate for duplicate block groups before saving
            var duplicateValidationResult = ValidateBlockGroupDuplicates(SelectedShipCore);
            if (!duplicateValidationResult.IsValid)
            {
                toastService.ShowError("Duplicate Block Groups Found", duplicateValidationResult.Message);
                return;
            }

            // Use the ship core's source file path if available
            string? filePath = SelectedShipCore.SourceFilePath;
            
            if (string.IsNullOrEmpty(filePath))
            {
                // No source file path, prompt for save location
                SaveAsConfiguration();
                return;
            }

            try
            {
                xmlService.SaveShipCore(SelectedShipCore, filePath);
                toastService.ShowSuccess("Saved", 
                    $"Configuration saved to {Path.GetFileName(filePath)}");
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
            if (SelectedShipCore == null)
            {
                toastService.ShowWarning("Nothing to Save", 
                    "No ship core configuration to save.");
                return;
            }

            // Validate for duplicate block groups before saving
            var duplicateValidationResult = ValidateBlockGroupDuplicates(SelectedShipCore);
            if (!duplicateValidationResult.IsValid)
            {
                toastService.ShowError("Duplicate Block Groups Found", duplicateValidationResult.Message);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Save Ship Core Configuration",
                FileName = $"ShipCoreConfig_{SelectedShipCore.SubtypeId}.xml"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    xmlService.SaveShipCore(SelectedShipCore, dialog.FileName);
                    
                    // Store the file path with the ship core
                    SelectedShipCore.SourceFilePath = dialog.FileName;
                    CurrentFilePath = dialog.FileName;
                    
                    toastService.ShowSuccess("Saved", 
                        $"Configuration saved to {Path.GetFileName(dialog.FileName)}");
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Save Failed", 
                        $"Error saving configuration: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private void AddShipCore()
        {
            var newCore = new ShipCore
            {
                SubtypeId = "NewCore",
                UniqueName = "New Ship Class"
            };
            ShipCores.Add(newCore);
            SelectedShipCore = newCore;
        }

        [RelayCommand]
        private void RemoveShipCore()
        {
            if (SelectedShipCore != null)
            {
                ShipCores.Remove(SelectedShipCore);
                SelectedShipCore = ShipCores.FirstOrDefault();
            }
        }

        [RelayCommand]
        private void AddBlockLimit()
        {
            if (SelectedShipCore != null)
            {
                SelectedShipCore.BlockLimits.Add(new BlockLimit
                {
                    Name = "New Limit",
                    BlockGroups = "",
                    MaxCount = 1
                });
            }
        }

        [RelayCommand]
        private void RemoveBlockLimit(BlockLimit? limit)
        {
            if (SelectedShipCore != null && limit != null)
            {
                SelectedShipCore.BlockLimits.Remove(limit);
            }
        }

        [RelayCommand]
        private void ImportBlockLimits()
        {
            if (SelectedShipCore == null)
            {
                toastService.ShowWarning("No Selection", 
                    "Please select a ship core to import block limits into.");
                return;
            }

            var dialog = new OpenFileDialog
            {
                Filter = "Ship Core Config Files (ShipCoreConfig_*.xml)|ShipCoreConfig_*.xml|XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Select Ship Core Configuration to Import Block Limits From"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var sourceShipCore = xmlService.LoadShipCore(dialog.FileName);
                    if (sourceShipCore != null)
                    {
                        // Clear existing block limits
                        SelectedShipCore.BlockLimits.Clear();
                        
                        // Copy block limits from source
                        foreach (var blockLimit in sourceShipCore.BlockLimits)
                        {
                            SelectedShipCore.BlockLimits.Add(new BlockLimit
                            {
                                Name = blockLimit.Name,
                                BlockGroups = blockLimit.BlockGroups,
                                MaxCount = blockLimit.MaxCount,
                                TurnedOffByNoFlyZone = blockLimit.TurnedOffByNoFlyZone,
                                PunishmentType = blockLimit.PunishmentType,
                                AllowedDirections = new ObservableCollection<string>(blockLimit.AllowedDirections)
                            });
                        }
                        
                        toastService.ShowSuccess("Import Successful", 
                            $"Imported {sourceShipCore.BlockLimits.Count} block limit(s) from {Path.GetFileName(dialog.FileName)}");
                    }
                    else
                    {
                        toastService.ShowError("Import Failed", 
                            "Could not load the selected ship core configuration file.");
                    }
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Import Failed", 
                        $"Error importing block limits: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private void SaveAllConfigurations()
        {
            int savedCount = 0;
            int failedCount = 0;
            int validationFailedCount = 0;
            
            foreach (var shipCore in ShipCores)
            {
                if (!string.IsNullOrEmpty(shipCore.SourceFilePath))
                {
                    // Validate for duplicate block groups before saving
                    var duplicateValidationResult = ValidateBlockGroupDuplicates(shipCore);
                    if (!duplicateValidationResult.IsValid)
                    {
                        validationFailedCount++;
                        continue;
                    }

                    try
                    {
                        xmlService.SaveShipCore(shipCore, shipCore.SourceFilePath);
                        savedCount++;
                    }
                    catch
                    {
                        failedCount++;
                    }
                }
            }
            
            if (savedCount > 0)
            {
                var message = $"Saved {savedCount} configuration(s)";
                if (failedCount > 0 || validationFailedCount > 0)
                {
                    message += $", {failedCount} failed";
                    if (validationFailedCount > 0)
                    {
                        message += $", {validationFailedCount} had duplicate block groups";
                    }
                }
                toastService.ShowSuccess("Save All Complete", message);
            }
            else if (failedCount > 0 || validationFailedCount > 0)
            {
                var message = $"Failed to save {failedCount} configuration(s)";
                if (validationFailedCount > 0)
                {
                    message += $", {validationFailedCount} had duplicate block groups";
                }
                toastService.ShowError("Save All Failed", message);
            }
            else
            {
                toastService.ShowWarning("Nothing to Save", 
                    "No configurations with file paths to save");
            }
        }

        [RelayCommand]
        private async Task ExportToExcel()
        {
            if (ShipCores.Count == 0)
            {
                toastService.ShowWarning("No Data", 
                    "No ship cores loaded to export.");
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                Title = "Export Ship Cores to Excel",
                FileName = $"ShipCores_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var excelService = new ExcelExportService();
                    excelService.ExportShipCoresToExcel(ShipCores, dialog.FileName);
                    
                    toastService.ShowSuccess("Export Complete", 
                        $"Exported {ShipCores.Count} ship core(s) to Excel.");
                    
                    // Show Material Design dialog
                    var exportDialog = new ExcelExportDialog();
                    var result = await DialogHost.Show(exportDialog, "RootDialog");
                    
                    if (result is bool boolResult && boolResult)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = dialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Export Failed", 
                        $"Error exporting to Excel: {ex.Message}");
                }
            }
        }

        private (bool IsValid, string Message) ValidateBlockGroupDuplicates(ShipCore shipCore)
        {
            if (shipCore.BlockLimits == null || shipCore.BlockLimits.Count == 0)
            {
                return (true, string.Empty);
            }

            // Group block limits by their BlockGroups property to find duplicates
            var blockGroupCounts = shipCore.BlockLimits
                .Where(bl => !string.IsNullOrWhiteSpace(bl.BlockGroups))
                .GroupBy(bl => bl.BlockGroups, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => new { BlockGroup = g.Key, Count = g.Count() })
                .ToList();

            if (blockGroupCounts.Any())
            {
                var duplicateDetails = blockGroupCounts
                    .Select(item => $"{item.BlockGroup} ({item.Count} times)")
                    .ToList();

                var message = $"The following block groups appear multiple times in the ship core configuration:\\n\\n{string.Join("\\n", duplicateDetails)}\\n\\nEach block group can only be referenced once per ship core.";
                
                return (false, message);
            }

            return (true, string.Empty);
        }

    }
}