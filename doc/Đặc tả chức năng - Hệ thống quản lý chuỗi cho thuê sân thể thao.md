# Đặc tả chức năng: Hệ thống quản lý chuỗi cho thuê sân thể thao

*Tài liệu tổng hợp toàn bộ chức năng, quy tắc nghiệp vụ, vòng đời đơn, dữ liệu và kỹ thuật của đồ án.*

## 1. Tổng quan

### 1.1 Mục tiêu

Xây dựng hệ thống cho phép khách hàng tìm và đặt sân thể thao (bóng đá, cầu lông, pickleball, tennis, bóng rổ...) theo ca 1 tiếng tại nhiều chi nhánh; chi nhánh vận hành sân và thu tiền tại quầy; Admin quản lý toàn chuỗi.

### 1.2 Vai trò (3 vai trò)

- **Admin (Quản lý chuỗi):** toàn hệ thống.
- **Quản lý chi nhánh:** một tài khoản dùng chung cho chủ và nhân viên của chi nhánh (đã gộp vai trò Nhân viên sân). Chỉ thấy dữ liệu chi nhánh của mình.
- **Khách hàng:** chỉ thấy dữ liệu của chính mình. Khách vãng lai tại quầy được tạo hồ sơ bởi Quản lý chi nhánh.

### 1.3 Ký hiệu ưu tiên

- **\[L\] Lõi:** làm kỹ, demo chính.
- **\[T\] Tiếp:** làm nếu còn thời gian.
- **\[M\] Mở rộng:** có thể bỏ hoặc để hướng phát triển.

### 1.4 Hệ quả của việc dùng chung tài khoản chi nhánh

Không truy vết được từng nhân viên. Giảm nhẹ bằng: (1) trường **Người thực hiện** (tên nhân viên do quản lý khai báo, không cần tài khoản) ghi kèm mọi thao tác thanh toán, check-in, hủy, đổi sân và lưu vào audit log; (2) hành động nhạy cảm (sửa giá, xóa sân, xem báo cáo doanh thu, đổi chính sách) yêu cầu nhập lại mật khẩu; (3) \[M\] khóa nhanh bằng mã PIN theo nhân viên.

## 2. Quy tắc nghiệp vụ cốt lõi

| Mã | Quy tắc |
| --- | --- |
| QT01 | Mỗi ca dài đúng 1 tiếng. Khách chọn theo ca, không nhập giờ tự do. Chọn "giờ bắt đầu/kết thúc" thực chất là chọn ca đầu và ca cuối liên tiếp. |
| QT02 | Chống trùng lịch: mỗi cặp (sân, ngày, ca) chỉ thuộc một đơn đang hiệu lực hoặc một khóa ca. Giữ ca ngay khi khách bấm đặt, bảo đảm bằng ràng buộc unique và transaction ở CSDL. |
| QT03 | Giữ chỗ 15 phút (cấu hình được), có đồng hồ đếm ngược. Quá hạn mà chưa cọc thì tự hủy và nhả ca. |
| QT04 | Đơn tự xác nhận khi cọc thành công, không cần quản lý duyệt tay. |
| QT05 | Chờ check-in: từ giờ bắt đầu ca đầu đến +15 phút (cấu hình được). Không cho check-in sớm. Quá +15 phút chưa check-in thì no-show, mất cọc, nhả ca. |
| QT06 | Hủy sau giờ bắt đầu nhưng trước +15 phút coi như no-show, mất cọc. |
| QT07 | Đơn nhiều ca: check-in một lần cho cả đơn, tính từ ca đầu tiên. |
| QT08 | Hủy theo chính sách hoàn cọc theo mốc thời gian (ví dụ trước 24h hoàn 100%, trước 12h hoàn 50%, sau đó mất cọc). Chính sách riêng từng chi nhánh nhưng nằm trong khung Admin cho phép. |
| QT09 | Hủy thủ công của Quản lý chi nhánh/Admin bắt buộc nhập lý do, ghi log, gửi email cho khách, tính hoàn tiền (nếu lỗi do chi nhánh thì hoàn 100%). |
| QT10 | Đơn lưu **giá snapshot** (giá từng ca, tiền cọc, chính sách hoàn tại thời điểm đặt). Đổi giá sau đó không ảnh hưởng đơn cũ. |
| QT11 | Xóa mềm cho chi nhánh, sân, dịch vụ, thiết bị, tài khoản để không mất lịch sử đơn. |
| QT12 | Khách vi phạm: no-show từ 3 lần (cấu hình được) thì bắt thanh toán toàn bộ trước, hoặc khóa đặt 30 ngày. Admin có danh sách khách vi phạm và có thể gỡ hạn chế. |
| QT13 | Giá ca = giá theo (loại ngày: thường/cuối tuần) × (loại ca: cao điểm/thấp điểm) của sân hoặc loại sân; cộng dịch vụ; trừ giảm giá (nếu có). |
| QT14 | Đổi sân/đổi ca, gia hạn: chỉ khi ca đích còn trống; tính chênh lệch giá và tiền cọc, ghi lịch sử thay đổi. |
| QT15 | Khóa ca (bảo trì, giải đấu) khi đã có đơn: hệ thống liệt kê đơn bị ảnh hưởng, buộc đổi lịch hoặc hoàn tiền trước khi khóa. |
| QT16 | Trạng thái sân: thủ công (đang dọn dẹp) ghi đè trạng thái tự động đến khi ca kế tiếp bắt đầu. |
| QT17 | Khiếu nại: chi nhánh phản hồi trước; khách không đồng ý thì chuyển lên Admin quyết định. |
| QT18 | Mọi thao tác quan trọng ghi audit log (ai, làm gì, lúc nào, giá trị trước/sau, người thực hiện). |

## 3. Vòng đời đơn đặt sân

### 3.1 Luồng chính

Chọn chi nhánh → sân → ngày → 1..n ca liên tiếp → xem tổng tiền và tiền cọc → bấm đặt → **Đang giữ chỗ** → **Đã cọc/Xác nhận** → **Đã check-in** → **Hoàn thành**.

Nhánh hủy: Hết hạn cọc, Đã hủy (khách/quản lý/Admin), No-show.

### 3.2 Hai trường trạng thái riêng

- **Trạng thái đơn:** Đang giữ chỗ, Đã cọc, Đã check-in, Hoàn thành, Đã hủy, Hết hạn cọc, No-show.
- **Trạng thái thanh toán:** Chưa thanh toán, Đã cọc, Đã thanh toán đủ, Chờ hoàn, Đã hoàn.

Tách riêng vì khách có thể đã check-in nhưng còn nợ phần còn lại, hoặc đơn đã hủy nhưng tiền hoàn chưa xử lý xong.

### 3.3 Bảng chuyển trạng thái

| Từ | Sang | Kích hoạt | Điều kiện | Việc kèm theo |
| --- | --- | --- | --- | --- |
| (mới) | Đang giữ chỗ | Khách bấm đặt | Ca còn trống, khách không bị khóa đặt | Khóa ca, lưu giá snapshot, `het_han_luc = now + 15'` |
| Đang giữ chỗ | Đã cọc | Callback cổng thanh toán | Đúng số tiền, còn hạn | Ghi giao dịch cọc, gửi email kèm QR |
| Đang giữ chỗ | Hết hạn cọc | Job mỗi phút | `now > het_han_luc` | Nhả ca, gửi email |
| Đang giữ chỗ | Đã hủy | Khách | Không cần lý do | Nhả ca |
| Đã cọc | Đã check-in | Quản lý quét QR/nhập mã | Từ giờ bắt đầu đến +15' | Ghi người thực hiện, giờ check-in, sân sang "Đang sử dụng" |
| Đã cọc | No-show | Job (hoặc thủ công nếu job lỗi) | Quá +15' chưa check-in | Nhả ca, tăng đếm no-show, mất cọc, gửi email |
| Đã cọc | Đã hủy | Khách/Quản lý/Admin | Khách: theo mốc chính sách. Quản lý/Admin: bắt buộc lý do | Tính hoàn tiền, ghi giao dịch hoàn, nhả ca, gửi email |
| Đã check-in | Hoàn thành | Job | Hết giờ ca cuối | Sân về "Trống/Đang dọn", mở quyền đánh giá, gửi email mời đánh giá |

### 3.4 Thao tác không đổi trạng thái

Gia hạn thêm ca, đổi sân/ca, thêm dịch vụ phát sinh, thanh toán phần còn lại: chỉ thay đổi dòng ca hoặc hóa đơn, ghi lịch sử thay đổi.

## 4. Chức năng Admin

### 4.1 Quản lý chi nhánh và danh mục

- &#91;L\] Xem danh sách, chi tiết, lọc/tìm tất cả chi nhánh.
- &#91;L\] Duyệt/từ chối đăng ký mở chi nhánh mới (kèm lý do từ chối, gửi email).
- &#91;L\] Khóa/mở chi nhánh vi phạm (chi nhánh bị khóa không nhận đơn mới, đơn cũ được xử lý theo QT15).
- &#91;L\] Quản lý danh mục loại sân toàn hệ thống (thêm/sửa/xóa mềm: bóng đá, cầu lông, pickleball, tennis, bóng rổ...).

### 4.2 Người dùng và phân quyền

- &#91;L\] Quản lý tài khoản toàn hệ thống: xem, khóa/mở, đặt lại mật khẩu.
- &#91;L\] Phân quyền theo vai trò (Admin, Quản lý chi nhánh, Khách hàng); chặn ở tầng API, không chỉ ẩn nút.
- &#91;T\] Danh sách khách vi phạm (hay đặt rồi không đến), gỡ hạn chế.

### 4.3 Chính sách và cấu hình

- &#91;L\] Chính sách chung: thời gian giữ cọc (mặc định 15 phút), thời gian chờ check-in (mặc định 15 phút), tỷ lệ đặt cọc, khung chính sách hoàn cọc cho phép, ngưỡng no-show gây hạn chế.
- &#91;L\] Cấu hình hệ thống: SMTP email, thông tin thanh toán, khóa API AI (mã hóa khi lưu).
- &#91;L\] Xem log hoạt động (lọc theo người dùng, hành động, thời gian, chi nhánh).
- &#91;M\] Sao lưu/phục hồi dữ liệu.

### 4.4 Đơn và khiếu nại

- &#91;L\] Hủy đơn bất kỳ (bắt buộc lý do, ghi log, email khách, tính hoàn tiền).
- &#91;T\] Duyệt xử lý khiếu nại giữa Khách hàng và Chi nhánh (trạng thái: Mới, Đang xử lý, Đã giải quyết; xem ảnh bằng chứng; ghi kết luận và gửi kết quả cho khách).

### 4.5 Dashboard và báo cáo

- &#91;L\] Dashboard toàn hệ thống: doanh thu toàn chuỗi, chi nhánh đông khách nhất, khung giờ cao điểm, tỷ lệ lấp đầy trung bình.
- &#91;L\] Thống kê tỷ lệ hủy và tỷ lệ no-show theo chi nhánh.
- &#91;L\] So sánh hiệu quả giữa các chi nhánh (doanh thu, tỷ lệ lấp đầy, điểm đánh giá).
- &#91;L\] Xuất báo cáo doanh thu toàn chuỗi theo năm/quý/tháng (Excel/PDF), so sánh kỳ trước.
- &#91;T\] Heatmap thứ × giờ toàn hệ thống.

### 4.6 Liên lạc

- &#91;L\] Chat hỗ trợ các chi nhánh.
- &#91;L\] Gửi thông báo chung đến toàn bộ chi nhánh (hiển thị chuông thông báo trong app và email).

## 5. Chức năng Quản lý chi nhánh

### 5.1 Nhóm Quản lý (hành động nhạy cảm, yêu cầu nhập lại mật khẩu)

**Chi nhánh và sân**

- &#91;L\] Đăng ký mở chi nhánh mới (chờ Admin duyệt): tên, địa chỉ, tọa độ bản đồ, hình ảnh, tiện ích, giờ mở/đóng.
- &#91;L\] Cấu hình giờ mở/đóng cửa; hệ thống tự sinh các ca 1 tiếng.
- &#91;L\] CRUD sân: tên sân, loại sân, hình ảnh, mô tả; xóa mềm.
- &#91;L\] Quản lý trạng thái sân: Đang hoạt động / Đang bảo trì / Ngừng cho thuê.
- &#91;L\] Cấu hình giá theo ca (cao điểm/thấp điểm) và theo ngày (thường/cuối tuần).
- &#91;T\] Khóa ca theo khoảng thời gian (bảo trì định kỳ, giải đấu nội bộ), kiểm tra đơn bị ảnh hưởng (QT15).
- &#91;L\] Cấu hình chính sách hủy/hoàn cọc riêng của chi nhánh (trong khung Admin cho phép).

**Dịch vụ và thiết bị**

- &#91;T\] Quản lý dịch vụ đi kèm và giá (thuê vợt, bóng, áo bib, nước uống, thuê giày), bật/tắt, xóa mềm.
- &#91;T\] Quản lý thiết bị/cơ sở vật chất của sân và tình trạng (đèn, lưới, bảng điểm...). Sự cố gắn với thiết bị (gộp chung module thiết bị và sự cố).
- &#91;T\] Khai báo danh sách tên nhân viên thao tác (để ghi vào trường Người thực hiện).

