using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Requests
{
    public class ConfirmEmailAuthRequest
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string MailCode { get; set; }
    }
}
