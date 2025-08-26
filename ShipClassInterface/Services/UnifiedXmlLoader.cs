using Microsoft.Win32;
using ShipClassInterface.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ShipClassInterface.Services
{
    public class UnifiedXmlLoader
    {
        private readonly XmlService xmlService = new();
        private readonly MainViewModel mainViewModel;
        private readonly ToastNotificationService toastService = ToastNotificationService.Instance;
        private readonly ShipCoreRepositoryService shipCoreRepository = ShipCoreRepositoryService.Instance;

        public UnifiedXmlLoader(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public async void LoadXmlFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Space Engineers XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Load SE Configuration File(s) - Multi-select supported",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                await LoadXmlFilesAsync(dialog.FileNames);
            }
        }

        public async Task LoadXmlFilesAsync(IEnumerable<string> filePaths)
        {
            var filesList = filePaths.ToList();
            var totalFiles = filesList.Count;
            
            if (totalFiles == 0) return;

            if (totalFiles == 1)
            {
                // Single file - load normally with individual success messages
                LoadXmlFile(filesList[0], true);
                return;
            }

            // Multiple files - show batch loading message
            var loadResults = new List<LoadResult>();
            var fileTypeCount = new Dictionary<XmlFileType, int>();

            toastService.ShowInfo("Loading Files", 
                $"Loading {totalFiles} files...");

            // Create tasks for parallel loading
            var loadTasks = filesList.Select(filePath => Task.Run(() =>
            {
                var result = LoadXmlFileInternal(filePath, false);
                lock (loadResults)
                {
                    loadResults.Add(result);
                    if (result.Success)
                    {
                        if (fileTypeCount.ContainsKey(result.FileType))
                            fileTypeCount[result.FileType]++;
                        else
                            fileTypeCount[result.FileType] = 1;
                    }
                }
                return result;
            }));

            await Task.WhenAll(loadTasks);

            var successCount = loadResults.Count(r => r.Success);
            var failedCount = loadResults.Count(r => !r.Success);

            if (successCount > 0)
            {
                var summary = string.Join(", ", fileTypeCount
                    .Where(kvp => kvp.Key != XmlFileType.Unknown)
                    .Select(kvp => $"{kvp.Value} {XmlFileTypeDetector.GetFileTypeDescription(kvp.Key)}"));
                
                toastService.ShowSuccess("Load Complete", 
                    $"Successfully loaded {successCount} of {totalFiles} files: {summary}" +
                    (failedCount > 0 ? $" ({failedCount} failed)" : ""));
            }
            else
            {
                toastService.ShowError("Load Failed", 
                    "Failed to load any files");
            }
        }

        public void LoadXmlFile(string filePath)
        {
            LoadXmlFile(filePath, true);
        }

        private void LoadXmlFile(string filePath, bool showMessages)
        {
            var result = LoadXmlFileInternal(filePath, showMessages);
            if (!result.Success && showMessages)
            {
                toastService.ShowError("Load Failed", 
                    result.ErrorMessage ?? "Unknown error");
            }
        }

        private LoadResult LoadXmlFileInternal(string filePath, bool showMessages)
        {
            if (!File.Exists(filePath))
            {
                return new LoadResult 
                { 
                    Success = false, 
                    ErrorMessage = $"File not found: {filePath}",
                    FileType = XmlFileType.Unknown
                };
            }

            try
            {
                var fileType = XmlFileTypeDetector.DetectFileType(filePath);
                var fileTypeDescription = XmlFileTypeDetector.GetFileTypeDescription(fileType);

                switch (fileType)
                {
                    case XmlFileType.ShipCore:
                        LoadShipCore(filePath, fileTypeDescription, showMessages);
                        return new LoadResult { Success = true, FileType = fileType };

                    case XmlFileType.BlockGroups:
                        LoadBlockGroups(filePath, fileTypeDescription, showMessages);
                        return new LoadResult { Success = true, FileType = fileType };

                    case XmlFileType.WorldConfig:
                        LoadWorldConfig(filePath, fileTypeDescription, showMessages);
                        return new LoadResult { Success = true, FileType = fileType };

                    case XmlFileType.Manifest:
                        LoadManifest(filePath, fileTypeDescription, showMessages);
                        return new LoadResult { Success = true, FileType = fileType };

                    case XmlFileType.Unknown:
                        return new LoadResult 
                        { 
                            Success = false, 
                            ErrorMessage = "Could not determine the type of XML file",
                            FileType = fileType
                        };

                    default:
                        return new LoadResult 
                        { 
                            Success = false, 
                            ErrorMessage = "Unsupported file type",
                            FileType = fileType
                        };
                }
            }
            catch (Exception ex)
            {
                return new LoadResult 
                { 
                    Success = false, 
                    ErrorMessage = ex.Message,
                    FileType = XmlFileType.Unknown
                };
            }
        }

        private void LoadShipCore(string filePath, string fileTypeDescription, bool showMessages)
        {
            var shipCore = xmlService.LoadShipCore(filePath);
            if (shipCore != null)
            {
                // Store the source file path with the ship core
                shipCore.SourceFilePath = filePath;
                
                // Switch to Ship Cores tab
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mainViewModel.SelectedIndex = 0;
                    
                    // Add to ship cores collection
                    var shipCoreVM = mainViewModel.ShipCoreViewModel;
                    
                    // When loading multiple files, don't clear the collection
                    if (showMessages && shipCoreVM.ShipCores.Count > 0)
                    {
                        shipCoreVM.ShipCores.Clear();
                    }
                    
                    // Check if this ship core is already loaded (by SubtypeId)
                    var existing = shipCoreVM.ShipCores.FirstOrDefault(sc => sc.SubtypeId == shipCore.SubtypeId);
                    if (existing != null)
                    {
                        // Replace existing with newly loaded version
                        var index = shipCoreVM.ShipCores.IndexOf(existing);
                        shipCoreVM.ShipCores[index] = shipCore;
                        if (shipCoreVM.SelectedShipCore == existing)
                        {
                            shipCoreVM.SelectedShipCore = shipCore;
                        }
                    }
                    else
                    {
                        // Add new ship core
                        shipCoreVM.ShipCores.Add(shipCore);
                        shipCoreVM.SelectedShipCore = shipCore;
                    }
                    
                    shipCoreVM.CurrentFilePath = filePath;
                    
                    // Add to repository
                    shipCoreRepository.AddShipCore(shipCore);

                    if (showMessages)
                    {
                        toastService.ShowSuccess("File Loaded", 
                            $"{fileTypeDescription} loaded successfully! Ship Class: {shipCore.UniqueName}");
                    }
                });
            }
        }

        private void LoadBlockGroups(string filePath, string fileTypeDescription, bool showMessages)
        {
            var groups = xmlService.LoadBlockGroups(filePath);
            if (groups != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Switch to Block Groups tab
                    mainViewModel.SelectedIndex = 1;
                    
                    // Load into block groups view model
                    var blockGroupVM = mainViewModel.BlockGroupViewModel;
                    blockGroupVM.BlockGroups = groups.BlockGroups;
                    blockGroupVM.SelectedBlockGroup = groups.BlockGroups.FirstOrDefault();
                    blockGroupVM.CurrentFilePath = filePath;
                    
                    // Update shared service for dropdowns
                    BlockGroupService.Instance.LoadBlockGroups(groups.BlockGroups, filePath);

                    if (showMessages)
                    {
                        toastService.ShowSuccess("File Loaded", 
                            $"{fileTypeDescription} loaded successfully! Loaded {groups.BlockGroups.Count} block groups. Now available in Ship Core dropdowns!");
                    }
                });
            }
        }

        private void LoadWorldConfig(string filePath, string fileTypeDescription, bool showMessages)
        {
            var config = xmlService.LoadWorldConfig(filePath);
            if (config != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Switch to World Config tab
                    mainViewModel.SelectedIndex = 2;
                    
                    // Load into world config view model
                    var worldConfigVM = mainViewModel.WorldConfigViewModel;
                    worldConfigVM.WorldConfig = config;
                    worldConfigVM.SelectedNoFlyZone = config.NoFlyZones.FirstOrDefault();
                    worldConfigVM.CurrentFilePath = filePath;

                    if (showMessages)
                    {
                        toastService.ShowSuccess("File Loaded", 
                            $"{fileTypeDescription} loaded successfully! No-Fly Zones: {config.NoFlyZones.Count}");
                    }
                });
            }
        }

        private void LoadManifest(string filePath, string fileTypeDescription, bool showMessages)
        {
            var manifest = xmlService.LoadManifest(filePath);
            if (manifest != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Switch to Manifest tab
                    mainViewModel.SelectedIndex = 3;
                    
                    // Load into manifest view model
                    var manifestVM = mainViewModel.ManifestViewModel;
                    manifestVM.Manifest = manifest;
                    manifestVM.SelectedShipCoreFile = manifest.ShipCoreFiles.FirstOrDefault();
                    manifestVM.CurrentFilePath = filePath;

                    if (showMessages)
                    {
                        toastService.ShowSuccess("File Loaded", 
                            $"{fileTypeDescription} loaded successfully! Contains {manifest.ShipCoreFiles.Count} ship core files.");
                    }
                });
            }
        }

        private class LoadResult
        {
            public bool Success { get; set; }
            public string? ErrorMessage { get; set; }
            public XmlFileType FileType { get; set; }
        }
    }
}