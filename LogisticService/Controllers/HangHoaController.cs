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
    public class HangHoaController : ControllerBase
    {
        public IHangHoaService _hangHoaService;
        public RedisHelper _redisHelper;
        public HangHoaController(IHangHoaService hangHoaService, RedisHelper redisHelper)
        {
            _hangHoaService = hangHoaService;
            _redisHelper = redisHelper;
        }

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
        [Authorize(Roles = "VT002")]
        [HttpPost("CreateHangHoa")]
        public async Task<IActionResult> CreateHangHoa(HangHoaVM hangHoaVM)
        {
            HangHoa hangHoa = new HangHoa()
            {
                MaHangHoa = hangHoaVM.MaHangHoa,
                MaLoaiHangHoa = hangHoaVM.MaLoaiHangHoa,
                TenHangHoa = hangHoaVM.TenHangHoa,
                NgaySanXuat = hangHoaVM.NgaySanXuat,
                HinhAnh = hangHoaVM.HinhAnh
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
        [Authorize(Roles = "VT002")]
        [HttpPut("UpdateHangHoa/{id}")]
        public async Task<IActionResult> UpdateHangHoa([FromRoute] string id, HangHoaVM hangHoaVM)
        {
            HangHoa hangHoa = new HangHoa()
            {
                MaHangHoa = hangHoaVM.MaHangHoa,
                MaLoaiHangHoa = hangHoaVM.MaLoaiHangHoa,
                TenHangHoa = hangHoaVM.TenHangHoa,
                NgaySanXuat = hangHoaVM.NgaySanXuat,
                HinhAnh = hangHoaVM.HinhAnh
            };
            await _hangHoaService.UpdateAsync(hangHoa);
            return Ok(new HTTPResponseClient<HangHoa>
            {
                StatusCode = 200,
                Data = hangHoa,
                DateTime = DateTime.Now,
                Message = "Successfully"
            });
        }
        [Authorize(Roles = "VT002")]
        [HttpDelete("DeleteHangHoaById/{id}")]
        public async Task<IActionResult> DeleteHangHoaById([FromRoute] string id)
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