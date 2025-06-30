using lab03.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UserAdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserAdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: /UserAdmin
    public async Task<IActionResult> Index(string search, string status)
    {
        var users = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            users = users.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
        }

        var userList = await users.ToListAsync();
        var result = new List<UserViewModel>();

        foreach (var user in userList)
        {
            var roles = await _userManager.GetRolesAsync(user);
            bool isLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow;

            if (status == "locked" && !isLocked) continue;
            if (status == "active" && isLocked) continue;

            result.Add(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "Chưa phân quyền",
                IsLocked = isLocked
            });
        }

        return View(result);
    }

    // POST: /UserAdmin/ToggleLock/id
    [HttpPost]
    public async Task<IActionResult> ToggleLock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            // Mở khoá
            user.LockoutEnd = null;
        }
        else
        {
            // Khoá trong 100 năm
            user.LockoutEnd = DateTime.UtcNow.AddYears(100);
        }

        await _userManager.UpdateAsync(user);
        return RedirectToAction("Index");
    }

    // POST: /UserAdmin/ChangeRole
    [HttpPost]
    public async Task<IActionResult> ChangeRole(string id, string newRole)
    {
        if (string.IsNullOrEmpty(newRole))
        {
            ModelState.AddModelError(string.Empty, "Role không thể trống.");
            return RedirectToAction("Index");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Kiểm tra xem role có tồn tại không
        if (!(await _roleManager.RoleExistsAsync(newRole)))
        {
            ModelState.AddModelError(string.Empty, "Role không hợp lệ.");
            return RedirectToAction("Index");
        }

        // Lấy các role hiện tại của người dùng
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Chỉ thay thế role nếu role hiện tại không giống với role mới
        if (!currentRoles.Contains(newRole))
        {
            // Loại bỏ tất cả role hiện tại và thêm role mới
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["Message"] = $"Đã thay đổi role của {user.UserName} thành {newRole}";
        }
        else
        {
            TempData["Message"] = $"User {user.UserName} đã có role {newRole} rồi!";
        }

        return RedirectToAction("Index");
    }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return Ok(); // Trả về 200
        }


    }

