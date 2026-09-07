using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HairdresserApp.Models;
using HairdresserApp.ViewModels;
using Microsoft.VisualBasic;
using HairdresserApp.Data;
using SQLitePCL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using HairdresserApp.Services;

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
        //Getting all bookings
        List<Booking> bookings = await _context.Booking.Where(booking => booking.DateAndTime >= DateTime.Today && booking.DateAndTime < DateTime.Today.AddDays(1)).Include(booked => booked.BookedServices).ThenInclude(service => service.Service).Include(booked => booked.Hairdresser).Include(booked => booked.Customer).OrderBy(booking => booking.DateAndTime).ToListAsync();
        ViewBag.Hairdressers = new SelectList(_context.Hairdressers, "Id", "Fname");

        return View(bookings);
    }

    //Filtering bookings
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetBookings(int hairdresser, string timeframe)
    {
        List<Booking> bookings = [];

        //Getting next weeks days
        var calendar = new SetupCalender();
        List<DateTime> weekDays = calendar.GetWeekdays(DateTime.Now);
        DateTime startDate = weekDays.First().Date;
        DateTime endDate = weekDays.Last().Date.AddDays(1);

        DateTime todayDate = DateTime.Today;
        DateTime tomorrowsDate = todayDate.AddDays(1);

        if(timeframe == "week" && hairdresser == 0)
        {
            //Filtering next week with all hairdressers
            bookings = await _context.Booking.Where(booking => booking.DateAndTime >= startDate && booking.DateAndTime <= endDate).Include(booked => booked.BookedServices).ThenInclude(service => service.Service).Include(booked => booked.Hairdresser).Include(booked => booked.Customer).OrderBy(booking => booking.DateAndTime).ToListAsync();
        } 
        else if (timeframe == "week" && hairdresser != 0) 
        {
            //Filtering next week by hairdresser
            bookings = await _context.Booking.Where(booking => booking.DateAndTime >= startDate && booking.DateAndTime <= endDate && booking.HairdresserId == hairdresser).Include(booked => booked.BookedServices).ThenInclude(service => service.Service).Include(booked => booked.Hairdresser).Include(booked => booked.Customer).OrderBy(booking => booking.DateAndTime).ToListAsync();  
        } else if(timeframe == "today" && hairdresser != 0)
        {
            //Filtering todays bookings by hairdresser
            bookings = await _context.Booking.Where(booking => booking.DateAndTime >= todayDate && booking.DateAndTime < tomorrowsDate && booking.HairdresserId == hairdresser).Include(booked => booked.BookedServices).ThenInclude(service => service.Service).Include(booked => booked.Hairdresser).Include(booked => booked.Customer).OrderBy(booking => booking.DateAndTime).ToListAsync();  

        } else
        {
            bookings = await _context.Booking.Where(booking => booking.DateAndTime >= todayDate && booking.DateAndTime < tomorrowsDate).Include(booked => booked.BookedServices).ThenInclude(service => service.Service).Include(booked => booked.Hairdresser).Include(booked => booked.Customer).OrderBy(booking => booking.DateAndTime).ToListAsync();
        }

        ViewBag.Hairdressers = new SelectList(_context.Hairdressers, "Id", "Fname");
        
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
    public IActionResult CreateBooking(int serviceId, string serviceTitle)
    {
        
        BookingViewModel bookingVm = new BookingViewModel();

        bookingVm.Hairdressers = new SelectList(_context.Hairdressers, "Id", "Fname");
        bookingVm.ServiceId = serviceId;
        bookingVm.ServiceTitle = serviceTitle;
        
        return View(bookingVm);
    }

    //Add booking
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBooking(BookingViewModel bookingVm)
    {

        //Setting booking steps
        if(bookingVm.BookingStep == 1)
        {

            //Validating only hairdresser
            ModelState.Remove(nameof(bookingVm.Email));
            ModelState.Remove(nameof(bookingVm.CustomerFname));
            ModelState.Remove(nameof(bookingVm.CustomerLname));
            ModelState.Remove(nameof(bookingVm.CustomerId));

            if(ModelState.IsValid)
            {
                /* Setting up calendar */
                var calendar = new SetupCalender();

                int week = calendar.GetWeek();
                ViewBag.Week = week;

                List<DateTime> weekDays = calendar.GetWeekdays(DateTime.Now);
                ViewBag.Day = weekDays;

                string[] startingHours = {"08:00", "09:00", "10:00", "11:00", "13:00", "14:00", "15:00", "16:00"};
                ViewBag.Hours = startingHours;   

                //Get datetimes of hairdresser within timerange
                List<DateTime> bookedDates = await _context.Booking!.Where(booking => booking.HairdresserId == bookingVm.SelectedHairdresserId && 
                    booking.DateAndTime >= weekDays.First().Date && //Where date is after/at startdate
                    booking.DateAndTime <= weekDays.Last().Date.AddDays(1)) //Where date is before/at enddate
                    .Select(booking => booking.DateAndTime) //Return date and time
                    .ToListAsync();

                ViewBag.BookedHours = bookedDates;

                //Set step
                bookingVm.BookingStep = 2;

            } else
            {
                //Creating select-list
                bookingVm.Hairdressers = new SelectList(_context.Hairdressers, "Id", "Fname");
            }

        } 
        else if(bookingVm.BookingStep == 2)
        {
            //Set step
            bookingVm.BookingStep = 3;

        }
        else if(bookingVm.BookingStep == 3)
        {

            //Validating only hairdresser & email
            ModelState.Remove(nameof(bookingVm.CustomerFname));
            ModelState.Remove(nameof(bookingVm.CustomerLname));
            ModelState.Remove(nameof(bookingVm.CustomerId));

            if(ModelState.IsValid)
            {

                //Check for existing customer
                var existingCustomer = await _context.Customers!.FirstOrDefaultAsync(customer => customer.Email == bookingVm.Email);

                if(existingCustomer != null)
                {   
                    //Setting customer values
                    bookingVm.CustomerId = existingCustomer.Id;
                    bookingVm.CustomerFname = existingCustomer.Fname;
                    bookingVm.CustomerLname = existingCustomer.Lname;
    
                }

                //Set step
                bookingVm.BookingStep = 4;

            }
        } 
        else if(bookingVm.BookingStep == 4)
        {

            if(ModelState.IsValid)
            {
                //Getting hairdresser name
                var hairdresser = await _context.Hairdressers!.SingleAsync(hairdresser => hairdresser.Id == bookingVm.SelectedHairdresserId);
                var hairdresserName = hairdresser.Fname + " " + hairdresser.Lname;

                ViewData["Hairdresser"] = hairdresserName;

                //Getting service price
                var service = await _context.Services!.SingleAsync(service => service.Id == bookingVm.ServiceId);
                var servicePrice = service.Price;

                bookingVm.ServicePrice = servicePrice;

                //Set step
                bookingVm.BookingStep = 5;
            }

        } else if(bookingVm.BookingStep == 5)
        {
            if(ModelState.IsValid)
            {
                
                //Create non-existing customer
                if(bookingVm.CustomerId == 0)
                {
                    var customer = new Customer
                    {
                        Email = bookingVm.Email!,
                        Fname = bookingVm.CustomerFname!,
                        Lname = bookingVm.CustomerLname!
                    };

                    _context.Customers!.Add(customer);
                    await _context.SaveChangesAsync();

                    bookingVm.CustomerId = customer.Id;
                }

                //Save booking
                var booking = new Booking
                {
                    DateAndTime = bookingVm.SelectedDateAndTime,
                    TotalCost = bookingVm.ServicePrice,
                    HairdresserId = bookingVm.SelectedHairdresserId!.Value,
                    CustomerId = bookingVm.CustomerId
                };

                _context.Booking.Add(booking);
                await _context.SaveChangesAsync();
                
                //Connecting to service table
                var bookedService = new BookedService
                {
                    BookingId = booking.Id,
                    ServiceId = bookingVm.ServiceId
                };

                _context.BookedServices!.Add(bookedService);
                await _context.SaveChangesAsync();
            }
            
        }

        return View(bookingVm);
    }

    /* DELETE */
    //Delete booking
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Booking.FindAsync(id);
        if(booking != null)
        {
            _context.Booking.Remove(booking);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(GetBookings));
    }

}