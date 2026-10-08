using Microsoft.AspNetCore.Mvc;
using HairdresserApp.Models;
using HairdresserApp.ViewModels;
using HairdresserApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HairdresserApp.Controllers;

//Controller inheriting from instance controller
public class HairdresserController : Controller
{

    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public HairdresserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /* GET */
    //Getting all hairdressers
    [Authorize]
    public async Task<IActionResult> GetHairdressers()
    {
        //Getting all hairdressers
        List<Hairdresser> hairdressers = await _context.Hairdressers!.Include(hairdresser => hairdresser.User).ToListAsync();

        //Getting current user Id
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.userId = userId;

        return View(hairdressers);
    }

    /* EDIT */
    //Returning view with update form
    [Authorize]
    public async Task<IActionResult> UpdateHairdresser(int id)
    {
        Hairdresser? hairdresser = await _context.Hairdressers!.FindAsync(id);

        if(hairdresser == null)
        {
            return NotFound();
        }

        HairdresserViewModel hairdresserVm = new HairdresserViewModel
        {
            Id = hairdresser.Id,
            Fname = hairdresser.Fname,
            Lname = hairdresser.Lname
        };

        return View(hairdresserVm);
    }

    //Update hairdresser
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateHairdresser(HairdresserViewModel hairdresserVm)
    {
        if(ModelState.IsValid)
        {

            //Checking for hairdresser
            Hairdresser? existingHairdresser = await _context.Hairdressers!.FirstOrDefaultAsync(hairdresser => hairdresser.Id == hairdresserVm.Id);

            if(existingHairdresser == null)
            {
                return NotFound();
            }

            //Updating only fname and lname
            existingHairdresser.Fname = hairdresserVm.Fname!;
            existingHairdresser.Lname = hairdresserVm.Lname!;

            await _context.SaveChangesAsync();
        

        return RedirectToAction(nameof(GetHairdressers));
        }

        return View(hairdresserVm);
    }

    /* DELETE */
    //Delete hairdresser
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteHairdresser(int? id)
    {
        var hairdresser = await _context.Hairdressers!.FindAsync(id);

        if(hairdresser == null)
        {
            return NotFound();
        }

        //Comparing to current user Id
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.userId = userId;

        if(userId == hairdresser.UserId)
        {
            return Forbid();
        }

        //Finding user
        IdentityUser? user = await _userManager.FindByIdAsync(hairdresser.UserId);
        
        //Deleting hairdresser
        _context.Hairdressers.Remove(hairdresser);
        await _context.SaveChangesAsync();

        //Deleting user
        if(user != null)
        {
            await _userManager.DeleteAsync(user);
        }
    
        return RedirectToAction(nameof(GetHairdressers));
    }

}
