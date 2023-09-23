using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shift_Tech.DbModels;
using Shift_Tech.Models;
using Shift_Tech.ViewModels;
using System.ComponentModel;

namespace Shift_Tech.Controllers
{
    public class ShopController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ShopController(UserManager<User> userManager, SignInManager<User> signInManager, ShopDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public List<Product> GetProducts() => _context.Products
                .Include(x => x.Category)
                .Include(x => x.Images)
            .Include(x => x.MainImage)
            .Include(x => x.Reviews)
            .Include(x => x.Purchases)
            .ToList();
        public List<Category> GetCategories()
        {
            return _context.Categories.Include(x => x.Image).Include(x => x.Products).ToList();
        }
        public List<Category> GetTopCategories()
        => GetCategories()
                .Take(4)
                .ToList();

        public List<Product> GetCurrentPage(List<Product> products, int page)
        {
            return products.Skip((page - 1) * 9).Take(9).ToList();
        }
        public int GetPageCount(List<Product> products)
        {
            return products.Count / 9 + (products.Count % 9 == 0 ? 0 : 1);
        }

        public List<Product> GetTopProducts() =>
                GetProducts()
                .OrderByDescending(x => x.Purchases.Count)
                .Take(8)
                .ToList();

        public List<Product> GetRecentProducts() =>
            GetProducts()
            .OrderByDescending(x => x.Date)
            .Take(8)
            .ToList();
        public IActionResult Index()
        {

            return View(new { FeaturedCategories = GetTopCategories(), FeaturedProducts = GetTopProducts(), RecentProducts = GetRecentProducts() });
            // return View();
        }
        public ShopFilter CreateShopFilter(List<Product> products)
        {
            return new()
            {
                SearchString = "",
                StartPrice = Convert.ToInt32(Math.Floor(products.Min(x => x.Price))),
                EndPrice = Convert.ToInt32(Math.Floor(products.Max(x => x.Price))),
                CurrentPage = 1,
                PageCount = GetPageCount(products),
                SelectedCategories = new List<Category>()

            };
        }
        public List<Category> GetShopListCategories(List<Product> products)
        {
            return products.OrderBy(product => product.CategoryId)
                                .Select(product => product.Category)
                                .Distinct()
                                 .Take(6)
                                 .ToList();
        }
        public List<Product> FilterProducts(ShopFilter filter, List<Product> products)
        {
            return products
              .Where(x =>
              x.Name.ToLower().Contains(filter.SearchString.ToLower()) ||
              x.ShortDescription.ToLower().Contains(filter.SearchString.ToLower()) ||
              x.Description.ToLower().Contains(filter.SearchString.ToLower()))
              .Where(x =>
                  x.Price >= filter.StartPrice && x.Price <= filter.EndPrice
              )
              .Where(x => filter.SelectedCategories.Count == 0 || filter.SelectedCategories.Any(c => c == x.Category)).ToList();
        }
        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            return View(new ProductListViewModel() { Products = GetProducts(), Categories = GetShopListCategories(GetProducts()), Filter = CreateShopFilter(GetProducts()) });
        }
        [HttpGet]
        public IActionResult ProductDetail(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound(); 
            }
            return RedirectToAction("ShowProductDetail", new { productId = id });
        }
        [HttpGet("Shop/ProductDetail/{productId}")]
        public IActionResult ShowProductDetail(int productId)
        {
            var product = _context.Products.Find(productId);

            if (product == null)
            {
                return NotFound(); 
            }
            var sameProducts = GetProducts().Where(x=> x.Category == product.Category).OrderByDescending(x => x.Purchases.Count).Take(6).ToList();
            return View("ProductDetail", new { Product = product, SameProducts = sameProducts }); 
        }
        public async Task<IActionResult> ProductList(ProductListViewModel model)
        {
            model.Products = FilterProducts(model.Filter, GetProducts());
            model.Filter.CurrentPage = 1;
            model.Filter.PageCount = GetPageCount(model.Products);
            model.Categories = GetShopListCategories(model.Products);
            return View(model);
        }
        public async Task<IActionResult> ProductList(ProductListViewModel model, int page)
        {
            model.Filter.CurrentPage = page;
            return View(model);
        }
        public async Task<IActionResult> ProductList(ProductListViewModel model, SortOptions options)
        {

            switch (options.SortOption)
            {
                case SortOption.Top:
                    model.Products = model.Products.OrderByDescending(x => x.Purchases.Count()).ToList();
                    break;
                case SortOption.Recent:
                    model.Products = model.Products.OrderByDescending(x => x.Date).ToList();
                    break;
                case SortOption.CheapestToExpensive:
                    model.Products = model.Products.OrderBy(x => x.Price).ToList();
                    break;
                case SortOption.ExpensiveToCheapest:
                    model.Products = model.Products.OrderByDescending(x => x.Price).ToList();
                    break;
                case SortOption.ByRating:
                    model.Products = model.Products.OrderByDescending(x => x.Reviews.Average(r => r.Rating)).ToList();
                    break;
            }
            return View(model);
        }
    }
}

