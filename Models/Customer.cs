/* Model for customers table */

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HairdresserApp.Models;

//Indexing email
[Index(nameof(Email), IsUnique = true)]
public class Customer
{
    //Properties
    public int Id { get; set; }

    [Required(ErrorMessage = "Ange förnamn")]
    [MaxLength(30)]
    public required string Fname { get; set; }

    [Required(ErrorMessage = "Ange efternamn")]
    [MaxLength(40)]
    public required string Lname { get; set; }

    [Required(ErrorMessage = "Ange epostadress")]
    [EmailAddress]
    public required string Email { get; set; }

    //Dependants
    public ICollection<Booking> Bookings { get; } = new List<Booking>();

}