using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using bus.Models;

namespace bus.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<bus.Models.buses> buses { get; set; } = default!;
        public DbSet<BookingDetails> BookingDetails { get; set; }


    }
}
