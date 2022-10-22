using System.ComponentModel.DataAnnotations;

namespace SBEU.Tasklet.Models.Requests
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
