using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AirelineReservationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using System;

[Authorize(Roles = "Customer,Admin")]
public class UserController : Controller
{
    private readonly NeondbContext _context;

    public UserController(NeondbContext context)
    {
        _context = context;
    }

    //public IActionResult Dashboard()
    //{
    //    var bookings = _context.Bookings.Include(b=>b.Flight).Include(b => b.Passenger).Where(id=>id.User.Id).ToList();
    //    return View(bookings);
    //}

    public IActionResult Dashboard()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var bookings = _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Flight)
            .Include(b => b.Passengers)
            .ToList();

        return View(bookings);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Booking(int flightId, DateTime departureTime, string fullName, int age, string gender, string passportNumber)
    {
        if (!User.Identity.IsAuthenticated)
        {
            TempData["Error"] = "You must be logged in to book a flight.";
            return RedirectToAction("Login", "Auth");
        }

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
        if (flight == null)
        {
            TempData["Error"] = "Flight not found.";
            return RedirectToAction("Dashboard");
        }

        try
        {
            var booking = new Booking
            {
                FlightId = flightId,
                UserId = userId,
                BookingDate = DateTime.Now,
                TotalAmount = flight.Price,
                Status = "Confirmed"

            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(); // Save to generate BookingId

            var passenger = new Passenger
            {
                BookingId = booking.BookingId, // foreign key reference
                FullName = fullName,
                Age = age,
                Gender = gender,
                PassportNumber = passportNumber
            };

            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync(); // Save passenger

            TempData["Success"] = "Booking and passenger details saved successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while processing your booking.";
            Console.WriteLine($"Error: {ex.Message}");
        }

        return RedirectToAction("dashboard");
    }


    [Route("Home/Booking/Payment/{id}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Payment(int id, string paymentMethod)
    {
        if (!User.Identity.IsAuthenticated)
        {
            TempData["Error"] = "You must be logged in to process a payment.";
            return RedirectToAction("Login", "Auth");
        }

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var latestBooking = await _context.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .FirstOrDefaultAsync();

        if (latestBooking == null)
        {
            TempData["Error"] = "No booking found to associate with the payment.";
            return Redirect($"/Home/Booking/Payment/{id}");
        }

        var payment = new Payment
        {
            BookingId = latestBooking.BookingId,
            Amount = latestBooking.TotalAmount ?? 0.00m,
            PaymentMethod = paymentMethod,
            PaymentDate = DateTime.Now,
            PaymentStatus = "Completed"
        };

        try
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Payment processed successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while processing your payment.";
            Console.WriteLine($"Error: {ex.Message}");
        }

        return Redirect($"/Home/Booking/Payment/{id}");
    }



    public async Task<IActionResult> Profile()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            ViewBag.Message = "User is not authenticated.";
            return View();
        }

        string userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        if (!int.TryParse(userIdStr, out int userId))
        {
            ViewBag.Message = "Invalid user ID.";
            return View();
        }

        string name = User.FindFirst(ClaimTypes.Name)?.Value ?? "No Name available";
        string username = User.FindFirst("Username")?.Value ?? "Unknown";
        string email = User.FindFirst(ClaimTypes.Email)?.Value ?? "No email available";
        string address = User.FindFirst("address")?.Value ?? "No Address available";

        // Fetch phone number from the Users table
        string phoneNumber = "Not available";
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId); // Compare int
        if (user != null && !string.IsNullOrEmpty(user.PhoneNumber))
        {
            phoneNumber = user.PhoneNumber;
        }

        ViewBag.UserId = userId;
        ViewBag.Name= name;
        ViewBag.Username = username;
        ViewBag.Email = email;
        ViewBag.Address = address;
        ViewBag.PhoneNumber = phoneNumber;

        return View();
    }

    public IActionResult AccountSettings()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Get current logged-in user's ID
        if (userId == null)
        {
            TempData["Error"] = "User is not authenticated.";
            return RedirectToAction("Login", "Auth");
        }

        var user = _context.Users.FirstOrDefault(u => u.UserId == int.Parse(userId)); // Get user from the DB
        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("Dashboard");
        }

        // Pass user data directly to the view
        return View(user);  // Ensure your view expects a "User" object
    }






    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AccountSettings(User model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Get current logged-in user's ID
            if (userId == null)
            {
                TempData["Error"] = "User is not authenticated.";
                return RedirectToAction("Login", "Auth");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == int.Parse(userId));  // Fetch user
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Dashboard");
            }

            // Update user fields directly
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            // Save changes
            _context.Users.Update(user);
            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        TempData["Error"] = "Failed to update profile. Please try again.";
        return View(model);  // Return the view with errors
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Login", "Auth");
        }

        string? userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            ViewBag.Message = "Invalid user ID.";
            return View();
        }


        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId); // Compare int
        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("Dashboard");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        await HttpContext.SignOutAsync();

        TempData["Success"] = "Your account has been deleted.";
        return RedirectToAction("Index", "Home");
    }
}