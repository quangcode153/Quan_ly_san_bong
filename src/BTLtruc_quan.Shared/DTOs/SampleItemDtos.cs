using BTLtruc_quan.Shared.Enums;

namespace BTLtruc_quan.Shared.DTOs;

public class SampleItemDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ItemStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSampleItemRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ItemStatus Status { get; set; } = ItemStatus.Active;
    public string? Description { get; set; }
}

public class UpdateSampleItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ItemStatus Status { get; set; }
    public string? Description { get; set; }
}

public class SampleItemFilterRequest
{
    public string? SearchKeyword { get; set; }
    public string? Category { get; set; }
    public ItemStatus? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
