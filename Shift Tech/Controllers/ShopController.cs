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
        //Getters
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
        public List<Cart> GetCarts()
        {
            return _context.Carts
                 .Include(x => x.User)
                 .Include(x => x.Products)
                 .ThenInclude(x => x.Product)
                 .ThenInclude(product => product.MainImage)
                 .Include(x => x.Products)
                 .ThenInclude(x => x.Product)
                 .ThenInclude(product => product.Category)
                    .Include(x => x.Products)
                 .ThenInclude(x => x.Product)
                 .ThenInclude(product => product.Reviews)
                     .Include(x => x.Products)
                 .ThenInclude(x => x.Product)
                 .ThenInclude(product => product.Images)
                 .ToList();
        }
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
        public List<Category> GetShopListCategories(List<Product> products)
        {
            return products.OrderBy(product => product.CategoryId)
                                .Select(product => product.Category)
                                .Distinct()
                                 .Take(6)
                                 .ToList();
        }
        public List<Product> FilterProductsByPrice(List<Product> products,PriceRange filter)
        {
            return products
              .Where(x =>
             x.Price >= filter.StartPrice && x.Price <= filter.EndPrice).ToList();
        }
        public List<Product> FilterProductsByCategory(List<Product> products,List<Category> selectedCategories)
        {
            if(selectedCategories.Count == 0) return products;
            return products
              .Where(x => selectedCategories.Any(c => c.Id == x.CategoryId)).ToList();
        }
        public PriceRange GetPriceRange(List<Product> products)
        {
            return new PriceRange() { StartPrice = (int)Math.Floor(products.Min(x => x.Price)), EndPrice = (int)Math.Floor(products.Max(x => x.Price)) };

        }
        //Home page
        public IActionResult Index()
        {

            return View(new { FeaturedCategories = GetTopCategories(), FeaturedProducts = GetTopProducts(), RecentProducts = GetRecentProducts() });
            // return View();
        }
        //Product Detail
        [HttpGet]
        public IActionResult ProductDetail(int id)
        {
            var product = GetProducts().First(x=> x.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return RedirectToAction("ShowProductDetail", new { productId = id });
        }
        [HttpGet("Shop/ProductDetail/{productId}")]
        public IActionResult ShowProductDetail(int productId)
        {
            var product = GetProducts().First(x => x.Id == productId);


            if (product == null)
            {
                return NotFound();
            }
            var sameProducts = GetProducts().Where(x => x.Category == product.Category).Where(x=> x.Id != productId).OrderByDescending(x => x.Purchases.Count).Take(6).ToList();
            return View("ProductDetail", new { Product = product, SameProducts = sameProducts });
        }
        //Review
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] Review review)
        {
            var user = await _userManager.GetUserAsync(User);
            review.Date = DateTime.Now;
            review.Publisher = user;
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return Ok("Success!");


        }
        //Cart
        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            int itemcount = 0;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var cart = GetCarts().FirstOrDefault(x => x.UserId == user.Id);

                itemcount = cart == null ? 0 : cart.Products.Count;
            }
            return Ok(new {Count = itemcount });
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = GetCarts().FirstOrDefault(x => x.UserId == user.Id);
            if (cart == null)
            {
                cart = new Cart()
                {
                    UserId = user.Id,
                };
            }
            var product = await _context.Products.FindAsync(model.productId);
            var cartproduct = new CartProduct()
            {
                Product = product,
                ProductCount = model.productAmount
            };

            var find = cart.Products.FirstOrDefault(x => x.Product.Id == cartproduct.Product.Id);
            if (find != null)
            {
                find.ProductCount += cartproduct.ProductCount;
            }
            else
            {
                _context.CartProducts.Add(cartproduct);

                cart.Products.Add(cartproduct);
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
            return Ok("Success!");
        }
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = GetCarts().FirstOrDefault(x => x.UserId == user.Id);
            if (cart == null)
            {
                cart = new Cart()
                {
                    UserId = user.Id,
                };
            }
            return View(cart);
        }
        [HttpGet]
        //Product List
        public async Task<IActionResult> ProductList()
        {
            ViewData["Products"] = GetProducts();
            ViewData["Categories"] = GetShopListCategories(GetProducts());
            ViewData["SelectedCategories"] = new List<Category>();
            ViewData["PriceRange"] = GetPriceRange(GetProducts());
            return View();
        }
        //Filter by Price
        public async Task<IActionResult> ProductList(PriceRange filter)
        {
            var products = GetProducts();
            var categories = ViewData["SelectedCategories"] as List<Category>;
            products = FilterProductsByCategory(products, categories);
            ViewData["Products"] = FilterProductsByPrice(products, filter);
            ViewData["PriceRange"] = filter;
            return Ok();
        }
        //Filter by Category
        public async Task<IActionResult> ProductList(List<Category> selectedCategories)
        {
            var products = GetProducts();
            var pricerange = ViewData["PriceRange"] as PriceRange;
            products = FilterProductsByCategory(products, selectedCategories);
            products = FilterProductsByPrice(products, pricerange);
            ViewData["Products"] = products;
            ViewData["SelectedCategories"] = selectedCategories;
            ViewData["PriceRange"] = GetPriceRange(products);
            return Ok();
        }
        //Sort
        public async Task<IActionResult> ProductList(SortOptions options)
        {
            var products = ViewData["Products"] as List<Product>;
            switch (options.SortOption)
            {
                case SortOption.Top:
                    products = products.OrderByDescending(x => x.Purchases.Count()).ToList();
                    break;
                case SortOption.Recent:
                    products = products.OrderByDescending(x => x.Date).ToList();
                    break;
                case SortOption.CheapestToExpensive:
                    products = products.OrderBy(x => x.Price).ToList();
                    break;
                case SortOption.ExpensiveToCheapest:
                    products = products.OrderByDescending(x => x.Price).ToList();
                    break;
                case SortOption.ByRating:
                    products = products.OrderByDescending(x => x.Reviews.Average(r => r.Rating)).ToList();
                    break;
            }
            ViewData["Products"] = products;
            return Ok();
        }
        //Search
        public async Task<IActionResult> ProductList(string searchStr)
        {
            var products = GetProducts().Where(x => x.Name.ToLower().Contains(searchStr.ToLower())).ToList();
            ViewData["Products"] = products;
            ViewData["Categories"] = GetShopListCategories(products);
            ViewData["SelectedCategories"] = new List<Category>();
            ViewData["ProductRange"] = GetPriceRange(products);
                return View();
        }
    }
}

