using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
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

    [HttpGet]
    public IActionResult Flight()
    {
        return View();
    }

    public IActionResult Flight(Flight flight)
    {
        if (ModelState.IsValid)
        {

            _context.Flights.Add(flight);
            _context.SaveChanges();
            return RedirectToAction("Flight");
        }
        else
        {
            return View(flight);

        }
    }



}
