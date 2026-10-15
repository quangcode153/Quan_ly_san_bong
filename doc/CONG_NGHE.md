# DANH MỤC CÔNG NGHỆ VÀ THƯ VIỆN SỬ DỤNG - SPORTCHAIN VN

Hệ thống **SportChain VN** (Hệ thống Quản lý Chuỗi Cụm Sân Thể Thao Đa Chi Nhánh) được xây dựng theo mô hình kiến trúc **N-Tier (Client - Server)** phân tán hiện đại trên nền tảng **.NET 8.0**.

---

## 1. TỔNG QUAN KIẾN TRÚC & PHÂN TẦNG

```
                    ┌──────────────────────────────────────────┐
                    │          SportChain.Shared               │
                    │   (Enums, DTOs, Requests/Responses)      │
                    └───────▲──────────────────────────▲───────┘
                            │                          │
        ┌───────────────────┴──────────┐   ┌───────────┴────────────────┐
        │     SportChain.WinForms      │   │    SportChain.Client       │
        │   Desktop Client (.NET 8)    │   │  Blazor WebAssembly (.NET 8│
        │  (Lễ tân, POS, Quản lý trạm) │   │     (Khách hàng đặt vé)    │
        └──────────────▲───────────────┘   └───────────▲────────────────┘
                       │   REST API + SignalR          │
                       └───────────────┬───────────────┘
                                       │
                    ┌──────────────────▼───────────────────────┐
                    │          SportChain.WebApi               │
                    │      ASP.NET Core Web API (.NET 8)       │
                    │ (JWT Bearer, SignalR Hub, EF Core 8)     │
                    └──────────────────┬───────────────────────┘
                                       │
                    ┌──────────────────▼───────────────────────┐
                    │          Microsoft SQL Server            │
                    │   (EF Core Migrations, RowVersion)       │
                    └──────────────────────────────────────────┘
```

---

## 2. BACKEND SERVICE (`SportChain.WebApi`)

* **Nền tảng cốt lõi:**
  * **ASP.NET Core Web API (.NET 8.0)**: Khung ứng dụng backend hiệu năng cao, hỗ trợ Dependency Injection, Middleware pipeline và kiến trúc Controller-Service-Repository.
* **Cơ sở dữ liệu & ORM:**
  * **Microsoft SQL Server**: Hệ quản trị cơ sở dữ liệu quan hệ chính.
  * **Entity Framework Core 8.0 (`Microsoft.EntityFrameworkCore.SqlServer` v8.0.11)**:
    * Phương pháp tiếp cận **Code-First**.
    * Quản lý tiến trình thay đổi cấu trúc bảng qua **EF Core Migrations**.
    * Sử dụng thuộc tính `RowVersion` / `[Timestamp]` để kiểm soát tương tranh lạc quan (**Optimistic Concurrency Control**), ngăn chặn đặt trùng sân khi có nhiều thao tác đồng thời.
