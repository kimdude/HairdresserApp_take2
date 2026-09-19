/* Model for Service table */

using System.ComponentModel.DataAnnotations;

namespace HairdresserApp.Models;

public class Service
{
    //Properties
    public int Id { get; set; }

    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "Titeln måste vara 3-35 tecken lång.")]
    [MaxLength(35), MinLength(3)]
    public required string Title { get; set; }

    [MaxLength(250)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Ange pris.")]
    [Range(1, 10000, ErrorMessage = "Ange pris.")]
    [DataType(DataType.Currency)]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Ange tidsspann.")]
    [Range(1, 600, ErrorMessage = "Ange tidsspann.")]
    public required int? Duration { get; set; }

    //Dependants
    public ICollection<OfferedService> OfferedServices { get; } = new List<OfferedService>();
    public ICollection<BookedService> BookedServices { get; } = new List<BookedService>();
}