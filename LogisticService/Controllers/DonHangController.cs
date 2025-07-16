using LogisticService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
[Route("api/[controller]")]
[ApiController]
public class DonHangController : ControllerBase
{
    private readonly IDonHangService _donHangService;
    private readonly IUnitOfWork _unitOfWork;
    public DonHangController(IDonHangService service, IUnitOfWork unitOfWork)
    {
        _donHangService = service;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("dat-hang")]
    public async Task<IActionResult> DatHang([FromBody] DatHangViewModel model)
    {
        try
        {
            var maDon = await _donHangService.DatHangAsync(model);
            return Ok(new { success = true, maDonHang = maDon });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
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

        var donHang = await _unitOfWork.DonHangRepository.GetByIdAsync(maDonHang);
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

        _unitOfWork.DonHangRepository.Update(donHang);
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

        var donHang = await _unitOfWork.DonHangRepository.GetByIdAsync(maDonHang);
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

}