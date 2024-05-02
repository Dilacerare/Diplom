using Diplom.Domain.Enum;
using Diplom.Domain.ViewModels;
using Diplom.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Diplom.App.Controllers;

public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IAccessPermissionService _accessService;

    public ProfileController(IProfileService profileService, IAccessPermissionService accessService)
    {
        _profileService = profileService;
        _accessService = accessService;
    }

    public async Task<IActionResult> Detail(string login)
    {
        if (User.Identity.IsAuthenticated && User.Identity.Name == login)
        {
            var response = await _profileService.GetProfile(login);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
                return View(response.Data);
        }
        else if (User.Identity.IsAuthenticated)
        {
            var response = await _profileService.GetProfileAccess(User.Identity.Name, login);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
                return View("ProfileAccess", response.Data);
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
    
    [HttpGet]
    public IActionResult AddMedReport(string login)
    {
        ViewData["Login"] = login;
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> AddMedReport(MedicalReportViewModel model)
    {
        ModelState.Remove("Login");
        ModelState.Remove("Initiator");
        ModelState.Remove("Conclusion");
        ModelState.Remove("Description");
        ModelState.Remove("Recommendation");
        if (ModelState.IsValid)
        {
            var response = await _profileService.AddMedReport(model);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return RedirectToAction("Detail", "Profile", response.Data);
            }
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [HttpGet]
    public IActionResult PatientRegistration()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> PatientRegistration(PatientRegisterViewModel model)
    {
        ModelState.Remove("Login");
        ModelState.Remove("Password");
        ModelState.Remove("PasswordConfirm");
        ModelState.Remove("PatientName");
        ModelState.Remove("Age");
        ModelState.Remove("BloodType");
        ModelState.Remove("Allergies");
        ModelState.Remove("Initiator");
        
        if (ModelState.IsValid)
        {
            var response = await _profileService.AddNewPatient(model);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return RedirectToAction("Detail", "Profile", response.Data);
            }
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost]
    public async Task<IActionResult> AccessRequest(AccessPermissionViewModel model)
    {
        var response = await _accessService.Create(model);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Json(new { description = response.Description });
        }
        return Json(new { description = response.Description });
    }
}