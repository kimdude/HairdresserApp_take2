using HairdresserApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HairdresserApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Models.Hairdresser>? Hairdressers { get; set; }
    public DbSet<Models.Service>? Services { get; set; }
    public DbSet<Models.Customer>? Customers { get; set; }
    public DbSet<Models.OfferedService>? OfferedServices { get; set; }
    public DbSet<Models.BookedService>? BookedServices { get; set; }
    public DbSet<Models.Booking> Booking { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //1:1 Relationship between Hairdresser and IdentityUser
        builder.Entity<Hairdresser>().HasOne(h => h.User).WithOne().HasForeignKey<Hairdresser>(h=>h.UserId).IsRequired();
    }
}