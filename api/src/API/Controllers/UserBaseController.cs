using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserBaseController : BaseAPIController
    {
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok("You are authorized");
        }
    }
}
