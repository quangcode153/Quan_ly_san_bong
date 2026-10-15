namespace BTLtruc_quan.WebApi.Entities;

public interface ISoftDelete
{
    DateTime? DeletedAt { get; set; }
}
