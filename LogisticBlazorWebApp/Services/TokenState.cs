using System.Net.Http.Headers;
using Microsoft.JSInterop;

public sealed class TokenState
{
    private readonly IHttpClientFactory _factory;
    private readonly IJSRuntime _js;

    public string? AccessToken { get; private set; }

    public TokenState(IHttpClientFactory factory, IJSRuntime js)
    {
        _factory = factory;
        _js = js;
    }

    // Giữ lại để tương thích (nếu nơi khác đang gọi)
    public void SetToken(string? token)
    {
        AccessToken = token;
        var client = _factory.CreateClient("LogisticApi");
        if (!string.IsNullOrWhiteSpace(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            client.DefaultRequestHeaders.Authorization = null;
    }

    // NẠP token từ localStorage (gọi trong /auth-redirect trước khi đọc roles)
    public async Task LoadFromStorageAsync()
    {
        AccessToken = await _js.InvokeAsync<string?>("localStorage.getItem", "accessToken");

        var client = _factory.CreateClient("LogisticApi");
        if (!string.IsNullOrWhiteSpace(AccessToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        else
            client.DefaultRequestHeaders.Authorization = null;
    }

    // GHI token vào localStorage + gắn header cho HttpClient
    public async Task SetTokenAsync(string? token)
    {
        AccessToken = token;

        if (!string.IsNullOrWhiteSpace(token))
            await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", token);
        else
            await _js.InvokeVoidAsync("localStorage.removeItem", "accessToken");

        var client = _factory.CreateClient("LogisticApi");
        if (!string.IsNullOrWhiteSpace(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            client.DefaultRequestHeaders.Authorization = null;
    }
}
