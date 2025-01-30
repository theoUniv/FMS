using FMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Ajouter AppDbContext avec la chaîne de connexion
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter HttpClient pour les appels HTTP
builder.Services.AddHttpClient();

// Ajouter le service GitHubService
builder.Services.AddScoped<GitHubService>();
builder.Services.AddSingleton<AesCipherService>();
builder.Services.AddSingleton<JwtService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "FMS",
        ValidAudience = "FMS",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("UneCleSecretePourLeJWTQuiEstLongueAssezPourHS256"))
    };
});

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

app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        Console.WriteLine($"? Token reçu dans l'Authorization Header : {context.Request.Headers["Authorization"]}");
    }
    else
    {
        Console.WriteLine("?? Aucun token reçu dans l'Authorization Header !");
    }

    await next();
});


app.UseAuthentication();  // Ajout du middleware d'authentification
app.UseAuthorization();

// Définir les routes pour les contrôleurs
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
