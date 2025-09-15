using System.Net.Http;
using System.Net.Http.Json;

public sealed class LoginService
{
    private readonly HttpClient _httpClient;
    public LoginService(IHttpClientFactory httpFactory)
    {
        _httpClient = httpFactory.CreateClient("LogisticApi");
    }

    public async Task<(bool ok, HTTPResponse<LoginResultViewModel>? data, string message)> LoginAsync(LoginViewModel model, CancellationToken ct = default)
    {
        try
        {
            var res = await _httpClient.PostAsJsonAsync("api/NguoiDung/DangNhap", model, ct);
            if (!res.IsSuccessStatusCode)
            {
                var body = await res.Content.ReadAsStringAsync(ct);
                return (false, null, string.IsNullOrWhiteSpace(body) ? $"Đăng nhập thất bại ({(int)res.StatusCode})" : body);
            }
            var result = await res.Content.ReadFromJsonAsync<HTTPResponse<LoginResultViewModel>>(cancellationToken: ct);
            return (true, result, result?.messsage ?? "OK");
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }
}
