using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecretKeeper.Models;
using SecretsKeeper.Models;

namespace SecretsKeeper.Controllers
{
    public class UserController : Controller
    {
        private readonly SecretsKeeperContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserController(SecretsKeeperContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<IActionResult> GetUsers()
        {
            return View(await _context.Users.ToListAsync());
        }

        [HttpGet]
        [Route("api/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Password,
                    u.IsEmailConfirmed
                })
                .ToListAsync();

            return Ok(users);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
                return View(user);
            }
            else return NotFound();
        }

        public User GetUserByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(m => m.Email == email);
            return user;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            if (user.Secrets != null && user.Secrets.Any())
            {
                foreach (var secret in user.Secrets)
                {
                    secret.User = user;
                    secret.UserId = user.Id;
                }
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
                return View(user);
            }
            else return NotFound();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("LogIn");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _context.Users.Include(u => u.Secrets).FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (result == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("UserSecrets", new { id = user.Id });
            }
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        [HttpGet("UserSecrets/{id}")]
        public async Task<IActionResult> UserSecrets(int id)
        {
            var user = await _context.Users
                .Include(u => u.Secrets)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        public bool VerifyPassword(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login", "Auth");
        }
    }
}
