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
using Microsoft.CodeAnalysis.Elfie.Serialization;
using HairdresserApp.Interfaces;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;

namespace HairdresserApp.Controllers;

//Controller inheriting from instance controller
public class BookingController : Controller
{

    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;

    //Constructor
    public BookingController(ApplicationDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    /* GET */
    //Getting bookings
    [Authorize]
    public async Task<IActionResult> GetBookings()
    {
        //Getting bookings
        List<Booking> bookings = await _context.Booking.Where(booking => booking.DateAndTime >= DateTime.Today && booking.DateAndTime < DateTime.Today.AddDays(1)).Include(booked => booked.BookedServices).ThenInclude(service => service.Service).Include(booked => booked.Hairdresser).Include(booked => booked.Customer).OrderBy(booking => booking.DateAndTime).ToListAsync();
        ViewBag.Hairdressers = new SelectList(_context.Hairdressers, "Id", "Fname");

        return View(bookings);
    }

    //Filtering bookings
    [Authorize]
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

    /* CREATE */
    //Returning view to bookingform
    public async Task<IActionResult> CreateBooking(int serviceId, string serviceTitle)
    {
        
        BookingViewModel bookingVm = new BookingViewModel();

        //Getting hairdresser that offer the given service
        List<Hairdresser> hairdresser = await _context.Hairdressers!.Where(hairdresser => hairdresser.OfferedServices.Any(offeredService => offeredService.ServiceId == serviceId)).ToListAsync();

        bookingVm.Hairdressers = new SelectList(hairdresser, "Id", "Fname");
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

                    //Getting hairdresser name
                    ViewData["Hairdresser"] = await GetHairdresserName(bookingVm.SelectedHairdresserId);

                    //Getting service price
                    bookingVm.ServicePrice = await GetServicePrice(bookingVm.ServiceId) ?? 0;

                    bookingVm.BookingStep = 5;
    
                } 
                else
                {
                    //Set step
                    bookingVm.BookingStep = 4;
                }

            }
        } 
        else if(bookingVm.BookingStep == 4)
        {

            if(ModelState.IsValid)
            {
                //Getting hairdresser name
                ViewData["Hairdresser"] = await GetHairdresserName(bookingVm.SelectedHairdresserId);

                //Getting service price
                bookingVm.ServicePrice = await GetServicePrice(bookingVm.ServiceId) ?? 0;

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

                //Validating time is available with given hairdresser
                Booking? unavailable = _context.Booking!.FirstOrDefault(booking => booking.DateAndTime == bookingVm.SelectedDateAndTime && bookingVm.SelectedHairdresserId == booking.HairdresserId);

                if(unavailable != null)
                {
                    TempData["message"] = "Tiden är inte längre tillgänglig. Vänligen välj en annan tid.";
                    return View(nameof(ConfirmBooking), bookingVm);
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

                //Check if everything succeeded
                
            
                
                //Sending email
                await _emailSender.SendEmailAsync(bookingVm.Email!, bookingVm.SelectedDateAndTime, bookingVm.ServiceTitle!, bookingVm.ServicePrice);

                return View(nameof(ConfirmBooking), bookingVm);
            }
            
        }

        return View(bookingVm);
    }

    /* CONFIRMATION */
    public IActionResult ConfirmBooking()
    {

        return View();
    }

    /* EDIT */
    //Returning view to edit booking
    [Authorize]
    public async Task<IActionResult> UpdateBooking(int id)
    {

        //Fetching booking
        Booking? booking = await _context.Booking
            .Include(booking => booking.BookedServices)
            .ThenInclude(bookedService => bookedService.Service)
            .Include(booking => booking.Customer)
            .FirstOrDefaultAsync(booking => booking.Id == id);

        if(booking == null)
        {
            return NotFound();
        }

        //Setting ViewModel
        UpdateBookingViewModel bookingVm = new UpdateBookingViewModel();
        bookingVm.BookingStep = 1;

        bookingVm.BookingId = id;
        bookingVm.ServiceTitle = booking.BookedServices.First().Service!.Title;
        bookingVm.ServicePrice = booking.BookedServices.First().Service!.Price ?? 0;

        bookingVm.SelectedDateAndTime = booking.DateAndTime;

        bookingVm.SelectedHairdresserId = booking.HairdresserId;

        //Getting hairdresser that offer the given service
        List<Hairdresser> hairdresser = await _context.Hairdressers!.Where(hairdresser => hairdresser.OfferedServices.Any(offeredService => offeredService.ServiceId == booking.BookedServices.First().ServiceId)).ToListAsync();
        bookingVm.Hairdressers = new SelectList(hairdresser, "Id", "Fname");

        bookingVm.Email = booking.Customer!.Email;

        return View(bookingVm);
    }

    //Updating booking
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBooking(UpdateBookingViewModel updateBookingVm)
    {

        if(updateBookingVm.BookingStep == 1)
        {

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
                List<DateTime> bookedDates = await _context.Booking!.Where(booking => booking.HairdresserId == updateBookingVm.SelectedHairdresserId && 
                    booking.DateAndTime >= weekDays.First().Date && //Where date is after/at startdate
                    booking.DateAndTime <= weekDays.Last().Date.AddDays(1)) //Where date is before/at enddate
                    .Select(booking => booking.DateAndTime) //Return date and time
                    .ToListAsync();

                ViewBag.BookedHours = bookedDates;

                //Set step
                updateBookingVm.BookingStep = 2;

            } else if(updateBookingVm.BookingStep == 2)
            {
                //Creating select-list
                updateBookingVm.Hairdressers = new SelectList(_context.Hairdressers, "Id", "Fname");
            }

        }
        else if(updateBookingVm.BookingStep == 2)
        {
            if(ModelState.IsValid)
            {

                //Fetching booking
                Booking? booking = await _context.Booking.FindAsync(updateBookingVm.BookingId);

                if(booking == null)
                {
                    return NotFound();
                }

                //Updating booking times
                booking.HairdresserId = updateBookingVm.SelectedHairdresserId;
                booking.DateAndTime = updateBookingVm.SelectedDateAndTime;

                await _context.SaveChangesAsync();                
                
                //Check if everything succeeded
            
                
                //Sending email
                await _emailSender.SendEmailAsync(updateBookingVm.Email!, updateBookingVm.SelectedDateAndTime, updateBookingVm.ServiceTitle!, updateBookingVm.ServicePrice);

                updateBookingVm.BookingStep = 3;
                
            }
            
        }

        return View(updateBookingVm);
        
    }

    /* DELETE */
    //Delete booking
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Booking.FindAsync(id);
        if(booking != null)
        {
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["message"] = "Tiden är avbokad. Mejl har skickats till kunden.";
            
        }

        return RedirectToAction(nameof(GetBookings));
    }
    
    //Function to get hairdresser name
    private async Task<string> GetHairdresserName(int? id)
    {

        if(id == null)
        {
            return "";
        }

        //Getting hairdresser name
        Hairdresser? hairdresser = await _context.Hairdressers!.SingleOrDefaultAsync(hairdresser => hairdresser.Id == id);

        if(hairdresser == null)
        {
            return "";
        }

        var hairdresserName = hairdresser.Fname + " " + hairdresser.Lname;
        
        return hairdresserName;
    }

    //Function to get service prive
    private async Task<decimal?> GetServicePrice(int id)
    {
        //Getting hairdresser name
        Service? service = await _context.Services!.SingleOrDefaultAsync(service => service.Id == id);

        Console.WriteLine("Pris:");
        Console.WriteLine(service?.Price);
        return service?.Price ?? 0;
    }


}