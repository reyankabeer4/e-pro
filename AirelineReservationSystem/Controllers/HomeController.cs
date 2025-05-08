using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AirelineReservationSystem.Models;

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

    public IActionResult Flights()
    {
        var flights = _context.Flights.ToList();
        return View(flights);
    }

    //public IActionResult ShowFlight()
    //{
    //    var flights = _context.Flights.ToList();
    //    return View(flights);
    //}

    public IActionResult ShowFlight(string source, string destination, string departureTime, string arrivalTime)
    {
        var filteredFlights = _context.Flights
            .Where(f => (string.IsNullOrEmpty(source) || f.Source == source) &&
                        (string.IsNullOrEmpty(destination) || f.Destination == destination))
       .ToList();

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
