using Microsoft.AspNetCore.Identity;
using SecretKeeper.Models;
using SecretsKeeper.Models;

namespace SecretsKeeper.Controllers
{
    public class SecretController
    {
        private readonly SecretsKeeperContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        public SecretController(SecretsKeeperContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

    }
}
