namespace TrackingSystem.App.Models.Enums;

public enum AssetStatus
{
    Green,   // OK - more than 6 months of life
    Yellow,  // Warning - 3 to 6 months remaining
    Red      // Critical - less than 3 months remaining
}
