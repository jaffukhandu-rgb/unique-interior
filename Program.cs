using Microsoft.EntityFrameworkCore;
using Unique_1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InteriorShopDbContext>(options =>
        options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL")));
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddSession();

var app = builder.Build();

// ✅ THIS IS CORRECT WAY
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // SHOW REAL ERROR
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();