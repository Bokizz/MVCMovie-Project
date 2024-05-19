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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.StaticFiles;

namespace BookProject1.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookProject1Context _context;
        private readonly IWebHostEnvironment _environment;
        public BooksController(BookProject1Context context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult ViewFile(string fileName)
        {
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            if (System.IO.File.Exists(filePath))
            {
                var provider = new FileExtensionContentTypeProvider();
                if (provider.TryGetContentType(fileName, out var contentType))
                {
                    return PhysicalFile(filePath, contentType);
                }
                else
                {
                    return PhysicalFile(filePath, "application/octet-stream"); // Default to octet-stream if MIME type cannot be determined
                }
            }
            else
            {
                return NotFound(); // Return 404 if the file does not exist
            }
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
            if (id == null || _context.Book == null)
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
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName");
            var genres = _context.Genre.AsEnumerable();

            BookGenreCreateViewModel viewmodel = new BookGenreCreateViewModel
            {
                GenreListCreate = new MultiSelectList(genres, "Id", "GenreName")
            };
            return View(viewmodel);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookGenreCreateViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                if (viewmodel.FrontPageFile != null && viewmodel.FrontPageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewmodel.FrontPageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewmodel.FrontPageFile.CopyToAsync(fileStream);
                    }

                    // Save file path in the database
                    viewmodel.Book.FrontPage = "/uploads/" + uniqueFileName;
                    //book.FrontPage = filePath;
                }
                if (viewmodel.PdfFile != null && viewmodel.PdfFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    string uniquePdfFileName = Guid.NewGuid().ToString() + "_" + viewmodel.PdfFile.FileName;
                    string pdfFilePath = Path.Combine(uploadsFolder, uniquePdfFileName);

                    using (var pdfFileStream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        await viewmodel.PdfFile.CopyToAsync(pdfFileStream);
                    }

                    // Save PDF file path in the database
                    viewmodel.Book.DownloadUrl = "/uploads/" + uniquePdfFileName;
                    //book.DownloadURL = pdfFilePath;
                }

                // Save your model to the database
                _context.Add(viewmodel.Book);
                await _context.SaveChangesAsync();
                foreach (int item in viewmodel.SelectedGenresCreate)
                {
                    _context.BookGenre.Add(new BookGenre { GenreId = item, BookId = viewmodel.Book.Id });
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewmodel.Book);
        }

        // GET: Books/Edit/5

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookGenresEdit viewModel)
        {
            if (id != viewModel.Book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                  if (viewModel.FrontPageFile != null && viewModel.FrontPageFile.Length > 0)
                    {
                        // Save FrontPageFile
                        string uniqueFrontPageFileName = Guid.NewGuid().ToString() + "_" + viewModel.FrontPageFile.FileName;
                        string frontPageFilePath = Path.Combine(_environment.WebRootPath, "uploads", uniqueFrontPageFileName);

                        using (var fileStream = new FileStream(frontPageFilePath, FileMode.Create))
                        {
                            await viewModel.FrontPageFile.CopyToAsync(fileStream);
                        }

                        viewModel.Book.FrontPage = "/uploads/" + uniqueFrontPageFileName; // Update file path
                    }

                    // Check if PdfFile is uploaded
                    if (viewModel.PdfFile != null && viewModel.PdfFile.Length > 0)
                    {
                        // Save PdfFile
                        string uniquePdfFileName = Guid.NewGuid().ToString() + "_" + viewModel.PdfFile.FileName;
                        string pdfFilePath = Path.Combine(_environment.WebRootPath, "uploads", uniquePdfFileName);

                        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
                        {
                            await viewModel.PdfFile.CopyToAsync(fileStream);
                        }

                        viewModel.Book.DownloadUrl = "/uploads/" + uniquePdfFileName; // Update file path
                    }

                    // If FrontPageFile and PdfFile are not uploaded, retain the existing values
                    if (viewModel.FrontPageFile == null && viewModel.PdfFile == null)
                    {
                        var existingBook = _context.Book.AsNoTracking().FirstOrDefault(b => b.Id == id);
                        if (existingBook != null)
                        {
                            viewModel.Book.FrontPage = existingBook.FrontPage;
                            viewModel.Book.DownloadUrl = existingBook.DownloadUrl;
                        }
                    }

                    // Update Book entity with other changes
                    _context.Update(viewModel.Book);
                    await _context.SaveChangesAsync();

                    // Remove old genres
                    var oldGenres = _context.BookGenre.Where(bg => bg.BookId == id);
                    _context.BookGenre.RemoveRange(oldGenres);
                    await _context.SaveChangesAsync();

                    // Add new genres
                    foreach (int genreId in viewModel.SelectedGenres)
                    {
                        _context.BookGenre.Add(new BookGenre { GenreId = genreId, BookId = id });
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(viewModel.Book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Return the view with the viewModel so the user can correct the errors
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", viewModel.Book.AuthorId);
            return View(viewModel);
        }
        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'BookStoreContext.Books'  is null.");
            }
            var books = await _context.Book.FindAsync(id);
            if (books != null)
            {
                _context.Book.Remove(books);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
       }
}
