using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Bookings;
using BTLtruc_quan.WebApi.Entities.Branches;

namespace BTLtruc_quan.WebApi.Entities.Users;

public class Role
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
}

public class User : BaseEntity, ISoftDelete
{
    public long RoleId { get; set; }
    public long? BranchId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PasswordHash { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsWalkIn { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public string? LockReason { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public int NoShowCount { get; set; }
    public DateTime? BookingRestrictedUntil { get; set; }
    public bool RequireFullPrepay { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Role Role { get; set; } = null!;
    public Branch? Branch { get; set; }
    public ICollection<AuthToken> AuthTokens { get; set; } = new List<AuthToken>();
    public ICollection<CustomerViolation> Violations { get; set; } = new List<CustomerViolation>();
}

public class AuthToken : BaseEntity
{
    public long UserId { get; set; }
    public AuthTokenType Type { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public User User { get; set; } = null!;
}

public class BranchStaffName : BaseEntity
{
    public long BranchId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Branch Branch { get; set; } = null!;
}

public class CustomerViolation : BaseEntity
{
    public long UserId { get; set; }
    public long BookingId { get; set; }
    public CustomerViolationType Type { get; set; }
    public CustomerViolationAction ActionTaken { get; set; } = CustomerViolationAction.None;
    public long? ResolvedByUserId { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public User User { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
    public User? ResolvedByUser { get; set; }
}
