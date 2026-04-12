/* Model for customers table */

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HairdresserApp;

//Indexing email
[Index(nameof(Email), IsUnique = true)]
public class Customer
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
    [EmailAddress]
    public required string Email { get; set; }

    //Dependants
    public ICollection<Booking> Bookings { get; } = new List<Booking>();

}