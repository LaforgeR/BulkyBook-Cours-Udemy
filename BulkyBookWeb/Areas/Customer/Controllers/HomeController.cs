using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWOrk;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWOrk = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWOrk.Product.GetAll(includeProperties:"Category,CoverType");
            return View(productList);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                Product = _unitOfWOrk.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType")
        };
            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cart = _unitOfWOrk.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId
                );

            if(cart == null)
            {
                _unitOfWOrk.ShoppingCart.Add(shoppingCart);
                _unitOfWOrk.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, 
                    _unitOfWOrk.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);

            }
            else
            {
                _unitOfWOrk.ShoppingCart.IncrementCount(cart, shoppingCart.Count);
                _unitOfWOrk.Save();
            }
            return Redirect(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}