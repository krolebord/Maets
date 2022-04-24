using AutoMapper.EquivalencyExpression;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Maets.Data;
using Maets.Domain.Constants;
using Maets.Domain.Entities.Identity;
using Maets.Extensions;
using Maets.Models.Exceptions;
using Maets.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddAuthorization(options => {
    options.AddPolicy(PolicyNames.Admin, policyBuilder => policyBuilder
        .RequireAuthenticatedUser()
        .RequireRole(RoleNames.Admin)
        .Build());
    options.AddPolicy(PolicyNames.AdminOrModerator, policyBuilder => policyBuilder
        .RequireAuthenticatedUser()
        .RequireRole(RoleNames.Admin, RoleNames.Moderator)
        .Build());
});

builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = new PathString("/Identity/Account/Login");
        options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
    });

builder.Services.AddRazorPages(options => {
    options.Conventions.AuthorizeFolder("/collection");
    options.Conventions.AuthorizeFolder("/stats", PolicyNames.AdminOrModerator);
});
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

var assembly = typeof(Program).Assembly;

builder.Services.AddAutoMapper(
    config => config.AddCollectionMappers(),
    assembly
);
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

#if !DEBUG
app.UseHttpsRedirection();
#endif

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

app.Use(async (context, next) => {
    try
    {
        await next(context);
    }
    catch (NotFoundException)
    {
        context.Response.Redirect("/notfound");
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=About}/{action=Home}/{id?}"
);
app.MapRazorPages();

app.Run();
