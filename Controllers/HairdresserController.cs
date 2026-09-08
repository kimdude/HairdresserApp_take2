using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HairdresserApp.Models;
using HairdresserApp.ViewModels;
using Microsoft.VisualBasic;
using HairdresserApp.Data;
using SQLitePCL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

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
    public async Task<IActionResult> GetHairdressers()
    {
        // Getting all hairdressers
        var hairdressers = await _context.Hairdressers!.Include(hairdresser => hairdresser.User).ToListAsync();
        
        return View(hairdressers);
    }

    //Getting specific hairdresser
    public async Task<IActionResult> HairdresserDetails(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        // Getting all hairdressers
        var hairdresser = await _context.Hairdressers!.Include(h => h.OfferedServices).FirstOrDefaultAsync(m => m.Id == id);

        if(hairdresser == null)
        {
            return NotFound();
        }
        
        return View(hairdresser);
    }

    /* EDIT */
    //Returning view with update form
    public async Task<IActionResult> UpdateHairdresser(int id)
    {
        Hairdresser? hairdresser = await _context.Hairdressers!.FindAsync(id);

        if(hairdresser == null)
        {
            return NotFound();
        }

        return View(hairdresser);
    }

    //Update hairdresser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateHairdresser([Bind("Id, Fname, Lname")] HairdresserViewModel hairdresserVm)
    {
        if(ModelState.IsValid)
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
            }

            return RedirectToAction(nameof(GetHairdressers));
        }

        return View(hairdresserVm);
    }

    /* DELETE */
    //Delete hairdresser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteHairdresser(int? id)
    {
        var hairdresser = await _context.Hairdressers!.FindAsync(id);

        if(hairdresser != null)
        {
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

        }

        return RedirectToAction(nameof(GetHairdressers));
    }

    //Function to validate hairdresser ID
    private bool HairdresserExists(int id)
    {
        return _context.Hairdressers!.Any(e => e.Id == id);
    }
}
