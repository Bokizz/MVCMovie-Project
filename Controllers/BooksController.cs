using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookProject1.Data;
using BookProject1.Models;
using BookProject1.ViewModels;
namespace BookProject1.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookProject1Context _context;

        public BooksController(BookProject1Context context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string bookGenre, string searchString, string authorsearchString)
        {
            // Query the distinct genre names
            var genreQuery = _context.Genre
                .OrderBy(g => g.GenreName)
                .Select(g => g.GenreName)
                .Distinct();

            // Query the books
            var booksQuery = _context.Book
                .Include(b => b.Author)
                .Include(b => b.Reviews)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                    .Include(b => b.Author)
                .AsQueryable();
            var authorsQuery = _context.Author.AsQueryable();


            // Apply genre filter if provided
            if (!string.IsNullOrEmpty(bookGenre))
            {
                booksQuery = booksQuery.Where(b => b.BookGenres.Any(bg => bg.Genre.GenreName == bookGenre));
            }

            // Apply search string filter if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(authorsearchString)) // Fixed to check autsrch instead of searchString
            {
                // Filter by full name
                booksQuery = booksQuery.Where(a => (a.Author.FirstName + " " + a.Author.LastName).Contains(authorsearchString));
            }

            // Execute the queries asynchronously
            var genres = await genreQuery.ToListAsync();
            var books = await booksQuery.ToListAsync();
            var authors = await authorsQuery.ToListAsync();
            // Construct the view model
            var viewModel = new BookGenreViewModel
            {
                Books = books,
                Genres = new SelectList(genres),
                BookGenre = bookGenre,
                SearchString = searchString,
                 Authors = authors,
                AuthorSearchString = authorsearchString
            };

            return View(viewModel);
        }
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Reviews)
                .Include(b => b.BookGenres).ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FirstName", book.AuthorId);
            return View(book);
        }

        // GET: Books/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null || _context.Book == null)
                {
                    return NotFound();
                }

                var books = _context.Book.Where(m => m.Id == id).Include(m => m.BookGenres).First();
                if (books == null)
                {
                    return NotFound();
                }
                var genres = _context.Genre.AsEnumerable();
                genres = genres.OrderBy(s => s.GenreName);

                BookGenresEdit viewModel = new BookGenresEdit
                {
                    Book = books,
                    GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                    SelectedGenres = books.BookGenres.Select(sa => sa.GenreId) 
                };

                ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", books.AuthorId);
                return View(viewModel);
            }

            // POST: Books/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookGenresEdit viewmodel)
        {
            if (id != viewmodel.Book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Book);
                    await _context.SaveChangesAsync();
                    IEnumerable<int> newGenreList = viewmodel.SelectedGenres;
                    IEnumerable<int> prevGenreList = _context.BookGenre.Where(s => s.BookId == id).Select(s => s.GenreId);
                    IQueryable<BookGenre> toBeRemoved = _context.BookGenre.Where(s => s.BookId == id);
                    if (newGenreList != null)
                    {
                        toBeRemoved = toBeRemoved.Where(s => !newGenreList.Contains(s.GenreId));
                        foreach (int genreId in newGenreList)
                        {
                            if (!prevGenreList.Any(s => s == genreId))
                            {
                                _context.BookGenre.Add(new BookGenre { GenreId = genreId, BookId = id });
                            }
                        }
                    }
                    _context.BookGenre.RemoveRange(toBeRemoved);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(viewmodel.Book.Id)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", viewmodel.Book.AuthorId);
            return View(viewmodel);
        }
            // GET: Books/Delete/5
            public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Reviews)
                .Include(b => b.BookGenres).ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
