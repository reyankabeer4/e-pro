using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AirelineReservationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;

[Authorize(Roles = "Customer,Admin")]
public class UserController : Controller
{
    private readonly NeondbContext _context;

    public UserController(NeondbContext context)
    {
        _context = context;
    }

    
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
    public async Task<IActionResult> CancelBooking(int id)
    {
        if (!User.Identity.IsAuthenticated)
        {
            TempData["Error"] = "You must be logged in to cancel a booking.";
            return RedirectToAction("Login", "Auth");
        }

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);

        if (booking == null)
        {
            TempData["Error"] = "Booking not found or you do not have permission to cancel it.";
            return RedirectToAction("Dashboard");
        }

        try
        {
            booking.Status = "Cancelled";
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while cancelling the booking.";
            return RedirectToAction("Dashboard");
        }
    }

    public IActionResult Booking()
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
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        // Handle profile picture path
        if (!string.IsNullOrEmpty(user.ProfilePic) && !user.ProfilePic.StartsWith("/"))
        {
            user.ProfilePic = $"/ProfilePictures/{user.ProfilePic}";
        }
        else if (string.IsNullOrEmpty(user.ProfilePic))
        {
            user.ProfilePic = "/images/default-avatar-profile.jpg";
        }

        return View(user);
    }








    [HttpGet]
    public IActionResult AccountSettings()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            TempData["Error"] = "User is not authenticated.";
            return RedirectToAction("Login", "Auth");
        }

        var user = _context.Users.FirstOrDefault(u => u.UserId == int.Parse(userId));
        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("Dashboard");
        }

        return View(user); // This view should be strongly typed to the User model
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AccountSettings(User model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            TempData["Error"] = "User is not authenticated.";
            return RedirectToAction("Login", "Auth");
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Validation failed. Please check all fields.";
            return View(model);
        }

        var user = _context.Users.FirstOrDefault(u => u.UserId == int.Parse(userId));
        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("Dashboard");
        }

        // Update user details (not UserId)
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.Username;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;

        _context.Users.Update(user); // Add this line before SaveChanges as a test
        _context.SaveChanges();

        TempData["Success"] = "Profile updated successfully.";
        return RedirectToAction("Profile");
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






    [HttpPost]
    public async Task<IActionResult> UploadProfilePic(IFormFile ProfilePic)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _context.Users.FindAsync(userId);

        if (ProfilePic == null || ProfilePic.Length == 0)
        {
            TempData["Error"] = "Please select a valid image file";
            return RedirectToAction("Profile");
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(ProfilePic.FileName).ToLower();

        if (!allowedExtensions.Contains(fileExtension))
        {
            TempData["Error"] = "Only image files (JPG, PNG, GIF) are allowed";
            return RedirectToAction("Profile");
        }

        // Create uploads directory if it doesn't exist
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfilePictures");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Save the file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await ProfilePic.CopyToAsync(stream);
        }

        // Delete old profile picture if it exists
        if (!string.IsNullOrEmpty(user.ProfilePic))
        {
            var oldFileName = user.ProfilePic.Replace("/ProfilePictures/", "");
            var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        // Update user record - store just the filename
        user.ProfilePic = fileName;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Profile picture updated successfully";
        return RedirectToAction("Profile");
    }





    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateProfile(User model, IFormFile ProfilePic)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        // Update only the fields we want to allow changing
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.Username;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;

        // Handle profile picture upload separately
        if (ProfilePic != null && ProfilePic.Length > 0)
        {
            return UploadProfilePic(ProfilePic).Result;
        }

        try
        {
            _context.SaveChanges();
            TempData["Success"] = "Profile updated successfully.";
        }
        catch (DbUpdateException ex)
        {
            TempData["Error"] = "Failed to update profile. Please try again.";
        }

        return RedirectToAction("Profile");
    }

}
