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

        if(services == null)
        {
            return NotFound();
        }
        
        //Return OfferedServices
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
    [Authorize]
    public async Task<IActionResult> AddOfferedService(int id, string title)
    {

        //Getting hairdresser that don't already offer the service
        List<Hairdresser> hairdressers = await _context.Hairdressers!.Where(hairdresser => !hairdresser.OfferedServices.Any(offeredService => offeredService.ServiceId == id)).ToListAsync();

        ViewData["Hairdresser"] = new SelectList(hairdressers, "Id", "Fname");
        ViewBag.ServiceId = id;
        ViewBag.ServiceTitle = title; 
 
        return View();
    }

    //Add available service
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOfferedService(string ServiceTitle,[Bind("HairdresserId,ServiceId")] OfferedService offeredService)
    {
        Console.Write("Test: ");

        foreach(var entry in ModelState)
        {
            foreach(var error in entry.Value.Errors)
            Console.WriteLine($"{entry.Key}: {error.ErrorMessage}");
        }

        if (ModelState.IsValid)
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

            ViewData["Hairdresser"] = new SelectList(hairdressers, "Id", "Fname");
            ViewBag.ServiceId = offeredService.ServiceId;
            ViewBag.ServiceTitle = ServiceTitle; 
        }

        return View(offeredService);
    }

    /* EDIT */
    //Return view to update service
    [Authorize]
    public async Task<IActionResult> UpdateService(int id)
    {

        //Null check

        var service = await _context.Services!.Where(service => service.Id == id).Include(service => service.OfferedServices).ThenInclude(offeredService => offeredService.Hairdresser).FirstOrDefaultAsync();

        //Null check

        
        return View(service);
    }

    //Update service
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateService([Bind("Id, Title, Description, Price, Duration")] Service service)
    {

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

            TempData["message"] = "Tjänsten har uppdaterats.";
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

        if(service != null)
        {
            Console.WriteLine(service.IsActive);

            //Soft deleting service
           service.IsActive = false;
           TempData["message"] = "Tjänsten har raderats.";

           Console.WriteLine(service.IsActive);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(GetServices));
    }

    //Delete available service
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOfferedService(int hairdresserId, int serviceId)
    {
        var offeredService = await _context.OfferedServices!.FindAsync(hairdresserId, serviceId);

        if(offeredService != null)
        {
            _context.OfferedServices.Remove(offeredService);
            await _context.SaveChangesAsync();

            TempData["message"] = "Tjänsten har uppdaterats.";
        }

        return RedirectToAction(nameof(GetServices));
    }

    //Function to validate hairdresser ID
    private bool ServiceExists(int id)
    {
        return _context.Services!.Any(e => e.Id == id);
    }
}
