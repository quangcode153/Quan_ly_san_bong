namespace BTLtruc_quan.Shared.Enums;

public enum BroadcastTarget
{
    AllBranches = 1,
    Selected = 2
}

public enum EmailQueueStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3
}

public enum ReminderType
{
    Deposit5Min = 1,
    Play2H = 2,
    Play30Min = 3,
    ReviewInvite = 4
}

public enum JobStatus
{
    Success = 1,
    Failed = 2
}
