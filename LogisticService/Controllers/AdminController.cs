using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisticService.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly LogisticDbServiceContext _context;
    private readonly IConfiguration _config;

    public AdminController(LogisticDbServiceContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginViewModel dto)
    {
        var user = await _context.NguoiDungs
            .FirstOrDefaultAsync(u => u.TenDanhNhap == dto.TenDanhNhap);

        if (user == null || !PasswordHasher.VerifyPassword(dto.MatKhau, user.MatKhau))
            return Unauthorized("Thông tin đăng nhập không hợp lệ!");

        var role = user.MaVaiTro?.Trim();

        if (!string.Equals(role, "VT000", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(role, "VT001", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(403, $"Bạn không có quyền đăng nhập vào hệ thống quản trị.");
        }

        var token = JwtTokenGenerator.GenerateToken(user, _config);

        return Ok(new UserResponseViewModel
        {
            HoTen = user.HoTen,
            TenDanhNhap = user.TenDanhNhap,
            Token = token
        });

    }
    [Authorize(Roles = "VT000,VT001")] // Chỉ SuperAdmin hoặc Admin được cấp quyền
    [HttpPost("cap-quyen/{maNguoiDung}/{maVaiTro}")]
    public async Task<IActionResult> CapQuyenTheoVaiTro(string maNguoiDung, string maVaiTro)
    {
        // Kiểm tra người dùng tồn tại
        var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.MaNguoiDung == maNguoiDung);
        if (user == null)
            return NotFound("Không tìm thấy người dùng.");

        // Kiểm tra vai trò có tồn tại trong hệ thống không
        var role = await _context.VaiTros.FirstOrDefaultAsync(v => v.MaVaiTro == maVaiTro);
        if (role == null)
            return BadRequest("Mã vai trò không hợp lệ.");

        // Nếu người dùng đã có vai trò này
        if (user.MaVaiTro == maVaiTro)
            return BadRequest("Người dùng đã có vai trò này.");

        user.MaVaiTro = maVaiTro;
        _context.NguoiDungs.Update(user);
        await _context.SaveChangesAsync();

        return Ok($"Đã cấp quyền {role.TenVaiTro} cho người dùng {user.HoTen}.");
    }
    [Authorize(Roles = "VT000,VT001")]
[HttpGet("GetAllUsers")]
public async Task<IActionResult> GetAllUsers()
{
    var users = await _context.NguoiDungs
        .Include(u => u.MaVaiTroNavigation)
        .Select(u => new
        {
            u.MaNguoiDung,
            u.HoTen,
            u.TenDanhNhap,
            u.NgaySinh,
            u.DiaChi,
            u.Sdt,
            u.Cccd,
            VaiTro = u.MaVaiTroNavigation != null ? u.MaVaiTroNavigation.TenVaiTro : null
        })
        .ToListAsync();

    return Ok(users);
}
    [Authorize(Roles = "VT000,VT001")]
    [HttpGet("ChiTietNguoiDung/{maNguoiDung}")]
    public async Task<IActionResult> GetUserById(string maNguoiDung)
    {
    var user = await _context.NguoiDungs
        .Include(u => u.MaVaiTroNavigation)
        .FirstOrDefaultAsync(u => u.MaNguoiDung == maNguoiDung);

    if (user == null)
        return NotFound("Không tìm thấy người dùng.");

    return Ok(new {
        user.MaNguoiDung,
        user.HoTen,
        user.TenDanhNhap,
        user.NgaySinh,
        user.DiaChi,
        user.Sdt,
        user.Cccd,
        user.MaVaiTroNavigation,
        user.MaTrangThai
    });
    }
    [Authorize(Roles = "VT000,VT001")]
    [HttpPut("cap-nhat-trang-thai/{maNguoiDung}/{maTrangThai}")]
    public async Task<IActionResult> CapNhatTrangThaiNguoiDung(string maNguoiDung, string maTrangThai)
    {
    var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.MaNguoiDung == maNguoiDung);
    if (user == null)
        return NotFound("Không tìm thấy người dùng.");

    var trangThai = await _context.TrangThaiNguoiDungs.FindAsync(maTrangThai);
    if (trangThai == null)
        return BadRequest("Mã trạng thái không hợp lệ.");

    user.MaTrangThai = maTrangThai;
    await _context.SaveChangesAsync();

    return Ok($" Cập nhật trạng thái người dùng {user.HoTen} thành '{trangThai.TenTrangThai}'.");
}
}
