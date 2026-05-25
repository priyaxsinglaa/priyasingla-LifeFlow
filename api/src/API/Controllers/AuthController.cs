using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Interfaces;
namespace API.Controllers
{
    public class AuthController(IAuthService service) : BaseAPIController
    {
        private readonly IAuthService _service = service;

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

        if (result is "Account does not exist" or "Password is incorrect" or "Enter your existing Email or Contact Number")
            return Unauthorized(result);

        return Ok(result);
    }   
}
}