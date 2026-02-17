using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace StatSimulation.Backend
{
    public class CalculationResult
    {
        public string MaxHp { get; set; }
        public string MaxSp { get; set; }
        public string HpRegen { get; set; }
        public string SpRegen { get; set; }
        public string HpRecoveryItem { get; set; }
        public string SpRecoveryItem { get; set; }
        public string Atk{ get; set; }
        public string Def { get; set; }
        public string Matk { get; set; }
        public string Mdef { get; set; }
        public string Hit { get; set; }
        public string Flee { get; set; }
        public string PerfectDodge { get; set; }
        public string CastTime { get; set; }  
        public string Aspd { get; set; }
        public int StatusPoints { get; set; }
        public string Crit { get; set; }
        public int NextStrCost { get; set; }
        public int NextAgiCost { get; set; }
        public int NextVitCost { get; set; }
        public int NextIntCost { get; set; }
        public int NextDexCost { get; set; }
        public int NextLukCost { get; set; }
        public bool IsOverspent { get; set; }
        public int Str { get; set; }
        public int Agi { get; set; }
        public int Vit { get; set; }
        public int Int { get; set; }
        public int Dex { get; set; }
        public int Luk { get; set; }
        public int BaseLv { get; set; }

    }

}
