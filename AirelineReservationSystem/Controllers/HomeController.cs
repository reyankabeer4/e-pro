using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AirelineReservationSystem.Models;

namespace AirelineReservationSystem.Controllers;

[Authorize(Roles = "Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
