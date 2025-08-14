using System.Net.Http.Json;
using System.Text.Json;

public class RegisterService
{
    private readonly HttpClient _httpClient;

    public RegisterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HTTPResponse<UserRegisterResultVM>?> Register(UserRegisterViewModel model)
    {
        try
        {
            // Chuyển định dạng NgaySinh về yyyy-MM-ddTHH:mm:ss nếu BE cần full
            var payload = new
            {
                HoTen = model.HoTen,
                NgaySinh = model.NgaySinh?.ToString("yyyy-MM-dd"), // hoặc "yyyy-MM-ddTHH:mm:ss"
                CCCD = model.CCCD,
                DiaChi = model.DiaChi,
                SDT = model.SDT,
                TenDanhNhap = model.TenDanhNhap,
                MatKhau = model.MatKhau
            };

            Console.WriteLine("📦 Payload gửi đi:");
            Console.WriteLine(JsonSerializer.Serialize(payload));

            var response = await _httpClient.PostAsJsonAsync("http://localhost:5103/api/NguoiDung/register", payload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<HTTPResponse<UserRegisterResultVM>>();
                Console.WriteLine("✅ Đăng ký thành công.");
                return result;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🚨 Lỗi đăng ký: {(int)response.StatusCode} - {error}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔥 Exception trong RegisterService: {ex.Message}");
            return null;
        }
    }
}
