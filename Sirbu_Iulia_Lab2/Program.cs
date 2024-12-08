using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sirbu_Iulia_Lab2.Data;
using Sirbu_Iulia_Lab2.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryContext")
    ?? throw new InvalidOperationException("Connection string 'LibraryContext' not found.")));

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityContextConnection")
    ?? throw new InvalidOperationException("Connection string 'IdentityContext' not found.")));


builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<IdentityContext>();


builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("OnlySales", policy =>
    {
        policy.RequireClaim("Department", "Sales");
    });
    opts.AddPolicy("SalesManager", policy =>
    {
        policy.RequireRole("Manager").RequireClaim("Department", "Sales");
    });
});

// Configure application cookies
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Register MVC and SignalR
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(10); 
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(20); 
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Database seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DbInitializer.Initialize(services);
}

// Middleware
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/Chat");
app.MapHub<NotificationHub>("/Notification");
app.MapRazorPages();

app.Run();
