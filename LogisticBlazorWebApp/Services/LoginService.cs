public class LoginService
{
    public HttpClient _httpClient;
    
    public LoginService(IHttpClientFactory httpFactory)
    {
        _httpClient = httpFactory.CreateClient("LogisticApi");
    }
    public async Task<HTTPResponse<LoginResultViewModel>> Login(LoginViewModel model)
    {
        var res = await _httpClient.PostAsJsonAsync("api/NguoiDung/DangNhap", model);

        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadFromJsonAsync<HTTPResponse<LoginResultViewModel>>();
            return result!;
        }
        var errorBody = await res.Content.ReadAsStringAsync();
        Console.WriteLine($"Đăng nhập lỗi: {(int)res.StatusCode} - {errorBody}");
        return null!;
    }

    
}