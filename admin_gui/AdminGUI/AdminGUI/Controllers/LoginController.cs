using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AdminGUI.Services;

namespace AdminGUI.Controllers;

public class LoginController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromServices] LoginCommand loginCommand, [FromBody] LoginRequest request )
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var executeStatus = loginCommand.Execute(request.Username, request.Password);
        if (!executeStatus)
        {
            return BadRequest(executeStatus);
        }

        return Ok();

    }
}

public class LoginRequest
{
    public LoginRequest(string username, string password)
    {
        Username = username;
        Password = password;
    }

    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
}