using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookProject1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BookProject1.Areas.Identity.Data;


namespace BookProject1.Data
{
    public class BookProject1Context : IdentityDbContext<BookProject1User>
    {
        public BookProject1Context (DbContextOptions<BookProject1Context> options)
            : base(options)
        {
        }

        public DbSet<BookProject1.Models.Book> Book { get; set; } = default!;
        public DbSet<BookProject1.Models.Author> Author { get; set; } = default!;
        public DbSet<BookProject1.Models.BookGenre> BookGenre { get; set; } = default!;
        public DbSet<BookProject1.Models.Genre> Genre { get; set; } = default!;
        public DbSet<BookProject1.Models.Review> Review { get; set; } = default!;
        public DbSet<BookProject1.Models.UserBooks> UserBooks { get; set; } = default!;
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
