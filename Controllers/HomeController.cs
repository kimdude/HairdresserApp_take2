using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HairdresserApp.Models;
using HairdresserApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HairdresserApp.Controllers;

//Controller inheriting from instance controller
public class HomeController : Controller
{

    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Homepage view
    public async Task<IActionResult> Index()
    {
        var OfferedServices = await _context.Services!.Where(service => service.OfferedServices.Any() && service.IsActive == true).Include(service => service.OfferedServices).ThenInclude(offeredService => offeredService.Hairdresser).ToListAsync();
        return View(OfferedServices);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
