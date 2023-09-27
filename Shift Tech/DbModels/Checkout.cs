using System.ComponentModel.DataAnnotations.Schema;

namespace Shift_Tech.DbModels
{
    public class Checkout
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart Cart { get; set; }
    }
}
 