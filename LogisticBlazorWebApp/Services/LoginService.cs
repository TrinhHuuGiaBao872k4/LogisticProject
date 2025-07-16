public class LoginService
{
    public HttpClient _httpClient;
    
    public LoginService(HttpClient http)
    {
        _httpClient = http;
    }
    public async Task<HTTPResponse<LoginResultViewModel>> Login(LoginViewModel model)
    {
        var url = "http://localhost:5103/api/NguoiDung/DangNhap";
        var res = await _httpClient.PostAsJsonAsync(url, model);

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