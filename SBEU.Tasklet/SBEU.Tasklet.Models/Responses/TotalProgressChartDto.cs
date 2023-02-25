using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class TotalProgressChartDto
    {
        public uint TotalTaskCount { get; set; }
        public uint NewTaskCount { get; set; }
        public uint DoingTaskCount { get; set; }
        public uint DoneTaskCount { get; set; }
        public uint ReviewTaskCount { get; set; }
        public uint ClosedTaskCount { get; set; }
    }
}