* **Xác thực & Phân quyền (Authentication & Authorization):**
  * **JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer` v8.0.11)**: Cơ chế xác thực không lưu trạng thái (Stateless), cấp phát Token khi đăng nhập và giải mã Claims tại Backend.
  * **Role-Based Access Control (RBAC)**: Phân quyền chặt chẽ giữa 4 vai trò: `SuperAdmin`, `BranchManager`, `Staff`, `Customer`.
* **Giao tiếp thời gian thực (Real-time Communication):**
  * **ASP.NET Core SignalR**:
    * Quản lý kết nối WebSocket/Long-Polling qua `BookingHub`.
    * Phục vụ cơ chế giữ chỗ 15 phút (**15-Minute Slot Holding Lock**).
    * Bắn thông báo cập nhật tức thì trạng thái ca sân (`PendingPayment`, `Confirmed`, `InUse`, `Completed`, `Cancelled`, `NoShow`) tới toàn bộ các máy trạm WinForms và Web Client.
* **Tài liệu hóa & Kiểm thử API:**
  * **Swagger / OpenAPI (`Swashbuckle.AspNetCore` v6.6.2)**: Tự động phát sinh giao diện tương tác và tài liệu API tại đường dẫn `/swagger`.
* **Mã vạch & QR Code:**
  * **QRCoder (v1.6.0)**: Thư viện tạo chuỗi Base64 / Bitmap mã QR động phục vụ check-in tại quầy và xuất hóa đơn điện tử.

---

## 3. DESKTOP CLIENT (`SportChain.WinForms`)
*(Sản phẩm trọng tâm cho học phần Lập trình trực quan - Dành cho Lễ tân, Quản lý chi nhánh & Thu ngân POS)*

* **Nền tảng:** **C# Windows Forms trên .NET 8.0 (`net8.0-windows`)**.
* **Nguyên tắc thiết kế giao diện (UI/UX):**
  * **Modern Flat Design**: Giao diện phẳng hiện đại, tinh gọn với bảng màu chuẩn nhận diện thương hiệu SportChain (Emerald Green, Navy, Slate Gray).
  * Chuẩn hóa theo mô hình Visual Studio Form Designer (`*.cs` và `*.Designer.cs`).
  * Cấu hình DPI thích ứng (`DpiUnaware` / `Font` Scaling) ngăn vỡ layout trên các màn hình có tỉ lệ phóng to khác nhau.
* **Đồ họa & Icon:**
  * **FontAwesome.Sharp (v6.6.0)**: Tích hợp thư viện biểu tượng vector sắc nét (Icons) cho Sidebar, thanh điều hướng, nút chức năng và trạng thái.
  * **Custom Controls (Owner-Drawn GDI+)**: Tự xây dựng các bộ điều khiển riêng như `StatusBadge`, `ToggleSwitch`, `MetricCard`, `EmptyState`...
  * **Kỹ thuật DoubleBuffered (Bộ đệm kép)**: Bật bộ đệm kép đồ họa cho bảng lưới ma trận lịch sân (`CourtMatrixGrid`) nhằm loại bỏ hoàn toàn hiện tượng nháy giật (flicker) khi tiếp nhận dữ liệu SignalR cập nhật liên tục.
* **Giao tiếp mạng:**
  * **System.Net.Http.Json & HttpClient**: Xử lý gọi API bất đồng bộ (`async`/`await`), đóng gói và giải mã JSON payload tốc độ cao.
  * **SignalR Client (`Microsoft.AspNetCore.SignalR.Client` v8.0.8)**: Duy trì kết nối thời gian thực với WebApi, xử lý vòng đời kết nối và hủy đăng ký sự kiện (`Dispose`) để chống rò rỉ bộ nhớ.
* **Tạo & Đọc mã QR:**
  * **QRCoder (v1.6.0)**: Dựng ảnh QR Code cho vé đặt sân và thanh toán chuyển khoản trực tiếp trên Form.

---

## 4. WEB CLIENT (`SportChain.Client`)

* **Nền tảng:** **Blazor WebAssembly (.NET 8.0)**.
* **Công nghệ Frontend Web:**
  * C# Razor Components chạy trực tiếp trên Browser qua môi trường thực thi WebAssembly.
  * HTML5, CSS3 hiện đại, Responsive layout.
* **Thời gian thực:**
  * `Microsoft.AspNetCore.SignalR.Client (v10.0.12)`: Kết nối `BookingHub` để người dùng thấy slot sân thay đổi trực tiếp trên trình duyệt.

---

## 5. THƯ VIỆN CHIA SẺ (`SportChain.Shared`)

* **Nền tảng:** **.NET 8.0 Class Library**.
* **Thành phần chia sẻ:**
  * **Domain Enums:** `BookingStatus`, `CourtType`, `UserRole`, `PaymentMethod`, `CourtSlotStatus`...
  * **Data Transfer Objects (DTOs):** Các cấu trúc dữ liệu truyền tải giữa Client và Server (LoginRequest/Response, BookingDto, CourtMatrixCellDto, POSCheckoutRequest...).
  * Đảm bảo tính nhất quán dữ liệu giữa cả 3 dự án (WebApi, WinForms, Blazor) mà không bị trùng lặp định nghĩa.

---

## 6. CÔNG CỤ & QUY TRÌNH PHÁT TRIỂN (TOOLING & DEVOPS)

* **IDE / Trình soạn thảo:**
  * Visual Studio 2022 (v17.8+) với .NET 8 SDK.
  * Visual Studio Code / JetBrains Rider.
* **Quản lý Solution:**
  * Sử dụng định dạng tệp mới của .NET: `SportChainManager.slnx` (XML-based solution format).
* **Quản lý phiên bản mã nguồn (VCS):**
  * Git với quy chuẩn phân nhánh tính năng (`feature/*`, `bugfix/*`), Commit convention rõ ràng.
* **Kiểm soát chất lượng mã nguồn:**
  * Bắt buộc giải pháp luôn đạt trạng thái **Build Clean**: 0 Errors, 0 Warnings (`dotnet build SportChainManager.slnx`).
