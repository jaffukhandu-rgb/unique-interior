using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Unique_1.Models;

public class AccountController : Controller
{
    InteriorShopDbContext db = new InteriorShopDbContext();

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = db.Users1
        .FirstOrDefault(x =>
        x.Username.ToLower().Trim() == username.ToLower().Trim() &&
        x.Password.Trim() == password.Trim()
         );
        if (user != null)
        {
            HttpContext.Session.SetString("User", user.Username);
            return RedirectToAction("OrderList", "Customer");
        }

        ViewBag.Error = "Invalid username or password";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.RouteData.Values["controller"]?.ToString();

        // ✅ allow all Account pages (Login, Logout)
        if (controller == "Account")
        {
            base.OnActionExecuting(context);
            return;
        }

        var user = HttpContext.Session.GetString("User");

        if (string.IsNullOrEmpty(user))
        {
            context.Result = RedirectToAction("Login", "Account");
        }

        base.OnActionExecuting(context);
    }
}