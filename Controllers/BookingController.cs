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

    /* GET */
    //Getting bookings
    public async Task<IActionResult> GetBookings()
    {
        //  QUERY FOR HAIRDRESSER OR CUSTROMER Id


        // Getting all bookings
        var bookings = await _context.Booking.Include(booked => booked.BookedServices).Include(booked => booked.Hairdresser).Include(booked => booked.Customer).ToListAsync();
        
        return View(bookings);
    }

    //Get specific booking
    public async Task<IActionResult> GetBooking(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var booking = await _context.Booking.Include(booked => booked.BookedServices).Include(booked => booked.Hairdresser).Include(booked => booked.Customer).FirstOrDefaultAsync(m => m.Id == id);

        if(booking == null)
        {
            return NotFound();
        }

        return View(booking);
        
    }

    /* CREATE */
    //Returning view to bookingform
    public async Task<IActionResult> CreateBooking()
    {
        BookingViewModel bookingVm = new BookingViewModel();
        bookingVm.Hairdressers = new SelectList(_context.Hairdressers, "Id", "Fname");
        
        return View(bookingVm);
    }

    //Add booking
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBooking([Bind("DateAndTime,TotalCost,HairdresserId,CustomerId")] Booking booking)
    {
        if(ModelState.IsValid)
        {
            _context.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); //RETURN SOMETHING ELSE
        }

        return View(booking);
    }

    /* DELETE */
    //Delete booking
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Booking.FindAsync(id);
        if(booking != null)
        {
            _context.Booking.Remove(booking);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
