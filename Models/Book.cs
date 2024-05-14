using BookProject1.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace BookProject1.Models
{
    public class Book
    {
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string? Title { get; set; }

        [Display(Name = "Year published:")]
        public int? YearPublished { get; set; }
        [Display(Name = "Number of pages:")]
        public int? NumPages { get; set; }


        public string? Description { get; set; }
        [Display(Name = "Published by:")]
        public string? Publisher { get; set; }
        public string? FrontPage { get; set; }

        [Display(Name = "Download link:")]
        public string? DownloadUrl { get; set; }
        public int? AuthorId { get; set; }
        public Author? Author { get; set; }

        [Display(Name = "Genres:")]
        public ICollection<BookGenre>? BookGenres { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserBooks>? UserBooks { get; set; }
    }
}
