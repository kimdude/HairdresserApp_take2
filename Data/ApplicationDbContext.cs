using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HairdresserApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Hairdresser>? Hairdressers { get; set; }
    public DbSet<Service>? Services { get; set; }
    public DbSet<Customer>? Customers { get; set; }
    public DbSet<OfferedService>? OfferedServices { get; set; }
    public DbSet<BookedService>? BookedServices { get; set; }
    public DbSet<Booking> Booking { get; set; } = default!;
}
