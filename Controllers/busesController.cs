using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using bus.Data;
using bus.Models;
using Microsoft.AspNetCore.Authorization;

namespace bus.Controllers
{
    public class busesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public busesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: buses
        public async Task<IActionResult> Index()
        {
            return View(await _context.buses.ToListAsync());
        }


        // GET: buses/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // GET: buses/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(string SearchStart, string SearchEnd)
        {
            return View("Index", await _context.buses
                .Where(j => j.Start.Contains(SearchStart) && j.End.Contains(SearchEnd))
                .ToListAsync());
        }


        // GET: buses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buses = await _context.buses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (buses == null)
            {
                return NotFound();
            }

            return View(buses);
        }

        // GET: buses/Create

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: buses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Start,End,Seats")] buses buses)
        {
            if (ModelState.IsValid)
            {
                _context.Add(buses);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(buses);
        }

        // GET: buses/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buses = await _context.buses.FindAsync(id);
            if (buses == null)
            {
                return NotFound();
            }
            return View(buses);
        }

        // POST: buses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Start,End,Seats")] buses buses)
        {
            if (id != buses.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(buses);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!busesExists(buses.Id))
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
            return View(buses);
        }

        // GET: buses/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buses = await _context.buses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (buses == null)
            {
                return NotFound();
            }

            return View(buses);
        }

        // POST: buses/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buses = await _context.buses.FindAsync(id);
            if (buses != null)
            {
                _context.buses.Remove(buses);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool busesExists(int id)
        {
            return _context.buses.Any(e => e.Id == id);
        }

        public IActionResult Book(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = _context.buses.FirstOrDefault(m => m.Id == id);
            if (bus == null)
            {
                return NotFound();
            }
            ViewBag.BusId = bus.Id;

            // Retrieve the number of available seats for this bus
            int availableSeats = bus.Seats;

            if (availableSeats > 0)
            {
                // Pass the number of available seats to the view
                ViewBag.AvailableSeats = availableSeats;

                // Return the booking form view
                return View("~/Views/Buses/bookticket.cshtml");
            }
            else
            {
                // Handle case when there are no available seats
                TempData["Message"] = "Sorry, there are no available seats for booking.";

                // Redirect back to the buses page or any other appropriate action
                return RedirectToAction("Index", "Buses");
            }
        }

    }
}
