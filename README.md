# Desktop Companions: Quantum Cat & Goblin's Jar

A lightweight Windows desktop application featuring two animated system monitoring widgets that provide visual feedback about your computer's performance.

## Features

### 🐱 Quantum Cat Widget
- **CPU/RAM Monitoring**: The cat's animation reflects your CPU usage in real-time
  - **Idle** (< 20% CPU): Cat sleeps, stretches, or grooms slowly
  - **Medium** (20-60% CPU): Cat trots or walks at moderate speed
  - **High** (60-90% CPU): Cat sprints quickly across the screen
  - **Critical** (>90% CPU): Cat panics with red glow and rapid movement

### 🧪 Goblin's Jar Widget
- **Battery Status Monitoring**: The goblin inside the jar reflects your laptop's battery level
  - **High Charge** (70-100%): Brightly lit goblin dancing playfully
  - **Medium Charge** (20-70%): Moderately lit goblin casually observing
  - **Low Charge** (< 20%): Dimly lit goblin shivering with flickering light
  - **Critical Charge** (< 10%): Panicking goblin with red flashing alerts
  - **Charging**: Goblin vigorously pumping with green/blue sparks
  - **Full Charge**: Sleeping goblin with bright, contented glow

### ⚙️ Configuration Features
- **System Tray Integration**: Access settings via system tray icon
- **Configurable Widgets**: Enable/disable, resize, and reposition each widget
- **Customizable Thresholds**: Adjust CPU and battery thresholds
- **Always on Top**: Keep widgets visible above other windows
- **Startup Option**: Automatically launch on Windows startup
- **Persistent Settings**: All preferences saved automatically

## Requirements

- **OS**: Windows 10/11
- **.NET**: .NET 8.0 Runtime (included in build)
- **Visual Studio** (for building from source): Visual Studio 2022 or later with .NET 8.0 SDK

## Installation

### Option 1: Download Pre-built Release (Recommended)

