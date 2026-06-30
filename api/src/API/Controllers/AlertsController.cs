using API.DTOs.Alert;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    // GET api/alerts
    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var alerts = await _alertService.GetActiveAlertsAsync();
        return Ok(alerts);
    }

    // POST api/alerts/evaluate
    [HttpPost("evaluate")]
    public async Task<IActionResult> Evaluate()
    {
        var newAlerts = await _alertService.EvaluateAndCreateAlertsAsync();
        return Ok(new
        {
            alerts = newAlerts
        });
    }

    // PATCH api/alerts/5/resolve
    [HttpPatch("{id}/resolve")]
    public async Task<IActionResult> Resolve(int id)
    {
        var alert = await _alertService.ResolveAlertAsync(id);
        return alert == null ? NotFound() : Ok(alert);
    }
}