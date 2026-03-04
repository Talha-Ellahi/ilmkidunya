using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IKDFrontEnd.Controllers
{
    [ApiController]
    public class UserInfoController : Controller
    {
        [HttpGet("_user/current")]
        public IActionResult Current()
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Unauthorized();
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var name = User.Identity?.Name ?? "";
            return Ok(new { id, name });
        }
    }
}
