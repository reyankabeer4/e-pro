using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AirelineReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AirelineReservationSystem.Controllers;

[AllowAnonymous]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NeondbContext _context;


    public HomeController(NeondbContext context, ILogger<HomeController> logger)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        // Check if the user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            ViewBag.Message = "User is logged in!";
        }
        else
        {
            ViewBag.Message = "User is not logged in!";
        }

        return View();
    }

    // public IActionResult Index()
    // {
    //     return View();
    // }
    
    public IActionResult About()
    {
        return View();
    }

    public IActionResult Dashboard()
    {
        
        return View();
    }

    public IActionResult Booking(int id)
    {
        var flight = _context.Flights.FirstOrDefault(f => f.FlightId == id);
        if (flight == null)
        {
            return NotFound();
        }

        return View(flight);
    }

    [Route("Home/Booking/Payment/{id}")]
    public IActionResult Payment(int id)
    {
        var flight = _context.Flights.FirstOrDefault(f => f.FlightId == id);
        return View(flight);

    }

    //public IActionResult ShowFlight(string source, string destination, string departureTime, string arrivalTime)
    //{
    //    // Parse times for filtering
    //    TimeSpan? departureFilter = null;
    //    TimeSpan? arrivalFilter = null;

    //    if (TimeSpan.TryParse(departureTime, out var parsedDeparture))
    //        departureFilter = parsedDeparture;

    //    if (TimeSpan.TryParse(arrivalTime, out var parsedArrival))
    //        arrivalFilter = parsedArrival;

    //    // Get today's date and tomorrow's date
    //    var today = DateTime.Today; // Today at 00:00 AM
    //    var tomorrow = today.AddDays(1); // Tomorrow at 00:00 AM

    //    var filteredFlights = _context.Flights
    //        .Where(f =>
    //            (string.IsNullOrEmpty(source) || f.Source == source) &&
    //            (string.IsNullOrEmpty(destination) || f.Destination == destination) &&
    //            (!departureFilter.HasValue ||
    //             (f.DepartureTime.HasValue &&
    //              Math.Abs(EF.Functions.DateDiffMinute(f.DepartureTime.Value.TimeOfDay, departureFilter.Value)) <= 60)) &&
    //            (!arrivalFilter.HasValue ||
    //             (f.ArrivalTime.HasValue &&
    //              Math.Abs(EF.Functions.DateDiffMinute(f.ArrivalTime.Value.TimeOfDay, arrivalFilter.Value)) <= 60)) &&
    //            // Filter flights that depart today or tomorrow (ignoring time)
    //            (f.DepartureTime.HasValue &&
    //             f.DepartureTime.Value.Date >= today && f.DepartureTime.Value.Date < tomorrow)
    //        )
    //        .ToList();

    //    return View(filteredFlights);
    //}


    public IActionResult ShowFlight(string source, string destination, string departureTime, string arrivalTime)
    {
        TimeSpan? departureFilter = null;
        TimeSpan? arrivalFilter = null;

        if (TimeSpan.TryParse(departureTime, out var parsedDeparture))
            departureFilter = parsedDeparture;

        if (TimeSpan.TryParse(arrivalTime, out var parsedArrival))
            arrivalFilter = parsedArrival;

        var today = DateTime.Today;

        // ✅ Change made here
        var flightQuery = _context.Flights
            .Where(f =>
                f.DepartureTime.HasValue &&
                f.DepartureTime.Value.Date >= today
            );

        if (!string.IsNullOrEmpty(source))
        {
            flightQuery = flightQuery.Where(f => f.Source == source);
        }

        if (!string.IsNullOrEmpty(destination))
        {
            flightQuery = flightQuery.Where(f => f.Destination == destination);
        }

        if (departureFilter.HasValue)
        {
            flightQuery = flightQuery.Where(f => f.DepartureTime.HasValue &&
                Math.Abs(EF.Functions.DateDiffMinute(f.DepartureTime.Value.TimeOfDay, departureFilter.Value)) <= 60);
        }

        if (arrivalFilter.HasValue)
        {
            flightQuery = flightQuery.Where(f => f.ArrivalTime.HasValue &&
                Math.Abs(EF.Functions.DateDiffMinute(f.ArrivalTime.Value.TimeOfDay, arrivalFilter.Value)) <= 60);
        }

        var filteredFlights = flightQuery.ToList();

        return View(filteredFlights);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
