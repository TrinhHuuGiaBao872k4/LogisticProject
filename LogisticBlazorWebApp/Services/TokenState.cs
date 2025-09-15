using System.Net.Http.Headers;
//Khi bạn login thành công, API trả về AccessToken.

//Nếu không lưu ở đâu, token đó chỉ nằm trong biến cục bộ của trang login, sau khi bạn NavigateTo trang khác thì token sẽ “mất”.
public sealed class TokenState
{
    private readonly IHttpClientFactory _factory;
    public string? AccessToken { get; private set; }

    public TokenState(IHttpClientFactory factory) => _factory = factory;

    public void SetToken(string? token)
    {
        AccessToken = token;
        var client = _factory.CreateClient("LogisticApi");
        if (!string.IsNullOrWhiteSpace(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            client.DefaultRequestHeaders.Authorization = null;
    }
}
