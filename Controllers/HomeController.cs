using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsAndCategories.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.AllProducts = _context.Products.OrderBy(p => p.Name).ToList();
            return View();
        }

        [HttpPost("addProduct")]
        public IActionResult addProduct(Product newProduct)
        {
            if(ModelState.IsValid)
            {
                _context.Products.Add(newProduct);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.AllProducts = _context.Products.OrderBy(p => p.Name).ToList();
                return View("Index");
            }
        }

        [HttpGet("products/{productId}")]
        public IActionResult OneProduct(int productId)
        {
            Product one = _context.Products.Include(a => a.CategoryList).ThenInclude(c => c.Category).FirstOrDefault(p => p.ProductId == productId);
            ViewBag.AllCategories = _context.Categories.ToList();
            return View(one);
        }

        [HttpPost("addToProductAssociation")]
        public IActionResult addToProductAssociation(Association newCategory)
        {
            _context.Add(newCategory);
            // this creates initial association and then puts into category list on product side
            _context.SaveChanges();
            return Redirect($"/products/{newCategory.ProductId}");
        }

        [HttpGet("Categories")]
        public IActionResult Categories()
        {
            ViewBag.AllCategories = _context.Categories.ToList();
            return View();
        }

        [HttpPost("addCategory")]
        public IActionResult AddCategory(Category newCategory)
        {
            if(ModelState.IsValid)
            {
                _context.Categories.Add(newCategory);
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            else
            {
                ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name).ToList();
                return View("Categories");
            }
        }

        [HttpGet("categories/{categoryId}")]
        public IActionResult OneCategory(int categoryId)
        {
            Category one = _context.Categories.Include(a => a.ProductList).ThenInclude(c => c.Product).FirstOrDefault(p => p.CategoryId == categoryId);
            ViewBag.AllProducts = _context.Products.ToList();
            return View(one);
        }

        [HttpPost("addToCategoryAssociation")]
        public IActionResult addToCategoryAssociation(Association newProduct)
        {
            _context.Add(newProduct);
            _context.SaveChanges();
            return RedirectToAction($"/categories/{newProduct.CategoryId}");
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
