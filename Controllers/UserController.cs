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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Password,IsEmailConfirmed")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                user.Password = _passwordHasher.HashPassword(user, user.Password);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetUsers));
            }
            return View(user);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = GetUserByEmail(model.Email);

                if (user == null || !VerifyPassword(user, model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View();
                }

                // Zapisujemy ID użytkownika do sesji
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                return RedirectToAction("Index", "Home");
            }

            // Jeśli model jest niepoprawny, zwracamy widok z błędami
            return View();
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
