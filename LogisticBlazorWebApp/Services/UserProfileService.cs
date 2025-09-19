using System.Net.Http.Headers;
using System.Net.Http.Json;            // cần cho ReadFromJsonAsync
using Microsoft.JSInterop;

public sealed class ProfileService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;

    public ProfileService(IHttpClientFactory httpFactory, IJSRuntime js)
    {
        _httpClient = httpFactory.CreateClient("LogisticApi");
        _js = js;
    }

    public async Task<UserProfileViewModel?> GetProfileAsync()
    {
        try
        {
            // Nếu ở /auth-redirect bạn dùng sessionStorage thì đổi "localStorage" -> "sessionStorage"
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "accessToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            // Gắn header cho request hiện tại (không để DefaultRequestHeaders dính về sau)
            using var req = new HttpRequestMessage(HttpMethod.Get, "api/NguoiDung/profile");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var res = await _httpClient.SendAsync(req);
            if (!res.IsSuccessStatusCode) return null;

            var result = await res.Content.ReadFromJsonAsync<HTTPResponse<UserProfileViewModel>>();
            return result?.data;
        }
        catch
        {
            return null; // có thể log nếu bạn có logger
        }
    }
    public async Task<bool> UpdateProfileAsync(UserProfileUpdateModel model)
    {
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "accessToken");
            if (string.IsNullOrWhiteSpace(token))
                return false;

            using var req = new HttpRequestMessage(HttpMethod.Put, "api/NguoiDung/update-profile");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = JsonContent.Create(model);

            using var res = await _httpClient.SendAsync(req);
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false; // có thể log nếu bạn có logger
        }
    }
}
