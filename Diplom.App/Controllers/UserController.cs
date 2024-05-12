using Diplom.Domain.Enum;
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

    public async Task<IActionResult> GetUsers(int role)
    {
        if (Enum.IsDefined(typeof(Role), role)) // Проверяем, существует ли такое значение в enum Role
        {
            var parsedRole = (Role)role;
            var response = await _userService.GetUsers(parsedRole);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                ViewData["Title"] = role == 1 ? "Список врачей" : "Список пациентов";
                return View(response.Data);
            }
        }
        return RedirectToAction("Index", "Home");
    }
}