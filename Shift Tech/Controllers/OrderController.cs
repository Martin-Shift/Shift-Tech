using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shift_Tech.DbModels;

namespace Shift_Tech.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public OrderController(UserManager<User> userManager, SignInManager<User> signInManager, ShopDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> ManageOrders()
        {          
            return View(_context.Orders.Include(x => x.Products).ThenInclude(x => x.Product).ToList());
        }
        public async Task<IActionResult> EditOrderDetail(int orderId)
        {
            return View(_context.Orders.Include(x => x.Products).ThenInclude(x => x.Product).First(x=> x.Id == orderId));
        }
        public IActionResult DeleteOrder([FromBody] int orderId)
        {
            var order = _context.Orders.Include(x=> x.Products).First(x => x.Id == orderId);
            order.User = null;
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return Ok();
        }
    }
}
