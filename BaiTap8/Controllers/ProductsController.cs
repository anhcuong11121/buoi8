using BaiTap8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaiTap8.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Product product,
            List<IFormFile> files)
        {
            if (!ModelState.IsValid)
                return View(product);

            product.Images = new List<ProductImage>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();

                    if (ext != ".jpg" &&
                        ext != ".jpeg" &&
                        ext != ".png")
                    {
                        ModelState.AddModelError("", "Chỉ cho phép jpg/png");
                        return View(product);
                    }

                    string fileName = Guid.NewGuid() + ext;

                    string folder = Path.Combine(
                        _env.WebRootPath,
                        "uploads",
                        "products");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = "/uploads/products/" + fileName
                    });
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            Product product,
            List<IFormFile> files)
        {
            var dbProduct = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (dbProduct == null)
                return NotFound();

            dbProduct.Name = product.Name;
            dbProduct.Price = product.Price;

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();

                    if (ext != ".jpg" &&
                        ext != ".jpeg" &&
                        ext != ".png")
                    {
                        ModelState.AddModelError("", "Chỉ cho phép jpg/png");
                        return View(dbProduct);
                    }

                    string fileName = Guid.NewGuid() + ext;

                    string folder = Path.Combine(
                        _env.WebRootPath,
                        "uploads",
                        "products");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    dbProduct.Images.Add(new ProductImage
                    {
                        ImageUrl = "/uploads/products/" + fileName
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}