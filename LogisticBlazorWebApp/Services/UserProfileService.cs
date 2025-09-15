using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.JSInterop;
public class ProfileService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;

    public ProfileService(IHttpClientFactory httpFactory, JSRuntime js)
    {
        _httpClient = httpFactory.CreateClient("LogisticApi");
        _js = js;
    }

    public async Task<UserProfileViewModel?> GetProfileAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "accessToken");

        if (string.IsNullOrEmpty(token))
            return null;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await _httpClient.GetAsync("api/NguoiDung/profile");

        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadFromJsonAsync<HTTPResponse<UserProfileViewModel>>();
            return result?.data;
        }

        return null;
    }
}
