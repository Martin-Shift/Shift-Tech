using Shift_Tech.DbModels;
using Shift_Tech.Models;

namespace Shift_Tech.ViewModels
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; }
        public PriceRange Filter { get; set; }
        public List<Category> Categories { get; set; }
    }
}
