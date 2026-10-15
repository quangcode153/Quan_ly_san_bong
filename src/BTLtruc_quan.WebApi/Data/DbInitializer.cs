using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Courts;
using BTLtruc_quan.WebApi.Entities.System;
using BTLtruc_quan.WebApi.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace BTLtruc_quan.WebApi.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. Seed Roles
        if (!await context.Roles.AnyAsync())
        {
            await context.Database.ExecuteSqlRawAsync(@"
                SET IDENTITY_INSERT [roles] ON;
                INSERT INTO [roles] ([id], [code], [name]) VALUES 
                (1, 'ADMIN', N'Quản trị viên toàn chuỗi'),
                (2, 'BRANCH_MANAGER', N'Quản lý chi nhánh'),
                (3, 'CUSTOMER', N'Khách hàng');
                SET IDENTITY_INSERT [roles] OFF;
            ");
        }

        // 2. Seed Admin User
        if (!await context.Users.AnyAsync(u => u.Email == "admin@sportchain.vn"))
        {
            var admin = new User
            {
                RoleId = 1, // ADMIN
                FullName = "Quản trị viên Hệ thống",
                Email = "admin@sportchain.vn",
                Phone = "0988888888",
                // Mật khẩu mặc định: Admin@123 (hash SHA-256 mẫu)
                PasswordHash = "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3",
                Status = UserStatus.Active,
                IsWalkIn = false,
                EmailVerifiedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }

        // 3. Seed Danh mục loại sân thể thao (CourtTypes)
        if (!await context.CourtTypes.AnyAsync())
        {
            var courtTypes = new List<CourtType>
            {
                new() { Name = "Sân bóng đá 7 người", Icon = "fa-futbol", Description = "Sân cỏ nhân tạo ngoài trời chuẩn thi đấu 7 người" },
                new() { Name = "Sân bóng đá 5 người", Icon = "fa-futbol", Description = "Sân cỏ nhân tạo trong nhà/ngoài trời 5 người" },
                new() { Name = "Sân cầu lông", Icon = "fa-feather", Description = "Sân thảm PVC chống trơn trượt tiêu chuẩn BWF" },
                new() { Name = "Sân Pickleball", Icon = "fa-table-tennis-paddle-ball", Description = "Sân mặt nhựa acrylic chuyên dụng" },
                new() { Name = "Sân Tennis", Icon = "fa-baseball", Description = "Sân cứng tiêu chuẩn giải đấu" },
                new() { Name = "Sân bóng rổ", Icon = "fa-basketball", Description = "Sân bóng rổ tiêu chuẩn nửa sân và cả sân" }
            };

            await context.CourtTypes.AddRangeAsync(courtTypes);
            await context.SaveChangesAsync();
        }

        // 4. Seed Cấu hình hệ thống mặc định (SystemSettings)
        var defaultSettings = new List<SystemSetting>
        {
            new() { Key = "hold_minutes", Value = "15", Description = "Thời gian tối đa giữ chỗ đơn đặt chưa cọc (phút)" },
            new() { Key = "checkin_grace_minutes", Value = "15", Description = "Thời gian cho phép trễ check-in trước khi tính no-show (phút)" },
            new() { Key = "default_deposit_percent", Value = "30", Description = "Tỷ lệ tiền cọc tối thiểu mặc định toàn hệ thống (%)" },
            new() { Key = "noshow_threshold", Value = "3", Description = "Số lần no-show tích lũy dẫn đến hạn chế đặt sân" },
            new() { Key = "restrict_days", Value = "30", Description = "Thời gian khóa quyền đặt sân online khi vi phạm (ngày)" },
            new() { Key = "recurring_weeks_ahead", Value = "4", Description = "Số tuần sinh cuốn chiếu trước cho lịch định kỳ" }
        };

        foreach (var setting in defaultSettings)
        {
            if (!await context.SystemSettings.AnyAsync(s => s.Key == setting.Key))
            {
                await context.SystemSettings.AddAsync(setting);
            }
        }

        await context.SaveChangesAsync();
    }
}
