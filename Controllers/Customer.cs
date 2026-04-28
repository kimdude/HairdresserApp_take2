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
public class CustomerController : Controller
{

    private readonly ApplicationDbContext _context;

    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
    }

    /* GET */
    //Get all customers
    public async Task<IActionResult> GetCustomers()
    {
        // Getting all customers
        var customers = await _context.Customers!.ToListAsync();
        
        return View(customers);
    }

    //Get specific customer
    public async Task<IActionResult> CustomerDetails(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var customers = await _context.Customers!.Include(c => c.Bookings).FirstOrDefaultAsync(m => m.Id == id);

        if(customers == null)
        {
            return NotFound();
        }
        
        return View(customers);
    }

    /* CREATE */
    //Add customer
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCustomer([Bind("Fname,Lname,Email")] Customer customer)
    {
        if(ModelState.IsValid)
        {
            _context.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); //RETURN SOMETHING ELSE
        }

        return View(customer);
    }

}