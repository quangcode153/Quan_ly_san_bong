# QUY ƯỚC LÀM VIỆC GIT DÀNH CHO NHÓM & AGENT (Team Git Workflow Guidelines)

> **Dự án**: Quản lý chuỗi sân bóng (`quangcode153/Quan_ly_san_bong`)  
> **Áp dụng cho**: Toàn bộ thành viên trong nhóm và AI Agent (Antigravity).

---

## 1. NGUYÊN TẮC BẤT DI BẤT DỊCH (VÀNG)
1. **TUYỆT ĐỐI KHÔNG sửa code trực tiếp trên nhánh `main`**.
2. **TUYỆT ĐỐI KHÔNG `push` code thẳng lên `main`** khi chưa qua nhánh phụ và chưa kiểm tra biên dịch (`build`).
3. **TUYỆT ĐỐI KHÔNG dùng `git push --force` (`-f`)** lên nhánh `main` dưới mọi hình thức vì sẽ làm mất code của cả nhóm.
4. Mọi tính năng, sửa lỗi, làm giao diện đều **bắt buộc tạo nhánh phụ riêng**.

---

## 2. QUY ƯỚC ĐẶT TÊN NHÁNH (Branch Naming)
Để tránh các thành viên trong nhóm tạo trùng tên nhánh, đặt tên theo cấu trúc:
- **Tính năng mới**: `feature/<ten-ban>/<ten-tinh-nang>`  
  *(Ví dụ: `feature/quang/dat-san`, `feature/nam/quan-ly-khach-hang`)*
- **Sửa lỗi**: `fix/<ten-ban>/<ten-loi>`  
  *(Ví dụ: `fix/quang/loi-tinh-tien`, `fix/nam/loi-dang-nhap`)*
- **Tối ưu / Dọn dẹp**: `refactor/<ten-ban>/<ten-module>`  
  *(Ví dụ: `refactor/quang/giao-dien-san`)*

---

## 3. QUY TRÌNH 5 BƯỚC CHUẨN KHI CODE

### Bước 1: Kéo code mới nhất từ remote và tạo nhánh phụ
Trước khi bắt đầu code bất cứ thứ gì:
```bash
git checkout main
git pull origin main
git checkout -b <ten-nhanh-phu>
```

### Bước 2: Viết code và commit trên nhánh phụ
- Làm việc và lưu các thay đổi trên nhánh phụ này:
```bash
git add .
git commit -m "<loai>: mô tả ngắn gọn thay đổi đã làm"
```
*(Tiền tố commit: `feat:` thêm mới, `fix:` sửa lỗi, `style:` chỉnh giao diện, `docs:` tài liệu, `refactor:` cấu trúc lại)*.

### Bước 3: Build kiểm tra trước khi hoàn tất
Đảm bảo project biên dịch thành công 0 lỗi:
```bash
dotnet build BTLtruc_quan.sln
```
*(Nếu lỗi biên dịch: Sửa xong và commit lại trước khi đi tiếp)*.

### Bước 4: Đồng bộ code mới nhất từ nhóm về nhánh phụ
Kéo code mới nhất của các bạn khác từ GitHub về nhánh của mình để kiểm tra:
```bash
git fetch origin
git merge origin/main
```

### Bước 5: Xử lý xung đột (Merge Conflict) & Ghép nhánh

#### 👉 Nếu KHÔNG có xung đột (Tự động hoặc ghép êm đẹp):
Chuyển về `main`, kéo nhanh kiểm tra, ghép nhánh phụ vào và đẩy lên GitHub:
```bash
git checkout main
git pull origin main
git merge <ten-nhanh-phu>
dotnet build BTLtruc_quan.sln
git push origin main
git branch -d <ten-nhanh-phu>
```

#### ⚠️ Nếu CÓ xung đột (Conflict):
1. **DỪNG LẠI NGAY**, không được tùy tiện xóa code của người khác.
2. **Mở file xung đột lên phân tích**:
   - Phần code của mình (HEAD): Có tác dụng gì?
   - Phần code của bạn trong nhóm (origin/main): Đang làm chức năng gì?
   - **Quy tắc vẹn cả đôi đường**: Giữ lại cả 2 logic cần thiết, kết hợp sao cho không làm hỏng tính năng của bạn kia và tính năng mới của mình.
3. Nếu không chắc chắn: **Chụp ảnh / gửi đoạn xung đột vào nhóm chat để cùng chốt phương án** trước khi lưu.
4. Sau khi sửa xong conflict:
```bash
git add .
git commit -m "merge: giai quyet xung dot giua <nhanh-phu> va main"
dotnet build BTLtruc_quan.sln
git checkout main
git pull origin main
git merge <ten-nhanh-phu>
git push origin main
git branch -d <ten-nhanh-phu>
```

---

## 4. DÀNH RIÊNG CHO AI AGENT (ANTIGRAVITY)
Nếu bạn làm việc cùng AI Agent Antigravity:
- Agent sẽ **tự động hóa 100% các câu lệnh ở Mục 3** cho bạn.
- Khi gặp xung đột ở Bước 5, Agent sẽ **tự động phân tích ưu/nhược điểm từng lựa chọn và đưa ra phương án tối ưu** trong khung chat để bạn duyệt trước khi tự tay sửa.
- Tuyệt đối không tự ý commit code chưa giải quyết conflict hoàn chỉnh.
