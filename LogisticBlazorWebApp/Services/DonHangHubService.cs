using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;

public class DonHangHubService : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private HubConnection? _hubConnection;


    public event Action<string, string>? OnTrangThaiUpdated;

    public DonHangHubService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task ConnectAsync()
    {
        if (_hubConnection != null)
            return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5103/donhanghub", options =>
            {
                // Lấy token từ localStorage mỗi lần SignalR cần
                options.AccessTokenProvider = async () =>
                {
                    var t = await _js.InvokeAsync<string>("localStorage.getItem", "accessToken");
                    return string.IsNullOrWhiteSpace(t) ? null : t;
                };
            })
            .WithAutomaticReconnect()
            .Build();


        _hubConnection.On<OrderStatusNotificationVM>("OrderStatusUpdated", data =>
        {
            OnTrangThaiUpdated?.Invoke(data.MaDonHang, data.MaTrangThai);
        });

        await _hubConnection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
            await _hubConnection.DisposeAsync();
    }
    public async Task JoinOrderGroup(string maDonHang)
    {
        if (_hubConnection != null)
            await _hubConnection.InvokeAsync("JoinOrderGroup", maDonHang);
    }
    public async Task LeaveOrderGroup(string maDonHang)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("LeaveOrderGroup", maDonHang);
        }
    }
}

