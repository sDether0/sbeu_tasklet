using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum XContentType
    {
        Image,
        Video,
        Sound,
        Other
    }
}
