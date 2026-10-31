using System.Net.Http.Json;
using System.Text.Json;
using BTLtruc_quan.Shared.DTOs;

namespace BTLtruc_quan.WinForms.Services.Network;

/// <summary>
/// Lớp trừu tượng cung cấp các phương thức gọi HTTP Generic bất đồng bộ
/// Tự động giải nén ApiResponse<T> từ Shared DTOs
/// </summary>
public abstract class BaseApiClient
{
    private static readonly Lazy<HttpClient> LazyClient = new(() =>
    {
        var handler = new JwtAuthHeaderHandler();
        var baseUri = NetworkConfig.BaseApiUrl.EndsWith("/")
            ? new Uri(NetworkConfig.BaseApiUrl)
            : new Uri(NetworkConfig.BaseApiUrl + "/");

        var client = new HttpClient(handler)
        {
            BaseAddress = baseUri,
            Timeout = NetworkConfig.RequestTimeout
        };
        return client;
    });

    protected static HttpClient HttpClient => LazyClient.Value;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected static async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var relativePath = CleanEndpoint(endpoint);
            using var response = await HttpClient.GetAsync(relativePath, cancellationToken);
            return await HandleApiResponseAsync<T>(response, cancellationToken);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            throw WrapNetworkException(ex, "GET", endpoint);
        }
    }

    protected static async Task<T> PostAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var relativePath = CleanEndpoint(endpoint);
            using var response = await HttpClient.PostAsJsonAsync(relativePath, data, JsonOptions, cancellationToken);
            return await HandleApiResponseAsync<T>(response, cancellationToken);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            throw WrapNetworkException(ex, "POST", endpoint);
        }
    }

    protected static async Task<T> PutAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var relativePath = CleanEndpoint(endpoint);
            using var response = await HttpClient.PutAsJsonAsync(relativePath, data, JsonOptions, cancellationToken);
            return await HandleApiResponseAsync<T>(response, cancellationToken);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            throw WrapNetworkException(ex, "PUT", endpoint);
        }
    }

    protected static async Task<T> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var relativePath = CleanEndpoint(endpoint);
            using var response = await HttpClient.DeleteAsync(relativePath, cancellationToken);
            return await HandleApiResponseAsync<T>(response, cancellationToken);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            throw WrapNetworkException(ex, "DELETE", endpoint);
        }
    }

    private static string CleanEndpoint(string endpoint)
    {
        if (endpoint.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            return endpoint.Substring(5);
        if (endpoint.StartsWith("/"))
            return endpoint.Substring(1);
        return endpoint;
    }

    private static async Task<T> HandleApiResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        int statusCode = (int)response.StatusCode;
        string content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
        {
            if (response.IsSuccessStatusCode)
                return default!;
            throw new ApiException(statusCode, $"Máy chủ phản hồi mã HTTP {statusCode} không có dữ liệu.");
        }

        ApiResponse<T>? apiResponse = null;
        try
        {
            apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, JsonOptions);
        }
        catch
        {
            // Dự phòng nếu API trả về trực tiếp đối tượng T
            try
            {
                var directData = JsonSerializer.Deserialize<T>(content, JsonOptions);
                if (response.IsSuccessStatusCode && directData != null)
                    return directData;
            }
            catch { }
        }

        if (apiResponse != null)
        {
            if (apiResponse.Success)
            {
                return apiResponse.Data!;
            }

            throw new ApiException(statusCode, apiResponse.Message, apiResponse.Errors);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(statusCode, $"Yêu cầu không thành công (HTTP {statusCode}): {content}");
        }

        return default!;
    }

    private static ApiException WrapNetworkException(Exception ex, string method, string endpoint)
    {
        if (ex is TaskCanceledException || ex is TimeoutException)
        {
            return new ApiException(408, $"Hết thời gian chờ phản hồi ({method} {endpoint}) sau {NetworkConfig.RequestTimeout.TotalSeconds}s.", null, ex);
        }
        if (ex is HttpRequestException httpEx)
        {
            int status = (int)(httpEx.StatusCode ?? 0);
            return new ApiException(status, $"Không thể kết nối đến máy chủ Backend ({NetworkConfig.BaseApiUrl}). Vui lòng kiểm tra dịch vụ WebApi.", null, ex);
        }
        return new ApiException(500, $"Lỗi xử lý mạng ({method} {endpoint}): {ex.Message}", null, ex);
    }
}

