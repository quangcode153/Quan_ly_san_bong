namespace BTLtruc_quan.WinForms.Services.Network;

/// <summary>
/// Ngoại lệ đại diện cho các phản hồi lỗi từ Backend WebApi (400, 401, 403, 404, 500...)
/// </summary>
public class ApiException : Exception
{
    public int StatusCode { get; }
    public string ErrorMessage { get; }
    public List<string> Errors { get; }

    public ApiException(int statusCode, string message, List<string>? errors = null, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorMessage = message;
        Errors = errors ?? new List<string>();
    }

    public override string ToString()
    {
        if (Errors.Count > 0)
        {
            return $"[HTTP {StatusCode}] {ErrorMessage}\nChi tiết: {string.Join(", ", Errors)}";
        }
        return $"[HTTP {StatusCode}] {ErrorMessage}";
    }
}

