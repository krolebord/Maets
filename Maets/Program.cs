using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Maets.Data;
using Maets.Extensions;
using Maets.Options;
using Maets.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MailOptions>().BindConfiguration("MailOptions");

// Add services to the container.
var authConnectionString = builder.Configuration.GetConnectionString("AuthConnection");
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(authConnectionString));

var dataConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MaetsDbContext>(options =>
    options.UseSqlServer(dataConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddSeedData(typeof(Program).Assembly);

var app = builder.Build();

await app.SeedAndMigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Store}/{action=Index}/{id?}"
);
app.MapRazorPages();

app.Run();
