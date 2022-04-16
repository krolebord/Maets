using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Maets.Data;
using Maets.Extensions;
using Maets.Options;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MailOptions>().BindConfiguration("MailOptions");
builder.Services.AddOptions<LocalFilesStorageOptions>().BindConfiguration(LocalFilesStorageOptions.ConfigurationKey);

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
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

var assembly = typeof(Program).Assembly;

builder.Services.AddDependencies(assembly);
builder.Services.AddSeedData(assembly);

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

var resourcesPath = app.Configuration[LocalFilesStorageOptions.ResourcesPathKey];
var resourcesFolder = Path.Combine(app.Environment.ContentRootPath, resourcesPath);
Directory.CreateDirectory(resourcesFolder);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(resourcesFolder),
    RequestPath = "/" + resourcesPath
});

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
