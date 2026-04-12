/* Model for Service table */

using System.ComponentModel.DataAnnotations;

namespace HairdresserApp.Models;

public class Service
{
    //Properties
    public int Id { get; set; }

    [Required]
    [MaxLength(35), MinLength(3)]
    public required string Title { get; set; }

    [MaxLength(250)]
    public string? Description { get; set; }

    [Required]
    [Range(0,10000)]
    public decimal Price { get; set; }

    [Required]
    public required TimeSpan Duration { get; set; }

    //Dependants
    public ICollection<OfferedService> OfferedServices { get; } = new List<OfferedService>();
    public ICollection<BookedService> BookedServices { get; } = new List<BookedService>();
}