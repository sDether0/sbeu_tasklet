using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Requests
{
    public class NoteRequest
    {
        [Required]
        public string TaskId { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
