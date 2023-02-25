using System.Diagnostics.CodeAnalysis;

namespace SBEU.Tasklet.Models.Responses
{
    public class UserDto
    {
        [NotNull]
        public string Id { get; set; }
        [NotNull]
        public string UserName { get; set; }
        [NotNull]
        public string Email { get; set; }
    }
}
