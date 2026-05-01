using Microsoft.EntityFrameworkCore;
using Unique_1.Models;

var builder = WebApplication.CreateBuilder(args);
var test = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(test))
{
    throw new Exception("❌ DATABASE_URL IS NULL");
}

builder.Services.AddDbContext<InteriorShopDbContext>(options =>
{
    var conn = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrEmpty(conn))
        throw new Exception("DATABASE_URL missing");

    options.UseNpgsql(conn);
});
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddSession();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        await context.Response.WriteAsync(exception?.ToString() ?? "Error");
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();