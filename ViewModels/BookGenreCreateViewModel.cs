using BookProject1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace BookProject1.ViewModels
{
    public class BookGenreCreateViewModel
    {
        public Book Book { get; set; }
        public IEnumerable<int>? SelectedGenresCreate { get; set; }
        public IEnumerable<SelectListItem>? GenreListCreate { get; set; }
        public IFormFile? FrontPageFile { get; set; }
        public IFormFile? PdfFile { get; set; }
    }
}
