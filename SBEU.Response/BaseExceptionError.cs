using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Response
{
    public class BaseExceptionError<T>
    {
        public string Exception { get; set; }
        public T? Details { get; set; }
        public string? Message { get; set; }
    }
}
