# Ship Class Interface

A WPF desktop application for creating and managing XML configuration files for the **Space Engineers Ship Class System** mod. This modern interface simplifies the process of defining ship classifications, block groups, and world settings for Space Engineers servers.

## 📋 Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Getting Started](#getting-started)
- [Features](#features)
  - [Ship Core Configuration](#ship-core-configuration)
  - [Block Groups Management](#block-groups-management)
  - [World Configuration](#world-configuration)
  - [Manifest Generation](#manifest-generation)
  - [Block Creator](#block-creator)
- [File Types and Formats](#file-types-and-formats)
- [User Interface Guide](#user-interface-guide)
- [XML Format Requirements](#xml-format-requirements)
- [Building and Development](#building-and-development)
- [Troubleshooting](#troubleshooting)
- [Support](#support)

## 🎯 Overview

The **Ship Class Interface** is a comprehensive configuration editor designed specifically for Space Engineers server administrators who use the Ship Class System mod. It provides an intuitive graphical interface to create, edit, and manage complex XML configurations without manual file editing.

### Key Benefits
- **No Manual XML Editing**: Visual interface eliminates error-prone manual XML editing
- **Intelligent File Detection**: Automatically detects and loads different configuration file types
- **Export Capabilities**: Export configurations to Excel for analysis and documentation
- **Modern UI**: Clean, Material Design interface with dark theme support
- **Validation**: Built-in validation prevents common configuration errors

## 💾 Installation

### Prerequisites
- Windows 10 or Windows 11
- .NET 8.0 Runtime (or .NET 8.0 SDK for development)

### Download Options

1. **Pre-built Release** (Recommended)
   - Download the latest release from the releases section
   - Extract the ZIP file to your preferred location
   - Run `ShipClassInterface.exe`

2. **Build from Source**
   ```bash
   git clone [repository-url]
   cd ShipClassInterface
   dotnet build -c Release
   dotnet run --project ShipClassInterface/ShipClassInterface.csproj
   ```

## 🚀 Getting Started

### First Launch
1. Launch the application
2. The interface will attempt to auto-load block groups from your Space Engineers directory
3. Use the navigation panel on the left to switch between different configuration sections
4. Start with the **Ship Cores** tab to create your first ship classification

### Loading Existing Configurations
1. Click the **Load XML File** button in the toolbar
2. Select any Space Engineers XML configuration file
3. The application automatically detects the file type and switches to the appropriate tab
4. Edit the configuration using the visual interface

### Creating Your First Configuration
1. Navigate to the **Ship Cores** tab
2. Click **New Configuration** in the toolbar
3. Fill in the ship class details (name, limits, restrictions)
4. Configure block limits and multipliers
5. Save your configuration using **Save** or **Save As**

## ✨ Features

### Ship Core Configuration

Create and manage individual ship class definitions with comprehensive settings:

#### Basic Settings
- **Unique Name**: Identifier for the ship class
- **Subtype ID**: Space Engineers block subtype identifier
- **Force Broadcast**: Enable/disable forced broadcasting
- **Broadcast Range**: Range for ship class broadcasting

#### Block Limits and Restrictions
- **Maximum Block Count**: Set overall block limits for the ship class
- **Block Type Multipliers**: Configure multipliers for specific block types
- **Block Group Limits**: Set limits for predefined block groups
- **Restriction Settings**: Define what blocks are allowed/restricted

#### Advanced Features
- **Grid Size Restrictions**: Limit to large grid, small grid, or both
- **Export to Excel**: Export ship core configurations for analysis
- **Template System**: Create reusable templates for common configurations
- **Validation**: Real-time validation of configuration values

### Block Groups Management

Define reusable collections of block types for easier ship class configuration:

#### Group Creation
- **Group Naming**: Create named groups for logical block collections
- **Block Type Addition**: Add multiple block types with TypeId and SubtypeId
- **Weight Configuration**: Set count weights for different blocks within groups
- **Flexible Matching**: Support for "any" subtype matching

#### Example Block Groups
```xml
<BlockGroup>
  <Name>Weapons</Name>
  <BlockTypes>
    <TypeId>LargeMissileTurret</TypeId>
    <SubtypeId>any</SubtypeId>
    <CountWeight>1</CountWeight>
  </BlockTypes>
</BlockGroup>
```

### World Configuration

Configure global server settings and no-fly zones:

#### Global Settings
- **World-wide Limits**: Set universal block limits across all ship classes
- **Default Multipliers**: Configure default multipliers for block types
- **Server Behavior**: Define how the mod behaves server-wide

#### No-Fly Zones
- **Zone Definition**: Create spherical no-fly zones with center coordinates and radius
- **Multiple Zones**: Support for multiple no-fly zones per world
- **Coordinate System**: Easy coordinate input with validation

### Manifest Generation

Create manifest files for mod distribution and organization:

#### Manifest Features
- **Mod Information**: Basic mod metadata and description
- **File Listing**: Automatic generation of included configuration files
- **Version Tracking**: Track configuration versions and changes
- **Distribution Ready**: Generate manifests ready for Steam Workshop upload

### Block Creator

Advanced tool for creating custom block definitions and categories:

#### Block Definition Creation
- **Visual Block Designer**: Create blocks using a visual interface
- **Category Management**: Organize blocks into logical categories
- **XML Generation**: Generate Space Engineers-compatible block definitions
- **Integration**: Seamlessly integrate with existing configurations

## 📄 File Types and Formats

The application supports multiple XML configuration file types:

### Supported File Types

| File Type | Pattern | Description |
|-----------|---------|-------------|
| Ship Core Config | `ShipCoreConfig_*.xml` | Individual ship class definitions |
| Block Groups | `ShipCoreConfig_Groups.xml` | Block group definitions |
| World Config | `ShipCoreConfig_World.xml` | World settings and no-fly zones |
| Manifest | `manifest.xml` | Mod manifest files |

### Automatic File Detection

The application uses intelligent file detection based on XML root elements:
- Analyzes XML structure upon file load
- Automatically switches to the appropriate tab
- Provides feedback on successful detection and loading
- Handles various XML namespace configurations

## 🖥️ User Interface Guide

### Navigation
- **Side Panel**: Collapsible navigation panel with main sections
- **Pin Option**: Pin the navigation panel open for easy access
- **Tab System**: Each configuration type has its dedicated interface
- **Toolbar**: Context-sensitive toolbars for each section

### Main Sections

1. **Ship Cores** 🚀
   - Create and edit individual ship classifications
   - Configure block limits and multipliers
   - Export configurations to Excel

2. **Block Groups** 👥
   - Define reusable block collections
   - Manage block types and weights
   - Create logical groupings for easier configuration

3. **World Config** 🌍
   - Set global world settings
   - Configure no-fly zones
   - Manage server-wide parameters

4. **Manifest** 📄
   - Generate mod manifest files
   - Track configuration versions
   - Prepare for distribution

5. **Block Creator** 🧱
   - Create custom block definitions
   - Design block categories
   - Generate SE-compatible XML

### Common UI Elements

#### Toolbars
- **New**: Create new configurations
- **Load**: Load existing XML files (with auto-detection)
- **Save**: Save current configuration
- **Save As**: Save with new filename
- **Save All**: Save all loaded configurations
- **Export**: Export to Excel (where applicable)

#### Data Grids
- **Sortable Columns**: Click column headers to sort
- **Inline Editing**: Double-click cells to edit values
- **Add/Remove**: Use toolbar buttons to manage rows
- **Validation**: Real-time validation with error highlighting

## 📋 XML Format Requirements

### Space Engineers Compatibility
The application generates XML files that are fully compatible with Space Engineers mod requirements:

#### Encoding and Structure
```xml
<?xml version="1.0" encoding="utf-8"?>
<Root xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
      xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <!-- Configuration content -->
</Root>
```

#### Key Requirements
- **UTF-8 Encoding**: All files use UTF-8 encoding
- **XML Namespaces**: Proper XSD schema namespace inclusion
- **Structure Validation**: Adherence to Space Engineers mod schema
- **Type Safety**: Proper data type handling for all fields

### File Format Examples

#### Ship Core Configuration
```xml
<ShipCore>
  <SubtypeId>MyShipClass</SubtypeId>
  <UniqueName>Destroyer Class</UniqueName>
  <ForceBroadCast>true</ForceBroadCast>
  <ForceBroadCastRange>15000</ForceBroadCastRange>
  <!-- Additional configuration -->
</ShipCore>
```

#### Block Group Configuration
```xml
<ArrayOfBlockGroup>
  <BlockGroup>
    <Name>Weapons</Name>
    <BlockTypes>
      <TypeId>LargeMissileTurret</TypeId>
      <SubtypeId>any</SubtypeId>
      <CountWeight>1</CountWeight>
    </BlockTypes>
  </BlockGroup>
</ArrayOfBlockGroup>
```

## 🔧 Building and Development

### Development Environment
- **IDE**: Visual Studio 2022 (recommended) or Visual Studio Code
- **Framework**: .NET 8.0 SDK
- **Platform**: Windows (WPF requirement)

### Build Commands

#### Debug Build
```bash
dotnet build
```

#### Release Build
```bash
dotnet build -c Release
```

#### Run Application
```bash
dotnet run --project ShipClassInterface/ShipClassInterface.csproj
```

#### Publish Self-Contained
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

### Project Structure
```
ShipClassInterface/
├── ShipClassInterface.sln          # Solution file
└── ShipClassInterface/             # Main project
    ├── Models/                     # Data models
    ├── ViewModels/                 # MVVM ViewModels
    ├── Views/                      # XAML views
    ├── Services/                   # Business logic services
    ├── Controls/                   # Custom UI controls
    ├── Converters/                 # WPF value converters
    └── Dialogs/                    # Modal dialogs
```

### Key Dependencies
- **MaterialDesignThemes** (5.1.0): Modern UI components
- **CommunityToolkit.Mvvm** (8.3.2): MVVM framework
- **EPPlus** (7.5.2): Excel export functionality
- **Microsoft.Xaml.Behaviors.Wpf** (1.1.135): WPF behaviors

## 🔍 Troubleshooting

### Common Issues

#### Application Won't Start
- **Cause**: Missing .NET 8.0 runtime
- **Solution**: Download and install .NET 8.0 Runtime from Microsoft

#### XML Files Won't Load
- **Cause**: Incorrect file format or encoding
- **Solution**: Ensure XML files are UTF-8 encoded and follow SE format

#### Export to Excel Fails
- **Cause**: Insufficient permissions or file in use
- **Solution**: Ensure Excel files aren't open in another application

#### Block Groups Don't Load
- **Cause**: Space Engineers directory not found
- **Solution**: Manually load block group files or check SE installation path

### Configuration Issues

#### Invalid Block Types
- Check TypeId values match Space Engineers block definitions
- Ensure SubtypeId values are correct or use "any" for wildcards
- Verify CountWeight values are positive numbers

#### Save Failures
- Ensure you have write permissions to the target directory
- Check that file paths don't exceed Windows limits
- Verify disk space availability

### Performance Issues

#### Slow Loading
- Large XML files may take time to process
- Consider breaking large configurations into smaller files
- Ensure adequate system memory

#### UI Responsiveness
- Close unused dialogs and windows
- Restart the application if performance degrades
- Check system resources usage

## 📞 Support

### Getting Help

#### Documentation
- Review this README for comprehensive usage information
- Check the CLAUDE.md file for development guidelines
- Examine example XML files for format reference

#### Community Support
- **Powered by**: [Continuum Gaming](https://stcontinuum.ca/)
- **Discord**: Join the Space Engineers modding community
- **Forums**: Space Engineers official forums

#### Reporting Issues
1. Provide detailed description of the issue
2. Include steps to reproduce the problem
3. Attach relevant XML files (if applicable)
4. Specify your system configuration

### Feature Requests
- Submit enhancement requests through appropriate channels
- Describe the desired functionality clearly
- Explain the use case and benefits
- Consider contributing to development if possible

---

## 📄 License

This project is developed for the Space Engineers modding community. Please refer to the appropriate license files for usage terms and conditions.

## 🙏 Acknowledgments

- **Space Engineers Community**: For ongoing support and feedback
- **Continuum Gaming**: For hosting and community support
- **Contributors**: All developers who have contributed to this project

---

*Last Updated: August 2025*