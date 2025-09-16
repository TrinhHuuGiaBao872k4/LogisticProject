using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize] // Yêu cầu authenticated (JWT)
public class DonHangHub : Hub
{
    // Khi client connect, có thể tự động join group user-{userId}
    public override async Task OnConnectedAsync()
    {
        // assume JWT contains NameIdentifier claim or custom "MaNguoiDung"
        var userId = Context.User?.FindFirst("MaNguoiDung")?.Value;


        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("MaNguoiDung")?.Value;


        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Client gọi để join một "order group" (ví dụ khi mở chi tiết đơn)
    public async Task JoinOrderGroup(string maDonHang)
    {
        if (!string.IsNullOrEmpty(maDonHang))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{maDonHang}");
        }
    }

    public async Task LeaveOrderGroup(string maDonHang)
    {
        if (!string.IsNullOrEmpty(maDonHang))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{maDonHang}");
        }
    }

    // Optional: server->client gọi method tên "OrderStatusUpdated"
}
