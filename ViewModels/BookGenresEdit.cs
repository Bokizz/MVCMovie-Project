using Microsoft.AspNetCore.Mvc.Rendering;
using BookProject1.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace BookProject1.ViewModels
{
    public class BookGenresEdit
    {
        public Book Book { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }
        public IFormFile? FrontPageFile { get; set; }
        public IFormFile? PdfFile { get; set; }
    }
}
