using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HairdresserApp.Models;
using HairdresserApp.ViewModels;
using Microsoft.VisualBasic;
using HairdresserApp.Data;
using SQLitePCL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HairdresserApp.Controllers;

//Controller inheriting from instance controller
public class BookingController : Controller
{

    private readonly ApplicationDbContext _context;

    //Constructor
    public BookingController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Booking view
    public async Task<IActionResult> Booking()
    {
        BookingViewModel bookingVm = new BookingViewModel();
        bookingVm.Hairdressers = new SelectList(_context.Hairdressers, "Id", "Fname");
        
        return View(bookingVm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
