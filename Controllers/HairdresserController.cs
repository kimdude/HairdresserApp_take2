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
        var hairdressers = await _context.Hairdressers!.ToListAsync();
        
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

    /* CREATE */
    //Add hairdresser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddHairdresser([Bind("Fname,Lname")] Hairdresser hairdresser)
    {
        if (ModelState.IsValid)
        {
            //Adding Current UserId
            hairdresser.UserId = _userManager.GetUserId(User);
            
            _context.Add(hairdresser);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(hairdresser);
    }

    /* EDIT */
    //Update hairdresser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateHairdresser(int id, [Bind("Id, Fname,Lname")] Hairdresser hairdresser)
    {
        if(id != hairdresser.Id)
        {
            return NotFound();
        }

        if(ModelState.IsValid)
        {
            try
            {
                _context.Update(hairdresser);
                await _context.SaveChangesAsync();
            } 
            catch(DbUpdateConcurrencyException)
            {
                if(!HairdresserExists(hairdresser.Id))
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

        return View(hairdresser);
    }

    /* DELETE */
    //Delete hairdresser
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteHairdresser(int? id)
    {
        var hairdresser = await _context.Hairdressers!.FindAsync(id);

        if(hairdresser != null)
        {
            _context.Hairdressers.Remove(hairdresser);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    //Function to validate hairdresser ID
    private bool HairdresserExists(int id)
    {
        return _context.Hairdressers!.Any(e => e.Id == id);
    }
}
