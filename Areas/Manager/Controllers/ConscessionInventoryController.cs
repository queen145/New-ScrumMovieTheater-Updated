using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrumMovieTheater.Data;
using ScrumMovieTheater.Models;

namespace ScrumMovieTheater.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class ConcessionInventoryController : Controller
    {
        private readonly AppDbContext _context;

        public ConcessionInventoryController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var inventory = _context.ConcessionInventories
                .Include(i => i.ConcessionItem)
                .Include(i => i.Theater)
                .ToList();

            return View(inventory);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.ConcessionInventories
                .Include(x => x.ConcessionItem)
                .Include(x => x.Theater)
                .FirstOrDefaultAsync(x => x.InventoryId == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }
        // Post method for edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConcessionInventory inventory)
        {
            if (id != inventory.InventoryId)
                return NotFound();

            var existingInventory = await _context.ConcessionInventories
                .FirstOrDefaultAsync(x => x.InventoryId == id);

            if (existingInventory == null)
                return NotFound();

            existingInventory.QuantityOnHand = inventory.QuantityOnHand;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Inventory updated successfully!";

            return RedirectToAction(nameof(Index));
        }

    }
}