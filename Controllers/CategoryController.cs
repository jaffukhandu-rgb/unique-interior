using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Unique_1.Models;

namespace Unique_1.Controllers
{
    public class CategoryController : Controller
    {
        InteriorShopDbContext db = new InteriorShopDbContext();

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetCategory()
        {
            var data = db.Categories.ToList();
            return Json(data);
        }

        public JsonResult AddCategory(Category c)
        {
            if (string.IsNullOrWhiteSpace(c.CategoryName))
            {
                return Json("Category name required");
            }
            var exists = db.Categories
           .Any(x => x.CategoryName.ToLower() == c.CategoryName.ToLower());

            if (exists)
            {
                return Json("Category already exists");
            }
            db.Categories.Add(c);
            db.SaveChanges();

            return Json("Added");

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var action = context.RouteData.Values["action"]?.ToString();

            if (action != "GetCategory")
            {
                var user = HttpContext.Session.GetString("User");

                if (string.IsNullOrEmpty(user))
                {
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}