1. Go to the [Releases](https://github.com/yourusername/desktop-companion/releases) page
2. Download the latest `DesktopCompanion.zip` file
3. Extract the ZIP file to a folder of your choice
4. Run `DesktopCompanion.exe`

### Option 2: Build from Source

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/desktop-companion.git
   cd desktop-companion
   ```

2. Open the project in Visual Studio 2022 or later

3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

4. Build the project:
   ```bash
   dotnet build --configuration Release
   ```

5. Run the executable from `bin/Release/net8.0-windows/DesktopCompanion.exe`

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

## Adding Custom Assets

The application currently uses placeholder graphics. To add custom pixel art or sprites:

### For Quantum Cat:
1. Create or obtain a sprite sheet with cat animations (e.g., `cat_spritesheet.png`)
2. Place the file in an `Assets` folder in the project root
3. Update `QuantumCatWidget.xaml` to reference your image:
   ```xml
   <Image Source="Assets/cat_spritesheet.png" ... />
   ```
4. Implement sprite sheet animation logic in `QuantumCatWidget.xaml.cs` to cycle through frames

### For Goblin's Jar:
1. Create or obtain jar and goblin graphics
2. Place files in the `Assets` folder
3. Update `GoblinJarWidget.xaml` to reference your images

**Note**: The current implementation uses simple geometric shapes as placeholders. Full sprite sheet support would require additional animation logic.

## Project Structure

```
DesktopCompanion/
├── Models/
│   ├── AppSettings.cs          # Settings model
│   └── SystemMetrics.cs       # System metrics model
├── Services/
│   ├── PerformanceMonitorService.cs  # CPU/RAM/Battery monitoring
│   ├── SettingsService.cs            # Settings persistence
│   ├── StartupService.cs             # Windows startup management
│   └── SystemTrayService.cs          # System tray integration
├── ViewModels/
│   ├── MainViewModel.cs              # Settings window ViewModel
│   ├── QuantumCatViewModel.cs        # Cat widget ViewModel
│   └── GoblinJarViewModel.cs         # Jar widget ViewModel
├── Views/
│   ├── QuantumCatWidget.xaml         # Cat widget UI
│   ├── QuantumCatWidget.xaml.cs      # Cat widget code-behind
│   ├── GoblinJarWidget.xaml          # Jar widget UI
│   └── GoblinJarWidget.xaml.cs       # Jar widget code-behind
├── Utils/
│   └── WindowUtils.cs                # Window manipulation utilities
├── Styles/
│   └── DefaultStyles.xaml            # Application styles
├── App.xaml                          # Application entry point
├── App.xaml.cs                       # Application startup logic
├── MainWindow.xaml                   # Settings window UI
├── MainWindow.xaml.cs                # Settings window code-behind
└── DesktopCompanion.csproj           # Project file
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
- **WMI (Windows Management Instrumentation)**: Battery status and RAM information
- **P/Invoke**: Windows API calls for click-through functionality

## Building for Distribution

### Prerequisites for Distribution

Before building, ensure you have:
- .NET 8.0 SDK installed
- Visual Studio 2022 or later (recommended)
- All project dependencies resolved

### Step 1: Create a Release Build

Build the project in Release configuration:

```bash
dotnet build --configuration Release
```

### Step 2: Publish as Self-Contained Application

Publish the application as a self-contained executable (includes .NET runtime, no installation required):

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

**Options explained:**
- `-c Release`: Release configuration
- `-r win-x64`: Target Windows x64 platform
- `--self-contained true`: Includes .NET runtime (larger but standalone)
- `-p:PublishSingleFile=true`: Creates a single executable file
- `-p:IncludeNativeLibrariesForSelfExtract=true`: Embeds native libraries

### Step 3: Verify Output

The published files will be in:
```
bin/Release/net8.0-windows/win-x64/publish/
```

**Expected files:**
- `DesktopCompanions.exe` (main executable)
- `DesktopCompanions.dll` (if not using single file)
- Supporting DLLs and runtime files
- `Icon.ico` (application icon, if present)

### Step 4: Create Distribution Package

1. **Create a distribution folder:**
   ```bash
   mkdir DesktopCompanions-v1.0.0
   cd DesktopCompanions-v1.0.0
   ```

2. **Copy published files:**
   - Copy all files from `bin/Release/net8.0-windows/win-x64/publish/`
   - Include `README.md` (optional but recommended)
   - Include `LICENSE` file (if applicable)

3. **Create a ZIP archive:**
   - Use Windows Explorer, 7-Zip, or command line:
     ```bash
     # Using PowerShell
     Compress-Archive -Path .\DesktopCompanions-v1.0.0\* -DestinationPath .\DesktopCompanions-v1.0.0-win-x64.zip
     ```

### Step 5: Create GitHub Release

1. **Create a release tag:**
   ```bash
   git tag -a v1.0.0 -m "Release version 1.0.0"
   git push origin v1.0.0
   ```

2. **Draft a new release on GitHub:**
   - Go to your repository → Releases → Draft a new release
   - Select the tag you just created
   - Add release title: "Desktop Companions v1.0.0"
   - Add release notes describing features and changes

3. **Upload the ZIP file:**
   - Drag and drop `DesktopCompanions-v1.0.0-win-x64.zip` to the release
   - Add a description of what's included

4. **Publish the release**

### Distribution Package Structure

```
DesktopCompanions-v1.0.0-win-x64.zip
├── DesktopCompanions.exe          # Main executable
├── DesktopCompanions.dll          # Application DLL (if not single-file)
├── [Runtime files]                # .NET runtime and dependencies
├── README.md                       # User documentation
└── LICENSE                         # License file
```

### Alternative: Framework-Dependent Build

For a smaller distribution (requires .NET 8.0 Runtime installed):

```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

**Note:** Users will need to install .NET 8.0 Runtime separately from [Microsoft's website](https://dotnet.microsoft.com/download/dotnet/8.0).

### Size Considerations

- **Self-contained single-file**: ~60-80 MB (includes full .NET runtime)
- **Framework-dependent**: ~2-5 MB (requires .NET runtime installation)
- **Self-contained multi-file**: ~60-80 MB total (multiple files, faster startup)

### Testing the Distribution Package

Before releasing:

1. Extract the ZIP to a clean test folder
2. Run `DesktopCompanions.exe` from the extracted location
3. Verify:
   - Application launches correctly
   - Widgets appear and function
   - Settings window opens
   - System tray icon appears
   - Settings save and load correctly
   - Startup option works

### Code Signing (Optional but Recommended)

For production releases, consider code signing to avoid Windows security warnings:

1. Obtain a code signing certificate
2. Sign the executable:
   ```bash
   signtool sign /f certificate.pfx /p password DesktopCompanions.exe
   ```

### Version Information

Update version in `DesktopCompanion.csproj`:
```xml
<PropertyGroup>
  <Version>1.0.0</Version>
  <AssemblyVersion>1.0.0.0</AssemblyVersion>
  <FileVersion>1.0.0.0</FileVersion>
</PropertyGroup>
```

## Troubleshooting

### Widgets Not Appearing
- Check if widgets are enabled in settings
- Ensure the application is running (check system tray)
- Try resetting widget positions

### Performance Counters Not Working
- Run the application as Administrator (may be required for some performance counters)
- Check Windows Performance Monitor to verify counters are accessible

### Battery Widget Not Showing Correct Status
- The widget requires WMI access (usually available by default)
- On desktop computers without a battery, the widget will show a default state

### Click-Through Not Working
- Click-through is implemented for the entire window except the drag handle
- The drag handle is a 10px transparent border around each widget
- If you can't drag widgets, try clicking closer to the edges

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

## Support

For issues, questions, or feature requests, please open an issue on the [GitHub Issues](https://github.com/yourusername/desktop-companion/issues) page.

---

**Enjoy your desktop companions!** 🐱🧪

