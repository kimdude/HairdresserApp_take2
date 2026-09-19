/* Model for Offered Services table */

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HairdresserApp.Models;

//Composite primary key
[PrimaryKey(nameof(HairdresserId), nameof(ServiceId))]
public class OfferedService
{
    //Properties
    [Range(1,1000000, ErrorMessage = "Ange frisör")]
    public int HairdresserId { get; set; }

    [Range(1,1000000, ErrorMessage = "Ange en tjänst")]
    public int ServiceId { get; set; }

    //Foreign keys reference
    public Hairdresser? Hairdresser { get; set; }
    public Service? Service { get; set; }
}