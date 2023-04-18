using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AdminGUI.Services;
using AdminGUI.Models;

namespace AdminGUI.Controllers;

public class LoginController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }

    /*
    [HttpPost]
    public async Task<IActionResult> Login([FromServices] LoginCommand loginCommand, [FromBody] LoginModel request )
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var executeStatus = loginCommand.Execute(request.Username, request.Password);
        if (!executeStatus)
        {
            return BadRequest(executeStatus);
        }

        return Ok();

    }
    */

    [HttpPost]
    public IActionResult Login(LoginModel l)
    {
        if (!ModelState.IsValid)
        {
            //return BadRequest(ModelState);
            return RedirectToAction("Index");
        }

        var executeStatus = LoginCommand.Execute(l.Username, l.Password);
        
        if (!executeStatus)
        {
            //return BadRequest(executeStatus);
            return RedirectToAction("Index");
        }

        return RedirectToAction("Home/Maintenance");
    }
}