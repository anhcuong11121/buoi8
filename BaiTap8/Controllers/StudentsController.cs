using BaiTap8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaiTap8.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public StudentsController(
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.Images)
                .ToListAsync();

            return View(students);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Student student,
            List<IFormFile> files)
        {
            if (!ModelState.IsValid)
                return View(student);

            student.Images = new List<StudentImage>();

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
                        return View(student);
                    }

                    string fileName = Guid.NewGuid() + ext;

                    string folder = Path.Combine(
                        _env.WebRootPath,
                        "uploads",
                        "students");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    student.Images.Add(new StudentImage
                    {
                        ImageUrl = "/uploads/students/" + fileName
                    });
                }
            }

            _context.Students.Add(student);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            Student student,
            List<IFormFile> files)
        {
            var dbStudent = await _context.Students
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (dbStudent == null)
                return NotFound();

            dbStudent.Name = student.Name;
            dbStudent.Age = student.Age;

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
                        return View(dbStudent);
                    }

                    string fileName = Guid.NewGuid() + ext;

                    string folder = Path.Combine(
                        _env.WebRootPath,
                        "uploads",
                        "students");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    dbStudent.Images.Add(new StudentImage
                    {
                        ImageUrl = "/uploads/students/" + fileName
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}