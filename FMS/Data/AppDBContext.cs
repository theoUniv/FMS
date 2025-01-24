using FMS.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }  // Corrected to match the table name "Users"

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=DESKTOP-MVTS569;Database=FMS;Trusted_Connection=True;TrustServerCertificate=True;");

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .ToTable("Users")  // Map UserModel to the "Users" table in the database
            .HasKey(u => u.user_id);  // Set user_id as the primary key
    }
}
