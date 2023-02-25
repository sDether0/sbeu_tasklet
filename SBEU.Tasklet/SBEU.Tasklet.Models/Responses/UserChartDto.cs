using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class UserChartDto
    {
        public UserDto User { get; set; }
        public ChartDto Chart { get; set; }
    }
}
