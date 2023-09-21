namespace Shift_Tech.DbModels
{
    public class Purchase
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductCount { get; set; }
        public User Customer { get; set; }
    }
}
