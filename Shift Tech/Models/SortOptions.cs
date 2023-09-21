namespace Shift_Tech.Models
{
    public enum SortOption
    {
        Top, Recent, CheapestToExpensive, ExpensiveToCheapest,ByRating
    };
    public class SortOptions
    {
        public SortOption SortOption { get; set; }
    }
}
