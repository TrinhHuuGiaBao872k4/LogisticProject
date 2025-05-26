using LogisticService.Models;
using LogisticService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace LogisticService.Controllers
{
    [Route("api/seller")]
    [ApiController]
    [Authorize(Roles = "VT002")]
    public class SellerController : ControllerBase
    {
        private readonly IHangHoaService _hangHoaService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SellerController(IHangHoaService hangHoaService, IHttpContextAccessor httpContextAccessor)
        {
            _hangHoaService = hangHoaService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Lấy danh sách sản phẩm của Seller
        [HttpGet("GetProductFromSeller")]
        public async Task<IActionResult> GetMyProducts()
        {
            var sellerId = _httpContextAccessor.HttpContext.User.FindFirst("MaNguoiDung")?.Value;
            if (string.IsNullOrEmpty(sellerId))
            {
                return Unauthorized(new HTTPResponseClient<string>
                {
                    StatusCode = 401,
                    Message = "Không tìm thấy thông tin người dùng.",
                    Data = null,
                    DateTime = DateTime.Now
                });
            }

            var products = await _hangHoaService.WhereAsync(h => h.MaNguoiDung == sellerId);

            return Ok(new HTTPResponseClient<IEnumerable<HangHoa>>
            {
                StatusCode = 200,
                Data = products,
                DateTime = DateTime.Now,
                Message = $"Danh sách {products.Count()} sản phẩm của bạn."
            });
        }

        // Thêm sản phẩm mới
        [HttpPost("AddProduct")]
        public async Task<IActionResult> CreateProduct(HangHoaVM hangHoaVM)
        {
            var sellerId = _httpContextAccessor.HttpContext.User.FindFirst("MaNguoiDung")?.Value;
            if (string.IsNullOrEmpty(sellerId))
            {
                return Unauthorized(new HTTPResponseClient<string>
                {
                    StatusCode = 401,
                    Message = "Không tìm thấy thông tin người dùng.",
                    Data = null,
                    DateTime = DateTime.Now
                });
            }

            var hangHoa = new HangHoa
            {
                // MaHangHoa = hangHoaVM.MaHangHoa,
                MaLoaiHangHoa = hangHoaVM.MaLoaiHangHoa,
                TenHangHoa = hangHoaVM.TenHangHoa,
                GiaHangHoa = hangHoaVM.GiaHangHoa,
                HinhAnh = hangHoaVM.HinhAnh,
                NgaySanXuat = hangHoaVM.NgaySanXuat,
                MaNguoiDung = sellerId,
            };

            await _hangHoaService.AddAsync(hangHoa);

            return Ok(new HTTPResponseClient<HangHoa>
            {
                StatusCode = 200,
                Data = hangHoa,
                DateTime = DateTime.Now,
                Message = "Thêm sản phẩm mới thành công."
            });
        }

        // Các API Update, Delete sẽ viết tương tự
    }
}
