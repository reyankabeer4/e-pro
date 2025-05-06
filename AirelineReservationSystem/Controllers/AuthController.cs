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

        // Register (GET)
        [HttpGet]
        public IActionResult Register() => View();

        // Register (POST)
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(@"
                INSERT INTO users (first_name, last_name, email, password_hash, phone_number, username, address, role, created_at)
                VALUES (@first_name, @last_name, @email, @hash, @phone, @username, @address, 'Customer', CURRENT_TIMESTAMP)", conn);

            cmd.Parameters.AddWithValue("first_name", model.FirstName);
            cmd.Parameters.AddWithValue("last_name", model.LastName);
            cmd.Parameters.AddWithValue("email", model.Email);
            cmd.Parameters.AddWithValue("hash", passwordHash);
            cmd.Parameters.AddWithValue("phone", model.PhoneNumber);
            cmd.Parameters.AddWithValue("username", model.Username);
            cmd.Parameters.AddWithValue("address", (object?)model.Address ?? DBNull.Value);

            try
            {
                await cmd.ExecuteNonQueryAsync();
                return RedirectToAction("Login");
            }
            catch (PostgresException ex) when (ex.SqlState == "23505") // Unique constraint violation
            {
                ModelState.AddModelError("", "Email or username already exists.");
                return View(model);
            }
        }

        // Login (GET)
        [HttpGet]
        public IActionResult Login() => View();

        // Login (POST)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(@"
                SELECT user_id, first_name, last_name, email, address, phone_number, password_hash, role, created_at, username
                FROM users 
                WHERE email = @email", conn);

            cmd.Parameters.AddWithValue("email", model.Email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var storedHash = reader.GetString(reader.GetOrdinal("password_hash"));
                if (BCrypt.Net.BCrypt.Verify(model.Password, storedHash))
                {
                    var role = reader["role"]?.ToString() ?? "Customer";
                    var userId = reader.GetInt32(reader.GetOrdinal("user_id")).ToString();
                    var firstName = reader["first_name"]?.ToString() ?? "";
                    var lastName = reader["last_name"]?.ToString() ?? "";
                    var email = reader["email"]?.ToString() ?? "";
                    var phone = reader["phone_number"]?.ToString() ?? "";
                    var username = reader["username"]?.ToString() ?? "";
                    var address = reader.IsDBNull(reader.GetOrdinal("address")) ? "" : reader.GetString(reader.GetOrdinal("address"));
                    var createdAt = reader["created_at"] is DateTime date ? date.ToString("o") : "Unknown";

                    // Set claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(ClaimTypes.Name, $"{firstName} {lastName}".Trim()),
                        new Claim(ClaimTypes.Email, email),
                        new Claim("PhoneNumber", phone),
                        new Claim(ClaimTypes.Role, role),
                        new Claim("CreatedAt", createdAt),
                        new Claim("Username", username),
                        new Claim("address", address),
                        new Claim("FirstName", firstName),
                        new Claim("LastName", lastName)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in the user
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return role == "Admin"
                        ? RedirectToAction("Dashboard", "Admin")
                        : RedirectToAction("Dashboard", "User");
                }
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
