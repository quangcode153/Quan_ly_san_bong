namespace BTLtruc_quan.Shared.Enums;

/// <summary>
/// Phương thức thanh toán giao dịch
/// </summary>
public enum PaymentMethod
{
    Cash = 1,         // Tiền mặt tại quầy lễ tân
    BankTransfer = 2, // Chuyển khoản ngân hàng / Quét mã VietQR
    EWallet = 3,      // Ví điện tử (Momo, ZaloPay, VNPay)
    CreditCard = 4    // Thẻ ngân hàng / POS quẹt thẻ
}
