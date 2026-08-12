using System.ComponentModel;

namespace Pulonia.Domain.Enums;

public enum IncidentCategory : byte
{
    [Description("Offline (Main and Backup are offline)")]
    TotalOffline = 0,

    [Description("Main Ip Down")]
    MainIpDown = 1,

    [Description("Main Router Down")]
    MainRouterDown = 2,

    [Description("Backup Router Down")]
    BackupRouterDown = 3,

    [Description("Backup Ip Down")]
    BackupIpDown = 4
}