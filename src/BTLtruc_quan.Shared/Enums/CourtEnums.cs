namespace BTLtruc_quan.Shared.Enums;

public enum CourtStatus
{
    Active = 1,
    Maintenance = 2,
    Inactive = 3
}

public enum CourtLiveStatus
{
    Available = 1,
    InUse = 2,
    Cleaning = 3
}

public enum SlotType
{
    Peak = 1,
    OffPeak = 2
}

public enum DayType
{
    Weekday = 1,
    Weekend = 2
}

public enum BlockType
{
    Maintenance = 1,
    Tournament = 2,
    Other = 3
}
