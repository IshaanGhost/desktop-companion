# Desktop Companions: Quantum Cat & Goblin's Jar

A lightweight Windows desktop application featuring two animated system monitoring widgets that provide visual feedback about your computer's performance.

![Desktop Companions](https://img.shields.io/badge/platform-Windows-blue) ![.NET](https://img.shields.io/badge/.NET-8.0-purple) ![License](https://img.shields.io/badge/license-MIT-green)

## Features

### 🐱 Quantum Cat Widget
- **Size**: 75x75 pixel widget positioned above the taskbar
- **Visual Design**: 
  - Gray cat with white borders for high visibility
  - 8-frame running animation with smooth transitions
  - Icon size: 48x48 pixels (widget display)
- **CPU/RAM Monitoring**: The cat's animation speed reflects your CPU usage in real-time
  - **Idle** (< 20% CPU): Cat animates slowly (200ms per frame) - relaxed running
  - **Medium** (20-60% CPU): Cat animates at normal speed (100ms per frame) - steady pace
  - **High** (60-90% CPU): Cat animates quickly (60ms per frame) - fast running
  - **Critical** (>90% CPU): Cat animates very fast (40ms per frame) - frantic sprinting
- **Programmatic Icon Animation**: 8-frame running animation with dynamically generated icons
  - Icons are generated at runtime using System.Drawing
  - Each frame shows the cat in a different running pose
  - Animation loops seamlessly
- **Hover Tooltip**: Shows real-time system data when you hover over the widget
  - CPU Usage: Current CPU percentage with 1 decimal place
  - RAM Usage: Current RAM percentage with 1 decimal place
  - Animation State: Current state name (Idle/Medium/High/HighLoad)
- **Real-time Monitoring**: Updates every 500ms for responsive feedback

### 🧪 Goblin's Jar Widget
- **Size**: 90x105 pixel widget positioned above the taskbar
- **Visual Design**:
  - Transparent blue-tinted glass jar outline (no background fill)
  - Glowing goblin sprite inside that changes color based on battery status
  - Icon size: 60x75 pixels (widget display)
  - Smooth animations: floating, shaking, rotating, and pulsing effects
- **Battery Status Monitoring**: The goblin inside the jar reflects your laptop's battery level
  - **High Charge** (70-100%): Brightly lit goblin (cyan/green) dancing playfully with gentle vertical movement
  - **Medium Charge** (20-70%): Moderately lit goblin (orange) casually observing with slow horizontal sway
  - **Low Charge** (10-20%): Dimly lit goblin (dark orange) shivering with flickering light and fast horizontal movement
  - **Critical Charge** (< 10%): Panicking goblin (red) with red flashing alerts and frantic shaking
  - **Charging**: Goblin vigorously pumping (bright green) with yellow sparkles and rotating animation
  - **Full Charge** (100% while charging): Sleeping goblin (cyan) with bright, contented glow and gentle pulsing
- **Transparent Design**: Translucent blue-tinted glass jar outline with glowing goblin (no background fill)
  - Only the jar outline is drawn (no fill)
  - Blue color: #5AA5F5
  - Goblin color changes dynamically based on battery level
- **Hover Tooltip**: Shows real-time battery data when you hover over the widget
  - Battery: Current battery percentage (0-100%)
  - Status: "Charging" or "On Battery"
  - Animation State: Current state name (High/Medium/Low/Critical/Charging/Full)
- **Real-time Monitoring**: Updates every 500ms for responsive feedback

### ⚙️ Configuration Features
- **System Tray Integration**: Access settings via system tray icon with animated cat and jar icons
  - **Icon Animation**: System tray icon shows animated cat when system is normal
    - Icon animation speed reflects CPU usage (faster CPU = faster animation)
    - 8-frame cat animation cycles through running poses
  - **Icon Override**: Icon changes to goblin when battery conditions are met
    - Shows goblin charging icon when battery is plugged in and charging
    - Shows goblin low icon when battery is below 20% and discharging
  - **Hover Tooltip**: Hover over system tray icon to see:
    - CPU usage percentage
    - Battery percentage and charging status
  - **Context Menu**: Right-click for quick access to Settings and Exit
- **Widget Sizes**: 
  - Cat widget: 75x75 pixels
  - Jar widget: 90x105 pixels
  - Both widgets positioned above taskbar by default
- **Interactive Tooltips**: Hover over widgets to see real-time system data
  - Tooltips update automatically as values change
  - Shows formatted percentages and status text
- **Drag & Drop**: 
  - Click and drag the transparent border area (10px border) around each widget
  - Widgets remain visible during dragging
  - Click-through is temporarily disabled during drag for smooth operation
- **Configurable Widgets**: Enable/disable and reposition each widget
- **Customizable Thresholds**: Adjust CPU and battery thresholds in settings
- **Always on Top**: Keep widgets visible above other windows
- **Startup Option**: Automatically launch on Windows startup (registry-based)
- **Persistent Settings**: All preferences saved automatically to JSON file
- **Click-Through**: Widgets allow clicks to pass through except for drag handles
  - You can interact with windows behind the widgets
  - Only the drag handle area captures mouse clicks

## Requirements

- **OS**: Windows 10/11 (64-bit)
- **.NET**: .NET 8.0 Runtime (included in self-contained build)
- **Visual Studio** (for building from source): Visual Studio 2022 or later with .NET 8.0 SDK

## Installation

### Option 1: Download Pre-built Release

1. Go to the [Releases](https://github.com/IshaanGhost/desktop-companion/releases) page
2. Download the latest release ZIP file
3. Extract the ZIP file to a folder of your choice
4. **Important**: Keep the `Assets` folder next to `DesktopCompanions.exe`
5. Run `DesktopCompanions.exe`

**Note**: The executable is a self-contained single-file (~68 MB) that includes the .NET runtime, so no additional installation is required.

### Option 2: Build from Source

1. Clone the repository:
   ```bash
   git clone https://github.com/IshaanGhost/desktop-companion.git
   cd desktop-companion
   ```

2. Open the project in Visual Studio 2022 or later, OR use the command line:

3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

4. Build the project:
   ```bash
   dotnet build --configuration Release
   ```

5. For a single-file executable, use the provided build script:
   ```bash
   # Windows
   .\build-single-exe.bat
   
   # Or PowerShell
   .\build-single-exe.ps1
   ```

6. The executable will be in `bin/Release/net8.0-windows/win-x64/publish/DesktopCompanions.exe`

## Usage

### First Launch

1. **Launch the application** - Run `DesktopCompanions.exe`
2. **Widgets appear automatically** - You'll see both widgets appear above your taskbar
   - Cat widget on the right
   - Jar widget to the left of the cat
3. **System Tray Icon** - Look for the animated icon in your system tray (notification area)
4. **Hover over widgets** - Move your mouse over either widget to see real-time system data
5. **Access Settings** - Right-click the system tray icon (or double-click) to open configuration window

### Interacting with Widgets

**Hover Tooltips:**
- **Cat Widget**: Shows CPU usage, RAM usage, and current animation state
- **Jar Widget**: Shows battery percentage, charging status, and current animation state
- Tooltips update automatically as values change (every 500ms)
- Tooltips appear after hovering for ~1 second

**Moving Widgets:**
- **Drag Handle**: Click and drag the transparent border area around each widget (10px border on all sides)
- Widgets remain visible during dragging
- Widgets can be positioned anywhere on your desktop
- Position is remembered between sessions
- **Reset Position**: Use settings window to reset widgets to default position (above taskbar)

**Click-Through Behavior:**
- Widgets allow clicks to pass through to windows behind them
- Only the drag handle area (10px border) captures mouse clicks
- This allows you to interact with desktop icons and windows behind the widgets
- Click-through is temporarily disabled during dragging for smooth operation

### System Tray Menu

Access by right-clicking the system tray icon:

- **Settings**: Opens configuration window
  - Configure widget visibility
  - Adjust CPU and battery thresholds
  - Set startup behavior
  - Configure always-on-top
- **Exit**: Closes the application completely

**System Tray Icon Behavior:**
- **Normal Operation**: Shows animated cat icon (8 frames, speed based on CPU)
- **Charging**: Shows goblin charging icon (green with sparkles)
- **Low Battery**: Shows goblin low icon (orange/red with warning)
- **Hover Tooltip**: Shows current CPU and battery status

## Configuration

### General Settings
- **Run on Windows Startup**: Automatically launch when Windows starts
- **Always on Top**: Keep widgets visible above other windows

### Quantum Cat Settings
- **Enable Quantum Cat**: Toggle widget visibility on/off
- **CPU Thresholds**: Customize when animation state changes
  - Idle threshold: Default < 20% CPU
  - Medium threshold: Default 20-60% CPU
  - High threshold: Default 60-90% CPU
  - Critical threshold: Default > 90% CPU
- **Reset Cat Position**: Reset widget to default position (above taskbar, bottom-right corner)
- **Widget Size**: Fixed at 75x75 pixels (not configurable in UI)

### Goblin's Jar Settings
- **Enable Goblin's Jar**: Toggle widget visibility on/off
- **Battery Thresholds**: Customize when battery state changes
  - Low battery threshold: Default < 20%
  - Critical battery threshold: Default < 10%
  - Medium threshold: Default 20-70%
  - High threshold: Default 70-100%
- **Reset Jar Position**: Reset widget to default position (above taskbar, left of cat)
- **Widget Size**: Fixed at 90x105 pixels (not configurable in UI)

## Assets & Icon Generation

The application generates all icons programmatically at runtime using System.Drawing:

### Cat Icons
- **Widget Display**: 48x48 pixels per frame
  - 8-frame running animation
  - Gray cat body with white borders for visibility
  - Black eyes for contrast
  - Smooth anti-aliased rendering
- **System Tray**: 16x16 pixels per frame
  - Same 8-frame animation, scaled down
  - Simplified for small icon size
- **Animation Speed**: Dynamically adjusts based on CPU load
  - Idle: 200ms per frame
  - Medium: 100ms per frame
  - High: 60ms per frame
  - Critical: 40ms per frame

### Jar Icons
- **Widget Display**: 60x75 pixels
  - Transparent background (no fill)
  - Blue jar outline (#5AA5F5) with 3px stroke
  - Glowing goblin sprite inside (12x12 pixels)
  - Goblin color varies by battery level:
    - Cyan: Full (>80%)
    - Green: Good (50-80%)
    - Orange: Medium (30-50%)
    - Dark Orange: Low (20-30%)
    - Red: Critical (<20%)
    - Bright Green: Charging
- **System Tray**: 16x16 pixels
  - Simplified jar and goblin representation
  - Same color-coding as widget version
- **Special Effects**:
  - Charging: Yellow sparkles around goblin
  - Low Battery: Red warning border

**Note**: All icons are generated at runtime using System.Drawing.Graphics - no external image files required. This ensures consistent rendering across different systems and allows for dynamic color changes.

## Project Structure

```
DesktopCompanion/
├── Assets/
│   └── cat_running_spritesheet.png  # Legacy sprite sheet (not used in current version)
├── Models/
│   ├── AppSettings.cs               # Settings model with serialization
│   └── SystemMetrics.cs             # System metrics data structures
├── Services/
│   ├── PerformanceMonitorService.cs  # CPU/RAM/Battery monitoring via Performance Counters and WMI
│   ├── SettingsService.cs            # Settings persistence (JSON file in AppData)
│   ├── StartupService.cs             # Windows startup management (registry)
│   └── SystemTrayService.cs         # System tray integration with dynamic icon animation
├── ViewModels/
│   ├── ConfigurationViewModel.cs     # Configuration window ViewModel
│   ├── GoblinJarViewModel.cs        # Jar widget ViewModel (battery state logic)
│   ├── MainViewModel.cs              # Settings window ViewModel
│   ├── MainWindowViewModel.cs        # Main window ViewModel
│   └── QuantumCatViewModel.cs       # Cat widget ViewModel (CPU state logic)
├── Views/
│   ├── GoblinJarWidget.xaml          # Jar widget UI (XAML)
│   ├── GoblinJarWidget.xaml.cs       # Jar widget code-behind (icon generation, drag handling)
│   ├── QuantumCatWidget.xaml         # Cat widget UI (XAML)
│   └── QuantumCatWidget.xaml.cs      # Cat widget code-behind (icon generation, animation, drag handling)
├── Utils/
│   ├── IconConverter.cs              # Icon to BitmapImage conversion utility
│   └── WindowUtils.cs                # Window manipulation utilities (click-through, P/Invoke)
├── Styles/
│   └── DefaultStyles.xaml            # Application-wide styles
├── App.xaml                          # Application entry point (XAML)
├── App.xaml.cs                       # Application startup logic and exception handling
├── MainWindow.xaml                   # Settings/Configuration window UI
├── MainWindow.xaml.cs                # Settings window code-behind
├── DesktopCompanion.csproj           # Project file with build configuration
├── build-single-exe.bat              # Build script for Windows (batch)
├── build-single-exe.ps1              # Build script for PowerShell
└── README.md                         # This file
```

## Architecture

The application follows the **MVVM (Model-View-ViewModel)** pattern:

- **Models**: Data structures for settings and system metrics
- **Views**: XAML UI definitions
- **ViewModels**: Presentation logic and data binding
- **Services**: Business logic for system monitoring, settings, and Windows integration

### Key Technologies

- **WPF (Windows Presentation Foundation)**: UI framework for transparent windows and rich animations
- **Windows Performance Counters**: System metrics collection for CPU and RAM monitoring
  - CPU: "% Processor Time" counter
  - RAM: "% Committed Bytes In Use" counter
- **WMI (Windows Management Instrumentation)**: Battery status monitoring
  - `Win32_Battery` class for battery percentage
  - `Win32_PowerSupply` for charging status
- **System.Windows.Forms**: 
  - `NotifyIcon` for system tray integration
  - `PowerLineStatus` enum for battery charging detection
- **System.Drawing**: Programmatic icon generation
  - `Graphics` and `Bitmap` for drawing icons
  - `GraphicsPath` for complex shapes (jar outline)
  - Anti-aliasing for smooth rendering
- **P/Invoke**: Windows API calls for advanced window functionality
  - `SetWindowLong` for window styles
  - `WM_NCHITTEST` for click-through implementation
  - `HTTRANSPARENT` for transparent hit testing
- **MVVM Pattern**: Separation of concerns
  - ViewModels handle business logic and state
  - Views handle UI presentation
  - Data binding for automatic updates
- **DispatcherTimer**: For smooth animation timing
  - Thread-safe updates to UI elements
  - Configurable intervals based on system state

## Building for Distribution

### Quick Build (Single-File Executable)

Use the provided build scripts:

**Windows Batch:**
```bash
.\build-single-exe.bat
```

**PowerShell:**
```bash
.\build-single-exe.ps1
```

**Manual Build:**
```bash
dotnet publish -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false
```

### Build Output

The published files will be in:
```
bin/Release/net8.0-windows/win-x64/publish/
```

**Expected files:**
- `DesktopCompanions.exe` (main executable, ~68 MB)
- `Assets/cat_running_spritesheet.png` (sprite sheet)
- Supporting runtime files (if not single-file)

**Important**: For distribution, include both the executable and the `Assets` folder.

### Distribution Package

1. Create a folder for distribution:
   ```bash
   mkdir DesktopCompanions-v1.0.0
   ```

2. Copy the executable:
   - `DesktopCompanions.exe` (single-file, self-contained)

3. Create ZIP archive:
   ```powershell
   Compress-Archive -Path DesktopCompanions-v1.0.0\* -DestinationPath DesktopCompanions-v1.0.0.zip
   ```

**Note**: The `Assets` folder is no longer required as icons are generated programmatically.

### Size Considerations

- **Self-contained single-file**: ~68-70 MB (includes full .NET 8.0 runtime)
- **No external assets required**: All icons generated at runtime
- **Total distribution**: Single executable file (~68-70 MB)
- **Memory usage**: ~50-100 MB when running (depends on system state)

## Troubleshooting

### Widgets Not Appearing
- Check if widgets are enabled in settings (right-click system tray icon → Settings)
- Ensure the application is running (check system tray for animated cat/jar icon)
- Widgets appear above the taskbar by default - check bottom of screen
- Try resetting widget positions in settings (may have been moved off-screen)
- Check error log at `%AppData%\DesktopCompanion\error.log` for detailed error messages
- Verify Windows allows transparent windows (some themes may interfere)
- Ensure no other applications are blocking window creation

### Tooltips Not Showing Data
- Tooltips show labels but no values: This indicates a DataContext binding issue
  - Fixed in latest version by setting DataContext programmatically
  - Ensure you're using the latest build
  - Tooltips require the ViewModel to be properly initialized
- Tooltips not appearing: Hover over the widget and wait ~1 second
- Tooltip data not updating: Check if PerformanceMonitorService is running (should update every 500ms)

### Cat Animation Not Working
- Cat icons are generated programmatically - no external files needed
- Check error log for icon generation errors: `%AppData%\DesktopCompanion\error.log`
- Verify the application has proper graphics context (System.Drawing should work on all Windows systems)
- Ensure .NET 8.0 runtime is available (included in self-contained build)
- Try running as Administrator if issues persist

### Performance Counters Not Working
- Run the application as Administrator (may be required for some performance counters)
- Check Windows Performance Monitor to verify counters are accessible
- Some antivirus software may block performance counter access

### Battery Widget Not Showing Correct Status
- The widget requires WMI access (usually available by default)
- On desktop computers without a battery, the widget will show a default state
- Check if your system reports battery status correctly in Windows

### Click-Through Not Working
- Click-through is implemented for the entire window except the drag handle
- The drag handle is a 10px transparent border around each widget
- If you can't drag widgets, try clicking closer to the edges (within 10px of widget border)
- Ensure the widget is not minimized or hidden
- Click-through is temporarily disabled during dragging - this is normal behavior
- If widgets are completely unresponsive, check error log for WindowUtils issues

### Application Crashes on Startup
- Check error log at `%AppData%\DesktopCompanion\error.log`
  - Log file location: `%LocalAppData%\DesktopCompanion\error.log`
  - Contains detailed stack traces and error messages
- Ensure .NET 8.0 runtime is available (included in self-contained build)
- Try running as Administrator (may be required for some performance counters)
- Check Windows Event Viewer for detailed error messages
  - Open Event Viewer → Windows Logs → Application
  - Look for errors from "DesktopCompanions"
- Verify Windows Performance Counters are accessible
- Some antivirus software may block performance counter access
- Ensure WMI service is running (usually automatic)

## Development

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or later (or Visual Studio Code with C# extension)
- Windows 10/11 SDK

### Running in Debug Mode
```bash
dotnet run
```

### Icon Generation
All icons are generated programmatically at runtime using System.Drawing:
- **Cat Icons**: Generated in `QuantumCatWidget.xaml.cs` using `CreateCatIcon()` method
- **Jar Icons**: Generated in `GoblinJarWidget.xaml.cs` using `CreateGoblin*Icon()` methods
- **System Tray Icons**: Generated in `SystemTrayService.cs` using `CreateCatIcon()` and `CreateGoblin*Icon()` methods

**Note**: The legacy Python script `generate_cat_sprite.py` and sprite sheet are no longer used. All icons are now generated programmatically for better performance and consistency.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is open source and available under the [MIT License](LICENSE).

## Acknowledgments

- Built with C# and WPF
- Inspired by desktop pet widgets and system monitoring tools
- Pixel art sprite sheet generated programmatically

## Support

For issues, questions, or feature requests, please open an issue on the [GitHub Issues](https://github.com/IshaanGhost/desktop-companion/issues) page.

## Repository

**GitHub**: [https://github.com/IshaanGhost/desktop-companion](https://github.com/IshaanGhost/desktop-companion)

---

**Enjoy your desktop companions!** 🐱🧪
