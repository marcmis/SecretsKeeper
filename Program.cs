using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SecretKeeper.Models;
using SecretsKeeper.Models;

var builder = WebApplication.CreateBuilder(args);

// Dodanie Swaggera
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Secrets Keeper API",
        Version = "v1"
    });
});

// Konfiguracja us³ug
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SecretsKeeperContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MySqlConnectionString")));
builder.Services.AddSession();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Odczyt ustawienia Swagger z appsettings.json
var redirectToSwagger = builder.Configuration.GetSection("SwaggerSettings:RedirectToSwagger").Get<bool>();

var app = builder.Build();

// Dodanie Swaggera
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Secrets Keeper API v1");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Przekierowanie do Swaggera lub domyœlnej strony
if (redirectToSwagger)
{
    app.MapGet("/", () => Results.Redirect("/swagger"));
}
else
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Mapowanie trasy dla kontrolerów
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
