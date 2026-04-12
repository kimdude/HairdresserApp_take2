/* Model for bookings table */

using System.ComponentModel.DataAnnotations;

namespace HairdresserApp;

public class Booking
{
    //Properties
    public int Id { get; set; }

    [Required]
    public required DateTime DateAndTime { get; set; }

    [Required]
    public required decimal TotalCost { get; set; }

    [Required]
    public required int HairdresserId { get; set; }

    [Required]
    public required int CustomerId { get; set; }

    //Foreign keys reference
    public Hairdresser? Hairdresser { get; set; }
    public Customer? Customer { get; set; }

    //Dependants
    public ICollection<BookedService> BookedServices { get; } = new List<BookedService>();

}