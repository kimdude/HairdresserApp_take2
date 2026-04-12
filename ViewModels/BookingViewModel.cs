using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HairdresserApp.ViewModels;

public class BookingViewModel
{
    public int ServiceId { get; set; }

    //Hairdresser selection
    public SelectList? Hairdressers { get; set; }
    public int SelectedHairdresserId { get; set; } 

    //Time selection
    public List<DateTime>? DateAndTime { get; set; }
    public DateTime SelectedDateAndTime { get; set; } 

    //User selection
    [EmailAddress]
    public string? Email { get; set; }
    public int? CustomerId { get; set; }

    public string? CustomerFname { get; set; }
    public string? CustomerLname { get; set; }

}