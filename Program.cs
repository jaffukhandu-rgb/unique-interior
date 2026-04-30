using Microsoft.EntityFrameworkCore;
using Unique_1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InteriorShopDbContext>(options =>
        options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL")));
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