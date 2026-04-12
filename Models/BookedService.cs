/* Model for booked services table */

using Microsoft.EntityFrameworkCore;

namespace HairdresserApp;

//Composite primary key
[PrimaryKey(nameof(BookingId), nameof(ServiceId))]
public class BookedService
{
    //Properties
    public int BookingId { get; set; }
    public int ServiceId { get; set; }

    //Foreign key references
    public Booking? Booking { get; set; }
    public Service? Service { get; set; }
}