using BookProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookProject1.Models
{
    public class BookGenre
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

    }
}
