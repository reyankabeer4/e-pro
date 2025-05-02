using AirelineReservationSystem.Models;
using AirelineReservationSystem.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Npgsql;
using System.Threading.Tasks;
using System.Collections.Generic;
using BCrypt.Net;

namespace AirelineReservationSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        private string ConnectionString => _config.GetConnectionString("DefaultConnection");

        // =============================
        // Register (GET)
        // =============================
        [HttpGet]
        public IActionResult Register() => View();

        // =============================
        // Register (POST)
        // =============================
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(@"
                INSERT INTO users (full_name, email, password_hash, phone_number, role)
                VALUES (@full_name, @email, @hash, @phone, 'Customer')", conn);

            cmd.Parameters.AddWithValue("full_name", model.FullName);
            cmd.Parameters.AddWithValue("email", model.Email);
            cmd.Parameters.AddWithValue("hash", passwordHash);
            cmd.Parameters.AddWithValue("phone", model.PhoneNumber);

            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Login");
        }

        // =============================
        // Login (GET)
        // =============================
        [HttpGet]
        public IActionResult Login() => View();

        // =============================
        // Login (POST)
        // =============================
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(@"
                SELECT user_id, full_name, email, password_hash, role 
                FROM users 
                WHERE email = @email", conn);

            cmd.Parameters.AddWithValue("email", model.Email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var storedHash = reader.GetString(reader.GetOrdinal("password_hash"));
                if (BCrypt.Net.BCrypt.Verify(model.Password, storedHash))
                {
                    var role = reader["role"]?.ToString();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, reader["user_id"]?.ToString() ?? ""),
                        new Claim(ClaimTypes.Name, reader["full_name"]?.ToString() ?? ""),
                        new Claim(ClaimTypes.Role, role ?? "")
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Redirect based on role
                    if (role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (role == "Customer")
                    {
                        return RedirectToAction("Dashboard", "User");
                    }
                   
                }
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        // =============================
        // Logout
        // =============================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
