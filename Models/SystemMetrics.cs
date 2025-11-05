namespace DesktopCompanions.Models
{
    /// <summary>
    /// System metrics model for real-time monitoring
    /// </summary>
    public class SystemMetrics
    {
        public double CpuUsage { get; set; } = 0.0;
        public double RamUsage { get; set; } = 0.0;
        public double BatteryPercent { get; set; } = 100.0;
        public BatteryStatus BatteryStatus { get; set; } = BatteryStatus.Unknown;
    }

    public enum BatteryStatus
    {
        Unknown,
        Charging,
        Discharging,
        Full,
        NotPresent // For desktops without battery
    }
}
