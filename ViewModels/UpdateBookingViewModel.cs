// ViewMdeol for updating booking

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HairdresserApp.ViewModels;

public class UpdateBookingViewModel
{
    public int BookingId { get; set; }
    public string? ServiceTitle { get; set;}
    public decimal ServicePrice {get; set; }

    //Hairdresser selection
    public SelectList? Hairdressers { get; set; }

    [Display(Name = "Välj frisör")]
    [Required(ErrorMessage = "Välj frisör")]
    public int SelectedHairdresserId { get; set; } 

    //Time selection
    public List<DateTime>? DateAndTime { get; set; }

    [Display(Name = "Välj tid")]
    public DateTime SelectedDateAndTime { get; set; } 

    //User selection
    [EmailAddress]
    [Required(ErrorMessage = "Ange e-postadress")]
    public string? Email { get; set; }

    //Booking step to change between inputs
    public int BookingStep { get; set; } = 1;

}