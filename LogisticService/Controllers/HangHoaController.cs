using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LogisticService.Models;
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

    }
}