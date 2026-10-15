namespace BTLtruc_quan.Shared.Enums;

public enum BookingPaymentStatus
{
    Unpaid = 1,
    Deposited = 2,
    Paid = 3,
    RefundPending = 4,
    Refunded = 5
}

public enum PaymentTransactionStatus
{
    Pending = 1,
    Success = 2,
    Failed = 3
}

public enum PaymentType
{
    Deposit = 1,
    Balance = 2,
    Full = 3,
    Service = 4,
    Refund = 5
}

public enum PaymentMethod
{
    Gateway = 1,
    Cash = 2,
    Transfer = 3,
    Card = 4,
    BankTransfer = 3, // Alias tuong thich code DTO
    EWallet = 1,      // Alias tuong thich code DTO
    CreditCard = 4    // Alias tuong thich code DTO
}

public enum PaymentGateway
{
    VnPay = 1,
    MoMo = 2,
    VietQr = 3,
    Mock = 4
}

public enum InvoiceItemType
{
    Slot = 1,
    Service = 2,
    Discount = 3
}

public enum VoucherDiscountType
{
    Percent = 1,
    Fixed = 2
}
