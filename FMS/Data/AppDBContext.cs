using FMS.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{

    /// <summary>
    /// Représente la collection d'utilisateurs dans la base de données.
    /// </summary>
    public DbSet<UserModel> Users { get; set; }
    public DbSet<GitHubLangageDataModel> GitHubLanguagesData { get; set; }
    
    public DbSet<GitHubYearlyStatsModel> GitHubYearlyStatsModel { get; set; }

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="AppDbContext"/> avec les options spécifiées.
    /// </summary>
    /// <param name="options">Options de configuration pour le contexte de la base de données.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configure les paramètres de connexion à la base de données.
    /// </summary>
    /// <param name="optionsBuilder">Le constructeur d'options de base de données.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=master;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");
    }


    /// <summary>
    /// Configure le modèle de données en mappant les entités aux tables de la base de données.
    /// </summary>
    /// <param name="modelBuilder">L'objet de configuration du modèle de données.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .ToTable("Users")  // Map UserModel to the "Users" table in the database
            .HasKey(u => u.user_id);  // Set user_id as the primary key

        modelBuilder.Entity<GitHubLangageDataModel>()
            .ToTable("GitHubLangagesData")
            .HasKey(g => g.id_github_langage_data);
        
        modelBuilder.Entity<GitHubYearlyStatsModel>()
            .ToTable("GitHubYearlyStats")
            .HasKey(g => g.id_github_yearly_stats);
    }
}
