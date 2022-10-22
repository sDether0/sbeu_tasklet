using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Requests
{
    public class CreateChatRequest
    {
        public string? Title { get; set; }
        public List<string> UserIds { get; set; }
        public bool Private { get; set; }
    }
}
