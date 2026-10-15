namespace BTLtruc_quan.Shared.Enums;

public enum BookingStatus
{
    Holding = 1,
    PendingPayment = 1, // Alias tuong thich code DTO
    Deposited = 2,
    Confirmed = 2,      // Alias tuong thich code DTO
    CheckedIn = 3,
    Completed = 4,
    Cancelled = 5,
    Expired = 6,
    NoShow = 7
}

public enum BookingSource
{
    Online = 1,
    Counter = 2
}

public enum BookingSlotStatus
{
    Active = 1,
    Released = 2,
    Moved = 3
}

public enum SlotLockSourceType
{
    Booking = 1,
    Block = 2
}

public enum BookingAction
{
    Create = 1,
    PayDeposit = 2,
    Checkin = 3,
    Extend = 4,
    ChangeSlot = 5,
    ChangeCourt = 6,
    AddService = 7,
    Cancel = 8,
    Expire = 9,
    NoShow = 10,
    Complete = 11
}

public enum CheckinMethod
{
    Qr = 1,
    Code = 2
}

public enum CancelType
{
    Customer = 1,
    Manager = 2,
    Admin = 3,
    Expired = 4,
    NoShow = 5
}

public enum RecurringStatus
{
    Active = 1,
    Paused = 2,
    Ended = 3
}

public enum RecurringSkipReason
{
    Conflict = 1,
    CustomerSkip = 2,
    Blocked = 3
}

public enum WaitlistStatus
{
    Waiting = 1,
    Notified = 2,
    Booked = 3,
    Expired = 4
}
