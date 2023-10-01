using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shift_Tech.DbModels;

namespace Shift_Tech.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CheckoutController(UserManager<User> userManager, SignInManager<User> signInManager, ShopDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
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
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var carts = GetCarts();
            var cart = carts.First(x => x.User.Id == user.Id);
         
            return View(cart);
        }
    }
}