**Báo cáo và phản hồi**

- &#91;L\] Dashboard chi nhánh: doanh thu theo ngày/tháng, ca đặt nhiều nhất, sân được thuê nhiều nhất.
- &#91;L\] Báo cáo doanh thu chi nhánh theo năm/quý/tháng, xuất Excel/PDF.
- &#91;T\] Báo cáo tỷ lệ lấp đầy theo từng sân/ca.
- &#91;L\] Xem log các đơn bị tự hủy để đánh giá tỷ lệ vắng mặt.
- &#91;T\] Trả lời đánh giá của khách; xử lý khiếu nại liên quan chi nhánh (phản hồi trước, khách không đồng ý thì lên Admin).
- &#91;T\] Heatmap thứ × giờ của chi nhánh.

### 5.2 Nhóm Vận hành (dùng hằng ngày tại quầy)

**Lịch và đặt sân**

- &#91;L\] Xem lịch đặt dạng lưới theo ngày (giống Google Calendar): ca trống / đang giữ / đã đặt / khóa; cập nhật realtime.
- &#91;L\] Nhận đặt sân trực tiếp tại quầy cho khách vãng lai (tìm khách theo SĐT, tạo hồ sơ nếu chưa có).
- &#91;L\] Danh sách đơn sắp đến giờ và đơn sắp bị tự hủy.
- &#91;L\] Tra cứu khách hàng: họ tên, SĐT, lịch sử đặt, số lần no-show.

**Check-in và trạng thái**

- &#91;L\] Xác nhận khách đến bằng quét QR hoặc nhập mã đơn (từ giờ bắt đầu đến +15'). QR có chữ ký, chống dùng lại.
- &#91;L\] Cập nhật trạng thái sân thời gian thực: đang sử dụng / trống / đang dọn dẹp.
- &#91;T\] Đánh dấu no-show thủ công (ngoại lệ khi job lỗi), giải phóng ca.

**Thanh toán và hóa đơn**

- &#91;L\] Xác nhận thanh toán tại quầy: tiền mặt / chuyển khoản / quẹt thẻ.
- &#91;T\] Ghi nhận dịch vụ phát sinh trong buổi chơi (thuê vợt, mua nước), cộng vào hóa đơn.
- &#91;T\] In/xuất hóa đơn thanh toán (PDF).
- &#91;T\] Chốt doanh thu theo ngày (tiền mặt/chuyển khoản/thẻ), đối soát.

**Thay đổi đơn**

- &#91;T\] Gia hạn thêm ca ngay tại sân (nếu ca kế tiếp còn trống).
- &#91;T\] Đổi sân/đổi ca khi sân gặp sự cố hoặc khách yêu cầu (nếu còn trống), xử lý chênh lệch giá.
- &#91;L\] Hủy đơn khi cần (bắt buộc lý do, tự gửi email cho khách).
- &#91;T\] Xử lý đơn đặt lịch định kỳ: tạm dừng, chấm dứt, hủy một buổi lẻ.

**Sự cố**

- &#91;L\] Ghi nhận sự cố sân (hỏng đèn, mặt sân xuống cấp...), đính kèm ảnh, tự chuyển sân sang "Đang bảo trì" khi cần.
- &#91;T\] Khi sân bảo trì đột xuất: tự liệt kê đơn bị ảnh hưởng, đề xuất đổi sân hoặc hoàn tiền.

**Thông báo**

- &#91;L\] Nhận email và thông báo trong app khi có đơn mới, đơn bị hủy.

## 6. Chức năng Khách hàng

### 6.1 Tài khoản

- &#91;L\] Đăng ký/đăng nhập, xác thực email, quản lý thông tin cá nhân, đổi mật khẩu, quên mật khẩu.

### 6.2 Tìm kiếm và xem thông tin

- &#91;L\] Xem danh sách chi nhánh trên bản đồ, tìm chi nhánh gần nhất, chỉ đường.
- &#91;L\] Xem chi tiết chi nhánh: hình ảnh, tiện ích, bảng giá, giờ mở cửa, đánh giá.
- &#91;L\] Tìm sân theo bộ lọc: loại sân, khu vực, khoảng giá, ca còn trống, điểm đánh giá.
- &#91;L\] Xem lưới ca của từng sân theo ngày (trống / đang được giữ / đã đặt).
- &#91;M\] Lưu chi nhánh/sân yêu thích để đặt nhanh.

### 6.3 Đặt sân và thanh toán

- &#91;L\] Đặt sân theo ca 1 tiếng: chi nhánh → sân → ngày → một hoặc nhiều ca liên tiếp.
- &#91;L\] Xem trước tổng tiền và số tiền cọc trước khi xác nhận.
- &#91;L\] Đặt cọc giữ chỗ trong 15 phút (đồng hồ đếm ngược); quá hạn đơn tự hủy.
- &#91;L\] Thanh toán toàn bộ online hoặc thanh toán phần còn lại tại sân (cổng sandbox VNPay/MoMo hoặc VietQR, nhận kết quả bằng callback/webhook; nếu không tích hợp thật thì có màn hình giả lập cổng thanh toán).
- &#91;T\] Đặt thêm dịch vụ đi kèm khi đặt sân.
- &#91;T\] Đặt lịch định kỳ (cố định 1 ca mỗi tuần), xem mục 8.
- &#91;T\] Nhập mã giảm giá.

### 6.4 Sau khi đặt

- &#91;L\] Nhận vé điện tử có mã QR để check-in.
- &#91;L\] Xem lịch sử các lần đã đặt và trạng thái từng đơn.
- &#91;L\] Hủy đặt sân, xem trước số tiền được hoàn theo chính sách.
- &#91;T\] Đổi lịch sang ca/sân khác (nếu còn trống và trong hạn cho phép).
- &#91;T\] Đánh giá sân/chi nhánh sau khi chơi xong (chỉ khách có đơn Hoàn thành).
- &#91;T\] Gửi khiếu nại về chi nhánh (kèm ảnh bằng chứng) và theo dõi kết quả.
- &#91;L\] Chat với Quản lý chi nhánh hoặc trợ lý AI.
- &#91;M\] Đăng ký danh sách chờ khi ca đã kín, được báo khi có người hủy.

## 7. Trợ lý AI

Kỹ thuật: gọi mô hình AI kèm các hàm truy vấn CSDL (tool/function calling hoặc RAG), khóa API do Admin cấu hình.

- &#91;L\] Trả lời giá theo ca, giờ mở cửa, chính sách hủy/hoàn cọc, địa chỉ chi nhánh.
- &#91;L\] Tra cứu ca trống theo yêu cầu (ví dụ "tối mai còn sân cầu lông nào không?"), lấy dữ liệu thật từ DB.
- &#91;L\] Gợi ý chi nhánh, sân, ca phù hợp theo loại sân, khu vực, khoảng giá.
- &#91;L\] Tra cứu đơn của chính khách đang chat (trạng thái, giờ chơi, tiền cọc).
- &#91;M\] Hỗ trợ đặt sân qua chat (bước cuối khách vẫn bấm xác nhận trên giao diện).
- &#91;L\] Chuyển tiếp sang Quản lý chi nhánh khi không xử lý được hoặc khách yêu cầu.
- &#91;L\] Không tiết lộ thông tin của khách hàng khác (truy vấn luôn ràng buộc theo `user_id` của người đang chat).

## 8. Quy tắc đặt lịch định kỳ

- Khách chọn 1 ca cố định mỗi tuần (ví dụ tối thứ 3), số tuần hoặc không giới hạn.
- Hệ thống sinh cuốn chiếu 4 tuần phía trước, job mỗi tuần sinh thêm.
- Tuần bị trùng: báo cho khách, cho chọn bỏ qua tuần đó hoặc đổi sân/ca.
- Hủy một buổi lẻ không hủy cả chuỗi; Quản lý có thể tạm dừng hoặc chấm dứt chuỗi.
- Cọc theo từng buổi (mỗi buổi có thời hạn giữ chỗ và hoàn cọc riêng).
- &#91;M\] Giảm giá cho khách đặt định kỳ.

## 9. Thông báo

### 9.1 Email

- Đặt sân thành công (ca, sân, số tiền, mã QR).
- Nhắc đặt cọc (còn 5 phút giữ chỗ).
- Xác nhận đã nhận cọc.
- Nhắc trước giờ chơi (trước 2 giờ và trước 30 phút).
- Đơn bị hủy do quá hạn cọc.
- Đơn bị hủy do không đến (no-show).
- Đơn bị hủy bởi Quản lý chi nhánh/Admin (kèm lý do và số tiền hoàn).
- Đổi lịch thành công.
- Mời đánh giá sau khi chơi xong.
- Kết quả xử lý khiếu nại.
- Cho chi nhánh: đơn mới, đơn bị hủy.
- &#91;M\] Thông báo có ca trống cho khách trong danh sách chờ.

### 9.2 Thông báo trong app

Chuông thông báo cho mọi sự kiện trên (rẻ công, demo đẹp), dùng chung cho thông báo của Admin gửi chi nhánh. Gửi email chạy nền, không làm đứng giao diện.

## 10. Tác vụ chạy nền (job)

| Job | Chu kỳ | Việc làm |
| --- | --- | --- |
| Hết hạn giữ chỗ | Mỗi phút | Hủy đơn chưa cọc quá 15', nhả ca, gửi email |
| No-show | Mỗi phút | Hủy đơn đã cọc chưa check-in quá +15', tăng đếm vi phạm |
| Nhắc cọc | Mỗi phút | Gửi email khi còn 5' giữ chỗ |
| Nhắc lịch chơi | Mỗi phút | Email trước 2h và 30' |
| Cập nhật trạng thái sân | Mỗi phút | Sắp tới giờ / đang chơi / hết giờ theo lịch (tôn trọng QT16) |
| Hoàn thành đơn | Mỗi phút | Đơn đã check-in hết giờ ca cuối chuyển Hoàn thành, gửi mời đánh giá |
| Sinh lượt định kỳ | Hằng ngày/tuần | Sinh thêm lượt đặt cho lịch định kỳ |
| Ghi log | Liên tục | Ghi audit log mọi thao tác quan trọng |

## 11. Chức năng điểm nhấn (chọn làm kỹ)

1. &#91;T\] **Realtime** (WebSocket/SignalR/SSE): lưới ca và trạng thái sân cập nhật tức thì giữa khách và quản lý, demo hai màn hình cùng lúc.
2. &#91;T\] **Heatmap thứ × giờ** trên dashboard.
3. &#91;T\] **Gợi ý khi ca kín:** đề xuất ca/sân trống gần nhất hoặc chi nhánh khác, mời vào danh sách chờ.
4. &#91;M\] **Giá động rule-based:** tăng giá khi lấp đầy cao, giảm giá khung giờ ế.
5. &#91;M\] Mã giảm giá, tích điểm hạng thành viên, ghép kèo/chia tiền sân nhóm, dự báo doanh thu bằng trung bình động.
6. &#91;T\] Seed data mẫu để dashboard demo không trống; giao diện responsive, dark mode.

## 12. Yêu cầu phi chức năng và kỹ thuật

- **Chống trùng ca:** unique `(san_id, ngay, ca_id)` trên `booking_slot` cho đơn đang hiệu lực + transaction + khóa bản ghi.
- **Phân quyền RBAC:** kiểm tra vai trò và `branch_id` ở API; quản lý chi nhánh A không xem/sửa được dữ liệu chi nhánh B dù biết ID.
- **Bảo mật:** băm mật khẩu (bcrypt/argon2), JWT + refresh token, chống SQL injection/XSS/CSRF, mã hóa khóa SMTP/API AI, kiểm tra chữ ký callback thanh toán, QR có chữ ký và hết hạn.
- **Hiệu năng:** lưới ca truy vấn theo ngày có index `(san_id, ngay)`; email gửi bất đồng bộ.
- **Dữ liệu:** CSDL chuẩn hóa (3NF), xóa mềm, lưu snapshot giá.
- **Kiểm thử:** unit test tính giá, kiểm tra trùng ca, tính hoàn tiền, chuyển trạng thái.
- **Triển khai:** Docker/docker-compose, tài liệu cài đặt, seed data.
- **Tài liệu báo cáo:** ERD, use case, sơ đồ trạng thái đơn, sequence diagram cho luồng đặt sân + thanh toán + check-in.

## 13. Mô hình dữ liệu gợi ý

