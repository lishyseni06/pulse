using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pulse.Data;

namespace Pulse.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db) => _db = db;

    // Ballina
    public async Task<IActionResult> Index()
    {
        var latest = await _db.News
            .Include(n => n.Category)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();

        return View(latest);
    }

    // Faqja e kategorive: /Lajme/{name}
    public async Task<IActionResult> Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RedirectToAction(nameof(Index));

        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Name == name);

        if (category == null)
            return NotFound();

        var items = await _db.News
            .Include(n => n.Category)
            .Where(n => n.CategoryId == category.Id)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();

        ViewData["Category"] = category.Name;
        return View("List", items);
    }

    // Faqja e detajeve: /Home/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.News
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (item == null)
            return NotFound();

        ViewData["Related"] = await _db.News
            .Include(n => n.Category)
            .Where(n => n.CategoryId == item.CategoryId && n.Id != item.Id)
            .OrderByDescending(n => n.PublishedAt)
            .Take(3)
            .ToListAsync();

        return View(item);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
