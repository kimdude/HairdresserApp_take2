/* Model for Hairdresser table */

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

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

    [Required]
    public string UserId { get; set; } = null!;

    public IdentityUser User { get; set; } = null!;

    //Dependants
    public ICollection<OfferedService> OfferedServices { get; } = new List<OfferedService>();
    public ICollection<Booking> Bookings { get; } = new List<Booking>();
}