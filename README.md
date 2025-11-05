# Desktop Companions: Quantum Cat & Goblin's Jar

A lightweight Windows desktop application featuring two animated system monitoring widgets that provide visual feedback about your computer's performance.

![Desktop Companions](https://img.shields.io/badge/platform-Windows-blue) ![.NET](https://img.shields.io/badge/.NET-8.0-purple) ![License](https://img.shields.io/badge/license-MIT-green)

## Features

### 🐱 Quantum Cat Widget
- **CPU/RAM Monitoring**: The cat's animation speed reflects your CPU usage in real-time
  - **Idle** (< 20% CPU): Cat animates slowly (200ms per frame)
  - **Medium** (20-60% CPU): Cat animates at normal speed (100ms per frame)
  - **High** (60-90% CPU): Cat animates quickly (60ms per frame)
  - **Critical** (>90% CPU): Cat animates very fast (40ms per frame)
- **Pixel Art Animation**: 16-frame running animation sprite sheet
- **Real-time Monitoring**: Updates every 500ms

### 🧪 Goblin's Jar Widget
- **Battery Status Monitoring**: The goblin inside the jar reflects your laptop's battery level
  - **High Charge** (70-100%): Brightly lit goblin dancing playfully
  - **Medium Charge** (20-70%): Moderately lit goblin casually observing
  - **Low Charge** (< 20%): Dimly lit goblin shivering with flickering light
  - **Critical Charge** (< 10%): Panicking goblin with red flashing alerts
  - **Charging**: Goblin vigorously pumping with green/blue sparks
  - **Full Charge**: Sleeping goblin with bright, contented glow
- **Stylized Design**: Translucent blue-tinted glass jar with glowing green goblin sprite
- **Real-time Monitoring**: Updates every 500ms

### ⚙️ Configuration Features
- **System Tray Integration**: Access settings via system tray icon (shows cat and jar)
- **Configurable Widgets**: Enable/disable, resize, and reposition each widget
- **Customizable Thresholds**: Adjust CPU and battery thresholds
- **Always on Top**: Keep widgets visible above other windows
- **Startup Option**: Automatically launch on Windows startup
- **Persistent Settings**: All preferences saved automatically
- **Click-Through**: Widgets allow clicks to pass through except for drag handles

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

1. Launch the application - you'll see the widgets appear on your desktop
2. Right-click the system tray icon (or double-click) to open settings
3. Configure widgets to your preference:
   - Enable/disable widgets
   - Adjust size sliders
   - Set CPU/battery thresholds
   - Configure startup and always-on-top behavior

### Moving Widgets

- **Drag Handle**: Click and drag the transparent border area around each widget (10px border)
- The widgets have click-through enabled, so clicks pass through to windows behind them except for the drag handle area
- Widgets can be positioned anywhere on your desktop

### System Tray Menu

- **Settings**: Open configuration window
- **Exit**: Close the application

## Configuration

### General Settings
- **Run on Windows Startup**: Automatically launch when Windows starts
- **Always on Top**: Keep widgets visible above other windows

### Quantum Cat Settings
- **Enable Quantum Cat**: Toggle widget visibility
- **Cat Size Scale**: Adjust size from 50% to 200%
- **CPU Thresholds**: Customize idle, medium, and high load thresholds
- **Reset Cat Position**: Reset widget to default position

### Goblin's Jar Settings
- **Enable Goblin's Jar**: Toggle widget visibility
- **Jar Size Scale**: Adjust size from 50% to 200%
- **Battery Thresholds**: Customize low and critical battery levels
- **Reset Jar Position**: Reset widget to default position

## Assets

The application uses the following assets:

- **Cat Sprite Sheet**: `Assets/cat_running_spritesheet.png`
  - 16-frame pixel art running animation
  - 512x32 pixels (16 frames × 32x32 each)
  - Retro 16-bit style with 4-color palette

**Important**: The `Assets` folder must be in the same directory as the executable for the sprite sheet to load correctly.

## Project Structure

```
DesktopCompanion/
├── Assets/
│   └── cat_running_spritesheet.png  # Cat animation sprite sheet
├── Models/
│   ├── AppSettings.cs               # Settings model
│   └── SystemMetrics.cs             # System metrics model
├── Services/
│   ├── PerformanceMonitorService.cs  # CPU/RAM/Battery monitoring
│   ├── SettingsService.cs            # Settings persistence
│   ├── StartupService.cs             # Windows startup management
│   └── SystemTrayService.cs         # System tray integration
├── ViewModels/
│   ├── ConfigurationViewModel.cs     # Configuration ViewModel
│   ├── GoblinJarViewModel.cs        # Jar widget ViewModel
│   ├── MainViewModel.cs              # Settings window ViewModel
│   ├── MainWindowViewModel.cs        # Main window ViewModel
│   └── QuantumCatViewModel.cs       # Cat widget ViewModel
├── Views/
│   ├── GoblinJarWidget.xaml          # Jar widget UI
│   ├── GoblinJarWidget.xaml.cs       # Jar widget code-behind
│   ├── QuantumCatWidget.xaml         # Cat widget UI
│   └── QuantumCatWidget.xaml.cs      # Cat widget code-behind
├── Utils/
│   └── WindowUtils.cs                # Window manipulation utilities
├── Styles/
│   └── DefaultStyles.xaml            # Application styles
├── App.xaml                          # Application entry point
├── App.xaml.cs                       # Application startup logic
├── MainWindow.xaml                   # Settings window UI
├── MainWindow.xaml.cs                # Settings window code-behind
├── DesktopCompanion.csproj           # Project file
├── build-single-exe.bat              # Build script (Windows)
├── build-single-exe.ps1              # Build script (PowerShell)
└── README.md                         # This file
```

## Architecture

The application follows the **MVVM (Model-View-ViewModel)** pattern:

- **Models**: Data structures for settings and system metrics
- **Views**: XAML UI definitions
- **ViewModels**: Presentation logic and data binding
- **Services**: Business logic for system monitoring, settings, and Windows integration

### Key Technologies

- **WPF (Windows Presentation Foundation)**: UI framework
- **Windows Performance Counters**: System metrics collection
- **System.Windows.Forms**: Battery status and system tray integration
- **P/Invoke**: Windows API calls for click-through functionality
- **Sprite Sheet Animation**: Custom frame-by-frame animation for the cat widget

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

2. Copy files:
   - `DesktopCompanions.exe`
   - `Assets/` folder (with `cat_running_spritesheet.png`)

3. Create ZIP archive:
   ```powershell
   Compress-Archive -Path DesktopCompanions-v1.0.0\* -DestinationPath DesktopCompanions-v1.0.0.zip
   ```

### Size Considerations

- **Self-contained single-file**: ~68 MB (includes full .NET 8.0 runtime)
- **Assets folder**: ~2 KB (sprite sheet)
- **Total distribution**: ~68 MB

## Troubleshooting

### Widgets Not Appearing
- Check if widgets are enabled in settings (right-click system tray icon)
- Ensure the application is running (check system tray for cat/jar icon)
- Try resetting widget positions in settings
- Check error log at `%AppData%\DesktopCompanion\error.log`

### Cat Animation Not Working
- Ensure `Assets/cat_running_spritesheet.png` is in the same folder as the executable
- Check error log for sprite sheet loading errors
- Verify the sprite sheet file is not corrupted

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
- If you can't drag widgets, try clicking closer to the edges
- Ensure the widget is not minimized or hidden

### Application Crashes on Startup
- Check error log at `%AppData%\DesktopCompanion\error.log`
- Ensure .NET 8.0 runtime is available (included in self-contained build)
- Try running as Administrator
- Check Windows Event Viewer for detailed error messages

## Development

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or later (or Visual Studio Code with C# extension)
- Windows 10/11 SDK

### Running in Debug Mode
```bash
dotnet run
```

### Generating Cat Sprite Sheet
A Python script is included to generate the cat sprite sheet:
```bash
python generate_cat_sprite.py
```

This generates `cat_running_spritesheet.png` with 16 frames of pixel art animation.

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
