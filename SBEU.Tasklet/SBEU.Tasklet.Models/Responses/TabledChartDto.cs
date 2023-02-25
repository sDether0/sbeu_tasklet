using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class TabledChartDto
    {
        public SmallTableDto Table {  get; set; }
        public List<UserChartDto> Chart { get; set; }
    }
}
