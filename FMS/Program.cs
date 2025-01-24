using FMS.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ajouter AppDbContext avec la chaîne de connexion
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter HttpClient pour les appels HTTP
builder.Services.AddHttpClient();

// Ajouter le service GitHubService
builder.Services.AddScoped<GitHubService>();

// Ajouter les contrôleurs avec vues
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuration des middlewares
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Définir les routes pour les contrôleurs
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
