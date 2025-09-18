using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;

public class DonHangService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;

    public DonHangService(IHttpClientFactory httpFactory, IJSRuntime js)
    {
        _httpClient = httpFactory.CreateClient("LogisticApi");
        _js = js;
    }

    public async Task<List<DonHangViewModel>> GetAllDonHangOfUserAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "accessToken");

        if (string.IsNullOrEmpty(token))
            return new();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await _httpClient.GetAsync("api/DonHang/GetAllDonHangOffUser");

        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadFromJsonAsync<HTTPResponse<List<DonHangViewModel>>>();
            return result?.data ?? new();
        }

        return new();
    }
    
}

