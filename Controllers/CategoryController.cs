using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Unique_1.Models;

namespace Unique_1.Controllers
{
    public class CategoryController : Controller
    {
        private readonly InteriorShopDbContext _context;

        public CategoryController(InteriorShopDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetCategory()
        {
            var data = _context.Categories.ToList();
            return Json(data);
        }

        public JsonResult AddCategory(Category c)
        {
            if (string.IsNullOrWhiteSpace(c.CategoryName))
            {
                return Json("Category name required");
            }
            var exists = _context.Categories
           .Any(x => x.CategoryName.ToLower() == c.CategoryName.ToLower());

            if (exists)
            {
                return Json("Category already exists");
            }
            _context.Categories.Add(c);
            _context.SaveChanges();

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