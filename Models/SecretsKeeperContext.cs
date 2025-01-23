using Microsoft.EntityFrameworkCore;
using SecretKeeper.Models;
using System;

namespace SecretsKeeper.Models
{
    public class SecretsKeeperContext : DbContext
    {
        public SecretsKeeperContext(DbContextOptions<SecretsKeeperContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Secret> Secrets { get; set; }

    }
}
