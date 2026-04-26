using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HairdresserApp.Models;
using HairdresserApp.ViewModels;
using Microsoft.VisualBasic;
using HairdresserApp.Data;
using SQLitePCL;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HairdresserApp.Controllers;

//Controller inheriting from instance controller
public class HairdresserController : Controller
{

    private readonly ApplicationDbContext _context;

    public HairdresserController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Hairdressers view
    public IActionResult Index()
    {
        // Getting all hairdressers
        //Query to get hairdresser open for certain service?
        
        return View();
    }

    //Add hairdresser
    public IActionResult Add()
    {
        //Add hairdresser while registering new user?
        
        return View();
    }

    //Update hairdresser
    public IActionResult Update()
    {
        
        return View();
    }

    //Delete hairdresser
    public IActionResult Delete()
    {
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
