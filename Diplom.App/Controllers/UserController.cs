using Diplom.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Diplom.App.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> GetUsers()
    {
        var response = await _userService.GetUsers();
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return View(response.Data);
        }
        return RedirectToAction("Index", "Home");
    }
}