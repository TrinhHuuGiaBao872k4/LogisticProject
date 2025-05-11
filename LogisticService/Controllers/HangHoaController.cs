using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using LogisticService.Models;

namespace LogisticService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangHoaController : ControllerBase
    {
        IHangHoaService _hangHoaService;
        public HangHoaController(IHangHoaService hangHoaService)
        {
            _hangHoaService = hangHoaService;
        }

        [HttpGet("GetAllHangHoa")]
        public async Task<ActionResult> GetAllHangHoa()
        {
            return Ok( await _hangHoaService.GetAllHangHoa());
        }
    }
}