using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AirelineReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly NeondbContext _context;

    public AdminController(NeondbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    // GET: Show Flight Form
    [HttpGet]
    public IActionResult Flight()
    {
        var flight = new Flight();
        return View(flight);
    }


    public IActionResult ShowFlight()
    {
        var flight = _context.Flights.ToList();
        return View(flight);
    }

    // POST: Save Flight
    [HttpPost]
    public async Task<IActionResult> Flight(Flight flight, IFormFile flight_pic)
    {
        if (!ModelState.IsValid)
            return View(flight);

        // Handle flight image upload
        if (flight_pic != null && flight_pic.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(flight_pic.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await flight_pic.CopyToAsync(stream);
            }

            flight.FlightPic = fileName;
        }

        // Calculate duration from DepartureTime and ArrivalTime
        if (flight.DepartureTime.HasValue && flight.ArrivalTime.HasValue)
        {
            var duration = flight.ArrivalTime.Value - flight.DepartureTime.Value;
            flight.Flightduration = $"{(int)duration.TotalHours}h {duration.Minutes}m";
        }
        else
        {
            flight.Flightduration = "0h 0m"; // fallback if no valid time is given
        }

        // Add and save flight
        _context.Flights.Add(flight);
        await _context.SaveChangesAsync();

        return RedirectToAction("Flight");
    }
}
