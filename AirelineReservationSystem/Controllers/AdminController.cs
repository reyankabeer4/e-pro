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
        // Assuming email is stored as claim
        var email = User.Identity?.Name;

        // Find user by email (or user ID if you store that in claims)
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        var fullName = user != null ? $"{user.FirstName} {user.LastName}" : "Admin";

        var viewModel = new AdminDashboardViewModel
        {
            TotalUsers = _context.Users.Count(),
            TotalBookings = _context.Bookings.Count(),
            Username = fullName
        };

        return View(viewModel);
    }


    [HttpGet]
    public IActionResult Flight()
    {
        var flight = new Flight();
        return View(flight);
    }


    public IActionResult ShowFlight(int id)
    {
        var flight = _context.Flights.OrderBy(f=>f.FlightId).ToList();
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
            flight.Flightduration = "0h 0m"; 
        }

        _context.Flights.Add(flight);
        await _context.SaveChangesAsync();

        return RedirectToAction("Flight");
    }



    public IActionResult Bookings()
    {
        var booking = _context.Bookings.ToList();
        return View(booking);
    }

    public IActionResult Users()
    {
        //var users = _context.Users.ToList();
        var users = _context.Users.OrderBy(u => u.UserId).ToList();//To retrieve in Order by Id ascending
        return View(users);
    }


    public IActionResult EditFlight(int id)
    {
        var flight = _context.Flights.Where(f => f.FlightId == id).FirstOrDefault();
        if (flight == null)
        {
            return NotFound();
        }


        return View(flight);
    }



    [HttpPost]
    public async Task<IActionResult> UpdateFlight(Flight flight, int id, IFormFile flight_pic)
    {
        if (!ModelState.IsValid)
            return View("EditFlight", flight);

        var existingFlight = await _context.Flights.FindAsync(id);

        if (existingFlight == null)
        {
            ViewData["Error"] = "Flight Not Found";
            return View("EditFlight", flight);
        }

        // Update flight properties
        existingFlight.AirlineName = flight.AirlineName;
        existingFlight.FlightNumber = flight.FlightNumber;
        existingFlight.Source = flight.Source;
        existingFlight.Destination = flight.Destination;
        existingFlight.DepartureTime = flight.DepartureTime;
        existingFlight.ArrivalTime = flight.ArrivalTime;
        existingFlight.Numberofstops = flight.Numberofstops;
        existingFlight.TotalSeats = flight.TotalSeats;
        existingFlight.AvailableSeats = flight.AvailableSeats;
        existingFlight.Operatingairline = flight.Operatingairline;
        existingFlight.Price = flight.Price;

        // Calculate duration
        if (flight.DepartureTime.HasValue && flight.ArrivalTime.HasValue)
        {
            var duration = flight.ArrivalTime.Value - flight.DepartureTime.Value;
            existingFlight.Flightduration = $"{(int)duration.TotalHours}h {duration.Minutes}m";
        }

        // Handle picture
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

            existingFlight.FlightPic = fileName;
        }

        _context.Flights.Update(existingFlight);
        await _context.SaveChangesAsync();

        return RedirectToAction("ShowFlight");
    }



}
