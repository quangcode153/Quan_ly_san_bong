using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Bookings;
using BTLtruc_quan.WebApi.Entities.Branches;
using BTLtruc_quan.WebApi.Entities.Courts;
using BTLtruc_quan.WebApi.Entities.Users;

namespace BTLtruc_quan.WebApi.Entities.Interactions;

public class Review : BaseEntity, ISoftDelete
{
    public long BookingId { get; set; }
    public long CustomerId { get; set; }
    public long BranchId { get; set; }
    public long CourtId { get; set; }
    public byte Rating { get; set; } // 1..5
    public string? Comment { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Booking Booking { get; set; } = null!;
    public User Customer { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Court Court { get; set; } = null!;
    public ReviewReply? Reply { get; set; }
}

public class ReviewReply : BaseEntity
{
    public long ReviewId { get; set; }
    public long UserId { get; set; }
    public string Content { get; set; } = string.Empty;

    public Review Review { get; set; } = null!;
    public User User { get; set; } = null!;
}

public class Complaint : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public long CustomerId { get; set; }
    public long BranchId { get; set; }
    public long? BookingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public ComplaintStatus Status { get; set; } = ComplaintStatus.New;
    public DateTime? EscalatedAt { get; set; }
    public string? Resolution { get; set; }
    public long? ResolvedByUserId { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public User Customer { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Booking? Booking { get; set; }
    public User? ResolvedByUser { get; set; }
    public ICollection<ComplaintMessage> Messages { get; set; } = new List<ComplaintMessage>();
}

public class ComplaintMessage : BaseEntity
{
    public long ComplaintId { get; set; }
    public long SenderId { get; set; }
    public ComplaintSenderRole SenderRole { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AttachmentUrls { get; set; } // JSON

    public Complaint Complaint { get; set; } = null!;
    public User Sender { get; set; } = null!;
}

public class ChatThread : BaseEntity
{
    public ChatThreadType Type { get; set; }
    public long? CustomerId { get; set; }
    public long? BranchId { get; set; }
    public string Status { get; set; } = "OPEN";
    public DateTime? LastMessageAt { get; set; }

    public User? Customer { get; set; }
    public Branch? Branch { get; set; }
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

public class ChatMessage : BaseEntity
{
    public long ThreadId { get; set; }
    public ChatSenderType SenderType { get; set; }
    public long? SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Meta { get; set; } // JSON
    public DateTime? ReadAt { get; set; }

    public ChatThread Thread { get; set; } = null!;
    public User? Sender { get; set; }
}
