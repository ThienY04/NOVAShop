using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using lab03.Models;

namespace lab03.Controllers
{
    [Authorize]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Profile(bool edit = false)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();
        ViewBag.IsEditMode = edit;
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(IFormCollection form)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.FullName = form["FullName"];
        user.PhoneNumber = form["PhoneNumber"];
        user.Address = form["Address"];

        await _userManager.UpdateAsync(user);

        TempData["Message"] = "Cập nhật thành công.";
        return RedirectToAction("Profile");
    }
}

}
