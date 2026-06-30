using API.DTOs.BloodStock;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/bloodstock")]
public class BloodStockController : ControllerBase
{
    private readonly IBloodStockService _stockService;

    public BloodStockController(IBloodStockService stockService)
    {
        _stockService = stockService;
    }

    // GET api/bloodstock
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stocks = await _stockService.GetAllAsync();
        return Ok(stocks);
    }

    // GET api/bloodstock/A+
    [HttpGet("{bloodType}")]
    public async Task<IActionResult> GetByType(string bloodType)
    {
        var stock = await _stockService.GetByBloodTypeAsync(bloodType);
        return stock == null ? NotFound() : Ok(stock);
    }

    // PUT api/bloodstock/A+
    [HttpPut("{bloodType}")]
    public async Task<IActionResult> Update(string bloodType, [FromBody] UpdateBloodStockDto dto)
    {
        try
        {
            var result = await _stockService.UpdateAsync(bloodType, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}