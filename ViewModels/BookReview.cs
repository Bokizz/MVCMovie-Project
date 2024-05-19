using BookProject1.Models;

namespace BookProject1.ViewModels
{
    public class BookReview
    {
        public string Title { get; set; }
        public int Id { get; set; } 
        public Review Review { get; set; } = new Review();
    }
}