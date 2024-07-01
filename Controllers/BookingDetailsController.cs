using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using bus.Data;
using bus.Models;

namespace bus.Controllers
{
    public class BookingDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BookingDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.BookingDetails.ToListAsync());
        }

        // GET: BookingDetails/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingDetails = await _context.BookingDetails
                .FirstOrDefaultAsync(m => m.IdNumber == id);
            if (bookingDetails == null)
            {
                return NotFound();
            }

            return View(bookingDetails);
        }

        // GET: BookingDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookingDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int busId, [Bind("IdNumber,Name,Email,Phone,BookingDate,SelectedSeats")] BookingDetails bookingDetails)
        {
            if (ModelState.IsValid)
            {
                // Check if the ID number has already booked 5 tickets
                var bookedSeatsCount = _context.BookingDetails.Count(b => b.IdNumber == bookingDetails.IdNumber);
                if (bookedSeatsCount >= 5)
                {
                    // Display message to the user that the ID number has reached the maximum booking limit
                    ModelState.AddModelError("IdNumber", "Sorry, you have already booked the maximum number of seats.");
                    return View(bookingDetails);
                }

                // Save booking details to the BookingDetails table
                _context.Add(bookingDetails);
                await _context.SaveChangesAsync();

                // Decrement available seats count for the corresponding bus
                var bus = await _context.buses.FindAsync(ViewBag.BusId);
                if (bus != null)
                {
                    bus.Seats -= bookingDetails.SelectedSeats.Count;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(bookingDetails);
        }



        // GET: BookingDetails/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingDetails = await _context.BookingDetails.FindAsync(id);
            if (bookingDetails == null)
            {
                return NotFound();
            }
            return View(bookingDetails);
        }

        // POST: BookingDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdNumber,Name,Email,Phone,BookingDate,SelectedSeats")] BookingDetails bookingDetails)
        {
            if (id != bookingDetails.IdNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookingDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingDetailsExists(bookingDetails.IdNumber))
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
            return View(bookingDetails);
        }

        // GET: BookingDetails/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingDetails = await _context.BookingDetails
                .FirstOrDefaultAsync(m => m.IdNumber == id);
            if (bookingDetails == null)
            {
                return NotFound();
            }

            return View(bookingDetails);
        }

        // POST: BookingDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bookingDetails = await _context.BookingDetails.FindAsync(id);
            if (bookingDetails != null)
            {
                _context.BookingDetails.Remove(bookingDetails);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingDetailsExists(string id)
        {
            return _context.BookingDetails.Any(e => e.IdNumber == id);
        }
    }
}
