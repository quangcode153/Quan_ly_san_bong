namespace BTLtruc_quan.Shared.Enums;

public enum UserStatus
{
    Active = 1,
    Locked = 2
}

public enum AuthTokenType
{
    Refresh = 1,
    ResetPassword = 2,
    VerifyEmail = 3
}

public enum CustomerViolationType
{
    NoShow = 1,
    LateCancel = 2
}

public enum CustomerViolationAction
{
    None = 1,
    RequirePrepay = 2,
    RestrictBooking = 3
}
