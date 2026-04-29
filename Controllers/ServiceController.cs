using Microsoft.AspNetCore.Mvc;
using HairdresserApp.Models;
using HairdresserApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    //Return form to add service
    public IActionResult CreateService()
    {
        return View();
    }

    //Add service
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateService([Bind("Title, Description, Price, Duration")] Service service)
    {
        if (ModelState.IsValid)
        {
            
            _context.Add(service);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetServices));
        }

        return View(service);
    }

    //Return view to add available service
    public IActionResult AddOfferedService(int serviceId, string serviceTitle)
    {

        ViewData["Hairdresser"] = new SelectList(_context.Hairdressers, "Id", "Fname");
        ViewBag.ServiceId = serviceId;
        ViewBag.ServiceTitle = serviceTitle; 

        return View();
    }

    //Add available service
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOfferedService([Bind("HairdresserId,ServiceId")] OfferedService offeredService)
    {
        Console.WriteLine(offeredService.ServiceId);
        if (ModelState.IsValid)
        {
            _context.Add(offeredService);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetServices));
        }

        return View();
    }

    /* EDIT */
    //Return view to update service
    public IActionResult UpdateService()
    {
        return View();
    }

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
