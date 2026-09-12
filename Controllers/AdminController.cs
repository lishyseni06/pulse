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
    private readonly IWebHostEnvironment _env;

    public AdminController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

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
    public async Task<IActionResult> Edit(News model, IFormFile? imageFile)
    {
        // ImageUrl mbushet nga file-i i ngarkuar (jo nga përdoruesi) — hiqe nga validimi.
        ModelState.Remove(nameof(News.ImageUrl));

        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            var savedPath = await SaveImageAsync(imageFile);
            if (savedPath == null)
            {
                ModelState.AddModelError(string.Empty, "Foto duhet të jetë imazh (jpg, png, gif, webp) deri në 5 MB.");
                await PopulateCategoriesAsync(model.CategoryId);
                return View(model);
            }
            model.ImageUrl = savedPath;
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

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private async Task<string?> SaveImageAsync(IFormFile file)
    {
        const long maxBytes = 5 * 1024 * 1024; // 5 MB
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (file.Length > maxBytes || !AllowedExtensions.Contains(ext))
            return null;

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{fileName}";
    }

    private async Task PopulateCategoriesAsync(int selectedId)
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedId);
    }
}
