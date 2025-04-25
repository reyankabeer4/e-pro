// using Microsoft.AspNetCore.Authorization;/
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using AirelineReservationSystem.Models;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }

    // Add other admin-only actions here...
}
