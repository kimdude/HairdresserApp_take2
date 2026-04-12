/* Model for Hairdresser table */

using System.ComponentModel.DataAnnotations;

namespace HairdresserApp.Models;

public class Hairdresser
{
    //Properties
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Fname { get; set; }

    [Required]
    [MaxLength(40)]
    public required string Lname { get; set; }

    public string? UserId { get; set; }

    //Dependants
    public ICollection<OfferedService> OfferedServices { get; } = new List<OfferedService>();
    public ICollection<Booking> Bookings { get; } = new List<Booking>();
}