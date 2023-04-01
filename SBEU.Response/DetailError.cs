using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Response
{
    public class DetailError
    {
        public List<string> Loc { get; set; }
        public string Code { get; set; }
        public string Msg { get; set; }
        public string Type { get; set; }
    }
}