| Nhóm | Bảng chính |
| --- | --- |
| Người dùng | `users`, `roles`, `branch_staff_names` (tên nhân viên thao tác), `customer_violations` |
| Chi nhánh | `branches` (trạng thái duyệt/khóa), `branch_images`, `amenities`, `branch_amenities`, `branch_policies` |
| Sân | `court_types`, `courts` (trạng thái), `time_slots`, `price_rules` (loại sân/sân × loại ngày × loại ca), `slot_blocks` (khóa ca) |
| Đơn | `bookings` (trạng thái đơn, trạng thái thanh toán, het\_han\_luc, người thực hiện, loai\_huy, ly\_do), `booking_slots` (sân, ngày, ca, giá snapshot), `booking_history` (đổi sân/ca, gia hạn), `recurring_bookings` |
| Thanh toán | `payments` (cọc/thanh toán/hoàn/phát sinh, phương thức, mã giao dịch cổng, trạng thái), `invoices`, `invoice_items` |
| Dịch vụ | `services`, `booking_services` |
| Thiết bị, sự cố | `equipment`, `incidents` |
| Tương tác | `reviews`, `review_replies`, `complaints`, `complaint_messages`, `chat_threads`, `chat_messages` |
| Hệ thống | `notifications`, `email_queue`, `system_settings`, `audit_logs` |
| Mở rộng | `waitlist`, `favorites`, `vouchers`, `loyalty_points` |

## 14. Ma trận phân quyền

| Chức năng | Admin | Quản lý chi nhánh | Khách hàng |
| --- | --- | --- | --- |
| Duyệt/khóa chi nhánh, danh mục loại sân, cấu hình hệ thống | Có | Không | Không |
| Quản lý tài khoản người dùng | Có | Không | Không |
| CRUD sân, giá, dịch vụ, thiết bị | Không | Chi nhánh mình | Không |
| Xem lịch lưới | Xem tất cả | Chi nhánh mình | Xem công khai (không thấy thông tin người đặt) |
| Đặt sân | Không | Tại quầy (khách vãng lai) | Có |
| Check-in, thanh toán tại quầy | Không | Chi nhánh mình | Không |
| Hủy đơn | Bất kỳ (có lý do) | Chi nhánh mình (có lý do) | Đơn của mình (theo chính sách) |
| Dashboard/báo cáo | Toàn chuỗi | Chi nhánh mình | Không |
| Khiếu nại | Quyết định cuối | Phản hồi trước | Gửi và theo dõi |
| Đánh giá | Xem | Trả lời | Viết (sau khi chơi) |
| Chat | Với chi nhánh | Với khách, với Admin | Với chi nhánh, trợ lý AI |
| Audit log | Xem tất cả | Không | Không |

## 15. Lộ trình ưu tiên

1. **Giai đoạn 1 (nền tảng):** tài khoản + phân quyền, chi nhánh/sân/ca/giá, lưới ca, seed data.
2. **Giai đoạn 2 (lõi đặt sân):** giữ chỗ + chống trùng, cọc + cổng thanh toán, QR check-in, hủy/hoàn, job hết hạn và no-show, email, audit log.
3. **Giai đoạn 3 (quản trị và thống kê):** dashboard, báo cáo xuất Excel/PDF, tỷ lệ hủy/no-show, so sánh chi nhánh, trợ lý AI.
4. **Giai đoạn 4 (mở rộng):** định kỳ, gia hạn, đổi sân/ca, dịch vụ phát sinh, hóa đơn PDF, khiếu nại, đánh giá, thông báo trong app.
5. **Giai đoạn 5 (điểm nhấn):** realtime, heatmap, gợi ý khi ca kín, giá động, danh sách chờ.

*Gợi ý: nếu thiếu thời gian, cắt từ giai đoạn 5 ngược lên; giai đoạn 1 đến 3 là phần bắt buộc để đồ án chạy trọn vẹn.*

## 16. Thiết kế bảng CSDL chi tiết

Phần này thay thế mục 13 (chỉ liệt kê tên bảng) bằng thiết kế đầy đủ cột, kiểu, ràng buộc. Có 40 bảng chia thành 9 nhóm. Bảng đánh dấu \[M\] là mở rộng, có thể bỏ.

### 16.1 Quy ước chung

- **Khóa chính:** `id` BIGINT tự tăng (hoặc UUID). Khóa ngoại đặt tên `<bảng>_id`.
- **Thời gian:** DATETIME lưu UTC (hoặc giờ địa phương thống nhất toàn hệ thống). Mọi bảng có `created_at`, `updated_at`.
- **Xóa mềm:** các bảng có cột `deleted_at` (NULL = còn dùng). Truy vấn mặc định lọc `deleted_at IS NULL`.
- **Tiền:** DECIMAL(12,0), đơn vị VND, không dùng FLOAT.
- **Trạng thái:** dùng ENUM hoặc VARCHAR kèm CHECK, giá trị liệt kê ngay trong cột Ghi chú.
- **Snapshot:** các cột có hậu tố `_snapshot` lưu giá trị tại thời điểm đặt, không đổi khi dữ liệu gốc đổi (QT10).
- **Người thực hiện:** các bảng thao tác tại quầy có `performed_by_name` (tên nhân viên, vì dùng chung tài khoản) bên cạnh `user_id` (tài khoản).

**Chuỗi tiếng Việt dùng NVARCHAR (bổ sung):** mọi cột chứa chữ có dấu (tên, địa chỉ, mô tả, lý do, nội dung chat, email) phải là `NVARCHAR(n)` hoặc `NVARCHAR(MAX)`, không dùng `VARCHAR`. Trong các bảng 16.2 đến 16.10 bên dưới, ký hiệu `VARCHAR(n)` ở cột có chữ tiếng Việt được hiểu là `NVARCHAR(n)`; danh sách chính xác từng cột và bảng quy đổi kiểu nằm ở mục 16.14. Cột chỉ chứa ký tự ASCII (mã, email, URL, băm, trạng thái) giữ `VARCHAR` để tiết kiệm dung lượng.

### 16.2 Nhóm A: Người dùng và xác thực

**`roles`**: danh mục vai trò

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| code | VARCHAR(30) | UNIQUE. ADMIN, BRANCH\_MANAGER, CUSTOMER |
| name | VARCHAR(100) | Tên hiển thị |

**`users`**: tài khoản (gồm cả khách vãng lai tạo tại quầy)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| role\_id | FK roles |  |
| branch\_id | FK branches, NULL | Chỉ có với Quản lý chi nhánh; dùng để chặn dữ liệu chéo chi nhánh |
| email | VARCHAR(150) | UNIQUE, NULL với khách vãng lai |
| phone | VARCHAR(20) | Index. Dùng tìm khách tại quầy |
| password\_hash | VARCHAR(255) | NULL với khách vãng lai chưa có tài khoản |
| full\_name | VARCHAR(150) |  |
| avatar\_url | VARCHAR(255) |  |
| is\_walk\_in | BOOLEAN | Khách vãng lai do quầy tạo, về sau có thể kích hoạt tài khoản |
| status | ENUM | ACTIVE, LOCKED |
| lock\_reason | VARCHAR(255) |  |
| email\_verified\_at | DATETIME | NULL = chưa xác thực |
| no\_show\_count | INT | Số lần no-show tích lũy (QT12) |
| booking\_restricted\_until | DATETIME | Khóa đặt đến thời điểm này |
| require\_full\_prepay | BOOLEAN | Bắt thanh toán toàn bộ trước |
| last\_login\_at | DATETIME |  |
| deleted\_at | DATETIME | Xóa mềm |

**`auth_tokens`**: refresh token, đặt lại mật khẩu, xác thực email

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| user\_id | FK users |  |
| type | ENUM | REFRESH, RESET\_PASSWORD, VERIFY\_EMAIL |
| token\_hash | VARCHAR(255) | Chỉ lưu bản băm, không lưu token gốc |
| expires\_at | DATETIME |  |
| used\_at | DATETIME | NULL = chưa dùng; token dùng một lần |
| revoked\_at | DATETIME | Thu hồi khi đăng xuất/đổi mật khẩu |

**`branch_staff_names`**: danh sách tên nhân viên thao tác (không có tài khoản)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches |  |
| full\_name | VARCHAR(100) | Hiện trong dropdown khi thanh toán, check-in |
| is\_active | BOOLEAN |  |

**`customer_violations`**: lịch sử vi phạm của khách

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| user\_id | FK users | Khách vi phạm |
| booking\_id | FK bookings | Đơn gây vi phạm |
| type | ENUM | NO\_SHOW, LATE\_CANCEL |
| action\_taken | ENUM | NONE, REQUIRE\_PREPAY, RESTRICT\_BOOKING |
| resolved\_by | FK users, NULL | Admin gỡ hạn chế |
| resolved\_at | DATETIME |  |

### 16.3 Nhóm B: Chi nhánh và chính sách

**`branches`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| owner\_user\_id | FK users | Tài khoản Quản lý đăng ký chi nhánh |
| name | VARCHAR(150) |  |
| address | VARCHAR(255) |  |
| district / city | VARCHAR(100) | Dùng cho bộ lọc khu vực |
| latitude / longitude | DECIMAL(10,7) | Hiển thị bản đồ, tìm chi nhánh gần nhất |
| phone | VARCHAR(20) |  |
| description | TEXT |  |
| open\_time / close\_time | TIME | Giờ mở/đóng cửa, sinh ra các ca |
| status | ENUM | PENDING, ACTIVE, REJECTED, LOCKED |
| reject\_reason / lock\_reason | VARCHAR(255) | Bắt buộc khi từ chối/khóa |
| approved\_by | FK users | Admin duyệt |
| approved\_at | DATETIME |  |
| avg\_rating | DECIMAL(2,1) | Cột tổng hợp, cập nhật khi có đánh giá |
| deleted\_at | DATETIME |  |

**`branch_images`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches |  |
| url | VARCHAR(255) |  |
| sort\_order | INT |  |
| is\_cover | BOOLEAN | Ảnh đại diện |

**`amenities`** và **`branch_amenities`**: tiện ích (bãi xe, phòng thay đồ, wifi, căng tin...)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| amenities.id, name, icon |  | Danh mục do Admin quản lý |
| branch\_amenities.branch\_id, amenity\_id | PK kép | Bảng nối nhiều-nhiều |

**`branch_policies`**: chính sách riêng của chi nhánh (trong khung Admin cho phép)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches | UNIQUE, mỗi chi nhánh một dòng |
| deposit\_percent | INT | Tỷ lệ cọc (%), phải nằm trong khoảng Admin cho phép |
| allow\_change\_booking | BOOLEAN | Cho khách đổi lịch |
| change\_before\_hours | INT | Đổi lịch trước tối thiểu bao nhiêu giờ |
| max\_hold\_extend | INT | Dự phòng, mặc định 0 |

**`cancel_policy_tiers`**: các mốc hoàn cọc khi hủy

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches, NULL | NULL = mốc mặc định toàn hệ thống do Admin đặt |
| hours\_before | INT | Hủy khi còn ít nhất N giờ đến giờ bắt đầu |
| refund\_percent | INT | % tiền cọc hoàn lại (0 đến 100) |
| sort\_order | INT | Mốc lớn nhất trước. Ví dụ (24h, 100%), (12h, 50%), (0h, 0%) |

### 16.4 Nhóm C: Sân, ca và giá

**`court_types`**: loại sân toàn hệ thống do Admin quản lý

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| name | VARCHAR(100) | Bóng đá, cầu lông, pickleball, tennis, bóng rổ... UNIQUE |
| icon | VARCHAR(100) |  |
| description | VARCHAR(255) |  |
| is\_active | BOOLEAN |  |
| deleted\_at | DATETIME |  |

**`courts`**: sân của chi nhánh

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches | Index, luôn lọc theo cột này ở API |
| court\_type\_id | FK court\_types |  |
| name | VARCHAR(100) | UNIQUE trong cùng chi nhánh |
| description | TEXT |  |
| image\_url | VARCHAR(255) |  |
| capacity | INT | Số người tối đa |
| status | ENUM | ACTIVE, MAINTENANCE, INACTIVE (cấu hình, ảnh hưởng đặt sân) |
| live\_status | ENUM | AVAILABLE, IN\_USE, CLEANING (vận hành theo thời gian thực) |
| live\_status\_manual\_until | DATETIME | Trạng thái thủ công ghi đè tự động đến thời điểm này (QT16) |
| deleted\_at | DATETIME |  |

**`time_slots`**: ca 1 tiếng của chi nhánh, sinh từ giờ mở/đóng

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches |  |
| slot\_index | INT | Thứ tự ca trong ngày; UNIQUE (branch\_id, slot\_index) |
| start\_time / end\_time | TIME | end\_time = start\_time + 1 giờ |
| slot\_type | ENUM | PEAK, OFFPEAK. Chi nhánh gán từng ca |
| is\_active | BOOLEAN | Đổi giờ mở/đóng thì ca cũ chuyển false, không xóa vì còn đơn tham chiếu |

**`price_rules`**: giá ca theo loại ngày và loại ca

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches |  |
| court\_type\_id | FK court\_types, NULL | Giá áp cho cả loại sân |
| court\_id | FK courts, NULL | Giá riêng cho một sân, ưu tiên cao hơn |
| day\_type | ENUM | WEEKDAY, WEEKEND |
| slot\_type | ENUM | PEAK, OFFPEAK |
| price | DECIMAL(12,0) | Giá 1 ca |
| valid\_from / valid\_to | DATE | Hiệu lực. Đổi giá thì thêm dòng mới, không sửa dòng cũ |

