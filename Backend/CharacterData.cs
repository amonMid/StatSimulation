using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatSimulation.Backend
{
    public class CharacterData
    {
        public int BaseLevel { get; set; } = 1;
        public int JobLevel { get; set; } = 1;

        // Primary Stats
        public int Str { get; set; } = 1;
        public int Agi { get; set; } = 1;
        public int Vit { get; set; } = 1;
        public int Int { get; set; } = 1;
        public int Dex { get; set; } = 1;
        public int Luk { get; set; } = 1;

        //can add more later, like Job type or equipment
        public string Job { get; set; } = "Novice";
    }
}
