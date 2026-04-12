/* Model for Offered Services table */

using Microsoft.EntityFrameworkCore;

namespace HairdresserApp.Models;

//Composite primary key
[PrimaryKey(nameof(HairdresserId), nameof(ServiceId))]
public class OfferedService
{
    //Properties
    public int HairdresserId { get; set; }
    public int ServiceId { get; set; }

    //Foreign keys reference
    public Hairdresser? Hairdresser { get; set; }
    public Service? Service { get; set; }
}