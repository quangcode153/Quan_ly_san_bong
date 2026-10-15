# BÁO CÁO TIẾN ĐỘ THỰC HIỆN DATABASE

* **Ngày thực hiện:** 15/10/2026
* **Người thực hiện:** Trần Minh Trí (Vai trò: Database Lead)
* **Dự án:** Quản lý chuỗi cho thuê sân thể thao (SportChain VN)
* **Nền tảng:** .NET 8.0 C# | Entity Framework Core 8.0 | Microsoft SQL Server (Code-First)

---

## 📌 CÁC CÔNG VIỆC ĐÃ LÀM VÀ HOÀN THÀNH

### 1. Thiết lập thư viện & Nền tảng Core Data
- Cài đặt và cấu hình thư viện `EFCore.NamingConventions` (tự động chuẩn hóa tên bảng và cột sang `snake_case` khớp với tài liệu đặc tả).
- Xây dựng [BaseEntity.cs](file:///c:/Users/Minh%20Tr%C3%AD/.gemini/antigravity-ide/scratch/Quan_ly_san_bong/src/BTLtruc_quan.WebApi/Entities/BaseEntity.cs) (`Id`, `CreatedAt`, `UpdatedAt`) và interface [ISoftDelete.cs](file:///c:/Users/Minh%20Tr%C3%AD/.gemini/antigravity-ide/scratch/Quan_ly_san_bong/src/BTLtruc_quan.WebApi/Entities/ISoftDelete.cs) (`DeletedAt`).

### 2. Định nghĩa toàn bộ Enums chuẩn hóa ([BTLtruc_quan.Shared/Enums](file:///c:/Users/Minh%20Tr%C3%AD/.gemini/antigravity-ide/scratch/Quan_ly_san_bong/src/BTLtruc_quan.Shared/Enums))
- Định nghĩa đúng **3 vai trò**: `Admin`, `BranchManager`, `Customer`.
- Xây dựng đầy đủ các bộ Enum định danh trạng thái:
  - Tài khoản: `UserStatus`, `AuthTokenType`, `CustomerViolationType`, `CustomerViolationAction`.
  - Chi nhánh: `BranchStatus`.
  - Sân & Ca: `CourtStatus`, `CourtLiveStatus`, `SlotType`, `DayType`, `BlockType`.
  - Đơn đặt: `BookingStatus`, `BookingSource`, `BookingSlotStatus`, `SlotLockSourceType`, `BookingAction`, `CheckinMethod`, `CancelType`, `RecurringStatus`, `RecurringSkipReason`, `WaitlistStatus`.
  - Thanh toán: `BookingPaymentStatus`, `PaymentTransactionStatus`, `PaymentType`, `PaymentMethod`, `PaymentGateway`, `InvoiceItemType`, `VoucherDiscountType`.
  - Dịch vụ, Thiết bị & Sự cố: `ServiceAddedWhen`, `EquipmentStatus`, `IncidentSeverity`, `IncidentStatus`.
  - Tương tác & Hệ thống: `ComplaintStatus`, `ComplaintSenderRole`, `ChatThreadType`, `ChatSenderType`, `BroadcastTarget`, `EmailQueueStatus`, `ReminderType`, `JobStatus`.

### 3. Xây dựng 40 Entities theo 9 nhóm đặc tả ([BTLtruc_quan.WebApi/Entities](file:///c:/Users/Minh%20Tr%C3%AD/.gemini/antigravity-ide/scratch/Quan_ly_san_bong/src/BTLtruc_quan.WebApi/Entities))
- **Nhóm A (Người dùng & Xác thực):** `Role`, `User`, `AuthToken`, `BranchStaffName`, `CustomerViolation`.
- **Nhóm B (Chi nhánh & Chính sách):** `Branch`, `BranchImage`, `Amenity`, `BranchAmenity`, `BranchPolicy`, `CancelPolicyTier`.
- **Nhóm C (Sân, Ca & Bảng giá):** `CourtType`, `Court`, `TimeSlot`, `PriceRule`, `SlotBlock`, `Holiday`.
- **Nhóm D (Đơn đặt sân & Khóa ca chống trùng):** `Booking`, `BookingSlot`, `SlotLock`, `BookingHistory`, `RecurringBooking`, `RecurringSkip`.
- **Nhóm E (Thanh toán & Hóa đơn):** `Payment`, `Invoice`, `InvoiceItem`, `DailyClosing`, `Voucher`.
- **Nhóm F (Dịch vụ & Thiết bị sự cố):** `Service`, `BookingService`, `Equipment`, `Incident`.
- **Nhóm G (Tương tác):** `Review`, `ReviewReply`, `Complaint`, `ComplaintMessage`, `ChatThread`, `ChatMessage`.
- **Nhóm H (Hệ thống & Nhật ký):** `Notification`, `Broadcast`, `EmailQueue`, `BookingReminder`, `SystemSetting`, `AuditLog`, `JobRun`.
- **Nhóm I (Mở rộng):** `Waitlist`, `Favorite`.
- Tích hợp thuộc tính `RowVersion` kiểm soát tương tranh lạc quan (**Optimistic Concurrency Control**) trên `Booking` và `SlotLock`.

### 4. Cấu hình Fluent API, Ràng buộc & Chuẩn hóa 3NF ([BTLtruc_quan.WebApi/Data/Configurations](file:///c:/Users/Minh%20Tr%C3%AD/.gemini/antigravity-ide/scratch/Quan_ly_san_bong/src/BTLtruc_quan.WebApi/Data/Configurations))
- Thiết lập **Filtered Unique Index** chống trùng:
  - `courts`: UNIQUE `(branch_id, name)` với điều kiện `[deleted_at] IS NULL`.
  - `users`: UNIQUE `(email)` với điều kiện `[email] IS NOT NULL`.
  - `price_rules`: UNIQUE `(branch_id, court_type_id, court_id, day_type, slot_type, valid_from)` với `HasFilter(null)`.
  - **`slot_locks`**: Ràng buộc then chốt UNIQUE `(court_id, slot_date, slot_id)` để chống trùng ca tuyệt đối (QT02).
- Thiết lập 10 ràng buộc **CHECK nghiệp vụ trên `bookings`**: kiểm tra giờ bắt đầu/kết thúc, số tiền không âm, công thức tổng tiền, tỷ lệ cọc, bắt buộc hạn giữ chỗ khi `Holding`, bắt buộc lý do hủy khi Quản lý/Admin hủy đơn.
- Ràng buộc CHECK vai trò `users`: Quản lý chi nhánh (`role_id = 2`) bắt buộc có `branch_id`, vai trò khác phải có `branch_id IS NULL`.
- Cấu hình chuẩn hóa kiểu chuỗi: Toàn bộ chuỗi tiếng Việt dùng `nvarchar`/`nvarchar(max)`, chuỗi ASCII dùng `.IsUnicode(false)`.
- Thiết lập quy chuẩn xóa khóa ngoại: Mặc định `Restrict` toàn cục chống lỗi vòng lặp cascade của SQL Server; chỉ `Cascade` cho các bảng con phụ thuộc trực tiếp.

### 5. Xây dựng DbContext & Xử lý tự động ([AppDbContext.cs](file:///c:/Users/Minh%20Tr%C3%AD/.gemini/antigravity-ide/scratch/Quan_ly_san_bong/src/BTLtruc_quan.WebApi/Data/AppDbContext.cs))
- Cấu hình Collation chuẩn tiếng Việt: `Vietnamese_100_CI_AS`.
- Quy chuẩn toàn cục: Tiền tệ `decimal(12, 0)`, ngày giờ `datetime2(0)`.
- Áp dụng Global Query Filter cho các thực thể xóa mềm (`ISoftDelete`).
- Ghi đè `SaveChangesAsync`: Tự động cập nhật `CreatedAt`, `UpdatedAt` và tự động chuyển thao tác xóa cứng thành cập nhật `DeletedAt = DateTime.UtcNow`.

### 6. Khởi tạo Dữ liệu hạt giống ([DbInitializer.cs](file:///c:/Users/Minh%20Tr%C3%AD/.gemini/antigravity-ide/scratch/Quan_ly_san_bong/src/BTLtruc_quan.WebApi/Data/DbInitializer.cs))
- Seed 3 vai trò hệ thống: `Admin` (Id=1), `BranchManager` (Id=2), `Customer` (Id=3).
- Seed tài khoản Admin tối cao mặc định (`admin@sportchain.vn`).
- Seed danh mục 6 loại sân thể thao tiêu chuẩn: Bóng đá 7 người, bóng đá 5 người, cầu lông, pickleball, tennis, bóng rổ.
- Seed các cấu hình hệ thống: `hold_minutes` (15p), `checkin_grace_minutes` (15p), `default_deposit_percent` (30%), `noshow_threshold` (3 lần).

### 7. Tự động hóa đồng bộ nhóm & Xuất Script SQL
- Cập nhật [Program.cs](file:///c:/Users/Minh%20Tr%C3%AD/.gemini/antigravity-ide/scratch/Quan_ly_san_bong/src/BTLtruc_quan.WebApi/Program.cs): Tự động kiểm tra và chạy `db.Database.Migrate()` cùng `DbInitializer.SeedAsync()` khi Web API khởi động (các thành viên nhóm chỉ cần kéo code về và chạy F5 là CSDL tự sinh).
- Tạo thành công Migration đầu tiên: `InitialCreate`.
- Xuất file script SQL độc lập [doc/script_database.sql](file:///c:/Users/Minh%20Tr%C3%AD/.gemini/antigravity-ide/scratch/Quan_ly_san_bong/doc/script_database.sql) (2,154 dòng SQL) để thành viên có thể chạy trực tiếp trên SSMS mà không cần công cụ dòng lệnh .NET CLI.

---

## 🎯 KẾT QUẢ CUỐI BUỔI
- Toàn bộ Solution biên dịch thành công: **Build succeeded - 0 Errors, 0 Warnings**.
- Cơ sở dữ liệu đáp ứng 100% tài liệu Đặc tả chức năng (Mục 16 & 17) và tài liệu Công nghệ.
- Đã bàn giao mã nguồn và kịch bản CSDL sẵn sàng cho các thành viên phụ trách Web API và WinForms kết nối làm việc.