Quy tắc tra giá: ưu tiên dòng theo `court_id`, nếu không có thì theo `court_type_id`; lấy dòng có hiệu lực tại ngày chơi.

**`holidays`** \[M\]: ngày lễ phụ thu

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches, NULL | NULL = áp toàn hệ thống |
| holiday\_date | DATE |  |
| name | VARCHAR(100) |  |
| surcharge\_percent | INT | Phụ thu % |

**`slot_blocks`**: khóa ca (bảo trì định kỳ, giải đấu nội bộ)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| court\_id | FK courts |  |
| start\_date / end\_date | DATE | Khoảng ngày |
| start\_slot\_id / end\_slot\_id | FK time\_slots | Khoảng ca mỗi ngày (NULL = cả ngày) |
| block\_type | ENUM | MAINTENANCE, TOURNAMENT, OTHER |
| reason | VARCHAR(255) |  |
| performed\_by\_name | VARCHAR(100) |  |
| cancelled\_at | DATETIME | Gỡ khóa thì ghi thời điểm và xóa các dòng `slot_locks` tương ứng |

### 16.5 Nhóm D: Đơn đặt sân (trọng tâm đồ án)

**`bookings`**: đơn đặt sân

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| code | VARCHAR(20) | UNIQUE. Mã đơn ngắn, dùng nhập tay khi check-in và nằm trong QR |
| customer\_id | FK users |  |
| branch\_id | FK branches | Index. Tách ra để lọc nhanh theo chi nhánh |
| source | ENUM | ONLINE, COUNTER |
| status | ENUM | HOLDING, DEPOSITED, CHECKED\_IN, COMPLETED, CANCELLED, EXPIRED, NO\_SHOW |
| payment\_status | ENUM | UNPAID, DEPOSITED, PAID, REFUND\_PENDING, REFUNDED |
| play\_date | DATE | Ngày chơi |
| start\_at / end\_at | DATETIME | Giờ bắt đầu ca đầu, giờ kết thúc ca cuối. Index cho job quét |
| slot\_amount | DECIMAL(12,0) | Tổng tiền các ca (snapshot) |
| service\_amount | DECIMAL(12,0) | Tổng dịch vụ |
| discount\_amount | DECIMAL(12,0) | Giảm giá (voucher, định kỳ) |
| total\_amount | DECIMAL(12,0) | = slot + service - discount |
| deposit\_percent\_snapshot | INT | Tỷ lệ cọc tại thời điểm đặt |
| deposit\_amount | DECIMAL(12,0) | Tiền cọc phải trả |
| paid\_amount | DECIMAL(12,0) | Đã thu (cọc + thanh toán thêm) |
| refund\_amount | DECIMAL(12,0) | Số tiền hoàn khi hủy |
| cancel\_policy\_snapshot | JSON | Các mốc hoàn cọc tại thời điểm đặt |
| hold\_expires\_at | DATETIME | Index. Hạn giữ chỗ (now + 15 phút) |
| recurring\_id | FK recurring\_bookings, NULL | Đơn thuộc chuỗi định kỳ nào |
| voucher\_id | FK vouchers, NULL | \[M\] |
| qr\_secret | VARCHAR(64) | Khóa ký QR; QR chứa code + chữ ký + hạn, chống giả và dùng lại |
| checked\_in\_at | DATETIME |  |
| checkin\_method | ENUM | QR, CODE |
| checked\_in\_by\_name | VARCHAR(100) | Người thực hiện |
| completed\_at | DATETIME |  |
| cancel\_type | ENUM, NULL | CUSTOMER, MANAGER, ADMIN, EXPIRED, NO\_SHOW |
| cancel\_reason | VARCHAR(255) | Bắt buộc khi MANAGER/ADMIN hủy |
| cancelled\_at | DATETIME |  |
| cancelled\_by\_user\_id | FK users, NULL |  |
| cancelled\_by\_name | VARCHAR(100) |  |
| created\_by\_user\_id | FK users | Khách (online) hoặc Quản lý (tại quầy) |
| created\_by\_name | VARCHAR(100) | Tên nhân viên nhận đặt tại quầy |
| note | VARCHAR(255) |  |

**`booking_slots`**: từng ca thuộc đơn (đơn nhiều ca có nhiều dòng)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| booking\_id | FK bookings |  |
| court\_id | FK courts | Mỗi ca có thể ở sân khác nếu đổi sân giữa chừng |
| slot\_id | FK time\_slots |  |
| slot\_date | DATE |  |
| start\_at / end\_at | DATETIME |  |
| slot\_type\_snapshot | ENUM | PEAK, OFFPEAK tại thời điểm đặt |
| day\_type\_snapshot | ENUM | WEEKDAY, WEEKEND |
| price\_snapshot | DECIMAL(12,0) | Giá chốt của ca này (QT10) |
| status | ENUM | ACTIVE, RELEASED (nhả ca khi hủy/đổi), MOVED |

**`slot_locks`**: bảng chống trùng lịch (QT02), cốt lõi của hệ thống

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| court\_id | FK courts |  |
| slot\_date | DATE |  |
| slot\_id | FK time\_slots |  |
| source\_type | ENUM | BOOKING, BLOCK |
| booking\_slot\_id | FK booking\_slots, NULL | Có khi source là BOOKING |
| block\_id | FK slot\_blocks, NULL | Có khi source là BLOCK |
| locked\_at | DATETIME |  |

**Ràng buộc quan trọng nhất:** `UNIQUE (court_id, slot_date, slot_id)`. Có dòng nghĩa là ca đang bị chiếm. Khi hủy/hết hạn/no-show/đổi ca thì **xóa dòng** (lịch sử vẫn nằm ở `booking_slots` và `booking_history`). Đặt sân là một transaction: kiểm tra điều kiện, INSERT các dòng `slot_locks`, INSERT `bookings` và `booking_slots`. Nếu hai khách đặt cùng lúc, một bên vi phạm UNIQUE và nhận thông báo ca vừa bị người khác đặt. Cách này không phụ thuộc loại CSDL, không cần partial index.

**`booking_history`**: lịch sử thay đổi của đơn

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| booking\_id | FK bookings |  |
| action | ENUM | CREATE, PAY\_DEPOSIT, CHECKIN, EXTEND, CHANGE\_SLOT, CHANGE\_COURT, ADD\_SERVICE, CANCEL, EXPIRE, NO\_SHOW, COMPLETE |
| old\_value / new\_value | JSON | Trạng thái hoặc ca trước/sau |
| price\_diff | DECIMAL(12,0) | Chênh lệch giá khi đổi/gia hạn (QT14) |
| performed\_by\_user\_id | FK users, NULL | NULL nếu do job hệ thống |
| performed\_by\_name | VARCHAR(100) |  |
| created\_at | DATETIME |  |

**`recurring_bookings`**: lịch định kỳ

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| customer\_id | FK users |  |
| branch\_id | FK branches |  |
| court\_id | FK courts |  |
| weekday | TINYINT | 1 đến 7 |
| slot\_id | FK time\_slots | Ca cố định mỗi tuần |
| start\_date | DATE |  |
| end\_date | DATE, NULL | NULL = không giới hạn |
| weeks\_ahead | INT | Sinh cuốn chiếu trước bao nhiêu tuần, mặc định 4 |
| generated\_until | DATE | Đã sinh đơn đến ngày nào |
| discount\_percent | INT | \[M\] Giảm giá khách định kỳ |
| status | ENUM | ACTIVE, PAUSED, ENDED |

**`recurring_skips`**: tuần bị bỏ qua (do trùng lịch hoặc khách yêu cầu)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| recurring\_id | FK recurring\_bookings |  |
| skip\_date | DATE | UNIQUE (recurring\_id, skip\_date) |
| reason | ENUM | CONFLICT, CUSTOMER\_SKIP, BLOCKED |

### 16.6 Nhóm E: Thanh toán, hóa đơn, giảm giá

**`payments`**: sổ giao dịch tiền (không sửa, chỉ thêm dòng mới)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| booking\_id | FK bookings |  |
| type | ENUM | DEPOSIT, BALANCE, FULL, SERVICE, REFUND |
| method | ENUM | GATEWAY, CASH, TRANSFER, CARD |
| gateway | ENUM, NULL | VNPAY, MOMO, VIETQR, MOCK (cổng giả lập) |
| amount | DECIMAL(12,0) | Luôn dương; loại giao dịch xác định chiều tiền |
| status | ENUM | PENDING, SUCCESS, FAILED |
| gateway\_txn\_id | VARCHAR(100) | UNIQUE khi có; chống xử lý trùng callback (idempotent) |
| gateway\_payload | JSON | Dữ liệu callback gốc để đối soát |
| related\_payment\_id | FK payments, NULL | Giao dịch hoàn trỏ về giao dịch thu |
| received\_by\_name | VARCHAR(100) | Người thu tại quầy |
| paid\_at | DATETIME |  |
| note | VARCHAR(255) |  |

**`invoices`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| booking\_id | FK bookings | UNIQUE |
| invoice\_no | VARCHAR(30) | UNIQUE. Ví dụ HD-202610-000123 |
| total\_amount | DECIMAL(12,0) |  |
| issued\_at | DATETIME |  |
| issued\_by\_name | VARCHAR(100) |  |
| pdf\_path | VARCHAR(255) | Đường dẫn file PDF đã xuất |

**`invoice_items`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| invoice\_id | FK invoices |  |
| item\_type | ENUM | SLOT, SERVICE, DISCOUNT |
| description | VARCHAR(255) | Ví dụ: Sân 2, 18:00 đến 19:00, Thứ Ba 20/10 |
| quantity | INT |  |
| unit\_price | DECIMAL(12,0) |  |
| amount | DECIMAL(12,0) | Giảm giá ghi số âm |

**`daily_closings`**: chốt doanh thu theo ngày (thay cho đóng ca nhân viên)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches | UNIQUE (branch\_id, closing\_date) |
| closing\_date | DATE |  |
| cash\_total / transfer\_total / card\_total | DECIMAL(12,0) | Hệ thống tính từ `payments` |
| actual\_cash | DECIMAL(12,0) | Tiền mặt thực đếm |
| difference | DECIMAL(12,0) | actual\_cash trừ cash\_total |
| closed\_by\_name | VARCHAR(100) |  |
| note | VARCHAR(255) |  |

**`vouchers`** \[M\]

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| code | VARCHAR(30) | UNIQUE |
| branch\_id | FK branches, NULL | NULL = dùng toàn hệ thống |
| discount\_type | ENUM | PERCENT, FIXED |
| value | DECIMAL(12,0) |  |
| max\_discount / min\_order | DECIMAL(12,0) |  |
| usage\_limit / used\_count | INT |  |
| valid\_from / valid\_to | DATETIME |  |
| is\_active | BOOLEAN |  |

### 16.7 Nhóm F: Dịch vụ, thiết bị, sự cố

**`services`**: dịch vụ đi kèm của chi nhánh

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| branch\_id | FK branches |  |
| name | VARCHAR(100) | Thuê vợt, bóng, áo bib, nước uống, thuê giày |
| unit | VARCHAR(30) | Cái, chai, đôi... |
| price | DECIMAL(12,0) |  |
| stock\_qty | INT, NULL | \[M\] Tồn kho, NULL = không quản lý tồn |
| is\_active | BOOLEAN |  |
| deleted\_at | DATETIME |  |

**`booking_services`**: dịch vụ gắn vào đơn

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| booking\_id | FK bookings |  |
| service\_id | FK services |  |
| name\_snapshot | VARCHAR(100) |  |
| quantity | INT |  |
| unit\_price\_snapshot | DECIMAL(12,0) |  |
| amount | DECIMAL(12,0) | = quantity × unit\_price\_snapshot |
| added\_when | ENUM | AT\_BOOKING, ON\_SITE |
| added\_by\_name | VARCHAR(100) |  |
| added\_at | DATETIME |  |

**`equipment`**: thiết bị, cơ sở vật chất của sân

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| court\_id | FK courts |  |
| name | VARCHAR(100) | Đèn, lưới, bảng điểm... |
| status | ENUM | GOOD, NEEDS\_REPAIR, BROKEN |
| note | VARCHAR(255) |  |
| deleted\_at | DATETIME |  |

**`incidents`**: sự cố sân (gắn thiết bị)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| court\_id | FK courts |  |
| equipment\_id | FK equipment, NULL | Sự cố của thiết bị nào (nếu có) |
| description | TEXT |  |
| severity | ENUM | LOW, MEDIUM, HIGH |
| image\_url | VARCHAR(255) |  |
| reported\_by\_name | VARCHAR(100) |  |
| status | ENUM | OPEN, IN\_PROGRESS, RESOLVED |
| set\_maintenance | BOOLEAN | Có chuyển sân sang bảo trì không |
| resolved\_note | VARCHAR(255) |  |
| resolved\_at | DATETIME |  |

### 16.8 Nhóm G: Tương tác (đánh giá, khiếu nại, chat)

