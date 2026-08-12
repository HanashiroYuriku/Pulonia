namespace Pulonia.Domain.Enums;

public enum StoreStatus
{
    // All router are up
    Up = 1,

    // Main Router up, but Branch Ip down
    WarningMainIpDown = 2,

    // Main Router down, but Backup Router Up
    WarningMainRouterDown = 3,

    // Main Router and Main Ip are up, but Backup Router down
    WarningBackupRouterDown = 4,

    // Main Router, Main Ip, and Backup Router are up, but Backup Ip down
    WarningBackupIpDown = 5,

    // Main Ip (OR Main Router) and Backup Ip (OR Backup Router) down
    Down = 0
}