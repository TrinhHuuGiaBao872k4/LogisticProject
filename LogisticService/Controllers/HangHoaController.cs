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
        public HangHoaController()
        {
        }

        [HttpGet("GetAllHangHoa")]
        public ActionResult<IActionResult> GetAllHangHoa()
        {
            return Ok();
        }
    }
}