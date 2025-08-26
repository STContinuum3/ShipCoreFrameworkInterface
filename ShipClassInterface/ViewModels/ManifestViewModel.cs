using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ShipClassInterface.Models;
using ShipClassInterface.Services;
using System.Collections.ObjectModel;

namespace ShipClassInterface.ViewModels
{
    public partial class ManifestViewModel : ObservableObject
    {
        private readonly XmlService xmlService = new();
        private readonly ToastNotificationService toastService = ToastNotificationService.Instance;

        [ObservableProperty]
        private Manifest manifest = new();

        [ObservableProperty]
        private string? selectedShipCoreFile;

        [ObservableProperty]
        private string currentFilePath = string.Empty;

        [RelayCommand]
        private void NewConfiguration()
        {
            // Create a new blank manifest
            var newManifest = new Manifest
            {
                ShipCoreFiles = new ObservableCollection<string>()
            };

            Manifest = newManifest;
            SelectedShipCoreFile = null;
            CurrentFilePath = string.Empty;
            
            toastService.ShowSuccess("New Configuration", 
                "Created new manifest configuration. Add ship core files and save when ready!");
        }

        [RelayCommand]
        private void LoadConfiguration()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Load Manifest Configuration"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var manifest = xmlService.LoadManifest(dialog.FileName);
                    if (manifest != null)
                    {
                        Manifest = manifest;
                        SelectedShipCoreFile = Manifest.ShipCoreFiles.FirstOrDefault();
                        CurrentFilePath = dialog.FileName;
                        
                        toastService.ShowSuccess("File Loaded", 
                            $"Manifest loaded successfully! Contains {Manifest.ShipCoreFiles.Count} ship core files.");
                    }
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Load Failed", 
                        $"Error loading manifest: {ex.Message}");
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
                xmlService.SaveManifest(Manifest, CurrentFilePath);
                toastService.ShowSuccess("Saved", 
                    "Manifest configuration saved successfully!");
            }
            catch (Exception ex)
            {
                toastService.ShowError("Save Failed", 
                    $"Error saving manifest: {ex.Message}");
            }
        }

        [RelayCommand]
        private void SaveAsConfiguration()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Save Manifest Configuration",
                FileName = "CoreManifest.xml"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    xmlService.SaveManifest(Manifest, dialog.FileName);
                    CurrentFilePath = dialog.FileName;
                    toastService.ShowSuccess("Saved", 
                        "Manifest configuration saved successfully!");
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Save Failed", 
                        $"Error saving manifest: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private void AddShipCoreFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Ship Core XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Select Ship Core File"
            };

            if (dialog.ShowDialog() == true)
            {
                var fileName = System.IO.Path.GetFileName(dialog.FileName);
                if (!Manifest.ShipCoreFiles.Contains(fileName))
                {
                    Manifest.ShipCoreFiles.Add(fileName);
                    SelectedShipCoreFile = fileName;
                    toastService.ShowSuccess("File Added", 
                        $"Added '{fileName}' to manifest");
                }
                else
                {
                    toastService.ShowWarning("Duplicate File", 
                        $"'{fileName}' is already in the manifest");
                }
            }
        }

        [RelayCommand]
        private void RemoveShipCoreFile()
        {
            if (SelectedShipCoreFile != null)
            {
                Manifest.ShipCoreFiles.Remove(SelectedShipCoreFile);
                SelectedShipCoreFile = Manifest.ShipCoreFiles.FirstOrDefault();
                toastService.ShowSuccess("File Removed", 
                    "Ship core file removed from manifest");
            }
        }

        [RelayCommand]
        private void AddShipCoreFileManually()
        {
            // Add a placeholder filename that user can edit
            var newFileName = "NewShipCore.xml";
            var counter = 1;
            while (Manifest.ShipCoreFiles.Contains(newFileName))
            {
                newFileName = $"NewShipCore{counter}.xml";
                counter++;
            }
            
            Manifest.ShipCoreFiles.Add(newFileName);
            SelectedShipCoreFile = newFileName;
        }
    }
}