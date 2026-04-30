using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using Unique_1.Models;
namespace Unique_1.Controllers
{
    public class CustomerController : Controller
    {
        private readonly InteriorShopDbContext _context;

        public CustomerController(InteriorShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult SaveData([FromBody] CustomerViewModel model)
        {
            if (model == null)
                return Json("Model null ❌");

            var db = _context;
            try
            {
                // 🔥 EDIT
                if (model.OrderId > 0)
                {
                    var order = db.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.OrderDetails)
                        .FirstOrDefault(o => o.OrderId == model.OrderId);

                    if (order == null)
                        return Json("Order not found ❌");

                    // update customer
                    order.Customer.CustomerName = model.CustomerName;
                    order.Customer.Address = model.Address;
                    order.Customer.PhoneNumber = model.PhoneNumber;

                    // update totals
                    order.Discount = model.Discount;
                    order.Tax = model.Tax;
                    order.GrandTotal = model.GrandTotal;

                    // remove old items
                    db.OrderDetails.RemoveRange(order.OrderDetails);
                    db.SaveChanges();

                    // add new items
                    foreach (var item in model.Works)
                    {
                        db.OrderDetails.Add(new OrderDetail
                        {
                            CategoryId = item.CategoryId,
                            Unit = item.Unit.ToString(),
                            Rate = item.Rate,
                            Amount = item.Amount,
                            OrderId = order.OrderId
                        });
                    }

                    db.SaveChanges();

                    return Json("Updated Successfully ✅");
                }

                // 🔥 NEW SAVE
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                };

                db.Customers.Add(customer);
                db.SaveChanges();

                var orderNew = new Order
                {
                    CustomerId = customer.CustomerId,
                    Discount = model.Discount,
                    Tax = model.Tax,
                    GrandTotal = model.GrandTotal
                };

                db.Orders.Add(orderNew);
                db.SaveChanges();

                foreach (var item in model.Works)
                {
                    db.OrderDetails.Add(new OrderDetail
                    {
                        CategoryId = item.CategoryId,
                        Unit = item.Unit.ToString(),
                        Rate = item.Rate,
                        Amount = item.Amount,
                        OrderId = orderNew.OrderId
                    });
                }

                db.SaveChanges();

                return Json("Saved Successfully ✅");
            }
            catch (Exception ex)
            {
                return Json("Error ❌: " + ex.Message);
            }
        }
        public IActionResult Edit(int id)
        {
            var db = _context;
            var order = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return RedirectToAction("OrderList");

            ViewBag.EditData = new
            {
                orderId = order.OrderId,
                discount = order.Discount,
                tax = order.Tax,

                customer = new
                {
                    customerName = order.Customer.CustomerName,
                    address = order.Customer.Address,
                    phoneNumber = order.Customer.PhoneNumber
                },

                orderDetails = order.OrderDetails.Select(d => new
                {
                    categoryId = d.CategoryId,

                    // 🔥 FIX: convert string → number
                    unit = d.Unit,
                    rate = d.Rate,
                    amount = d.Amount
                })
            };

            return View("Index");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var db = _context;
            var order = db.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderId == id);

            if (order != null)
            {
                db.OrderDetails.RemoveRange(order.OrderDetails);
                db.Orders.Remove(order);
                db.SaveChanges();
            }

            return RedirectToAction("OrderList");
        }
        public IActionResult Dashboard()
        {
            var db = _context;
            var totalOrders = db.Orders.Count();
            var totalCustomers = db.Customers.Count();
            var totalRevenue = db.OrderDetails.Sum(x => x.Amount);
            var recentOrders = db.Orders
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer.CustomerName
                })
                .OrderByDescending(o => o.OrderId)
                .Take(5)
                .ToList();

            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.RecentOrders = recentOrders;

            return View();
        }
        public IActionResult Invoice(int id)
        {
            var db = _context;
            var order = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Category)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return RedirectToAction("OrderList"); // safety
            }

            return View(order); // ✅ THIS IS IMPORTANT
        }
        public IActionResult OrderList()
        {
            var db = _context;
            var data = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Category)
                .ToList();

            return View(data);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(user))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }

            base.OnActionExecuting(context);
        }
    }

}