**`reviews`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| booking\_id | FK bookings | UNIQUE: mỗi đơn Hoàn thành đánh giá một lần |
| customer\_id | FK users |  |
| branch\_id / court\_id | FK |  |
| rating | TINYINT | 1 đến 5, CHECK |
| comment | TEXT |  |
| deleted\_at | DATETIME |  |

**`review_replies`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| review\_id | FK reviews | UNIQUE (một phản hồi chính thức mỗi đánh giá) |
| user\_id | FK users |  |
| content | TEXT |  |

**`complaints`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| code | VARCHAR(20) | UNIQUE |
| customer\_id | FK users |  |
| branch\_id | FK branches |  |
| booking\_id | FK bookings, NULL |  |
| title | VARCHAR(200) |  |
| content | TEXT |  |
| status | ENUM | NEW, BRANCH\_REPLIED, ESCALATED, RESOLVED, REJECTED (QT17) |
| escalated\_at | DATETIME | Khi khách không đồng ý và leo lên Admin |
| resolution | TEXT | Kết luận cuối |
| resolved\_by | FK users |  |
| resolved\_at | DATETIME |  |

**`complaint_messages`**: trao đổi qua lại trong khiếu nại

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| complaint\_id | FK complaints |  |
| sender\_id | FK users |  |
| sender\_role | ENUM | CUSTOMER, BRANCH, ADMIN |
| content | TEXT |  |
| attachment\_urls | JSON | Ảnh bằng chứng |

**`chat_threads`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| type | ENUM | CUSTOMER\_BRANCH, ADMIN\_BRANCH, CUSTOMER\_AI |
| customer\_id | FK users, NULL |  |
| branch\_id | FK branches, NULL |  |
| status | ENUM | OPEN, HANDED\_OFF (AI chuyển cho người), CLOSED |
| last\_message\_at | DATETIME |  |

**`chat_messages`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| thread\_id | FK chat\_threads | Index (thread\_id, created\_at) |
| sender\_type | ENUM | USER, AI, SYSTEM |
| sender\_id | FK users, NULL | NULL nếu AI |
| content | TEXT |  |
| meta | JSON | Ghi lại hàm DB mà AI đã gọi, để chứng minh dữ liệu thật |
| read\_at | DATETIME |  |

### 16.9 Nhóm H: Hệ thống

**`notifications`**: thông báo trong app (chuông)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| user\_id | FK users | Index (user\_id, is\_read) |
| type | VARCHAR(50) | BOOKING\_CREATED, BOOKING\_CANCELLED, BROADCAST... |
| title / body | VARCHAR / TEXT |  |
| link | VARCHAR(255) | Đường dẫn mở khi bấm |
| is\_read | BOOLEAN |  |

**`broadcasts`**: thông báo chung của Admin, mỗi lần gửi sinh một dòng ở đây và nhiều dòng ở `notifications`

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| sent\_by | FK users |  |
| title / content | VARCHAR / TEXT |  |
| target | ENUM | ALL\_BRANCHES, SELECTED |

**`email_queue`**: hàng đợi email gửi nền

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| to\_email | VARCHAR(150) |  |
| template | VARCHAR(50) | Tên mẫu email |
| subject | VARCHAR(200) |  |
| body | MEDIUMTEXT |  |
| status | ENUM | PENDING, SENT, FAILED |
| retry\_count | INT | Thử lại tối đa 3 lần |
| send\_after | DATETIME |  |
| sent\_at | DATETIME |  |
| error | VARCHAR(255) |  |

**`booking_reminders`**: đánh dấu đã nhắc, tránh gửi trùng khi job chạy lại

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| booking\_id | FK bookings | PK kép với type |
| type | ENUM | DEPOSIT\_5MIN, PLAY\_2H, PLAY\_30MIN, REVIEW\_INVITE |
| sent\_at | DATETIME |  |

**`system_settings`**: cấu hình hệ thống dạng khóa-giá trị

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| key | VARCHAR(100) PK |  |
| value | TEXT |  |
| is\_encrypted | BOOLEAN | TRUE với SMTP password, khóa API AI, khóa cổng thanh toán |
| description | VARCHAR(255) |  |
| updated\_by | FK users |  |

Các khóa cần có: `hold_minutes` (15), `checkin_grace_minutes` (15), `default_deposit_percent`, `deposit_percent_min/max`, `noshow_threshold` (3), `restrict_days` (30), `recurring_weeks_ahead` (4), `smtp_*`, `payment_gateway_*`, `ai_api_key`.

**`audit_logs`**: nhật ký hoạt động (QT18)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| actor\_user\_id | FK users, NULL | NULL nếu do job hệ thống |
| actor\_role | VARCHAR(30) |  |
| performed\_by\_name | VARCHAR(100) | Tên nhân viên thao tác tại quầy |
| branch\_id | FK branches, NULL | Để Admin lọc theo chi nhánh |
| action | VARCHAR(80) | UPDATE\_PRICE, CANCEL\_BOOKING, LOCK\_BRANCH... |
| entity\_type / entity\_id | VARCHAR / BIGINT | Đối tượng bị tác động |
| old\_value / new\_value | JSON | Giá trị trước/sau |
| ip / user\_agent | VARCHAR |  |
| created\_at | DATETIME | Index; chỉ thêm, không sửa, không xóa |

**`job_runs`** \[M\]: lịch sử chạy job, để demo trang theo dõi job

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| job\_name | VARCHAR(50) |  |
| started\_at / finished\_at | DATETIME |  |
| status | ENUM | SUCCESS, FAILED |
| affected\_rows | INT | Số đơn được xử lý |
| error | VARCHAR(255) |  |

### 16.10 Nhóm I: Mở rộng \[M\]

**`waitlist`**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | BIGINT PK |  |
| customer\_id | FK users |  |
| court\_id | FK courts |  |
| slot\_date / slot\_id | DATE / FK | Ca đang chờ |
| status | ENUM | WAITING, NOTIFIED, BOOKED, EXPIRED |
| notified\_at | DATETIME |  |

**`favorites`**: `customer_id`, `branch_id`, `court_id` (NULL nếu yêu thích cả chi nhánh); UNIQUE theo bộ ba.

### 16.11 Quan hệ chính giữa các bảng

- `branches` 1-n `courts`, `time_slots`, `price_rules`, `services`, `branch_staff_names`; `branches` 1-1 `branch_policies`.
- `court_types` 1-n `courts`; `courts` 1-n `equipment`, `incidents`, `slot_blocks`.
- `users` (khách) 1-n `bookings`; `bookings` 1-n `booking_slots`, `booking_services`, `payments`, `booking_history`; `bookings` 1-1 `invoices`, `reviews`.
- `booking_slots` 1-1 `slot_locks` (khi đang hiệu lực); `slot_blocks` 1-n `slot_locks`.
- `recurring_bookings` 1-n `bookings`, 1-n `recurring_skips`.
- `complaints` 1-n `complaint_messages`; `reviews` 1-1 `review_replies`.
- `chat_threads` 1-n `chat_messages`; `users` 1-n `notifications`.

### 16.12 Chỉ mục và ràng buộc cần tạo

| Bảng | Ràng buộc / Index | Mục đích |
| --- | --- | --- |
| slot\_locks | UNIQUE (court\_id, slot\_date, slot\_id) | Chống trùng lịch (QT02) |
| bookings | INDEX (status, hold\_expires\_at) | Job hết hạn giữ chỗ quét mỗi phút |
| bookings | INDEX (status, start\_at) | Job no-show, nhắc lịch, hoàn thành |
| bookings | UNIQUE (code) | Tra đơn khi check-in |
| bookings | INDEX (branch\_id, play\_date) | Lưới ca, dashboard chi nhánh |
| booking\_slots | INDEX (court\_id, slot\_date) | Dựng lưới ca theo ngày |
| payments | UNIQUE (gateway\_txn\_id) | Chống xử lý trùng callback |
| time\_slots | UNIQUE (branch\_id, slot\_index) |  |
| courts | UNIQUE (branch\_id, name) |  |
| reviews | UNIQUE (booking\_id) | Mỗi đơn một đánh giá |
| users | UNIQUE (email), INDEX (phone) | Đăng nhập, tìm khách tại quầy |
| audit\_logs | INDEX (created\_at), (branch\_id, created\_at) | Lọc log |
| notifications | INDEX (user\_id, is\_read) | Đếm chuông chưa đọc |
| (mọi bảng nghiệp vụ) | CHECK số tiền >= 0, rating 1 đến 5 | Toàn vẹn dữ liệu |

**Lưu ý khi làm báo cáo:** nên giải thích với giám khảo ba quyết định thiết kế: (1) tách `bookings` và `booking_slots` để đơn nhiều ca và đổi ca; (2) bảng `slot_locks` riêng để chống trùng ở tầng CSDL kể cả với khóa ca bảo trì; (3) `payments` là sổ cái chỉ thêm, hoàn tiền là một dòng giao dịch mới chứ không sửa dòng cũ.

### 16.13 Bộ ràng buộc đầy đủ (bổ sung cho 16.12)

Mục 16.12 chỉ liệt kê các index và UNIQUE chính. Mục này là bộ ràng buộc hoàn chỉnh, chia làm 5 lớp: khóa ngoại, UNIQUE, CHECK, NOT NULL/DEFAULT, và quy tắc nghiệp vụ kiểm trong code. Ràng buộc nào CSDL làm được thì đặt ở CSDL, phần còn lại kiểm trong service và bọc trong transaction.

#### A. Khóa ngoại và hành vi khi xóa (ON DELETE)

Nguyên tắc: dữ liệu chính dùng **xóa mềm** nên khóa ngoại mặc định là `RESTRICT` (không cho xóa cứng nếu còn tham chiếu). Chỉ `CASCADE` cho bảng chi tiết thuần túy, và `SET NULL` cho tham chiếu tùy chọn.

| Hành vi | Các khóa ngoại |
| --- | --- |
| RESTRICT (mặc định) | users.role\_id, users.branch\_id, branches.owner\_user\_id, courts.branch\_id, courts.court\_type\_id, time\_slots.branch\_id, price\_rules.(branch\_id, court\_id, court\_type\_id), bookings.(customer\_id, branch\_id), booking\_slots.(booking\_id, court\_id, slot\_id), booking\_history.booking\_id, payments.booking\_id, invoices.booking\_id, services.branch\_id, booking\_services.(booking\_id, service\_id), reviews.(booking\_id, customer\_id), complaints.(customer\_id, branch\_id), customer\_violations.(user\_id, booking\_id) |
| CASCADE | branch\_images.branch\_id, branch\_amenities.(branch\_id, amenity\_id), branch\_policies.branch\_id, cancel\_policy\_tiers.branch\_id, auth\_tokens.user\_id, invoice\_items.invoice\_id, complaint\_messages.complaint\_id, chat\_messages.thread\_id, notifications.user\_id, booking\_reminders.booking\_id, recurring\_skips.recurring\_id, equipment.court\_id, favorites.(customer\_id), slot\_locks.(booking\_slot\_id, block\_id) |
| SET NULL | bookings.(recurring\_id, voucher\_id), incidents.equipment\_id, payments.related\_payment\_id, audit\_logs.(actor\_user\_id, branch\_id), booking\_history.performed\_by\_user\_id, chat\_messages.sender\_id |

Mọi cột khóa ngoại đều phải có index (nhiều CSDL không tự tạo), nếu không join và xóa sẽ chậm.

#### B. Ràng buộc UNIQUE bổ sung

| Bảng | UNIQUE | Ý nghĩa |
| --- | --- | --- |
| users | (email); (phone) khi khác NULL | Một email, một số điện thoại cho một khách; tránh trùng hồ sơ vãng lai |
| branches | (owner\_user\_id, name) | Một chủ không tạo hai chi nhánh trùng tên |
| branch\_amenities | (branch\_id, amenity\_id) |  |
| cancel\_policy\_tiers | (branch\_id, hours\_before) | Không có hai mốc cùng số giờ |
| court\_types | (name) |  |
| courts | (branch\_id, name) |  |
| time\_slots | (branch\_id, slot\_index); (branch\_id, start\_time) |  |
| price\_rules | (branch\_id, court\_type\_id, court\_id, day\_type, slot\_type, valid\_from) | Không trùng luật giá cùng hiệu lực |
| holidays | (branch\_id, holiday\_date) |  |
| slot\_locks | (court\_id, slot\_date, slot\_id); (booking\_slot\_id) | Chống trùng ca (QT02); mỗi ca của đơn chỉ có một khóa |
| bookings | (code) |  |
| booking\_slots | (booking\_id, court\_id, slot\_date, slot\_id) | Một đơn không lặp cùng ca |
| recurring\_bookings | (customer\_id, court\_id, weekday, slot\_id) khi status = ACTIVE | Không tạo hai chuỗi định kỳ giống nhau |
| recurring\_skips | (recurring\_id, skip\_date) |  |
| payments | (gateway\_txn\_id) | Chống xử lý trùng callback |
| invoices | (booking\_id); (invoice\_no) |  |
| daily\_closings | (branch\_id, closing\_date) | Mỗi ngày chốt một lần |
| vouchers | (code) |  |
| services | (branch\_id, name) |  |
| reviews | (booking\_id) | Mỗi đơn một đánh giá |
| review\_replies | (review\_id) |  |
| complaints | (code) |  |
| waitlist | (customer\_id, court\_id, slot\_date, slot\_id) khi status = WAITING | Không đăng ký chờ trùng |
| favorites | (customer\_id, branch\_id, court\_id) |  |

