using System.Net.Http.Json;
using System.Text.Json;
public class RegisterService
{
    private readonly HttpClient _httpClient;

    public RegisterService(IHttpClientFactory httpFactory)
    {
        _httpClient = httpFactory.CreateClient("LogisticApi");
    }

    public async Task<HTTPResponse<UserRegisterResultVM>?> Register(UserRegisterViewModel model)
    {
        var payload = new
        {
            HoTen = model.HoTen,
            NgaySinh = model.NgaySinh.ToString("yyyy-MM-dd"),
            CCCD = model.CCCD,
            DiaChi = model.DiaChi,
            SDT = model.SDT,
            TenDanhNhap = model.TenDanhNhap,
            MatKhau = model.MatKhau
        };

        Console.WriteLine("📦 Payload gửi đi:");
        Console.WriteLine(JsonSerializer.Serialize(payload));

        var response = await _httpClient.PostAsJsonAsync("api/NguoiDung/register", payload);


        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<HTTPResponse<UserRegisterResultVM>>();
            return result;
        }

        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"🚨 Đăng ký lỗi: {error}");
        return null;
    }

}
