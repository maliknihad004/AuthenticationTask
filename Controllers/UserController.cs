
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTask.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(Roles = "User,Admin")]
public class UserController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult GetUser()
    {
        return Ok(new
        {
            Message = "Welcome! You have access to the User endpoint."
        });
    }
}
