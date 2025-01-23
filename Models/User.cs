using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretKeeper.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public ICollection<Secret> Secrets { get; set; } = new List<Secret>();
    }
}
