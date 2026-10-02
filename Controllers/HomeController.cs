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
        var OfferedServices = await _context.Services!.Where(service => service.OfferedServices.Any() 
            && service.IsActive == true)
            .ToListAsync();
        return View(OfferedServices);
    }

}
