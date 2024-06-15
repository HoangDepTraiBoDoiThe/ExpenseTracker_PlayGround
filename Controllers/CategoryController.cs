using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

public class CategoryController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}