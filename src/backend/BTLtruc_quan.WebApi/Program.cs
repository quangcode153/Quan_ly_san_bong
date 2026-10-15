using BTLtruc_quan.WebApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình CSDL SQL Server với EF Core & Snake Case
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=BTLtruc_quanDb;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString)
           .UseSnakeCaseNamingConvention();
});

// 2. Đăng ký Services & Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 4. Tự động kiểm tra và áp dụng Migrations + Seed Data khi khởi động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        if (dbContext.Database.IsRelational())
        {
            dbContext.Database.Migrate();
            DbInitializer.SeedAsync(dbContext).GetAwaiter().GetResult();
            logger.LogInformation("Đã đồng bộ CSDL SQL Server và nạp Seed Data thành công.");
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Chưa thể kết nối CSDL SQL Server để tự động chạy migration. Vui lòng kiểm tra dịch vụ SQL Server và chuỗi kết nối trong appsettings.json.");
    }
}

// 5. Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
