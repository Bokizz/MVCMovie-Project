using Microsoft.AspNetCore.Mvc.Rendering;
using BookProject1.Models;
namespace BookProject1.ViewModels
{
    public class AuthorViewModel
    {
        public IList<Author> Authors { get; set; }

        public string SearchString { get; set; }

       
    }
}
