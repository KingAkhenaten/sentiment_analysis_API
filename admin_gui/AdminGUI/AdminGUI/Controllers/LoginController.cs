using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
    public IActionResult Login(string username, string password)
    {
        if (username == "admin" && password == "password")
        {
            HttpContext.Session.SetString("username", username);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.ErrorMessage = "Invalid username or password";
            return View("Index");
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Login");
    }
}