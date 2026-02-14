using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatSimulation.Backend
{
    public class StatUpdateMessage
    {
        [JsonProperty("stat")]
        public string Stat { get; set; }

        [JsonProperty("newValue")]
        public int NewValue { get; set; }
    }
}
