using System.ComponentModel.DataAnnotations;

namespace Unique_1.Models
{
    public class Users1
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
