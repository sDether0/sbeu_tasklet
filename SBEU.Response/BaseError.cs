using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Response
{
    public class BaseError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public BaseExceptionError<List<DetailError>> Exception { get; set; }
    }
}
