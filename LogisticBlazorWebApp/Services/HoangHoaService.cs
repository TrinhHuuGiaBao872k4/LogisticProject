using System.Net.Http;
using System.Net.Http.Json;

public class HangHoaService
{
    
    public List<HangHoaVM> lstHangHoa = new List<HangHoaVM>();
    public HttpClient _httpClient;
    public HangHoaService(HttpClient http)
    {
        _httpClient = http;
    }
    public async Task GetAllHangHoaApi()
    {
        var url = "http://localhost:5103/api/HangHoa/GetAllHangHoa";
        var res = await _httpClient.GetFromJsonAsync<HTTPResponse<List<HangHoaVM>>>(url);
        lstHangHoa = res?.data ?? new List<HangHoaVM>();;
        SetStateHasChange();
    }
    public async Task<HangHoaVM> GetHangHoaById(string id)
        {
    var url = $"http://localhost:5103/api/HangHoa/GetHangHoaById/{id}";

    var response = await _httpClient.GetFromJsonAsync<HTTPResponse<HangHoaVM>>(url);
    return response?.data;
    }
    public event Action Onchange;
    public void SetStateHasChange() => Onchange?.Invoke();
}