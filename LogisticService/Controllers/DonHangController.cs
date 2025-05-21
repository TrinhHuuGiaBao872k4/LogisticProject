using LogisticService.Models;
using LogisticService.ViewModels;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DonHangController : ControllerBase
{
    private readonly IDonHangService _service;
    public DonHangController(IDonHangService service)
    {
        _service = service;
    }

    [HttpPost("dat-hang")]
    public async Task<IActionResult> DatHang([FromBody] DatHangViewModel model)
    {
        try
        {
            var maDon = await _service.DatHangAsync(model);
            return Ok(new { success = true, maDonHang = maDon });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}