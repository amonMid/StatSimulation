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

        // to handle stat inputs

        [JsonProperty("newValue")]
        public int NewValue { get; set; }

        // "STAT_UPDATE" or "CLASS_CHANGE" or "JOB_LEVEL_CHANGE"

        [JsonProperty("type")]
        public string Type { get; set; }

        // to handle "Swordsman", "Mage", etc.

        [JsonProperty("class")]
        public string ClassName { get; set; }
    }
}
