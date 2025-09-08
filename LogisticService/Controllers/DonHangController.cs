using LogisticService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using LogisticService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DonHangController : BaseController
{
    private readonly IDonHangService _donHangService;
    private readonly JwtAuthService _jwtAuthService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChiTietDonHangService _chiTietDonHangService;
    private readonly IHubContext<DonHangHub> _hubContext;
    public DonHangController(IDonHangService donHangService, IUnitOfWork unitOfWork, IHubContext<DonHangHub> hubContext, JwtAuthService jwtAuthService, IChiTietDonHangService chiTietDonHangService)
    {
        _donHangService = donHangService;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        _jwtAuthService = jwtAuthService;
        _chiTietDonHangService = chiTietDonHangService;
    }

    [HttpPost("dat-hang")]
    [Authorize]
    public async Task<ActionResult<HTTPResponseClient<object>>> DatHang([FromBody] DatHangViewModel model)
    {
        try
        {
            // Lấy token từ header
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Substring(7);
            if (string.IsNullOrEmpty(token))
                return Fail<object>("Token không hợp lệ", 401);

            // Decode token
            var userInfo = _jwtAuthService.DecodePayloadTokenInfo(token);
            if (userInfo == null)
                return Fail<object>("Decode Token thất bại", 401);

            // Truyền userId vào service
            var maDon = await _donHangService.DatHangAsync(model, userInfo.Id);

            // return Ok(new { success = true, maDonHang = maDon });
            return Success<object>(maDon, "Đặt hàng thành công");
        }
        catch (Exception ex)
        {
            // return BadRequest(new { success = false, message = ex.Message });
            return Fail<object>($"Lỗi: {ex.Message} | StackTrace: {ex.StackTrace}");
        }
    }
    [Authorize(Roles = "VT000")]
    [HttpPut("ChinhSuaDonHang/{maDonHang}")]
    public async Task<IActionResult> CapNhatDonHang(string maDonHang, [FromBody] UpdateDonHangViewModel model)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var user = await _unitOfWork.GetRepository<NguoiDung>().SingleOrDefaultAsync(u => u.MaVaiTro == role);

        if (user == null || user.MaVaiTro.Trim() != "VT000")
            return Unauthorized(new { success = false, message = "Chỉ SuperAdmin mới được cập nhật đơn hàng" });

        var donHang = await _unitOfWork._donHangRepository.GetByIdAsync(maDonHang);
        if (donHang == null)
            return NotFound("Không tìm thấy đơn hàng");
        // 🔎 Kiểm tra ngày hợp lệ
        if (model.NgayVanChuyen == default || model.NgayDenDuKien == default)
            return BadRequest("Ngày vận chuyển và ngày đến dự kiến không được để trống hoặc sai định dạng");

        if (model.NgayDenDuKien > model.NgayVanChuyen)
            return BadRequest("Ngày đến dự kiến không được lớn hơn ngày vận chuyển");

        var minValidDate = DateTime.Now.AddYears(-5);
        var maxValidDate = DateTime.Now.AddYears(10);
        if (model.NgayVanChuyen < minValidDate || model.NgayVanChuyen > maxValidDate ||
            model.NgayDenDuKien < minValidDate || model.NgayDenDuKien > maxValidDate)
        {
            return BadRequest("Ngày vận chuyển và ngày đến dự kiến phải nằm trong khoảng hợp lệ");
        }
        donHang.NgayVanChuyen = model.NgayVanChuyen;
        donHang.NgayDenDuKien = model.NgayDenDuKien;
        donHang.TienShip = model.TienShip;

        _unitOfWork._donHangRepository.Update(donHang);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { success = true, message = "Cập nhật đơn hàng thành công" });
    }
    [Authorize(Roles = "VT000")]
    [HttpPut("HuyDonHang/{maDonHang}")]
    public async Task<IActionResult> XoaDonHang(string maDonHang)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var user = await _unitOfWork.GetRepository<NguoiDung>().SingleOrDefaultAsync(u => u.MaVaiTro == role);

        if (user == null || user.MaVaiTro.Trim() != "VT000")
            return Unauthorized(new { success = false, message = "Chỉ SuperAdmin mới được cập nhật đơn hàng" });

        var donHang = await _unitOfWork._donHangRepository.GetByIdAsync(maDonHang);
        if (donHang == null)
            return NotFound("Không tìm thấy đơn hàng");

        // ✅ Ghi trạng thái mới vào LichSuTrangThaiDonHang
        var maTrangThai = "TT05"; // Mã trạng thái 'Đã hủy'

        var lichSu = new LichSuTrangThaiDonHang
        {
            MaLichSu = "LS" + DateTime.Now.Ticks,
            MaDonHang = maDonHang,
            MaTrangThai = maTrangThai,
            NgayCapNhat = DateTime.Now,
            GhiChu = "Đơn hàng bị hủy bởi Admin"
        };
        await _unitOfWork.GetRepository<LichSuTrangThaiDonHang>().AddAsync(lichSu);

        // ✅ Ghi tình trạng chi tiết
        var tinhTrang = new TinhTrangDonHangChiTiet
        {
            MaTinhTrangChiTiet = "TTCT" + DateTime.Now.Ticks,
            MaDonHang = maDonHang,
            NoiDung = "Đơn hàng đã bị hủy",
            ThoiGian = DateTime.Now,
            GhiChu = "Admin xử lý hủy"
        };
        await _unitOfWork.GetRepository<TinhTrangDonHangChiTiet>().AddAsync(tinhTrang);

        await _unitOfWork.SaveChangesAsync();

        return Ok(new { success = true, message = "Đơn hàng đã được chuyển sang trạng thái 'Đã hủy'" });
    }


    [HttpPut("Seller/UpdateTrangThai/{maDonHang}")]
    [Authorize]
    public async Task<ActionResult<HTTPResponseClient<object>>> UpdateTrangThaiBySeller(string maDonHang, [FromBody] UpdateTrangThaiRequest req)
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Substring(7);
        if (string.IsNullOrEmpty(token)) return Fail<object>("Token không hợp lệ", 401);

        var res = _jwtAuthService.DecodePayloadTokenInfo(token);
        if (res == null || res.Role != "Seller")
            return Fail<object>("Người dùng không phải Seller", 403);

        var updated = await _donHangService.CapNhatTrangThaiAsync(maDonHang, req, res.Id, "Seller");
        if (!updated) return Fail<object>("Không tìm thấy đơn hàng", 404);

        await _hubContext.Clients.Group($"order-{maDonHang}")
            .SendAsync("OrderStatusUpdated", new OrderStatusNotificationVM
            {
                MaDonHang = maDonHang,
                MaTrangThai = req.MaTrangThai,
                NoiDung = req.NoiDungChiTiet,
                ThoiGian = DateTime.Now,
                GhiChu = req.GhiChu
            });

        return Success<object>(null, "Seller cập nhật thành công");
    }

    [HttpPut("Shipper/UpdateTrangThai/{maDonHang}")]
    [Authorize]
    public async Task<ActionResult<HTTPResponseClient<object>>> UpdateTrangThaiByShipper(string maDonHang, [FromBody] UpdateTrangThaiRequest req)
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Substring(7);
        if (string.IsNullOrEmpty(token)) return Fail<object>("Token không hợp lệ", 401);

        var res = _jwtAuthService.DecodePayloadTokenInfo(token);
        if (res == null || res.Role != "Shipper")
            return Fail<object>("Người dùng không phải Shipper", 403);

        var updated = await _donHangService.CapNhatTrangThaiAsync(maDonHang, req, res.Id, "Shipper");
        if (!updated) return Fail<object>("Không tìm thấy đơn hàng", 404);

        await _hubContext.Clients.Group($"order-{maDonHang}")
            .SendAsync("OrderStatusUpdated", new OrderStatusNotificationVM
            {
                MaDonHang = maDonHang,
                MaTrangThai = req.MaTrangThai,
                NoiDung = req.NoiDungChiTiet,
                ThoiGian = DateTime.Now,
                GhiChu = req.GhiChu
            });

        return Success<object>(null, "Shipper cập nhật thành công");
    }
    [HttpGet("tracking/{maDonHang}")]
    [Authorize] // chỉ user đã login mới theo dõi được
    public async Task<ActionResult<HTTPResponseClient<OrderTrackingViewModel>>> GetOrderTracking(string maDonHang)
    {
        var result = await _donHangService.GetOrderTrackingAsync(maDonHang);
        if (result == null)
            return Fail<OrderTrackingViewModel>("Không tìm thấy đơn hàng", 404);

        return Success(result, "Lấy trạng thái đơn hàng thành công");
    }

    [HttpGet("{maDonHang}/chi-tiet")]
    [Authorize]
    public async Task<ActionResult<HTTPResponseClient<IEnumerable<ChiTietDonHang>>>> GetChiTietDonHang(string maDonHang)
    {
        var chiTiet = await _chiTietDonHangService.GetByDonHangAsync(maDonHang);
        if (chiTiet == null || !chiTiet.Any())
            return Fail<IEnumerable<ChiTietDonHang>>("Không tìm thấy chi tiết đơn hàng", 404);

        return Success(chiTiet, "Lấy chi tiết đơn hàng thành công");
    }
    [HttpGet("lich-su")]
    [Authorize]
    public async Task<ActionResult<HTTPResponseClient<IEnumerable<DonHang>>>> GetLichSuDonHang()
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Substring(7);
        if (string.IsNullOrEmpty(token))
            return Fail<IEnumerable<DonHang>>("Token không hợp lệ", 401);

        var userInfo = _jwtAuthService.DecodePayloadTokenInfo(token);
        if (userInfo == null)
            return Fail<IEnumerable<DonHang>>("Decode Token thất bại", 401);

        var donHangRepo = _unitOfWork.GetRepository<DonHang>();
        var lichSu = await donHangRepo.WhereAsync(dh => dh.MaNguoiDung == userInfo.Id);

        if (lichSu == null || !lichSu.Any())
            return Fail<IEnumerable<DonHang>>("Khách hàng chưa có đơn hàng nào");

        return Success(lichSu, "Lấy lịch sử đơn hàng thành công");
    }

}