**Xóa mềm và UNIQUE:** vì `deleted_at` giữ lại dòng cũ nên tên đã xóa sẽ chặn tên mới. Cách xử lý: UNIQUE thêm cột `deleted_at` (MySQL, các dòng chưa xóa có NULL nên cần cột sinh `active_flag = IF(deleted_at IS NULL, 1, NULL)` đưa vào UNIQUE) hoặc partial unique index `WHERE deleted_at IS NULL` (PostgreSQL, SQL Server).

#### C. Ràng buộc CHECK

| Bảng | CHECK |
| --- | --- |
| users | no\_show\_count >= 0; (role là Quản lý chi nhánh thì branch\_id NOT NULL, ngược lại NULL) |
| branches | open\_time < close\_time; latitude từ -90 đến 90; longitude từ -180 đến 180 |
| branch\_policies | deposit\_percent từ 0 đến 100; change\_before\_hours >= 0 |
| cancel\_policy\_tiers | refund\_percent từ 0 đến 100; hours\_before >= 0 |
| courts | capacity > 0 |
| time\_slots | end\_time = start\_time + 1 giờ (kiểm trong code nếu CSDL không hỗ trợ) |
| price\_rules | price >= 0; valid\_to IS NULL hoặc valid\_to >= valid\_from; court\_id IS NOT NULL hoặc court\_type\_id IS NOT NULL |
| holidays | surcharge\_percent từ 0 đến 100 |
| slot\_blocks | end\_date >= start\_date; start\_slot\_id và end\_slot\_id cùng NULL hoặc cùng có giá trị |
| bookings | end\_at > start\_at; slot\_amount, service\_amount, discount\_amount, deposit\_amount, paid\_amount, refund\_amount đều >= 0; total\_amount = slot\_amount + service\_amount - discount\_amount; deposit\_amount <= total\_amount; refund\_amount <= paid\_amount; deposit\_percent\_snapshot từ 0 đến 100 |
| bookings | status = HOLDING thì hold\_expires\_at NOT NULL |
| bookings | status thuộc (CANCELLED, EXPIRED, NO\_SHOW) thì cancel\_type và cancelled\_at NOT NULL |
| bookings | cancel\_type thuộc (MANAGER, ADMIN) thì cancel\_reason NOT NULL và không rỗng |
| bookings | status thuộc (CHECKED\_IN, COMPLETED) thì checked\_in\_at NOT NULL |
| booking\_slots | price\_snapshot >= 0; end\_at > start\_at |
| slot\_locks | source\_type = BOOKING thì booking\_slot\_id NOT NULL và block\_id NULL; source\_type = BLOCK thì ngược lại |
| recurring\_bookings | weekday từ 1 đến 7; weeks\_ahead từ 1 đến 12; discount\_percent từ 0 đến 100; end\_date IS NULL hoặc >= start\_date |
| payments | amount > 0; type = REFUND thì related\_payment\_id NOT NULL; method = GATEWAY thì gateway NOT NULL |
| daily\_closings | cash\_total, transfer\_total, card\_total, actual\_cash >= 0 |
| vouchers | value > 0; discount\_type = PERCENT thì value <= 100; valid\_to > valid\_from; used\_count <= usage\_limit |
| services | price >= 0; stock\_qty IS NULL hoặc >= 0 |
| booking\_services | quantity > 0; unit\_price\_snapshot >= 0 |
| reviews | rating từ 1 đến 5 |
| complaints | status = ESCALATED thì escalated\_at NOT NULL; status = RESOLVED thì resolution và resolved\_by NOT NULL |
| chat\_threads | CUSTOMER\_BRANCH cần customer\_id và branch\_id; ADMIN\_BRANCH cần branch\_id; CUSTOMER\_AI cần customer\_id |
| email\_queue | retry\_count từ 0 đến 3 |

#### D. NOT NULL và giá trị mặc định

- **NOT NULL:** mọi khóa ngoại bắt buộc, `status`, các cột tiền, `created_at`. Các cột tùy chọn ghi rõ NULL trong từng bảng ở trên.
- **DEFAULT:** `status` của branches = PENDING; users = ACTIVE; courts = ACTIVE; bookings = HOLDING; `payment_status` = UNPAID; mọi cột tiền = 0; `no_show_count` = 0; `is_active` = TRUE; `is_read` = FALSE; `created_at` = thời điểm hiện tại.
- **Thêm cột `version` INT DEFAULT 0** cho `bookings` và `courts` để khóa lạc quan (optimistic locking): mỗi lần cập nhật phải kèm điều kiện `WHERE version = ?`, tránh hai thao tác (ví dụ job no-show và nhân viên check-in) ghi đè nhau.

#### E. Quy tắc nghiệp vụ kiểm trong code (trong transaction)

CSDL không tự kiểm được các điều kiện này. Mỗi điều kiện cần có unit test.

| Thao tác | Điều kiện bắt buộc |
| --- | --- |
| Tạo đơn | Chi nhánh ACTIVE; sân ACTIVE và chưa xóa; khách không bị LOCKED và `booking_restricted_until` đã qua; các ca nằm trong giờ mở cửa, cùng một sân, cùng một ngày, **liên tiếp nhau**; số ca không vượt `max_slots_per_booking` (đề xuất 4); ca bắt đầu sau thời điểm hiện tại; ngày đặt không quá `max_advance_days` (đề xuất 30); khách không giữ quá 3 đơn HOLDING cùng lúc (chống chiếm ca) |
| Tạo đơn | Sắp xếp các ca theo (court\_id, slot\_date, slot\_id) trước khi INSERT `slot_locks` để mọi giao dịch khóa cùng thứ tự, tránh deadlock; bắt lỗi vi phạm UNIQUE để báo ca vừa bị người khác đặt |
| Tính tiền | Tiền cọc nằm trong khoảng cho phép (`deposit_percent_min/max`); khách có `require_full_prepay` thì cọc 100%; giá lấy theo luật giá có hiệu lực tại ngày chơi, rồi lưu snapshot |
| Callback thanh toán | Xác thực chữ ký cổng; số tiền khớp `deposit_amount`; đơn đang HOLDING và chưa quá hạn; xử lý idempotent theo `gateway_txn_id`. Callback đến muộn sau khi đơn đã hết hạn thì ghi giao dịch và **tự tạo giao dịch hoàn tiền** |
| Chuyển trạng thái | Chỉ cho phép các chuyển trong bảng 3.3. Mọi UPDATE kèm điều kiện trạng thái cũ (ví dụ `WHERE status = HOLDING AND hold_expires_at < now`) để job chạy lại hoặc chạy song song vẫn an toàn |
| Check-in | Đơn đang DEPOSITED; thời điểm nằm trong khoảng từ start\_at đến start\_at + 15 phút; chữ ký QR hợp lệ và chưa hết hạn; mỗi đơn check-in một lần |
| Khách hủy | Đơn đang HOLDING hoặc DEPOSITED và còn trước start\_at; hoàn tiền tính theo `cancel_policy_snapshot`, không theo chính sách hiện tại |
| Quản lý/Admin hủy | Bắt buộc lý do; nếu lỗi do chi nhánh (sự cố, bảo trì) thì hoàn 100% |
| Gia hạn | Đơn đang CHECKED\_IN hoặc DEPOSITED; ca kế tiếp cùng sân và còn trống; khóa ca mới bằng `slot_locks`; ghi chênh lệch vào `booking_history` |
| Đổi sân/đổi ca | Cùng chi nhánh; cùng loại sân (đề xuất); ca đích còn trống; nằm trong hạn `change_before_hours`; tính chênh lệch giá, nếu đắt hơn thì thu thêm, rẻ hơn thì giữ nguyên hoặc hoàn theo quy định |
| Khóa ca, sân bảo trì, khóa chi nhánh | Nếu có đơn đang hiệu lực trong phạm vi thì không cho khóa ngay; phải xử lý hết các đơn đó (đổi lịch hoặc hoàn tiền) hoặc xác nhận hủy hàng loạt kèm lý do (QT15) |
| Đổi giờ mở/đóng cửa | Không xóa ca đã có đơn; ca không còn dùng chuyển `is_active = FALSE` |
| Đổi giá | Chỉ thêm luật giá mới, không sửa luật cũ; đơn đã đặt giữ nguyên snapshot |
| Lịch định kỳ | Sinh đơn chỉ khi không trùng; ca trùng ghi vào `recurring_skips` và báo khách; hủy một buổi chỉ hủy đơn đó |
| Đánh giá | Chỉ khách của đơn COMPLETED, trong vòng 30 ngày, mỗi đơn một lần |
| Khiếu nại | Chỉ khiếu nại đơn của chính mình; chỉ leo lên Admin sau khi chi nhánh đã phản hồi |
| Voucher | Còn hạn, chưa hết lượt, đạt đơn tối thiểu, đúng chi nhánh áp dụng; tăng `used_count` trong cùng transaction |
| Hoàn tiền | Tổng tiền hoàn của một đơn không vượt tổng tiền đã thu thành công |
| Chặn dữ liệu chéo | Quản lý chi nhánh chỉ thao tác dữ liệu có `branch_id` trùng `branch_id` trong token; khách chỉ truy cập đơn, chat, thông báo có `customer_id` là chính mình; trợ lý AI luôn nhận `customer_id` từ phiên đăng nhập, không lấy từ nội dung chat |
| Vi phạm | Khi no-show làm `no_show_count` đạt ngưỡng thì tự đặt `require_full_prepay` hoặc `booking_restricted_until`, ghi `customer_violations` |
| Xóa mềm | Không cho xóa sân, dịch vụ, loại sân nếu còn đơn hoặc sân thuộc về nó đang hiệu lực trong tương lai; chỉ cho ngừng hoạt động |
| Mật khẩu và token | Token đặt lại mật khẩu dùng một lần, hết hạn sau 30 phút; đổi mật khẩu thu hồi toàn bộ refresh token |

#### F. Kiểm tra bằng bộ test tối thiểu

1. Hai luồng đặt cùng một ca cùng lúc: đúng một luồng thành công.
2. Callback thanh toán gửi hai lần: chỉ ghi một giao dịch, đơn chỉ chuyển trạng thái một lần.
3. Callback đến sau khi đơn hết hạn: sinh giao dịch hoàn tiền.
4. Đổi giá sau khi đặt: tổng tiền đơn cũ không đổi.
5. Hủy trước 24h, trước 12h, sau mốc: hoàn đúng 100%, 50%, 0%.
6. Check-in sớm, đúng giờ, quá +15 phút: lần lượt bị từ chối, thành công, bị từ chối.
7. Quản lý chi nhánh A gọi API với ID của chi nhánh B: bị chặn.
8. Job no-show chạy hai lần liên tiếp: đếm vi phạm của khách chỉ tăng một lần.

### 16.14 Kiểu dữ liệu chuỗi: NVARCHAR và quy đổi kiểu cho SQL Server

Tài liệu giả định CSDL là **SQL Server** (vì dùng `NVARCHAR`). Nếu dùng MySQL (bảng mã utf8mb4) hoặc PostgreSQL thì `VARCHAR` đã lưu được tiếng Việt có dấu, không cần chữ N; khi đó chỉ áp dụng cột Ghi chú ở bảng dưới.

#### A. Cột phải dùng NVARCHAR (có chữ tiếng Việt)

| Bảng | Cột NVARCHAR |
| --- | --- |
| users | full\_name, lock\_reason |
| branches | name, address, district, city, description, reject\_reason, lock\_reason |
| branch\_staff\_names | full\_name |
| amenities | name |
| court\_types | name, description |
| courts | name, description |
| holidays | name |
| slot\_blocks | reason, performed\_by\_name |
| bookings | note, cancel\_reason, created\_by\_name, checked\_in\_by\_name, cancelled\_by\_name |
| booking\_history | performed\_by\_name |
| payments | note, received\_by\_name |
| invoices | issued\_by\_name |
| invoice\_items | description |
| daily\_closings | closed\_by\_name, note |
| services | name, unit |
| booking\_services | name\_snapshot, added\_by\_name |
| equipment | name, note |
| incidents | description, reported\_by\_name, resolved\_note |
| reviews | comment (NVARCHAR(MAX)) |
| review\_replies | content (NVARCHAR(MAX)) |
| complaints | title, content (NVARCHAR(MAX)), resolution (NVARCHAR(MAX)) |
| complaint\_messages | content (NVARCHAR(MAX)) |
| chat\_messages | content (NVARCHAR(MAX)) |
| notifications | title, body |
| broadcasts | title, content (NVARCHAR(MAX)) |
| email\_queue | subject, body (NVARCHAR(MAX)), error |
| system\_settings | value, description |
| audit\_logs | performed\_by\_name, action nếu có mô tả tiếng Việt |
| job\_runs | error |

