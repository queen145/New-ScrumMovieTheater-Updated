using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScrumMovieTheater.Data;
using ScrumMovieTheater.Models;


namespace ScrumMovieTheater.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly AppDbContext _context;

        public MovieController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult AddMovie()
        {
            return View();
        }
        // save movie in database and return to home
        [HttpPost]
        [HttpPost]
        public IActionResult AddMovie(Movie movie, IFormFile ImageFile)
        {
        if (ImageFile != null && ImageFile.Length > 0)
        {
            string fileName = Path.GetFileName(ImageFile.FileName);

            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/images",
                fileName
            );

            using (var stream = new FileStream(path, FileMode.Create))
            {
                ImageFile.CopyTo(stream);
            }

           // THIS is what saves to database
           movie.ImageUrl = "/images/" + fileName;
        }
        else
        {
            movie.ImageUrl = "";
        }

        _context.Movies.Add(movie);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Movie added successfully!";
        return RedirectToAction("Index");
        }
          // LIST movies
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }

        [HttpGet]
        public IActionResult EditMovie(int movieId, int showtimeId)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.MovieId == movieId);

            if (movie == null)
                return NotFound();

            var showtime = _context.Showtimes
                .FirstOrDefault(s => s.Id == showtimeId);

            var model = new UpdateMovieViewModel
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                RuntimeMinutes = movie.RuntimeMinutes,
                Rating = movie.Rating,
                ReleaseDate = movie.ReleaseDate,
                ImageUrl = movie.ImageUrl
            };

            if (showtime != null)
            {
                model.ShowtimeId = showtime.Id;
                model.TheaterId = showtime.TheaterId;
                model.AuditoriumId = showtime.AuditoriumId;
                model.ShowDate = showtime.ShowDate;
                model.TimeSlot = showtime.TimeSlot;
                model.Price = showtime.Price;
            }

            ViewBag.Theaters = new SelectList(
                _context.Theaters,
                "TheaterId",
                "Name",
                model.TheaterId
            );

            ViewBag.Auditoriums = new SelectList(
                _context.Auditoriums,
                "AuditoriumId",
                "Name",
                model.AuditoriumId
            );

            ViewBag.Showtimes = new SelectList(
                _context.Showtimes
                    .Where(s => s.MovieId == movieId)
                    .Select(s => new
                    {
                        Id = s.Id,
                        Display = $"Theater {s.TheaterId} - {s.ShowDate:MM/dd/yyyy} - {s.TimeSlot}"
                    }),
                "Id",
                "Display",
                model.ShowtimeId
            );

            return View(model);
        }

        [HttpPost]
        public IActionResult EditMovie(UpdateMovieViewModel model)
        {
            var existing = _context.Movies
                .FirstOrDefault(m => m.MovieId == model.MovieId);

            if (existing == null)
                return NotFound();


            // Update Movie
            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.Genre = model.Genre;
            existing.RuntimeMinutes = model.RuntimeMinutes;
            existing.Rating = model.Rating;
            existing.ReleaseDate = model.ReleaseDate;
            existing.ImageUrl = model.ImageUrl;


            // Update Showtime
            var showtime = _context.Showtimes
                .FirstOrDefault(s => s.Id == model.ShowtimeId);

            if (showtime != null)
            {
                showtime.TheaterId = model.TheaterId;
                showtime.AuditoriumId = model.AuditoriumId;
                showtime.ShowDate = model.ShowDate;
                showtime.TimeSlot = model.TimeSlot;
                showtime.Price = model.Price;
            }


            _context.SaveChanges();

            TempData["SuccessMessage"] = "Movie and Showtime updated successfully!";

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult DeleteMovie(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.MovieId == id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.MovieId == id);

         if (movie == null)
        {
            return NotFound();
        }

        // ADD IMAGE DELETE CODE HERE (BEFORE REMOVE)
        if (!string.IsNullOrEmpty(movie.ImageUrl))
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                movie.ImageUrl.TrimStart('/')
            );

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        _context.Movies.Remove(movie);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Movie deleted successfully!";
        return RedirectToAction("Index");
        }

        // showtime get action

    public IActionResult AddShowTime()
    {
        ViewBag.Movies = _context.Movies.ToList();
        ViewBag.Theaters = _context.Theaters.ToList(); // if you have Theater table
        ViewBag.Auditoriums = _context.Auditoriums.ToList();

        return View();
    }

    // post method for showtime
    [HttpPost]
    public IActionResult AddShowTime(Showtime showTime)
    {
        if (ModelState.IsValid)
        {
            Console.WriteLine("AddShowTime POST reached");
            Console.WriteLine($"Theater: {showTime.TheaterId}");
            Console.WriteLine($"Auditorium: {showTime.AuditoriumId}");
            Console.WriteLine($"Date: {showTime.ShowDate}");
            Console.WriteLine($"Time: {showTime.TimeSlot}");

                // Check auditorium schedule conflict
                var conflict = _context.Showtimes
               .Any(s =>
                   s.TheaterId == showTime.TheaterId &&
                   s.AuditoriumId == showTime.AuditoriumId &&
                   s.ShowDate == showTime.ShowDate &&
                   s.TimeSlot == showTime.TimeSlot
               );

                if (conflict)
                {
                    ModelState.AddModelError("",
                        "This auditorium already has a showtime at this date and time.");
                    ViewBag.Movies = _context.Movies.ToList();
                    ViewBag.Theaters = _context.Theaters.ToList();
                    ViewBag.Auditoriums = _context.Auditoriums.ToList();

                    return View(showTime);

                }
                _context.Showtimes.Add(showTime);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Showtime added successfully!";
                return RedirectToAction("Index");
             }

             foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
             {
                Console.WriteLine(error.ErrorMessage);
             }

            ViewBag.Movies = _context.Movies.ToList();
            ViewBag.Theaters = _context.Theaters.ToList();
            ViewBag.Auditoriums = _context.Auditoriums.ToList();

            return View(showTime);
        }

    public IActionResult Bookings()
    {
        var bookings = _context.Bookings
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
           .Include(b => b.Showtime)
                .ThenInclude(s => s.Theater)
            .ToList();

        return View(bookings);
    }



        public IActionResult Manager()
        {
            return View();
        }
    }
}