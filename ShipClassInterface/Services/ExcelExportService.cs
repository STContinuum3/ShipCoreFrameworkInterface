using OfficeOpenXml;
using OfficeOpenXml.Style;
using ShipClassInterface.Models;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;

namespace ShipClassInterface.Services
{
    public class ExcelExportService
    {
        // Material Design color scheme
        private readonly Color _primaryColor = Color.FromArgb(103, 58, 183); // Deep Purple
        private readonly Color _primaryDarkColor = Color.FromArgb(69, 39, 160);
        private readonly Color _accentColor = Color.FromArgb(255, 64, 129); // Pink accent
        private readonly Color _headerColor = Color.FromArgb(48, 63, 159); // Indigo
        private readonly Color _lightGray = Color.FromArgb(245, 245, 245);
        private readonly Color _white = Color.White;

        public ExcelExportService()
        {
            // EPPlus requires license context in version 5+
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public void ExportShipCoresToExcel(ObservableCollection<ShipCore> shipCores, string filePath)
        {
            using var package = new ExcelPackage();
            
            foreach (var shipCore in shipCores)
            {
                // Create a worksheet for each ship core
                var worksheetName = SanitizeWorksheetName(shipCore.UniqueName ?? shipCore.SubtypeId ?? "Unknown");
                var worksheet = package.Workbook.Worksheets.Add(worksheetName);
                
                // Add ship core data to worksheet
                PopulateShipCoreWorksheet(worksheet, shipCore);
                
                // Apply styling
                ApplyWorksheetStyling(worksheet);
            }

            // Save the Excel file
            var fileInfo = new FileInfo(filePath);
            package.SaveAs(fileInfo);
        }

        private void PopulateShipCoreWorksheet(ExcelWorksheet worksheet, ShipCore shipCore)
        {
            int row = 1;

            // Title
            worksheet.Cells[row, 1, row, 4].Merge = true;
            worksheet.Cells[row, 1].Value = $"Ship Core: {shipCore.UniqueName}";
            worksheet.Cells[row, 1].Style.Font.Size = 18;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row += 2;

            // Basic Settings Section
            AddSectionHeader(worksheet, ref row, "Basic Settings");
            AddDataRow(worksheet, ref row, "Subtype ID", shipCore.SubtypeId);
            AddDataRow(worksheet, ref row, "Unique Name", shipCore.UniqueName);
            AddDataRow(worksheet, ref row, "Force Broadcast", shipCore.ForceBroadCast ? "Yes" : "No");
            AddDataRow(worksheet, ref row, "Broadcast Range", shipCore.ForceBroadCastRange.ToString("N0"));
            row++;

            // Grid Type Compatibility Section
            AddSectionHeader(worksheet, ref row, "Grid Type Compatibility");
            AddDataRow(worksheet, ref row, "Large Grid Static", shipCore.LargeGridStatic ? "Yes" : "No");
            AddDataRow(worksheet, ref row, "Large Grid Mobile", shipCore.LargeGridMobile ? "Yes" : "No");
            row++;

            // Limits and Constraints Section
            AddSectionHeader(worksheet, ref row, "Limits and Constraints");
            AddDataRow(worksheet, ref row, "Max Blocks", shipCore.MaxBlocks == -1 ? "Unlimited" : shipCore.MaxBlocks.ToString("N0"));
            AddDataRow(worksheet, ref row, "Max Mass", shipCore.MaxMass == -1 ? "Unlimited" : shipCore.MaxMass.ToString("N0"));
            AddDataRow(worksheet, ref row, "Max PCU", shipCore.MaxPCU == -1 ? "Unlimited" : shipCore.MaxPCU.ToString("N0"));
            AddDataRow(worksheet, ref row, "Max Per Faction", shipCore.MaxPerFaction == -1 ? "Unlimited" : shipCore.MaxPerFaction.ToString());
            AddDataRow(worksheet, ref row, "Max Per Player", shipCore.MaxPerPlayer == -1 ? "Unlimited" : shipCore.MaxPerPlayer.ToString());
            AddDataRow(worksheet, ref row, "Min Blocks", shipCore.MinBlocks == -1 ? "None" : shipCore.MinBlocks.ToString("N0"));
            AddDataRow(worksheet, ref row, "Min Players", shipCore.MinPlayers == -1 ? "None" : shipCore.MinPlayers.ToString());
            row++;

            // Performance Modifiers Section
            AddSectionHeader(worksheet, ref row, "Performance Modifiers");
            if (shipCore.Modifiers != null)
            {
                AddDataRow(worksheet, ref row, "Assembler Speed", $"{shipCore.Modifiers.AssemblerSpeed:F2}x");
                AddDataRow(worksheet, ref row, "Drill Harvest Multiplier", $"{shipCore.Modifiers.DrillHarvestMultiplier:F2}x");
                AddDataRow(worksheet, ref row, "Gyro Efficiency", $"{shipCore.Modifiers.GyroEfficiency:F2}x");
                AddDataRow(worksheet, ref row, "Gyro Force", $"{shipCore.Modifiers.GyroForce:F2}x");
                AddDataRow(worksheet, ref row, "Power Output", $"{shipCore.Modifiers.PowerProducersOutput:F2}x");
                AddDataRow(worksheet, ref row, "Refine Efficiency", $"{shipCore.Modifiers.RefineEfficiency:F2}x");
                AddDataRow(worksheet, ref row, "Refine Speed", $"{shipCore.Modifiers.RefineSpeed:F2}x");
                AddDataRow(worksheet, ref row, "Thruster Efficiency", $"{shipCore.Modifiers.ThrusterEfficiency:F2}x");
                AddDataRow(worksheet, ref row, "Thruster Force", $"{shipCore.Modifiers.ThrusterForce:F2}x");
                AddDataRow(worksheet, ref row, "Max Speed", $"{shipCore.Modifiers.MaxSpeed:F2}x");
                AddDataRow(worksheet, ref row, "Max Boost", $"{shipCore.Modifiers.MaxBoost:F2}x");
                AddDataRow(worksheet, ref row, "Boost Duration", $"{shipCore.Modifiers.BoostDuration:F0} seconds");
                AddDataRow(worksheet, ref row, "Boost Cooldown", $"{shipCore.Modifiers.BoostCoolDown:F0} seconds");
            }
            row++;

            // Passive Defense Modifiers Section
            AddSectionHeader(worksheet, ref row, "Passive Defense Modifiers");
            if (shipCore.PassiveDefenseModifiers != null)
            {
                AddDataRow(worksheet, ref row, "Bullet Resistance", $"{shipCore.PassiveDefenseModifiers.Bullet:F2}x");
                AddDataRow(worksheet, ref row, "Energy Resistance", $"{shipCore.PassiveDefenseModifiers.Energy:F2}x");
                AddDataRow(worksheet, ref row, "Kinetic Resistance", $"{shipCore.PassiveDefenseModifiers.Kinetic:F2}x");
                AddDataRow(worksheet, ref row, "Rocket Resistance", $"{shipCore.PassiveDefenseModifiers.Rocket:F2}x");
                AddDataRow(worksheet, ref row, "Explosion Resistance", $"{shipCore.PassiveDefenseModifiers.Explosion:F2}x");
                AddDataRow(worksheet, ref row, "Environment Resistance", $"{shipCore.PassiveDefenseModifiers.Environment:F2}x");
                AddDataRow(worksheet, ref row, "Duration", $"{shipCore.PassiveDefenseModifiers.Duration:F0} seconds");
                AddDataRow(worksheet, ref row, "Cooldown", $"{shipCore.PassiveDefenseModifiers.Cooldown:F0} seconds");
            }
            row++;

            // Active Defense Modifiers Section
            AddSectionHeader(worksheet, ref row, "Active Defense Modifiers");
            AddDataRow(worksheet, ref row, "Enabled", shipCore.EnableActiveDefenseModifiers ? "Yes" : "No");
            if (shipCore.ActiveDefenseModifiers != null && shipCore.EnableActiveDefenseModifiers)
            {
                AddDataRow(worksheet, ref row, "Bullet Resistance", $"{shipCore.ActiveDefenseModifiers.Bullet:F2}x");
                AddDataRow(worksheet, ref row, "Energy Resistance", $"{shipCore.ActiveDefenseModifiers.Energy:F2}x");
                AddDataRow(worksheet, ref row, "Kinetic Resistance", $"{shipCore.ActiveDefenseModifiers.Kinetic:F2}x");
                AddDataRow(worksheet, ref row, "Rocket Resistance", $"{shipCore.ActiveDefenseModifiers.Rocket:F2}x");
                AddDataRow(worksheet, ref row, "Explosion Resistance", $"{shipCore.ActiveDefenseModifiers.Explosion:F2}x");
                AddDataRow(worksheet, ref row, "Environment Resistance", $"{shipCore.ActiveDefenseModifiers.Environment:F2}x");
                AddDataRow(worksheet, ref row, "Duration", $"{shipCore.ActiveDefenseModifiers.Duration:F0} seconds");
                AddDataRow(worksheet, ref row, "Cooldown", $"{shipCore.ActiveDefenseModifiers.Cooldown:F0} seconds");
            }
            row++;

            // Speed and Reload Settings Section
            AddSectionHeader(worksheet, ref row, "Speed and Reload Settings");
            AddDataRow(worksheet, ref row, "Speed Boost Enabled", shipCore.SpeedBoostEnabled ? "Yes" : "No");
            AddDataRow(worksheet, ref row, "Reload Modifier Enabled", shipCore.EnableReloadModifier ? "Yes" : "No");
            if (shipCore.EnableReloadModifier)
            {
                AddDataRow(worksheet, ref row, "Reload Modifier", $"{shipCore.ReloadModifier:F2}x");
            }
            row++;

            // Block Limits Section
            if (shipCore.BlockLimits != null && shipCore.BlockLimits.Count > 0)
            {
                AddSectionHeader(worksheet, ref row, "Block Limits");
                
                // Add block limits table header
                worksheet.Cells[row, 1].Value = "Name";
                worksheet.Cells[row, 2].Value = "Block Groups";
                worksheet.Cells[row, 3].Value = "Max Count";
                worksheet.Cells[row, 4].Value = "No-Fly Zone";
                worksheet.Cells[row, 5].Value = "Punishment";
                worksheet.Cells[row, 6].Value = "Direction";
                
                // Style the header row
                var headerRange = worksheet.Cells[row, 1, row, 6];
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(_headerColor);
                headerRange.Style.Font.Color.SetColor(_white);
                row++;

                // Add block limit data
                foreach (var blockLimit in shipCore.BlockLimits)
                {
                    worksheet.Cells[row, 1].Value = blockLimit.Name;
                    worksheet.Cells[row, 2].Value = blockLimit.BlockGroups;
                    worksheet.Cells[row, 3].Value = blockLimit.MaxCount;
                    worksheet.Cells[row, 4].Value = blockLimit.TurnedOffByNoFlyZone ? "Yes" : "No";
                    worksheet.Cells[row, 5].Value = blockLimit.PunishmentType.ToString();
                    worksheet.Cells[row, 6].Value = blockLimit.AllowedDirectionsText;
                    
                    // Alternate row coloring
                    if (row % 2 == 0)
                    {
                        var rowRange = worksheet.Cells[row, 1, row, 6];
                        rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rowRange.Style.Fill.BackgroundColor.SetColor(_lightGray);
                    }
                    row++;
                }
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void AddSectionHeader(ExcelWorksheet worksheet, ref int row, string headerText)
        {
            worksheet.Cells[row, 1, row, 4].Merge = true;
            worksheet.Cells[row, 1].Value = headerText;
            worksheet.Cells[row, 1].Style.Font.Size = 14;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(_primaryColor);
            worksheet.Cells[row, 1].Style.Font.Color.SetColor(_white);
            worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            row++;
        }

        private void AddDataRow(ExcelWorksheet worksheet, ref int row, string label, string value)
        {
            worksheet.Cells[row, 1].Value = label;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2, row, 3].Merge = true;
            worksheet.Cells[row, 2].Value = value;
            
            // Alternate row coloring
            if (row % 2 == 0)
            {
                var rowRange = worksheet.Cells[row, 1, row, 4];
                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rowRange.Style.Fill.BackgroundColor.SetColor(_lightGray);
            }
            
            row++;
        }

        private void ApplyWorksheetStyling(ExcelWorksheet worksheet)
        {
            // Add borders to all cells with data
            if (worksheet.Dimension != null)
            {
                var allCells = worksheet.Cells[worksheet.Dimension.Address];
                allCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Top.Color.SetColor(Color.LightGray);
                allCells.Style.Border.Left.Color.SetColor(Color.LightGray);
                allCells.Style.Border.Right.Color.SetColor(Color.LightGray);
                allCells.Style.Border.Bottom.Color.SetColor(Color.LightGray);
                
                // Set default font
                allCells.Style.Font.Name = "Segoe UI";
                allCells.Style.Font.Size = 11;
                
                // Add padding
                allCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                allCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            }

            // Set column widths
            if (worksheet.Dimension != null && worksheet.Dimension.Columns > 0)
            {
                worksheet.Column(1).Width = 30; // Label column
                for (int i = 2; i <= worksheet.Dimension.Columns; i++)
                {
                    if (worksheet.Column(i).Width < 15)
                        worksheet.Column(i).Width = 15;
                }
            }

            // Freeze the first row (title)
            worksheet.View.FreezePanes(2, 1);
        }

        private string SanitizeWorksheetName(string name)
        {
            // Excel worksheet names have restrictions
            var invalidChars = new[] { ':', '\\', '/', '?', '*', '[', ']' };
            var sanitized = name;
            
            foreach (var c in invalidChars)
            {
                sanitized = sanitized.Replace(c.ToString(), "");
            }
            
            // Worksheet names can't exceed 31 characters
            if (sanitized.Length > 31)
            {
                sanitized = sanitized.Substring(0, 31);
            }
            
            // Can't be empty
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                sanitized = "Sheet";
            }
            
            return sanitized;
        }
    }
}