using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.JSInterop;
public class ProfileService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;

    public ProfileService(HttpClient httpClient, IJSRuntime js)
    {
        _httpClient = httpClient;
        _js = js;
    }

    public async Task<UserProfileViewModel?> GetProfileAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "accessToken");

        if (string.IsNullOrEmpty(token))
            return null;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await _httpClient.GetAsync("http://localhost:5103/api/NguoiDung/profile");

        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadFromJsonAsync<HTTPResponse<UserProfileViewModel>>();
            return result?.data;
        }

        return null;
    }
}