#### B. Cột giữ VARCHAR (chỉ ASCII)

`roles.code`, `users.email`, `users.phone`, `users.password_hash`, `auth_tokens.token_hash`, mọi cột `*_url` và `pdf_path`, `bookings.code`, `bookings.qr_secret`, `invoices.invoice_no`, `payments.gateway_txn_id`, `vouchers.code`, `complaints.code`, `email_queue.to_email` và `template`, `system_settings.key`, `audit_logs.entity_type`, `ip`, `user_agent`, `job_runs.job_name`, `notifications.type`, `notifications.link`, `court_types.icon`, `amenities.icon`, và toàn bộ cột trạng thái/loại (ENUM).

#### C. Quy đổi kiểu từ tài liệu sang SQL Server

| Kiểu trong tài liệu | Kiểu SQL Server |
| --- | --- |
| BIGINT PK tự tăng | BIGINT IDENTITY(1,1) |
| VARCHAR(n) có chữ tiếng Việt | NVARCHAR(n) |
| VARCHAR(n) chỉ ASCII | VARCHAR(n) |
| TEXT, MEDIUMTEXT | NVARCHAR(MAX) (kiểu TEXT đã bị Microsoft loại bỏ) |
| JSON | NVARCHAR(MAX) kèm CHECK (ISJSON(cột) = 1) |
| ENUM | VARCHAR(30) kèm CHECK (cột IN (...)) |
| BOOLEAN | BIT |
| DATETIME | DATETIME2(0) |
| DECIMAL(12,0), DATE, TIME, INT, TINYINT | Giữ nguyên |
| UNIQUE có điều kiện (status = ACTIVE, deleted\_at IS NULL) | Filtered unique index: CREATE UNIQUE INDEX ... WHERE điều kiện |
| Cột `version` khóa lạc quan | ROWVERSION hoặc INT tự tăng trong câu UPDATE |

#### D. Lưu ý khi dùng NVARCHAR

- **Chữ N trước chuỗi:** khi INSERT hoặc so sánh chuỗi có dấu bằng câu lệnh SQL phải viết `N'Sân bóng đá số 1'`. Thiếu chữ N thì dấu bị biến thành dấu hỏi. Quan trọng nhất với **file seed data**; tham số truyền từ code (ORM, ADO.NET, JDBC) thường tự xử lý đúng.
- **Collation:** đặt collation CSDL là `Vietnamese_100_CI_AS` (hoặc `Vietnamese_CI_AS`) để sắp xếp A đến Z đúng thứ tự tiếng Việt. Tìm kiếm không phân biệt dấu (gõ `san bong` ra `Sân bóng`) dùng `COLLATE Vietnamese_100_CI_AI` ở câu truy vấn.
- **Giới hạn khóa index:** khóa của index nhóm (clustered) tối đa 900 byte, index không nhóm tối đa 1700 byte; NVARCHAR tốn 2 byte mỗi ký tự. Không đặt index hoặc UNIQUE lên `NVARCHAR(MAX)`; các cột có index trong thiết kế (tên sân, tên chi nhánh, họ tên) đều nhỏ hơn 200 ký tự nên an toàn.
- **Kích thước:** đặt độ dài vừa đủ (tên 100 đến 150, địa chỉ 255, lý do 255). Chỉ dùng `NVARCHAR(MAX)` cho nội dung dài như đánh giá, khiếu nại, chat, thân email.
- **Chuỗi kết nối và ứng dụng:** giao diện, API và file seed đều lưu UTF-8; kiểm tra bằng cách thêm một chi nhánh tên có dấu và đọc lại.

## 17. Hướng dẫn thiết kế Code-First (EF Core + SQL Server)

Mục này chuyển toàn bộ thiết kế ở mục 16 thành cấu hình trong code. Giả định **ASP.NET Core + EF Core 8 + SQL Server**. Nếu bạn dùng Spring Boot (JPA/Hibernate) hoặc Node (Prisma/TypeORM) thì ý tưởng giống hệt, chỉ khác cú pháp; báo mình để viết lại.

Sửa nhỏ so với mục 16.13: khóa ngoại `slot_locks.booking_slot_id` và `block_id` đã đổi từ SET NULL sang CASCADE, vì SET NULL làm cả hai cột cùng NULL và vi phạm CHECK của bảng này. Cột `version` ở 16.13 được thay bằng `rowversion` do SQL Server tự tăng (xem 17.5).

### 17.1 Gói NuGet và quy ước đặt tên

- `Microsoft.EntityFrameworkCore.SqlServer`, `.Design`, `.Tools`
- `EFCore.NamingConventions`: gọi `UseSnakeCaseNamingConvention()` để bảng và cột trong CSDL là snake\_case, khớp với tài liệu và ERD. Hệ quả: chuỗi SQL trong CHECK và filter phải viết tên cột snake\_case (ví dụ `[end_at]`).
- `Bogus` (sinh seed data), `Testcontainers.MsSql` và `xUnit` (test).
- Bật `<Nullable>enable</Nullable>` trong file .csproj: kiểu `string` thành NOT NULL, kiểu `string?` thành NULL. Đây là cách khai báo NOT NULL gọn nhất khi code-first.
- Giá trị ENUM lưu bằng tên enum trong C# (`Holding`, `Deposited`, `CheckedIn`...). Tài liệu viết HOA chỉ để dễ đọc.

### 17.2 Các mặc định nguy hiểm của EF Core

| Mặc định của EF | Hậu quả | Cách xử lý |
| --- | --- | --- |
| `string` thành `nvarchar(max)` | Không đặt index được, tốn bộ nhớ | `HaveMaxLength(255)` toàn cục, cột khác khai báo riêng |
| `string` ASCII vẫn là nvarchar | Tốn gấp đôi dung lượng | `.IsUnicode(false)` cho mã, email, URL, băm, trạng thái |
| `decimal` không có độ chính xác | Cảnh báo, có thể cắt phần thập phân | `HavePrecision(12, 0)` toàn cục |
| `enum` lưu số nguyên | Khó đọc; đổi thứ tự enum làm sai dữ liệu cũ | `HasConversion<string>()` kèm CHECK IN |
| `DateTime` thành `datetime2(7)` | Dư độ chính xác | `HaveColumnType("datetime2(0)")` |
| FK bắt buộc mặc định CASCADE | SQL Server báo lỗi multiple cascade paths; xóa dây chuyền | Đặt RESTRICT toàn cục, CASCADE chỉ cho bảng chi tiết (16.13 A) |
| UNIQUE trên cột nullable | SQL Server chỉ cho đúng một dòng NULL | `HasFilter(...)` hoặc để provider tự thêm filter, luôn kiểm tra file migration |
| `HasData` và `DateTime` | Phải có giá trị cố định | Không dùng `DateTime.Now` trong seed |

Dùng `DateOnly` cho `play_date`, `slot_date`, `holiday_date`, `start_date`... và `TimeOnly` cho `open_time`, `close_time`, `start_time` (EF Core 8 ánh xạ sang `date` và `time`). Giờ ghi nhận luôn là `DateTime.UtcNow`.

### 17.3 Cấu hình toàn cục trong DbContext

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder b)
    {
        b.Properties<string>().HaveMaxLength(255);        // NVARCHAR(255) mặc định
        b.Properties<decimal>().HavePrecision(12, 0);     // tiền VND
        b.Properties<DateTime>().HaveColumnType("datetime2(0)");
    }

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.UseCollation("Vietnamese_100_CI_AS");

        // 1. RESTRICT cho mọi khóa ngoại. Phải đặt TRƯỚC dòng Apply bên dưới
        foreach (var fk in m.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        // 2. Mỗi entity một file IEntityTypeConfiguration<T>; bảng chi tiết tự đặt Cascade trong file của nó
        m.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // 3. Bộ lọc xóa mềm cho mọi entity có ISoftDelete
        foreach (var t in m.Model.GetEntityTypes().Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType)))
        {
            var e = Expression.Parameter(t.ClrType, "e");
            var isNull = Expression.Equal(
                Expression.Property(e, nameof(ISoftDelete.DeletedAt)),
                Expression.Constant(null, typeof(DateTime?)));
            t.SetQueryFilter(Expression.Lambda(isNull, e));
        }
    }
}
```

Vì dòng `HaveMaxLength(255)` áp cho mọi `string`, cột cần dài hơn phải khai báo `.HasColumnType("nvarchar(max)")` (đánh giá, nội dung khiếu nại, chat, thân email, JSON).

### 17.4 Entity gốc, xóa mềm, mốc thời gian

```csharp
public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public interface ISoftDelete { DateTime? DeletedAt { get; set; } }

// Trong AppDbContext
public override Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    var now = DateTime.UtcNow;
    foreach (var e in ChangeTracker.Entries<BaseEntity>())
    {
        if (e.State == EntityState.Added) e.Entity.CreatedAt = now;
        if (e.State is EntityState.Added or EntityState.Modified) e.Entity.UpdatedAt = now;
    }
    // Xóa mềm: chuyển lệnh DELETE thành UPDATE deleted_at
    foreach (var e in ChangeTracker.Entries<ISoftDelete>().Where(x => x.State == EntityState.Deleted))
    {
        e.State = EntityState.Modified;
        e.Entity.DeletedAt = now;
    }
    return base.SaveChangesAsync(ct);
}
```

Bảng cần **xóa cứng** thì không implement `ISoftDelete`: `slot_locks`, `auth_tokens`, `notifications`, `booking_reminders`. Khi đọc lịch sử đơn có liên quan sân hoặc dịch vụ đã xóa mềm, dùng `.IgnoreQueryFilters()` để không mất dòng.

Hàm hỗ trợ cho cột ENUM:

```csharp
public static class ModelExtensions
{
    public static PropertyBuilder<TEnum> AsStringEnum<TEnum>(this PropertyBuilder<TEnum> p)
        where TEnum : struct, Enum
        => p.HasConversion<string>().HasMaxLength(30).IsUnicode(false);

    // Sinh điều kiện: [status] IN ('Holding','Deposited',...)
    public static string EnumIn<TEnum>(string column) where TEnum : struct, Enum
        => $"[{column}] IN (" + string.Join(",", Enum.GetNames<TEnum>().Select(n => $"'{n}'")) + ")";
}
```

### 17.5 Mẫu cấu hình các entity then chốt

**User**: UNIQUE có điều kiện và CHECK vai trò

```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users", t =>
        {
            t.HasCheckConstraint("CK_users_branch_role",
                "([role_id] = 2 AND [branch_id] IS NOT NULL) OR ([role_id] <> 2 AND [branch_id] IS NULL)");
            t.HasCheckConstraint("CK_users_no_show", "[no_show_count] >= 0");
            t.HasCheckConstraint("CK_users_status", ModelExtensions.EnumIn<UserStatus>("status"));
        });
        b.Property(x => x.FullName).HasMaxLength(150);                 // NVARCHAR, NOT NULL nhờ string không nullable
        b.Property(x => x.Email).HasMaxLength(150).IsUnicode(false);    // string? nên cho phép NULL (khách vãng lai)
        b.Property(x => x.Phone).HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.PasswordHash).HasMaxLength(255).IsUnicode(false);
        b.Property(x => x.Status).AsStringEnum().HasDefaultValue(UserStatus.Active);
        b.Property(x => x.NoShowCount).HasDefaultValue(0);
        b.HasIndex(x => x.Email).IsUnique().HasFilter("[email] IS NOT NULL");
        b.HasIndex(x => x.Phone).IsUnique().HasFilter("[phone] IS NOT NULL");
    }
}
// Role cố định bằng HasData: 1 = Admin, 2 = BranchManager, 3 = Customer
```

**Court**: UNIQUE tên sân trong chi nhánh, bỏ qua dòng đã xóa mềm

```csharp
b.HasIndex(x => new { x.BranchId, x.Name }).IsUnique().HasFilter("[deleted_at] IS NULL");
b.Property(x => x.Status).AsStringEnum().HasDefaultValue(CourtStatus.Active);
b.Property(x => x.LiveStatus).AsStringEnum().HasDefaultValue(CourtLiveStatus.Available);
b.ToTable("courts", t => t.HasCheckConstraint("CK_courts_capacity", "[capacity] > 0"));
```

**PriceRule**: UNIQUE chứa cột nullable, phải tắt filter tự động

```csharp
b.ToTable("price_rules", t =>
{
    t.HasCheckConstraint("CK_price_rules_target", "[court_id] IS NOT NULL OR [court_type_id] IS NOT NULL");
    t.HasCheckConstraint("CK_price_rules_price", "[price] >= 0");
    t.HasCheckConstraint("CK_price_rules_dates", "[valid_to] IS NULL OR [valid_to] >= [valid_from]");
});
b.HasIndex(x => new { x.BranchId, x.CourtTypeId, x.CourtId, x.DayType, x.SlotType, x.ValidFrom })
    .IsUnique()
    .HasFilter(null);   // provider SQL Server tự thêm 'cột nullable IS NOT NULL'; dòng này tắt để NULL vẫn bị kiểm trùng
