using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class TimedUserChartDto
    {
        public UserDto User { get; set; }
        public List<TimedChartDto> Chart { get; set; }
    }
}
