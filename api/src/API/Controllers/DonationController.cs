using API.DTOs.Donation;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/donations")]
public class DonationsController : ControllerBase
{
    private readonly IDonationService _donationService;

    public DonationsController(IDonationService donationService)
    {
        _donationService = donationService;
    }

    // GET api/donations?bloodType=A+&hospital=CityHospital&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? bloodType,
        [FromQuery] string? hospital,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var donations = await _donationService.GetAllAsync(
            bloodType, hospital, from, to, page, pageSize);

        var total = await _donationService.GetTotalCountAsync(bloodType, hospital);

        return Ok(new
        {
            data = donations,
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    // GET api/donations/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var donation = await _donationService.GetByIdAsync(id);
        return donation == null ? NotFound() : Ok(donation);
    }

    // POST api/donations
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDonationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _donationService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT api/donations/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateDonationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _donationService.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    // DELETE api/donations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _donationService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}