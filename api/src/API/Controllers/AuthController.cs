using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers
{
    public class AuthController : BaseAPIController
    {
        private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _service.Register(dto);

        if (result == "Account is registered already")
            return BadRequest(result);

        return Ok(result);
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _service.Login(dto);

        if (result == "Account does not exist" || result == "Password is incorrect" || result == "Enter your existing Email or Contact Number")
            return Unauthorized(result);

        return Ok(result);
    }
}
}