using System.ComponentModel.DataAnnotations;

namespace SBEU.Tasklet.Models.Requests
{
    public class UserLoginRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
