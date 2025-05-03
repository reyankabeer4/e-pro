using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AirelineReservationSystem.Models;
using System.ComponentModel.Design;

[Authorize(Roles = "Customer,Admin")]
public class UserController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
    public IActionResult Profile()
    {
        return View();
    }

    public IActionResult AccountSettings()
    {
        return View();

    }


    //public IActionResult DeleteAccount()
    //{
    //    //return 
    //}

}
