using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LogisticService.Models;
using LogisticService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using LogisticService.Models;

namespace LogisticService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangHoaController(IHangHoaService _hangHoaService, RedisHelper _redisHelper, JwtAuthService _jwt, INguoiDungService _nguoiDungService) : ControllerBase
    {
        // public IHangHoaService _hangHoaService;
        // public RedisHelper _redisHelper;
        // public HangHoaController(IHangHoaService hangHoaService, RedisHelper redisHelper)
        // {
        //     _hangHoaService = hangHoaService;
        //     _redisHelper = redisHelper;
        // }
        [HttpGet("GetAllHangHoa")]
        public async Task<IActionResult> GetAllHangHoa()
        {
            _redisHelper.setDatabaseRedis(1);
            string cacheKey = "hanghoa_list_db";
            try
            {
                // Thử lấy dữ liệu từ Redis
                var cachedData = await _redisHelper.GetAsync<HTTPResponseClient<IEnumerable<HangHoa>>>(cacheKey);
                // Nếu có dữ liệu trong cache và không rỗng
                if (cachedData != null)
                {
                    return Ok(cachedData);
                }
                // Nếu không có trong cache, lấy từ database
                var hangHoaList = await _hangHoaService.GetAllHangHoaAsync();
                // Lưu vào cache với định dạng JSON rõ ràng
                await _redisHelper.SetAsync(cacheKey, hangHoaList, TimeSpan.FromDays(1));
                return Ok(hangHoaList);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và log nếu cần
                // Trong trường hợp lỗi deserialization, vẫn trả về dữ liệu từ database
                var hangHoaList = await _hangHoaService.GetAllHangHoaAsync();
                await _redisHelper.SetAsync(cacheKey, hangHoaList, TimeSpan.FromDays(1));
                return Ok(hangHoaList);
            }
        }
        [HttpGet("GetHangHoaById/{Id}")]
        public async Task<IActionResult> GetHangHoaById([FromRoute][Required] string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest("Id không được để trống");  // Trả về 400 nếu Id rỗng
            }

            var hangHoa = await _hangHoaService.GetHangHoaByIdAsync(Id);

            if (hangHoa == null)
            {
                return NotFound($"Không tìm thấy hàng hóa với Id = {Id}");  // 404 nếu không tồn tại
            }

            return Ok(hangHoa);  // 200 nếu thành công
        }
        [HttpPost("CreateHangHoa")]
        [Authorize]
        public async Task<IActionResult> CreateHangHoa(HangHoaVM hangHoaVM)
        {

            var header = HttpContext.Request.Headers;
            var token = header["Authorization"].First().Substring(7);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new HTTPResponseClient<object>
                {
                    StatusCode = 401,
                    Data = null,
                    DateTime = DateTime.Now,
                    Message = "Token không hợp lệ"
                });
            }
            TokenResult res = _jwt.DecodePayloadTokenInfo(token);
            string VaiTro = res.Role;
            Console.WriteLine(VaiTro);
            if (VaiTro == "Seller")
            {
                HangHoa hangHoa = new HangHoa()
                {
                    MaHangHoa = await _hangHoaService.GenerateMaHangHoaAsync(),
                    MaLoaiHangHoa = hangHoaVM.MaLoaiHangHoa,
                    TenHangHoa = hangHoaVM.TenHangHoa,
                    NgaySanXuat = hangHoaVM.NgaySanXuat,
                    HinhAnh = hangHoaVM.HinhAnh,
                    GiaHangHoa = hangHoaVM.GiaHangHoa,
                    MaNguoiDung = res.Id
                };
                await _hangHoaService.AddAsync(hangHoa);
                return Ok(new HTTPResponseClient<HangHoa>
                {
                    StatusCode = 200,
                    Data = hangHoa,
                    DateTime = DateTime.Now,
                    Message = "Successfully"
                });
            }
            else
            {
                return BadRequest(new HTTPResponseClient<HangHoa>
                {
                    StatusCode = 400,
                    Data = null,
                    DateTime = DateTime.Now,
                    Message = "Người dùng không phải Seller"
                });
            }
        }
        [HttpPut("UpdateHangHoa/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateHangHoa([FromRoute] string id,[FromBody] HangHoaVM hangHoaVM)
        {
            var header = HttpContext.Request.Headers;
            var token = header["Authorization"].First().Substring(7);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new HTTPResponseClient<object>
                {
                    StatusCode = 401,
                    Data = null,
                    DateTime = DateTime.Now,
                    Message = "Token không hợp lệ"
                });
            }
            TokenResult res = _jwt.DecodePayloadTokenInfo(token);
            string VaiTro = res.Role;
            if (VaiTro == "Seller")
            {
                var hangHoa = await _hangHoaService.GetHangHoaByIdAsync(id);
                if (hangHoa.Data == null)
                {
                    return NotFound(new HTTPResponseClient<object>
                    {
                        StatusCode = 404,
                        Data = null,
                        DateTime = DateTime.Now,
                        Message = $"Không tìm thấy hàng hóa với mã {id}"
                    });
                }
                var entity  = hangHoa.Data;
                entity.MaLoaiHangHoa = hangHoaVM.MaLoaiHangHoa ?? entity.MaLoaiHangHoa;
                entity.TenHangHoa = hangHoaVM.TenHangHoa ?? entity.TenHangHoa;
                entity.HinhAnh = hangHoaVM.HinhAnh ?? entity.HinhAnh;
                entity.GiaHangHoa = hangHoaVM.GiaHangHoa ?? entity.GiaHangHoa;
                await _hangHoaService.UpdateAsync(entity);
                return Ok(new HTTPResponseClient<HangHoa>
                {
                    StatusCode = 200,
                    Data = entity,
                    DateTime = DateTime.Now,
                    Message = "Successfully"
                });
            }
            else
            {
                return BadRequest(new HTTPResponseClient<HangHoa>
                {
                    StatusCode = 400,
                    Data = null,
                    DateTime = DateTime.Now,
                    Message = "Người dùng không phải Seller"
                });
            }
        }
        [HttpDelete("DeleteHangHoaById/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteHangHoaById([FromRoute] string id)
        {
            var header = HttpContext.Request.Headers;
            var token = header["Authorization"].First().Substring(7);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new HTTPResponseClient<object>
                {
                    StatusCode = 401,
                    Data = null,
                    DateTime = DateTime.Now,
                    Message = "Token không hợp lệ"
                });
            }
            TokenResult res = _jwt.DecodePayloadTokenInfo(token);
            string VaiTro = res.Role;
            if (VaiTro == "Seller")
            {
                var hangHoa = await _hangHoaService.GetByIdAsync(id);
                await _hangHoaService.DeleteAsync(id);
                return Ok(new HTTPResponseClient<HangHoa>
                {
                    StatusCode = 200,
                    Data = hangHoa,
                    DateTime = DateTime.Now,
                    Message = "Successfully"
                });
            }
            else
            {
                return BadRequest(new HTTPResponseClient<HangHoa>
                {
                    StatusCode = 400,
                    Data = null,
                    DateTime = DateTime.Now,
                    Message = "Người dùng không phải Seller"
                });
            }
        }
        [HttpGet("SearchHangHoa/{searchkey}")]
        public async Task<IActionResult> SearchHangHoa([FromRoute] string searchkey)
        {
            if (string.IsNullOrWhiteSpace(searchkey))
            {
                return BadRequest("Từ khóa tìm kiếm không được để trống.");
            }

            // Tìm kiếm theo tên hoặc mã hàng hóa, không phân biệt chữ hoa/thường
            var result = await _hangHoaService.WhereAsync(h =>
                EF.Functions.Like(h.TenHangHoa.ToLower(), $"%{searchkey.ToLower()}%") ||
                EF.Functions.Like(h.MaHangHoa.ToLower(), $"%{searchkey.ToLower()}%")
            );
            if (result == null || !result.Any())
            {
                return NotFound(new HTTPResponseClient<IEnumerable<HangHoa>>
                {
                    StatusCode = 404,
                    Data = new List<HangHoa>(),
                    DateTime = DateTime.Now,
                    Message = "Không tìm thấy hàng hóa nào phù hợp."
                });
            }
            return Ok(new HTTPResponseClient<IEnumerable<HangHoa>>
            {
                StatusCode = 200,
                Data = result,
                DateTime = DateTime.Now,
                Message = $"Tìm thấy {result.Count()} hàng hóa phù hợp."
            });
        }
        [HttpGet("FilterByLoai/{maLoai}")]
        public async Task<IActionResult> FilterByLoai(string maLoai)
        {
            var result = await _hangHoaService.WhereAsync(h => h.MaLoaiHangHoa == maLoai);

            return Ok(new HTTPResponseClient<IEnumerable<HangHoa>>
            {
                StatusCode = 200,
                Data = result,
                DateTime = DateTime.Now,
                Message = $"Tìm thấy {result.Count()} hàng hóa theo loại {maLoai}."
            });
        }
        [HttpGet("SortByPriceAsc")]
        public async Task<IActionResult> SortByPriceAsc()
        {
            var result = await _hangHoaService.GetAllAsync();
            var sorted = result.OrderBy(h => h.GiaHangHoa);

            return Ok(new HTTPResponseClient<IEnumerable<HangHoa>>
            {
                StatusCode = 200,
                Data = sorted,
                DateTime = DateTime.Now,
                Message = "Danh sách hàng hóa theo giá tăng dần."
            });
        }
        [HttpGet("SortByPriceDesc")]
        public async Task<IActionResult> SortByPriceDesc()
        {
            var result = await _hangHoaService.GetAllAsync();
            var sorted = result.OrderByDescending(h => h.GiaHangHoa);

            return Ok(new HTTPResponseClient<IEnumerable<HangHoa>>
            {
                StatusCode = 200,
                Data = sorted,
                DateTime = DateTime.Now,
                Message = "Danh sách hàng hóa theo giá giảm dần."
            });
        }
        [HttpGet("SortByNewest")]
        public async Task<IActionResult> SortByNewest()
        {
            var result = await _hangHoaService.GetAllAsync();
            var sorted = result.OrderByDescending(h => h.NgaySanXuat);

            return Ok(new HTTPResponseClient<IEnumerable<HangHoa>>
            {
                StatusCode = 200,
                Data = sorted,
                DateTime = DateTime.Now,
                Message = "Danh sách hàng hóa mới nhất."
            });
        }
        [HttpGet("SortByBestSeller")]
        public async Task<IActionResult> SortByBestSeller()
        {
            var result = await _hangHoaService.GetAllWithNavigationPropertiesAsync();
            var sorted = result.OrderByDescending(h => h.ChiTietDonHangs.Sum(ct => ct.SoLuong ?? 0)).ToList();
            return Ok(new HTTPResponseClient<IEnumerable<HangHoa>>
            {
                StatusCode = 200,
                Data = sorted,
                DateTime = DateTime.Now,
                Message = "Danh sách hàng hóa bán chạy nhất."
            });
        }


    }
}