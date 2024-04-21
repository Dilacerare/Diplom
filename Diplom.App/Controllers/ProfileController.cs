using Diplom.Domain.ViewModels;
using Diplom.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Diplom.App.Controllers;

public class ProfileController : Controller
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    public async Task<IActionResult> Detail(string login)
    {
        var response = await _profileService.GetProfile(login);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return View(response.Data);
        }
        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    public async Task<IActionResult> Save(ProfileViewModel model)
    {
        ModelState.Remove("Login");
        ModelState.Remove("PatientName");
        ModelState.Remove("Age");
        ModelState.Remove("BloodType");
        ModelState.Remove("Allergies");
        ModelState.Remove("MedicalReports");
        if (ModelState.IsValid)
        {
            var response = await _profileService.Save(model);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Json(new { description = response.Description });
            }
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
        
    }
}