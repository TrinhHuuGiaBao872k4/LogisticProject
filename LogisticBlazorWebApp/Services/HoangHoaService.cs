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

    public async Task<HangHoaVM?> GetHangHoaById(string id)
    {
        var res = await _httpClient.GetFromJsonAsync<HTTPResponse<HangHoaVM>>($"api/HangHoa/GetHangHoaById/{id}");
        return res?.data;
    }

    public event Action Onchange;
    public void SetStateHasChange() => Onchange?.Invoke();
}