# Agent Guidelines - Quan_ly_san_bong

## Cơ Chế Tự Động Hóa Toàn Bộ Git Trên Antigravity
Agent chịu trách nhiệm tự động hóa 100% quy trình Git tại local cho User, User không cần thao tác thủ công.

### Quy Trình Tự Động Khi Nhận Yêu Cầu Code:
1. **Tự động chuẩn bị**:
   - Chạy `git checkout main` và `git pull origin main` để lấy code mới nhất từ GitHub.
   - Tự động tạo nhánh phụ phù hợp: `git checkout -b <feature|fix|refactor>/<ten-cong-viec>`.
2. **Thực hiện công việc**:
   - Viết code, chỉnh sửa file trên nhánh phụ.
   - Chạy build kiểm tra (`dotnet build BTLtruc_quan.sln`).
   - Tự động commit code vào nhánh phụ với message chuẩn.
3. **Tự động đồng bộ và kiểm tra xung đột**:
   - Chạy `git fetch origin` và `git merge origin/main` vào nhánh phụ.
   - **Nếu KHÔNG có conflict**:
     - Tự động chuyển về `main`: `git checkout main`.
     - Ghép nhánh phụ vào `main`: `git merge <nhanh-phu>`.
     - Đẩy thẳng lên GitHub: `git push origin main`.
     - Xóa nhánh phụ local: `git branch -d <nhanh-phu>`.
     - Báo cáo hoàn tất cho User.
   - **Nếu CÓ conflict**:
     - Dừng lại ngay lập tức.
     - Trình bày chi tiết cho User: file conflict, ưu/nhược điểm giữa Code nhánh phụ vs Code từ main, và đề xuất phương án tối ưu vẹn cả đôi đường.
     - Chờ User bấm chọn hoặc xác nhận -> Tự tay sửa file conflict theo phương án đã chọn -> Commit -> Merge vào `main` -> `git push origin main`.

### Nguyên Tắc Bất Di Bất Dịch:
- Tuyệt đối không can thiệp code trực tiếp trên `main`.
- Luôn kiểm tra build thành công trước khi hoàn tất merge.
