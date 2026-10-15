namespace BTLtruc_quan.Shared.DTOs;

public class BranchDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
    public string OperatingHours => $"{OpenTime:hh\\:mm} - {CloseTime:hh\\:mm}";
    public bool IsActive { get; set; } = true;
    public int TotalCourts { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBranchRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public TimeSpan OpenTime { get; set; } = new TimeSpan(6, 0, 0);
    public TimeSpan CloseTime { get; set; } = new TimeSpan(23, 0, 0);
}

public class UpdateBranchRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
    public bool IsActive { get; set; }
}
