using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SBEU.Tasklet.Models.Requests
{
    public class UserLoginRequest
    {
        [Required, NotNull]
        public string Email { get; set; }
        [Required, NotNull]
        public string Password { get; set; }
    }
}
