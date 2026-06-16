using BaiTap8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaiTap8.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BooksController(
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ================= INDEX =================

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Images)
                .ToListAsync();

            return View(books);
        }

        // ================= CREATE =================

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Book book,
            List<IFormFile> files)
        {
            if (!ModelState.IsValid)
                return View(book);

            book.Images = new List<BookImage>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    string ext =
                        Path.GetExtension(file.FileName)
                        .ToLower();

                    if (ext != ".jpg" &&
                        ext != ".jpeg" &&
                        ext != ".png")
                    {
                        ModelState.AddModelError(
                            "",
                            "Chỉ cho phép jpg/png");

                        return View(book);
                    }

                    string fileName =
                        Guid.NewGuid() + ext;

                    string folder =
                        Path.Combine(
                            _env.WebRootPath,
                            "uploads",
                            "books");

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    string path =
                        Path.Combine(folder, fileName);

                    using (var stream =
                           new FileStream(
                               path,
                               FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    book.Images.Add(new BookImage
                    {
                        ImageUrl =
                            "/uploads/books/" + fileName
                    });
                }
            }

            _context.Books.Add(book);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= DETAILS =================

        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.Images)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // ================= EDIT =================

        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.Images)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            Book book,
            List<IFormFile> files)
        {
            var dbBook = await _context.Books
                .Include(b => b.Images)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (dbBook == null)
                return NotFound();

            dbBook.Name = book.Name;
            dbBook.Price = book.Price;

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    string ext =
                        Path.GetExtension(file.FileName)
                        .ToLower();

                    if (ext != ".jpg" &&
                        ext != ".jpeg" &&
                        ext != ".png")
                    {
                        ModelState.AddModelError(
                            "",
                            "Chỉ cho phép jpg/png");

                        return View(dbBook);
                    }

                    string fileName =
                        Guid.NewGuid() + ext;

                    string folder =
                        Path.Combine(
                            _env.WebRootPath,
                            "uploads",
                            "books");

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    string path =
                        Path.Combine(folder, fileName);

                    using (var stream =
                           new FileStream(
                               path,
                               FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    dbBook.Images.Add(new BookImage
                    {
                        ImageUrl =
                            "/uploads/books/" + fileName
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .Include(b => b.Images)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books
                .Include(b => b.Images)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            foreach (var img in book.Images)
            {
                string fullPath =
                    _env.WebRootPath +
                    img.ImageUrl.Replace("/", "\\");

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _context.Books.Remove(book);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}