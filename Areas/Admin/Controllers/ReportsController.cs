using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrumMovieTheater.Data;
using ScrumMovieTheater.Models;
using System.Text;

namespace ScrumMovieTheater.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Sales(DateTime? startDate, DateTime? endDate)
        {
            var bookings = _context.Bookings
                .Include(b => b.Showtime)
                .ThenInclude(s => s.Theater)
                .AsQueryable();


            if (startDate.HasValue)
            {
                bookings = bookings
                    .Where(b => b.BookedAt >= startDate.Value);
            }


            if (endDate.HasValue)
            {
                bookings = bookings
                    .Where(b => b.BookedAt <= endDate.Value);
            }


            var report = bookings
                .GroupBy(b => b.Showtime!.Theater!.Name)
                .Select(g => new SalesReportViewModel
                {
                    TheaterName = g.Key,

                    TicketsSold = g.Sum(b => b.Adults + b.Kids),

                    Revenue = g.Sum(b => b.TotalPrice)
                })
                .ToList();

            return View(report);
        }
        // Method for exporting file
        public IActionResult ExportCsv()
        {
            var report = _context.Bookings
                .Include(b => b.Showtime)
                .ThenInclude(s => s.Theater)
                .GroupBy(b => b.Showtime!.Theater!.Name)
                .Select(g => new SalesReportViewModel
                {
                    TheaterName = g.Key,
                    TicketsSold = g.Sum(b => b.Adults + b.Kids),
                    Revenue = g.Sum(b => b.TotalPrice)
                })
                .ToList();

            var csv = new StringBuilder();

            csv.AppendLine("Theater,Tickets Sold,Revenue");

            foreach (var item in report)
            {
                csv.AppendLine($"{item.TheaterName},{item.TicketsSold},{item.Revenue}");
            }

            csv.AppendLine();

            csv.AppendLine(
                $"TOTAL,{report.Sum(r => r.TicketsSold)},{report.Sum(r => r.Revenue)}");

            return File(
                Encoding.UTF8.GetBytes(csv.ToString()),
                "text/csv",
                "SalesReport.csv");
        }
    }
}