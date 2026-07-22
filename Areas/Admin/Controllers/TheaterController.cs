using Microsoft.AspNetCore.Mvc;
using ScrumMovieTheater.Data;
using ScrumMovieTheater.Models;

namespace ScrumMovieTheater.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TheaterController : Controller
    {
        private readonly AppDbContext _context;

        public TheaterController(AppDbContext context)
        {
            _context = context;
        }


        // Display all theaters
        public IActionResult Index()
        {
            var theaters = _context.Theaters.ToList();

            return View(theaters);
        }


        // Open Add Theater page
        [HttpGet]
        public IActionResult AddTheater()
        {
            return View();
        }


        // Save new theater
        [HttpPost]
        public IActionResult AddTheater(Theater theater)
        {
            if (ModelState.IsValid)
            {
                theater.Active = true;

                _context.Theaters.Add(theater);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Theater added successfully!";

                return RedirectToAction("Index");
            }

            return View(theater);

        }
            // Open Edit Theater page
        [HttpGet]
        public IActionResult EditTheater(int id)
        {
            var theater = _context.Theaters
                .FirstOrDefault(t => t.TheaterId == id);

            if (theater == null)
            {
                return NotFound();
            }

            return View(theater);
        }


        // Save edited theater
        [HttpPost]
        public IActionResult EditTheater(Theater theater)
        {
            var existing = _context.Theaters
                .FirstOrDefault(t => t.TheaterId == theater.TheaterId);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = theater.Name;
            existing.Address = theater.Address;
            existing.Description = theater.Description;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Theater updated successfully!";

            return RedirectToAction("Index");
        }


        // Deactivate theater
        [HttpPost]
        public IActionResult Deactivate(int id)
        {
            var theater = _context.Theaters
                .FirstOrDefault(t => t.TheaterId == id);

            if (theater == null)
            {
                return NotFound();
            }

            theater.Active = false;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Theater deactivated successfully!";

            return RedirectToAction("Index");
        }
    }
}
