using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pulse.Data;
using Pulse.Models;
using Pulse.Services;

namespace Pulse.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db) => _db = db;

    // ---------- Autentikimi ----------

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Username == model.Username);
        if (admin == null || !PasswordHasher.Verify(model.Password, admin.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Përdoruesi ose fjalëkalimi është i pasaktë.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, admin.Username),
            new(ClaimTypes.NameIdentifier, admin.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // ---------- Dashboard ----------

    public async Task<IActionResult> Dashboard()
    {
        var items = await _db.News
            .Include(n => n.Category)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();
        return View(items);
    }

    // ---------- Krijim / Editim ----------

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        News news;
        if (id == null)
        {
            news = new News { PublishedAt = DateTime.Now };
        }
        else
        {
            var existing = await _db.News.FindAsync(id.Value);
            if (existing == null) return NotFound();
            news = existing;
        }

        await PopulateCategoriesAsync(news.CategoryId);
        return View(news);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(News model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        if (model.Id == 0)
        {
            _db.News.Add(model);
        }
        else
        {
            _db.News.Update(model);
        }
        await _db.SaveChangesAsync();

        TempData["Msg"] = model.Id == 0 ? "Lajmi u shtua me sukses." : "Lajmi u përditësua me sukses.";
        return RedirectToAction(nameof(Dashboard));
    }

    // ---------- Fshirje ----------

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.News
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.Id == id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.News.FindAsync(id);
        if (item != null)
        {
            _db.News.Remove(item);
            await _db.SaveChangesAsync();
            TempData["Msg"] = "Lajmi u fshi.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    private async Task PopulateCategoriesAsync(int selectedId)
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedId);
    }
}