```

**SlotLock**: bảng chống trùng lịch, quan trọng nhất

```csharp
b.ToTable("slot_locks", t => t.HasCheckConstraint("CK_slot_locks_source",
    "([source_type] = 'Booking' AND [booking_slot_id] IS NOT NULL AND [block_id] IS NULL) OR " +
    "([source_type] = 'Block' AND [block_id] IS NOT NULL AND [booking_slot_id] IS NULL)"));
b.Property(x => x.SourceType).AsStringEnum();
b.HasIndex(x => new { x.CourtId, x.SlotDate, x.SlotId }).IsUnique().HasDatabaseName("UX_slot_locks_court_date_slot");
b.HasIndex(x => x.BookingSlotId).IsUnique().HasFilter("[booking_slot_id] IS NOT NULL");
b.HasOne(x => x.BookingSlot).WithOne().HasForeignKey<SlotLock>(x => x.BookingSlotId).OnDelete(DeleteBehavior.Cascade);
b.HasOne(x => x.Block).WithMany().HasForeignKey(x => x.BlockId).OnDelete(DeleteBehavior.Cascade);
```

**Booking**: CHECK tiền, trạng thái, khóa lạc quan

```csharp
b.ToTable("bookings", t =>
{
    t.HasCheckConstraint("CK_bookings_time", "[end_at] > [start_at]");
    t.HasCheckConstraint("CK_bookings_money",
        "[slot_amount] >= 0 AND [service_amount] >= 0 AND [discount_amount] >= 0 AND [deposit_amount] >= 0 AND [paid_amount] >= 0 AND [refund_amount] >= 0");
    t.HasCheckConstraint("CK_bookings_total", "[total_amount] = [slot_amount] + [service_amount] - [discount_amount]");
    t.HasCheckConstraint("CK_bookings_deposit", "[deposit_amount] <= [total_amount] AND [refund_amount] <= [paid_amount]");
    t.HasCheckConstraint("CK_bookings_percent", "[deposit_percent_snapshot] BETWEEN 0 AND 100");
    t.HasCheckConstraint("CK_bookings_hold", "[status] <> 'Holding' OR [hold_expires_at] IS NOT NULL");
    t.HasCheckConstraint("CK_bookings_cancel",
        "[status] NOT IN ('Cancelled','Expired','NoShow') OR ([cancel_type] IS NOT NULL AND [cancelled_at] IS NOT NULL)");
    t.HasCheckConstraint("CK_bookings_cancel_reason",
        "[cancel_type] NOT IN ('Manager','Admin') OR LEN(LTRIM(RTRIM([cancel_reason]))) > 0");
    t.HasCheckConstraint("CK_bookings_checkin", "[status] NOT IN ('CheckedIn','Completed') OR [checked_in_at] IS NOT NULL");
    t.HasCheckConstraint("CK_bookings_status", ModelExtensions.EnumIn<BookingStatus>("status"));
});
b.Property(x => x.Code).HasMaxLength(20).IsUnicode(false);
b.Property(x => x.QrSecret).HasMaxLength(64).IsUnicode(false);
b.Property(x => x.Status).AsStringEnum().HasDefaultValue(BookingStatus.Holding);
b.Property(x => x.PaymentStatus).AsStringEnum().HasDefaultValue(PaymentStatus.Unpaid);
b.Property(x => x.CancelPolicySnapshot).HasColumnType("nvarchar(max)");   // JSON dạng chuỗi
b.Property(x => x.RowVersion).IsRowVersion();                              // byte[] RowVersion
b.HasIndex(x => x.Code).IsUnique();
b.HasIndex(x => new { x.Status, x.HoldExpiresAt });
b.HasIndex(x => new { x.Status, x.StartAt });
b.HasIndex(x => new { x.BranchId, x.PlayDate });
```

Các điều kiện kiểu `[a] NOT IN (...) OR (...)` khi cột là NULL cho kết quả UNKNOWN; CHECK của SQL Server coi UNKNOWN là hợp lệ nên không chặn nhầm.

Cột JSON khác (`gateway_payload`, `old_value`, `new_value`, `meta`) cũng là `nvarchar(max)` kèm `HasCheckConstraint(..., "ISJSON([cot]) = 1")`; (de-)serialize bằng `System.Text.Json` trong service.

### 17.6 Bảng đối chiếu: ràng buộc ở mục 16.13 và cách cài trong EF Core

| Ràng buộc | Cách cài |
| --- | --- |
| Khóa ngoại, ON DELETE | `HasOne().WithMany().OnDelete(...)`; mặc định Restrict toàn cục (17.3) |
| UNIQUE | `HasIndex(...).IsUnique()` |
| UNIQUE kèm cột nullable | `.HasFilter("[cot] IS NOT NULL")`; nếu cột nullable nằm trong khóa cần so sánh NULL thì `.HasFilter(null)` |
| UNIQUE kèm xóa mềm | `.HasFilter("[deleted_at] IS NULL")` |
| UNIQUE kèm điều kiện trạng thái (định kỳ ACTIVE, waitlist WAITING) | `.HasFilter("[status] = 'Active'")` |
| CHECK | `ToTable(t => t.HasCheckConstraint(tên, sql))` |
| ENUM | `AsStringEnum()` kèm CHECK `EnumIn<T>` |
| NOT NULL | Bật nullable reference types; hoặc `.IsRequired()` |
| DEFAULT | `.HasDefaultValue(...)`; thời gian dùng `.HasDefaultValueSql("SYSUTCDATETIME()")` |
| Độ dài, NVARCHAR/VARCHAR | `.HasMaxLength(n)`; ASCII thêm `.IsUnicode(false)` |
| Khóa lạc quan (cột version) | `byte[] RowVersion` + `.IsRowVersion()` |
| Chống trùng ca | UNIQUE `slot_locks` + bắt `SqlException` số 2601 hoặc 2627 (17.7) |
| Quy tắc nghiệp vụ mục 16.13 E | Service layer trong transaction; mỗi quy tắc một integration test |
| Audit log bất biến | Trigger hoặc DENY quyền bằng `migrationBuilder.Sql` (17.8) |

### 17.7 Đặt sân và job nền trong code

**Tạo đơn**: kiểm tra nghiệp vụ, rồi chèn `slot_locks` theo thứ tự cố định để tránh deadlock; vi phạm UNIQUE nghĩa là ca vừa bị người khác đặt.

```csharp
public async Task<Booking> CreateAsync(CreateBookingDto dto, long customerId, CancellationToken ct)
{
    await ValidateAsync(dto, customerId, ct);   // quy tắc 16.13 E: chi nhánh, sân, khách, ca liên tiếp, giờ, hạn mức
    var booking = BuildBooking(dto, customerId); // gán giá snapshot, hold_expires_at = now + hold_minutes
    _db.Bookings.Add(booking);

    foreach (var s in booking.Slots.OrderBy(x => x.CourtId).ThenBy(x => x.SlotDate).ThenBy(x => x.SlotId))
        _db.SlotLocks.Add(new SlotLock
        {
            CourtId = s.CourtId, SlotDate = s.SlotDate, SlotId = s.SlotId,
            BookingSlot = s, SourceType = LockSource.Booking
        });

    try
    {
        await _db.SaveChangesAsync(ct);          // một lần SaveChanges đã là một transaction
        return booking;
    }
    catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 })
    {
        throw new SlotAlreadyTakenException();   // controller trả 409 Conflict
    }
}
```

**Job hết hạn giữ chỗ**: xử lý từng đơn, dùng `RowVersion` để nhường khi callback thanh toán vừa cọc xong.

```csharp
var expired = await _db.Bookings.Include(b => b.Slots)
    .Where(b => b.Status == BookingStatus.Holding && b.HoldExpiresAt < now).ToListAsync(ct);

foreach (var b in expired)
{
    try
    {
        b.MarkExpired(now);                                   // Status = Expired, CancelType = Expired, CancelledAt = now
        var slotIds = b.Slots.Select(s => s.Id).ToList();
        _db.SlotLocks.RemoveRange(_db.SlotLocks.Where(l => l.BookingSlotId != null && slotIds.Contains(l.BookingSlotId.Value)));
        _db.BookingHistory.Add(BookingHistory.System(b.Id, HistoryAction.Expire));
        await _db.SaveChangesAsync(ct);                       // kiểm tra RowVersion tại đây
        _emails.Enqueue(b, EmailTemplate.HoldExpired);
    }
    catch (DbUpdateConcurrencyException)
    {
        _db.ChangeTracker.Clear();                            // đơn vừa được cọc, bỏ qua
    }
}
```

Job no-show, hoàn thành, nhắc lịch làm cùng khuôn: lọc theo trạng thái, đổi từng đơn, bắt `DbUpdateConcurrencyException`. Chạy job bằng `BackgroundService` hoặc Hangfire, mỗi phút một lần; mỗi lần chạy ghi một dòng `job_runs`.

### 17.8 Quy trình migration và seed data

1. Mỗi lần xong một nhóm bảng (A, B, C... của mục 16) thì tạo một migration có tên rõ nghĩa: `dotnet ef migrations add Init_Identity`, `Add_BranchCourt`, `Add_Booking`...
2. Mở file migration vừa sinh và **đọc kỹ** trước khi chạy: kiểu cột `nvarchar` hay `varchar`, `decimal(12,0)`, `datetime2(0)`, filter của index, `onDelete`, danh sách CHECK. Xuất SQL để so với thiết kế: `dotnet ef migrations script --idempotent -o schema.sql`.
3. Không sửa migration đã chạy; muốn đổi thì tạo migration mới. Trước lúc nộp bài có thể gom lại thành một migration `Init` sạch.
4. Phần EF không biểu diễn được thì viết SQL thủ công trong migration, ví dụ audit log chỉ cho thêm:

```csharp
migrationBuilder.Sql(@"CREATE TRIGGER trg_audit_logs_immutable ON audit_logs INSTEAD OF UPDATE, DELETE AS
    BEGIN RAISERROR('audit_logs chi duoc them moi', 16, 1); END");
```

5. **Seed tĩnh** (`HasData`, id cố định): `roles`, `court_types`, `amenities`, mốc hoàn cọc mặc định, `system_settings`, tài khoản Admin đầu tiên.
6. **Seed demo** viết bằng C# (class `DemoSeeder`, chạy khi môi trường Development, dùng Bogus): 3 đến 4 chi nhánh, 15 đến 20 sân, 3 tháng đơn có đủ trạng thái, giao dịch, đánh giá, no-show, để dashboard và heatmap có số liệu. Chuỗi có dấu viết trực tiếp trong C# là an toàn, không cần chữ N. Băm mật khẩu khi seed.

### 17.9 Các bẫy thường gặp

- **Vòng tham chiếu `users.branch_id` và `branches.owner_user_id`:** hai khóa ngoại trỏ qua lại. Khi seed hoặc đăng ký chi nhánh phải làm hai bước: tạo user (branch\_id còn NULL), tạo branch, `SaveChanges`, rồi gán `user.BranchId` và `SaveChanges` lần nữa. Nếu muốn đơn giản có thể bỏ cột `owner_user_id`, chỉ giữ `users.branch_id`.
- **Multiple cascade paths:** xảy ra khi quên đặt Restrict toàn cục.
- **Đổi tên giá trị enum** là thay đổi dữ liệu, phải viết migration UPDATE tương ứng cả cột lẫn CHECK.
- **Query filter xóa mềm** ẩn luôn dòng cha khi join; báo cáo lịch sử phải `IgnoreQueryFilters()`.
- **Tracking:** truy vấn chỉ đọc (dashboard, lưới ca) dùng `AsNoTracking()` cho nhanh.
- **Lưới ca theo ngày:** truy vấn từ `slot_locks` (index `court_id, slot_date, slot_id`) thay vì quét `bookings`.
- **Không dùng InMemory hay SQLite khi test**: chúng bỏ qua CHECK, filtered index và rowversion nên test xanh nhưng chạy thật vẫn lỗi.

### 17.10 Test tích hợp bắt buộc

Dùng `Testcontainers.MsSql` (hoặc SQL Server LocalDB) dựng CSDL thật, chạy migration rồi test:

1. Hai mươi luồng đặt cùng một ca bằng `Parallel.ForEachAsync`: đúng một luồng thành công, 19 luồng nhận `SlotAlreadyTakenException`.
2. Chèn đơn vi phạm từng CHECK (tổng tiền sai, HOLDING thiếu hạn, hủy bởi Admin thiếu lý do): đều bị CSDL từ chối.
3. Hai tên sân trùng trong cùng chi nhánh: bị từ chối; sau khi xóa mềm sân cũ thì tạo lại được.
4. Hai khách cùng email NULL (vãng lai) được phép; trùng số điện thoại khác NULL bị từ chối.
5. Job hết hạn chạy đồng thời với callback thanh toán: đơn chỉ ở đúng một trạng thái cuối.
6. Tên chi nhánh `Sân Thể Thao Phú Mỹ Hưng` ghi vào rồi đọc ra đúng từng dấu (kiểm tra NVARCHAR và collation).
