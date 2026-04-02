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
        public string Stat { get; set; }  // Nullable - only for STAT_CHANGE

        // to handle stat inputs

        [JsonProperty("newValue")]
        public int NewValue { get; set; }

        // "STAT_UPDATE" or "CLASS_CHANGE" or "JOB_LEVEL_CHANGE"

        [JsonProperty("type")]
        public string Type { get; set; }

        // to handle "Swordsman", "Mage", etc.

        [JsonProperty("ClassName")]
        public string ClassName { get; set; } // Only for CLASS_CHANGE

        [JsonProperty("value")]
        public int Value { get; set; }  // Alternative field name from JS

        [JsonProperty("weapon")]
        public string Weapon { get; set; }  // Only for WEAPON_CHANGE

        // Constructor with safe defaults
        public StatUpdateMessage()
        {
            Type = "STAT_CHANGE";
            Stat = null;
            NewValue = 1;
            Value = 1;
            ClassName = null;
            Weapon = null;
        }
    }
}
