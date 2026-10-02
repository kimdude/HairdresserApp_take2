// View for booking and hairdressers

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HairdresserApp.ViewModels;

public class BookingViewModel
{
    //Service properties
    public int ServiceId { get; set; }
    public string? ServiceTitle { get; set;}
    public decimal ServicePrice {get; set; }

    //Hairdresser selection
    public SelectList? Hairdressers { get; set; }

    [Display(Name = "Välj frisör")]
    [Required(ErrorMessage = "Välj frisör")]
    public int? SelectedHairdresserId { get; set; } 

    //Time selection
    [Display(Name = "Välj tid")]
    public DateTime SelectedDateAndTime { get; set; } 

    //User selection
    [EmailAddress]
    [Required(ErrorMessage = "Ange e-postadress")]
    public string? Email { get; set; }

    public int CustomerId { get; set; }

    [Display(Name = "Förnamn")]
    [Required(ErrorMessage = "Ange förnamn")]
    public string? CustomerFname { get; set; }

    [Display(Name = "Efternamn")]
    [Required(ErrorMessage = "Ange efternamn")]
    public string? CustomerLname { get; set; }

    //Booking step to change between inputs
    public int BookingStep { get; set; } = 1;

}