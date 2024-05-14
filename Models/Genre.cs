using BookProject1.Models;
using System.ComponentModel.DataAnnotations;

namespace BookProject1.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 3)] public string? GenreName { get; set; }

        public ICollection<BookGenre>? BookGenres { get; set; }
    }
}
