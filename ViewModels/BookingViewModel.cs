using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HairdresserApp.ViewModels;

public class BookingViewModel
{
    public int ServiceId { get; set; }

    //Hairdresser selection
    public SelectList? Hairdressers { get; set; }

    [Display(Name = "Välj frisör")]
    public int SelectedHairdresserId { get; set; } 

    //Time selection
    public List<DateTime>? DateAndTime { get; set; }

    [Display(Name = "Välj tid")]
    public DateTime SelectedDateAndTime { get; set; } 

    //User selection
    [EmailAddress]
    public string? Email { get; set; }
    public int? CustomerId { get; set; }

    [Display(Name = "Förnamn")]
    public string? CustomerFname { get; set; }

    [Display(Name = "Efternamn")]
    public string? CustomerLname { get; set; }

    //Booking step to change between inputs
    public int BookingStep { get; set; } = 1;

}