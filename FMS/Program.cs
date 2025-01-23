using FMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Enregistrer HttpClient dans DI
builder.Services.AddHttpClient();

// Enregistrer GitHubService dans le conteneur DI
builder.Services.AddScoped<GitHubService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
