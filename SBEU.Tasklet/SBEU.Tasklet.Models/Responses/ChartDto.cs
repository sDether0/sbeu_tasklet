using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class ChartDto
    {
        public uint TotalTaskCount { get; set; }
        public uint CompletedTaskCount { get; set; }
        public uint ClosedTaskCount { get; set; }
        public uint AuthoredTaskCount { get; set; }
        public uint TotalPrice { get; set; }
    }
}
