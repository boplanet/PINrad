using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using PINrad.Data;
using PINrad.Models;

var builder = WebApplication.CreateBuilder(args);

// Dodavanje servisa aplikaciji
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<RegLogUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Onemogu�ite potvrdu za testiranje
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true; //�isto za test na 1
});
// Primijeni globalnu autorizaciju
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Kolacici
builder.Services.ConfigureApplicationCookie(options =>
{
   // options.Cookie.HttpOnly = true;
   // options.Cookie.SameSite = SameSiteMode.Strict; // None - kompatibilnost stari brows ili vanjski mehanizmi autentifikacije
   // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
   // options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Account/Login"; // Putanja do stranice za prijavu
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied"; // Putanja do stranice za zabranu pristupa
   // options.SlidingExpiration = true;
});
/*
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365); // po potrebi podesi ra�una u danima
    options.ExcludedHosts.Add("localhost");
});
*/
// Iznad
var app = builder.Build();

// Inicijalizacija baze podataka s po�etnim podacima  (poziv DbInitializer klase)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<RegLogUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await DbInitializer.InitializeAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Gre�ka inicijalizacije DB.");
    }
}

// Middleware za rukovanje pogre�kama, HTTPS preusmjeravanje, stati�ke datoteke, i usmjeravanje
if (app.Environment.IsDevelopment())
{
   // app.UseHsts(); //HSTS (HTTP Strict Transport Security)
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); //HTTP do HTTPS
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Mora biti prije Authorization
app.UseAuthorization();

// Postavljanje po�etne stranice kao Login stranica
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


// Dodavanje RazorPages ako je potrebno
app.MapRazorPages();

await app.RunAsync();
