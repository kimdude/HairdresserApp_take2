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
public class ServiceController : Controller
{

    private readonly ApplicationDbContext _context;

    public ServiceController(ApplicationDbContext context)
    {
        _context = context;
    }

    /* GET */
    //Getting all services with hairdressers
    public async Task<IActionResult> GetServices()
    {
        var services = await _context.Services!.Include(service => service.OfferedServices).ThenInclude(offeredService => offeredService.Hairdresser).ToListAsync();

        //Return OfferedServices
        return View(services);
    }

    //Getting service details
    public async Task<IActionResult> ServiceDetails(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var service = await _context.Services!.FindAsync(id);

        if(service == null)
        {
            return NotFound();
        }

        return View(service);
    }

    /* CREATE */
    //Add service
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddService([Bind("Title, Description, Price, Duration")] Service service)
    {
        if (ModelState.IsValid)
        {
            
            _context.Add(service);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(service);
    }

    //Add available service
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOfferedService([Bind("HairdresserId, ServiceId")] OfferedService offeredService)
    {
        if (ModelState.IsValid)
        {
            
            _context.Add(offeredService);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(offeredService);
    }

    /* EDIT */
    //Update service
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateService(int id, [Bind("Title, Description, Price, Duration")] Service service)
    {
        if(id != service.Id)
        {
            return NotFound();
        }

        if(ModelState.IsValid)
        {
            try
            {
                _context.Update(service);
                await _context.SaveChangesAsync();
            } 
            catch(DbUpdateConcurrencyException)
            {
                if(!ServiceExists(service.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        return View(service);
    }

    /* DELETE */
    //Deleting service
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteService(int id)
    {
        var service = await _context.Services!.FindAsync(id);

        if(service != null)
        {
           _context.Services.Remove(service);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    //Delete available service
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOfferedService(int? id)
    {
        var offeredService = await _context.OfferedServices!.FindAsync(id);

        if(offeredService != null)
        {
            _context.OfferedServices.Remove(offeredService);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    //Function to validate hairdresser ID
    private bool ServiceExists(int id)
    {
        return _context.Services!.Any(e => e.Id == id);
    }
}
