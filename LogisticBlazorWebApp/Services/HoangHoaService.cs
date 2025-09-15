using System.Net.Http;
using System.Net.Http.Json;

public class HangHoaService
{

    public List<HangHoaVM> lstHangHoa = new List<HangHoaVM>();
    public HttpClient _httpClient;
    public HangHoaService(IHttpClientFactory httpFactory)
    {
        _httpClient = httpFactory.CreateClient("LogisticApi");
    }
    public async Task GetAllHangHoaApi()
    {
        var res = await _httpClient.GetFromJsonAsync<HTTPResponse<List<HangHoaVM>>>("api/HangHoa/GetAllHangHoa");
        lstHangHoa = res?.data ?? new List<HangHoaVM>();
        SetStateHasChange();
    }
    // Tạo hàng hóa mới (trả về kết quả + message)
    public async Task<(bool ok, string message)> CreateHangHoaAsync(HangHoaCreateVM model, CancellationToken ct = default)
    {
        var res = await _httpClient.PostAsJsonAsync("api/HangHoa/CreateHangHoa", model, ct);

        if (res.IsSuccessStatusCode)
        {
            var payload = await res.Content.ReadFromJsonAsync<HTTPResponse<HangHoaVM>>(cancellationToken: ct);
            await GetAllHangHoaApi();
            return (true, payload?.messsage ?? "Tạo hàng hóa thành công");
        }
        else
        {
            try
            {
                var err = await res.Content.ReadFromJsonAsync<HTTPResponse<object>>(cancellationToken: ct);
                return (false, err?.messsage ?? $"Lỗi: {res.StatusCode}");
            }
            catch
            {
                var raw = await res.Content.ReadAsStringAsync(ct);
                return (false, string.IsNullOrWhiteSpace(raw) ? $"Lỗi: {res.StatusCode}" : raw);
            }
        }
    }


    public async Task<HangHoaVM?> GetHangHoaById(string id)
    {
        var res = await _httpClient.GetFromJsonAsync<HTTPResponse<HangHoaVM>>($"api/HangHoa/GetHangHoaById/{id}");
        return res?.data;
    }

    public event Action? Onchange;
    public void  SetStateHasChange() => Onchange?.Invoke();
    
}