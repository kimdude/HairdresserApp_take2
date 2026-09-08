using System.ComponentModel.DataAnnotations;

namespace HairdresserApp.ViewModels;

public class HairdresserViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Ange Förnamn")]
    public string? Fname { get; set; }

    [Required(ErrorMessage = "Ange Efternamn")]
    public string? Lname { get; set; }
}