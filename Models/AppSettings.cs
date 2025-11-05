using System;
using System.Windows;

namespace DesktopCompanions.Models
{
    /// <summary>
    /// Application settings model
    /// </summary>
    public class AppSettings
    {
        // General Settings
        public bool RunOnStartup { get; set; } = false;
        public bool AlwaysOnTop { get; set; } = true;

        // Quantum Cat Settings
        public bool EnableQuantumCat { get; set; } = true;
        public double CatSizeScale { get; set; } = 1.0;
        public double CatCpuIdleThreshold { get; set; } = 20.0;
        public double CatCpuMediumThreshold { get; set; } = 60.0;
        public double CatCpuHighThreshold { get; set; } = 90.0;
        public Point CatPosition { get; set; } = new Point(100, 100);

        // Goblin Jar Settings
        public bool EnableGoblinJar { get; set; } = true;
        public double JarSizeScale { get; set; } = 1.0;
        public double JarLowBatteryThreshold { get; set; } = 20.0;
        public double JarCriticalBatteryThreshold { get; set; } = 10.0;
        public Point JarPosition { get; set; } = new Point(200, 100);
    }
}
