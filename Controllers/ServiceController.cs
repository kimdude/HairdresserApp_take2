using Microsoft.AspNetCore.Mvc;
using HairdresserApp.Models;
using HairdresserApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HairdresserApp.Controllers;

//Controller inheriting from instance controller
public class ServiceController : Controller
{

    private readonly ApplicationDbContext _context;

    //Constructor
    public ServiceController(ApplicationDbContext context)
    {
        _context = context;
    }

    /* GET */
    //Getting all services with hairdressers
    [Authorize]
    public async Task<IActionResult> GetServices()
    {
        var services = await _context.Services!.Include(service => service.OfferedServices).ThenInclude(offeredService => offeredService.Hairdresser).Where(service => service.IsActive == true).ToListAsync();
        
        //Return Services
        return View(services);
    }

    //Getting service details
    [Authorize]
    public async Task<IActionResult> ServiceDetails(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var service = await _context.Services!.Where(service => service.Id == id).Include(service => service.OfferedServices).ThenInclude(offeredService => offeredService.Hairdresser).FirstOrDefaultAsync();

        if(service == null)
        {
            return NotFound();
        }

        return View(service);
    }

    /* CREATE */
    //Return form to add service
    [Authorize]
    public IActionResult CreateService()
    {
        return View();
    }

    //Add service
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateService(Service service)
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
    [Authorize]
    public async Task<IActionResult> AddOfferedService(int id, string title)
    {

        //Getting hairdresser that don't already offer the service
        List<Hairdresser> hairdressers = await _context.Hairdressers!.Where(hairdresser => !hairdresser.OfferedServices.Any(offeredService => offeredService.ServiceId == id)).ToListAsync();

        ViewBag.Hairdresser = new SelectList(hairdressers, "Id", "Fname");
        ViewBag.ServiceId = id;
        ViewBag.ServiceTitle = title; 
 
        return View();
    }

    //Add available service
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOfferedService(string ServiceTitle, OfferedService offeredService)
    {

        if(ModelState.IsValid)
        {
            _context.Add(offeredService);
            await _context.SaveChangesAsync();

            TempData["message"] = "Tjänsten har uppdaterats.";
            return RedirectToAction(nameof(GetServices));
        } 
        else
        {
            //Getting hairdresser that don't already offer the service
            List<Hairdresser> hairdressers = await _context.Hairdressers!.Where(hairdresser => !hairdresser.OfferedServices.Any(os => os.ServiceId == offeredService.ServiceId)).ToListAsync();

            ViewBag.Hairdresser = new SelectList(hairdressers, "Id", "Fname");
            ViewBag.ServiceId = offeredService.ServiceId;
            ViewBag.ServiceTitle = ServiceTitle; 
        }

        return View(offeredService);
    }

    /* EDIT */
    //Return view to update service
    [Authorize]
    public async Task<IActionResult> UpdateService(int? id)
    {

        //Null check
        if(id == null)
        {
            return NotFound();
        }

        var service = await _context.Services!.FindAsync(id);

        //Null check
        if(service == null)
        {
            return NotFound();
        }
        
        return View(service);
    }

    //Update service
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateService(Service service)
    {

        if(ModelState.IsValid)
        {
            //Checking if service exists
            var existingService = await _context.Services!.AnyAsync(s => s.Id == service.Id);

            if(existingService == false)
            {
                return NotFound();
            }
            
            //Updating service
            _context.Update(service);
            await _context.SaveChangesAsync();
            
            //Temporary message
            TempData["message"] = "Tjänsten har uppdaterats.";

            //Redirecting
            return RedirectToAction(nameof(GetServices));
        }

        return View(service);
    }

    /* DELETE */
    //Deleting service
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteService(int id)
    {
        Service? service = await _context.Services!.FindAsync(id);

        if(service == null)
        {
            return NotFound();
        } 

        //Soft deleting service
        service.IsActive = false;
        await _context.SaveChangesAsync();

        TempData["message"] = "Tjänsten har raderats.";
        return RedirectToAction(nameof(GetServices));
    }

    //Delete available service
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOfferedService(int hairdresserId, int serviceId)
    {
        var offeredService = await _context.OfferedServices!.FindAsync(hairdresserId, serviceId);

        if(offeredService == null)
        {
            return NotFound();
        }

        _context.OfferedServices.Remove(offeredService);
        await _context.SaveChangesAsync();

        TempData["message"] = "Tjänsten har uppdaterats.";
        return RedirectToAction(nameof(GetServices));
    }
}
