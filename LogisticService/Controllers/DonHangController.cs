using LogisticService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using LogisticService.Controllers;
using LogisticService.Helpers;
using LogisticService.ViewModels;
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

    [HttpPut("ChinhSuaDonHang/{maDonHang}")]
    [Authorize]
    public async Task<ActionResult<HTTPResponseClient<object>>> CapNhatDonHang(string maDonHang, [FromBody] UpdateDonHangViewModel model)
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Substring(7);
            if (string.IsNullOrEmpty(token))
                return Fail<object>("Token không hợp lệ", 401);

            var userInfo = _jwtAuthService.DecodePayloadTokenInfo(token);
            if (userInfo == null || userInfo.Role != "VT000")
                return Fail<object>("Chỉ SuperAdmin mới được cập nhật đơn hàng", 403);

            var donHang = await _unitOfWork._donHangRepository.GetByIdAsync(maDonHang);
            if (donHang == null)
                return Fail<object>("Không tìm thấy đơn hàng", 404);
            // 🔎 Kiểm tra ngày hợp lệ
            if (model.NgayVanChuyen == default || model.NgayDenDuKien == default)
                return Fail<object>("Ngày vận chuyển và ngày đến dự kiến không được để trống hoặc sai định dạng", 400);

            if (model.NgayDenDuKien > model.NgayVanChuyen)
                return Fail<object>("Ngày đến dự kiến không được lớn hơn ngày vận chuyển", 400);

            var minValidDate = DateTime.Now.AddYears(-5);
            var maxValidDate = DateTime.Now.AddYears(10);
            if (model.NgayVanChuyen < minValidDate || model.NgayVanChuyen > maxValidDate ||
                model.NgayDenDuKien < minValidDate || model.NgayDenDuKien > maxValidDate)
            {
                return Fail<object>("Ngày vận chuyển và ngày đến dự kiến phải nằm trong khoảng hợp lệ", 400);
            }
            donHang.NgayVanChuyen = model.NgayVanChuyen;
            donHang.NgayDenDuKien = model.NgayDenDuKien;
            donHang.TienShip = model.TienShip;

            _unitOfWork._donHangRepository.Update(donHang);
            await _unitOfWork.SaveChangesAsync();

            return Success<object>(donHang, "Cập nhật đơn hàng thành công");
        }
        catch (Exception ex)
        {
            return Fail<object>($"Lỗi: {ex.Message} | StackTrace: {ex.StackTrace}");
        }
    }

    [HttpPut("HuyDonHang/{maDonHang}")]
    [Authorize]
    public async Task<ActionResult<HTTPResponseClient<object>>> XoaDonHang(string maDonHang)
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Substring(7);
            if (string.IsNullOrEmpty(token))
                return Fail<object>("Token không hợp lệ", 401);

            var userInfo = _jwtAuthService.DecodePayloadTokenInfo(token);
            if (userInfo == null || userInfo.Role != "VT000")
                return Fail<object>("Chỉ SuperAdmin mới được hủy đơn hàng", 403);

            var donHang = await _unitOfWork._donHangRepository.GetByIdAsync(maDonHang);
            if (donHang == null)
                return Fail<object>("Không tìm thấy đơn hàng", 404);

            // ✅ Ghi trạng thái mới vào LichSuTrangThaiDonHang
            var maTrangThai = "TT05"; // Mã trạng thái 'Đã hủy'
            var lichSu = new LichSuTrangThaiDonHang
            {
                MaLichSu = IdHelper.GenerateId("LS", 20),
                MaDonHang = maDonHang,
                MaTrangThai = maTrangThai,
                NgayCapNhat = DateTime.Now,
                GhiChu = "Đơn hàng bị hủy bởi Admin"
            };
            await _unitOfWork.GetRepository<LichSuTrangThaiDonHang>().AddAsync(lichSu);

            // ✅ Ghi tình trạng chi tiết
            var tinhTrang = new TinhTrangDonHangChiTiet
            {
                MaTinhTrangChiTiet = IdHelper.GenerateId("TTCT", 20),
                MaDonHang = maDonHang,
                NoiDung = "Đơn hàng đã bị hủy",
                ThoiGian = DateTime.Now,
                GhiChu = "Admin xử lý hủy"
            };
            await _unitOfWork.GetRepository<TinhTrangDonHangChiTiet>().AddAsync(tinhTrang);

            await _unitOfWork.SaveChangesAsync();

            return Success<object>(null, "Đơn hàng đã được chuyển sang trạng thái 'Đã hủy'");
        }
        catch (Exception ex)
        {
            return Fail<object>($"Lỗi: {ex.Message} | StackTrace: {ex.StackTrace}");
        }
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
    public async Task<ActionResult<HTTPResponseClient<IEnumerable<ChiTietDonHangVM>>>> GetChiTietDonHang(string maDonHang)
    {
        var chiTiet = await _chiTietDonHangService.GetByDonHangAsync(maDonHang);
        if (chiTiet == null || !chiTiet.Any())
            return Fail<IEnumerable<ChiTietDonHangVM>>("Không tìm thấy chi tiết đơn hàng", 404);

        return Success(chiTiet, "Lấy chi tiết đơn hàng thành công");
    }
    [HttpGet("LichSu")]
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

    [HttpGet("GetAllDonHangOffUser")]
    [Authorize]
    public async Task<ActionResult<HTTPResponseClient<IEnumerable<DonHangUserVM>>>> GetAllDonHangOfUser()
    {
        // Lấy token từ header
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Substring(7);
        if (string.IsNullOrEmpty(token))
            return Fail<IEnumerable<DonHangUserVM>>("Token không hợp lệ", 401);

        // Decode token
        var userInfo = _jwtAuthService.DecodePayloadTokenInfo(token);
        if (userInfo == null)
            return Fail<IEnumerable<DonHangUserVM>>("Decode Token thất bại", 401);

        // var donHangRepo = _unitOfWork.GetRepository<DonHang>();
        // var donHangs = await donHangRepo.WhereAsync(dh => dh.MaNguoiDung == userInfo.Id);

        // if (donHangs == null || !donHangs.Any())
        //     return Fail<IEnumerable<DonHangUserVM>>("Khách hàng chưa có đơn hàng nào");

        // // Chỉ map các thông tin cơ bản

        // var donHangVMs = donHangs.Select(dh => new DonHangUserVM
        // {
        //     MaDonHang = dh.MaDonHang,
        //     NgayDat = dh.NgayKhoiTao,
        //     NgayVanChuyen = dh.NgayVanChuyen,
        //     NgayDenDuKien = dh.NgayDenDuKien,
        //     TienShip = dh.TienShip ?? 0,             
        //     TrangThai = dh.LichSuTrangThaiDonHangs
        //          .OrderByDescending(ls => ls.NgayCapNhat)
        //          .Select(ls => ls.MaTrangThaiNavigation.TenTrangThai)
        //          .FirstOrDefault() ?? "Chưa xác định"
        // }).ToList();


        // return Success(donHangVMs.AsEnumerable(), "Lấy lịch sử đơn hàng thành công");
        var donHangVMs = await _donHangService.GetAllDonHangOfUserAsync(userInfo.Id);

        if (!donHangVMs.Any())
            return Fail<IEnumerable<DonHangUserVM>>("Khách hàng chưa có đơn hàng nào");

        return Success(donHangVMs, "Lấy lịch sử đơn hàng thành công");
    }

}