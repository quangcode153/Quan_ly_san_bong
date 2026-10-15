using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Bookings;
using BTLtruc_quan.WebApi.Entities.Branches;
using BTLtruc_quan.WebApi.Entities.Users;

namespace BTLtruc_quan.WebApi.Entities.System;

public class Notification : BaseEntity
{
    public long UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Link { get; set; }
    public bool IsRead { get; set; } = false;

    public User User { get; set; } = null!;
}

public class Broadcast : BaseEntity
{
    public long SentByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public BroadcastTarget Target { get; set; } = BroadcastTarget.AllBranches;

    public User SentByUser { get; set; } = null!;
}

public class EmailQueue : BaseEntity
{
    public string ToEmail { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public EmailQueueStatus Status { get; set; } = EmailQueueStatus.Pending;
    public int RetryCount { get; set; } = 0;
    public DateTime? SendAfter { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Error { get; set; }
}

public class BookingReminder
{
    public long BookingId { get; set; }
    public ReminderType Type { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public Booking Booking { get; set; } = null!;
}

public class SystemSetting
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; } = false;
    public string? Description { get; set; }
    public long? UpdatedByUserId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? UpdatedByUser { get; set; }
}

public class AuditLog
{
    public long Id { get; set; }
    public long? ActorUserId { get; set; }
    public string ActorRole { get; set; } = string.Empty;
    public string? PerformedByName { get; set; }
    public long? BranchId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public string? OldValue { get; set; } // JSON
    public string? NewValue { get; set; } // JSON
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? ActorUser { get; set; }
    public Branch? Branch { get; set; }
}

public class JobRun : BaseEntity
{
    public string JobName { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Success;
    public int AffectedRows { get; set; } = 0;
    public string? Error { get; set; }
}
