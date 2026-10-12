# PHÔI CẤU TRÚC DỰ ÁN BÀI TẬP LỚN LẬP TRÌNH TRỰC QUAN (.NET 8.0)

> **Thư mục dự án**: `BTLtruc_quan`  
> **Nền tảng**: .NET 8.0 (C#)  
> **Mô hình kiến trúc**: Multi-tier / Client-Server (WinForms Desktop + Web API Backend + Shared Library + Unit Tests)

---

## 📂 SƠ ĐỒ CẤU TRÚC THƯ MỤC (PROJECT SCAFFOLDING)

```text
BTLtruc_quan/
├── BTLtruc_quan.sln                 # File Solution Visual Studio truyền thống
├── BTLtruc_quan.slnx                # File Solution định dạng hiện đại (.NET 8/9/10)
├── run_backend.bat                  # Script khởi chạy nhanh Backend Web API
├── run_winforms.bat                 # Script khởi chạy nhanh WinForms Desktop
├── doc/                             # Tài liệu phân công nhóm, đặc tả, báo cáo BTL
│   └── KE_HOACH_PHAN_CONG.md        # Kế hoạch mẫu chia việc cho nhóm 5 người
├── src/
│   ├── BTLtruc_quan.Shared/         # Thư viện dùng chung giữa Client & Server
│   │   ├── Constants/               # Hằng số toàn hệ thống
│   │   ├── DTOs/                    # Data Transfer Objects (Request/Response)
│   │   └── Enums/                   # Các kiểu liệt kê (Vai trò, trạng thái...)
│   │
│   ├── BTLtruc_quan.WebApi/         # Tầng Backend (RESTful Web API)
│   │   ├── Controllers/             # API Endpoints điều phối request
│   │   ├── Data/                    # DbContext (EF Core), Migrations, Seed Data
│   │   ├── Entities/                # Các lớp thực thể CSDL (Database Models)
│   │   ├── Hubs/                    # SignalR Realtime Hubs (nếu có cập nhật thời gian thực)
│   │   ├── Middlewares/             # Xử lý lỗi toàn cục, logging
│   │   ├── Services/                # Logic nghiệp vụ xử lý trung gian
│   │   ├── appsettings.json         # Chuỗi kết nối SQL Server & cấu hình hệ thống
│   │   └── Program.cs               # Cấu hình khởi tạo Web API, DI Container, Swagger
│   │
│   └── BTLtruc_quan.WinForms/       # Tầng Frontend Desktop Client
│       ├── Common/                  # Cấu hình Client, Helper dùng chung
│       ├── Forms/                   # Các cửa sổ giao diện chính (LoginForm, MainForm...)
│       ├── Services/                # Lớp gọi API (HttpClient), lưu phiên đăng nhập
│       ├── UI/
│       │   ├── Controls/            # Các Custom Controls tự vẽ (Button, TextBox, Card...)
│       │   └── Theme/               # Bảng mã màu chủ đạo, font chữ (Design Tokens)
│       ├── UserControls/            # Các màn hình nghiệp vụ nhúng trong MainForm
│       └── Program.cs               # Điểm bắt đầu khởi chạy ứng dụng WinForms
│
└── tests/
    └── BTLtruc_quan.Tests/          # Bộ kiểm thử tự động (Unit Test / xUnit)
```

---

## 👥 PHÂN CHIA NHIỆM VỤ GỢI Ý CHO NHÓM 5 NGƯỜI

1. **Thành viên 1 (Backend Lead)**: Phụ trách cấu hình WebApi `Program.cs`, viết Controllers, thiết lập Swagger, JWT Auth, kết nối Db.
2. **Thành viên 2 (Database & Service)**: Thiết kế Entities trong WebApi, cấu hình `DbContext`, migration CSDL SQL Server, viết Services nghiệp vụ.
3. **Thành viên 3 (Frontend Shell & Controls)**: Xây dựng Form chính (`MainForm`), thiết kế Sidebar/Menu điều hướng, hệ thống màu sắc `Theme` và Custom Controls.
4. **Thành viên 4 (Frontend Features & API Client)**: Thiết kế các màn hình `UserControls` nghiệp vụ cụ thể, kết nối gọi API từ Backend, xử lý hiển thị bảng dữ liệu.
5. **Thành viên 5 (QA & Báo cáo)**: Viết Unit Tests trong `BTLtruc_quan.Tests`, viết tài liệu Báo cáo BTL (Word/PDF) và thiết kế Slide bảo vệ đề tài.
