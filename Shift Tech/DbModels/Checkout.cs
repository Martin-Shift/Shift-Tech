using System.ComponentModel.DataAnnotations.Schema;

namespace Shift_Tech.DbModels
{
    public class Checkout
    {
        public int Id { get; set; }
        public User User { get; set; }
        public List<CartProduct> Products { get; set; }
    }
}
 