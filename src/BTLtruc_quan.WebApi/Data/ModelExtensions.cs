using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BTLtruc_quan.WebApi.Data;

public static class ModelExtensions
{
    public static PropertyBuilder<TEnum> AsStringEnum<TEnum>(this PropertyBuilder<TEnum> builder, int maxLength = 30)
        where TEnum : struct, Enum
    {
        return builder.HasConversion<string>()
                      .HasMaxLength(maxLength)
                      .IsUnicode(false);
    }

    public static PropertyBuilder<TEnum?> AsStringEnum<TEnum>(this PropertyBuilder<TEnum?> builder, int maxLength = 30)
        where TEnum : struct, Enum
    {
        return builder.HasConversion<string>()
                      .HasMaxLength(maxLength)
                      .IsUnicode(false);
    }

    public static string EnumIn<TEnum>(string column) where TEnum : struct, Enum
    {
        return $"[{column}] IN (" + string.Join(",", Enum.GetNames<TEnum>().Select(n => $"'{n}'")) + ")";
    }
}
