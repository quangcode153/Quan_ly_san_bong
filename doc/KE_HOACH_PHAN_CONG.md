# MẪU KẾ HOẠCH & PHÂN CÔNG CÔNG VIỆC NHÓM 5 NGƯỜI

> **Môn học**: Lập trình trực quan (LTTQ)  
> **Nền tảng**: .NET 8.0 C# (Windows Forms + Web API + SQL Server)

---

## 📌 PHÂN CÔNG VAI TRÒ & PHỤ TRÁCH MÃ NGUỒN

| TV | Họ tên & Vai trò | Module / Thư mục phụ trách chính | Nhiệm vụ chính |
| :---: | :--- | :--- | :--- |
| **1** | **Backend Lead** | `BTLtruc_quan.WebApi/Controllers`<br>`Program.cs` | Dựng khung Web API, cấu hình CORS, JWT, Swagger, định tuyến Controller |
| **2** | **Database Lead** | `BTLtruc_quan.WebApi/Entities`<br>`Data/DbContext` | Thiết kế CSDL, tạo bảng SQL Server bằng EF Core, viết hàm xử lý nghiệp vụ |
| **3** | **Frontend Lead (Shell & UI)** | `BTLtruc_quan.WinForms/Forms`<br>`UI/Theme`, `UI/Controls` | Thiết kế LoginForm, MainForm (Sidebar/Menu phẳng hiện đại), Custom Controls |
| **4** | **Frontend Dev (Màn hình chức năng)** | `BTLtruc_quan.WinForms/UserControls`<br>`Services/ApiClient` | Lập trình các màn hình chức năng (Danh mục, Thêm/Sửa/Xóa, Tìm kiếm, Báo cáo) |
| **5** | **QA & Báo cáo** | `BTLtruc_quan.Tests`<br>`doc/` | Viết kịch bản kiểm thử, chụp ảnh màn hình, soạn thảo Báo cáo BTL và Slide |

---

## 🗓️ TIẾN ĐỘ 4 TUẦN THỰC HIỆN

- **Tuần 1**: Khởi tạo Solution, thiết kế sơ đồ CSDL, dựng bảng thực thể và bộ giao diện cơ bản (Form mẫu).
- **Tuần 2**: Viết API Đăng nhập/Đăng xuất, kết nối CSDL SQL Server, dựng Form đăng nhập và Menu chính WinForms.
- **Tuần 3**: Hoàn thiện các chức năng nghiệp vụ CRUD trên WinForms (kết nối API Backend), tìm kiếm, phân trang, lọc dữ liệu.
- **Tuần 4**: Kiểm thử tổng thể, sửa lỗi giao diện, đóng gói ứng dụng, hoàn thiện Báo cáo BTL (Word/PDF) và Slide PowerPoint bảo vệ.
