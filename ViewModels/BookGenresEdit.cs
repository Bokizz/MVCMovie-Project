using Microsoft.AspNetCore.Mvc.Rendering;
using BookProject1.Models;
using System.Collections.Generic;
namespace BookProject1.ViewModels
{
    public class BookGenresEdit
    {
        public Book Book { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }

    }
}
