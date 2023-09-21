using Shift_Tech.DbModels;

namespace Shift_Tech.Models
{
    public class ShopFilter
    {
        public int StartPrice { get; set; }
        public int EndPrice { get; set; }
        public List<Category> SelectedCategories { get; set; }  
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public string SearchString { get; set; } = "";
    }
}
