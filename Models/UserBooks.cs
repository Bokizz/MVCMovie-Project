
using System.ComponentModel.DataAnnotations;

namespace BookProject1.Models
{
    public class UserBooks
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        [StringLength(450, MinimumLength = 3)] public string? AppUser { get; set; }
    }
}
