using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Studens.AspNetCore.Identity;
using Studens.AspNetCore.Mvc.Middleware.RegisteredServices;
using Studens.AspNetCore.Mvc.UI.TagHelpers.GoogleMaps;
using Studens.Commons.Extensions;
using Studens.Extensions.FileProviders;
using Studens.Extensions.FileProviders.FileSystem;
using Studens.MvcNet6.WebUI.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    var isDevelopment = context.HostingEnvironment.IsDevelopment();
    options.ValidateScopes = isDevelopment;
    options.ValidateOnBuild = isDevelopment;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDisplayRegisteredServices();

builder.Services.Configure<GoogleMapsOptions>(options =>
{
    options.UrlPlaceholder = "https://localhost:7132/";
    options.ApiKeys.Add("miki1", "miki1-value");
});

// Add db context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudensMvc6"));
}, ServiceLifetime.Scoped);

//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.Configure<PhysicalFileManagerOptions>(options => options.Path = "C://ITO");
builder.Services.AddScoped<IFileManager, PhysicalFileManager>();
builder.Services.AddScoped<FileProviderErrorDescriber>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddStudensEntityFrameworkStores<ApplicationDbContext>()
    .AddStudensRoleManager()
    .AddStudensUserManager()
    .AddStudensPasswordManager();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDisplayRegisteredServices();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseClaimsDisplay();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "auth_area",
    areaName: "Auth",
    pattern: "Auth/{controller=Account}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/files", async (context) =>
{
    var fileManager = context.RequestServices.GetService<IFileManager>();
    var fileName = Path.Combine(Directory.GetCurrentDirectory(), "upload.txt");
    using var fs = File.OpenRead(fileName);
    var bytes = fs.GetAllBytes();

    var result = await fileManager.SaveAsync(new PersistFileInfo(bytes, "tests.txt", "vlado/vlado2", false));
    var converted = JsonSerializer.Serialize(result);

    await context.Response.WriteAsync(converted);
});

app.MapGet("/files-delete", async (context) =>
{
    var fileManager = context.RequestServices.GetService<IFileManager>();

    var result = await fileManager.DeleteAsync("vlado/vlado2/test.txt");
    var converted = JsonSerializer.Serialize(result);

    await context.Response.WriteAsync(converted);
});

app.Run();