using SBEU.Tasklet.Models.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class ContentDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public XContentType ContentType { get; set; }
    }
}
