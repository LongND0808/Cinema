using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseInsightDotNet.Commons.Enums
{
    public class Enumerate
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Gender
        {
            Unknown = 0,
            Male = 1,
            Female = 2
        }
    }
}