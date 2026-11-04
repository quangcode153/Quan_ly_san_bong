using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Branches;
using BTLtruc_quan.WebApi.Entities.Courts;
using BTLtruc_quan.WebApi.Entities.System;
using BTLtruc_quan.WebApi.Entities.Users;
using Microsoft.EntityFrameworkCore;
using CourtType = BTLtruc_quan.WebApi.Entities.Courts.CourtType;

namespace BTLtruc_quan.WebApi.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. Seed 4 Roles chuẩn (QT08: ADMIN, BRANCH_MANAGER, STAFF, CUSTOMER)
        if (!await context.Roles.AnyAsync())
        {
            if (context.Database.IsRelational())
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    SET IDENTITY_INSERT [roles] ON;
                    INSERT INTO [roles] ([id], [code], [name]) VALUES 
                    (1, 'ADMIN', N'Quản trị viên toàn chuỗi'),
                    (2, 'BRANCH_MANAGER', N'Quản lý chi nhánh'),
                    (3, 'STAFF', N'Nhân viên sân'),
                    (4, 'CUSTOMER', N'Khách hàng');
                    SET IDENTITY_INSERT [roles] OFF;
                ");
            }
            else
            {
                context.Roles.AddRange(
                    new Role { Id = 1, Code = "ADMIN", Name = "Quản trị viên toàn chuỗi" },
                    new Role { Id = 2, Code = "BRANCH_MANAGER", Name = "Quản lý chi nhánh" },
                    new Role { Id = 3, Code = "STAFF", Name = "Nhân viên sân" },
                    new Role { Id = 4, Code = "CUSTOMER", Name = "Khách hàng" }
                );
                await context.SaveChangesAsync();
            }
        }

        // 2. Seed Chi nhánh mẫu (Branch 1)
        Branch? branch = await context.Branches.FirstOrDefaultAsync(b => b.Id == 1);
        if (branch == null)
        {
            branch = new Branch
            {
                Id = 1,
                OwnerUserId = 1,
                Name = "Cụm Sân Thể Thao Cầu Giấy",
                Address = "Số 1 Duy Tân, Cầu Giấy, Hà Nội",
                Phone = "0988888888",
                OpenTime = new TimeOnly(6, 0),
                CloseTime = new TimeOnly(23, 0),
                Status = BranchStatus.Active
            };
            await context.Branches.AddAsync(branch);
            await context.SaveChangesAsync();
        }

        // 3. Seed 4 Users mẫu khớp với LoginForm Quick-Account
        if (!await context.Users.AnyAsync(u => u.Email == "admin@sportchain.vn"))
        {
            string commonPasswordHash = "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3"; // Admin@123

            var users = new List<User>
            {
                new()
                {
                    RoleId = 1,
                    FullName = "Quản trị viên Hệ thống",
                    Email = "admin@sportchain.vn",
                    Phone = "0988888888",
                    PasswordHash = commonPasswordHash,
                    Status = UserStatus.Active
                },
                new()
                {
                    RoleId = 2,
                    BranchId = branch.Id,
                    FullName = "Quản lý Cầu Giấy",
                    Email = "manager.cn1@sportchain.vn",
                    Phone = "0977777777",
                    PasswordHash = commonPasswordHash,
                    Status = UserStatus.Active
                },
                new()
                {
                    RoleId = 3,
                    BranchId = branch.Id,
                    FullName = "Lễ tân Huỳnh Tấn Phát",
                    Email = "staff.cn1@sportchain.vn",
                    Phone = "0966666666",
                    PasswordHash = commonPasswordHash,
                    Status = UserStatus.Active
                },
                new()
                {
                    RoleId = 4,
                    FullName = "Khách hàng Minh Quân",
                    Email = "khachhang@gmail.com",
                    Phone = "0955555555",
                    PasswordHash = commonPasswordHash,
                    Status = UserStatus.Active
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        // 4. Seed Danh mục loại sân thể thao (CourtTypes)
        if (!await context.CourtTypes.AnyAsync())
        {
            var courtTypes = new List<CourtType>
            {
                new() { Id = 1, Name = "Sân bóng đá 7 người", Icon = "fa-futbol", Description = "Sân cỏ nhân tạo ngoài trời 7 người" },
                new() { Id = 2, Name = "Sân bóng đá 5 người", Icon = "fa-futbol", Description = "Sân cỏ nhân tạo trong nhà 5 người" },
                new() { Id = 3, Name = "Sân cầu lông", Icon = "fa-feather", Description = "Sân thảm PVC chống trơn trượt" },
                new() { Id = 4, Name = "Sân Pickleball", Icon = "fa-table-tennis-paddle-ball", Description = "Sân mặt nhựa acrylic" }
            };

            await context.CourtTypes.AddRangeAsync(courtTypes);
            await context.SaveChangesAsync();
        }

        // 5. Seed 17 Ca hoạt động (06:00 đến 23:00) theo QT01
        if (!await context.TimeSlots.AnyAsync(s => s.BranchId == branch.Id))
        {
            var slots = new List<TimeSlot>();
            for (int i = 0; i < 17; i++)
            {
                int startHour = 6 + i;
                int endHour = startHour + 1;
                bool isPeak = startHour >= 17 && startHour < 22; // QT05: 17:00-22:00 là giờ cao điểm

                slots.Add(new TimeSlot
                {
                    BranchId = branch.Id,
                    SlotIndex = i + 1,
                    StartTime = new TimeOnly(startHour, 0),
                    EndTime = new TimeOnly(endHour, 0),
                    SlotType = isPeak ? SlotType.Peak : SlotType.OffPeak,
                    IsActive = true
                });
            }
            await context.TimeSlots.AddRangeAsync(slots);
            await context.SaveChangesAsync();
        }

        // 6. Seed 4 Sân mẫu
        if (!await context.Courts.AnyAsync(c => c.BranchId == branch.Id))
        {
            var courts = new List<Court>
            {
                new() { BranchId = branch.Id, CourtTypeId = 1, Name = "Sân bóng 7A", Capacity = 14, Status = CourtStatus.Active },
                new() { BranchId = branch.Id, CourtTypeId = 1, Name = "Sân bóng 7B", Capacity = 14, Status = CourtStatus.Active },
                new() { BranchId = branch.Id, CourtTypeId = 3, Name = "Sân cầu lông số 1", Capacity = 4, Status = CourtStatus.Active },
                new() { BranchId = branch.Id, CourtTypeId = 4, Name = "Sân Pickleball số 1", Capacity = 4, Status = CourtStatus.Active }
            };
            await context.Courts.AddRangeAsync(courts);
            await context.SaveChangesAsync();
        }

        // 7. Seed Bảng giá mẫu (PriceRules)
        if (!await context.PriceRules.AnyAsync(p => p.BranchId == branch.Id))
        {
            var priceRules = new List<PriceRule>
            {
                new() { BranchId = branch.Id, CourtTypeId = 1, SlotType = SlotType.OffPeak, DayType = DayType.Weekday, Price = 300000m, ValidFrom = DateOnly.FromDateTime(DateTime.Today) },
                new() { BranchId = branch.Id, CourtTypeId = 1, SlotType = SlotType.Peak, DayType = DayType.Weekday, Price = 450000m, ValidFrom = DateOnly.FromDateTime(DateTime.Today) },
                new() { BranchId = branch.Id, CourtTypeId = 1, SlotType = SlotType.Peak, DayType = DayType.Weekend, Price = 500000m, ValidFrom = DateOnly.FromDateTime(DateTime.Today) },
                new() { BranchId = branch.Id, CourtTypeId = 3, SlotType = SlotType.OffPeak, DayType = DayType.Weekday, Price = 120000m, ValidFrom = DateOnly.FromDateTime(DateTime.Today) }
            };
            await context.PriceRules.AddRangeAsync(priceRules);
            await context.SaveChangesAsync();
        }

        // 8. Seed Cấu hình hệ thống mặc định (SystemSettings)
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
