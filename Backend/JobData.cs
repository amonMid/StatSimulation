using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatSimulation.Backend
{
    public class JobData
    {
        public string Name { get; set; }
        public int MaxJobLevel { get; set; }

        // HP/SP growth constants
        public double HpJobA { get; set; }
        public double HpJobB { get; set; }
        public double SpJobA { get; set; }
        public double SpJobB { get; set; }

        // Job level → stat bonus tables (sorted by job level)
        public Dictionary<int, int> StrBonusTable { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> AgiBonusTable { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> VitBonusTable { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> IntBonusTable { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> DexBonusTable { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> LukBonusTable { get; set; } = new Dictionary<int, int>();

        public double WeaponDelay { get; set; } = 50.0;

        /// <summary>
        /// Get stat bonus for a specific job level.
        /// Returns the highest bonus where jobLevel >= threshold.
        /// </summary>
        public int GetStatBonus(Dictionary<int, int> table, int jobLevel)
        {
            int bonus = 0;
            foreach (var kvp in table)
            {
                if (jobLevel >= kvp.Key)
                    bonus = kvp.Value;
                else
                    break;
            }
            return bonus;
        }
    }

    public static class JobRegistry
    {
        private static readonly Dictionary<string, JobData> _jobs = new Dictionary<string, JobData>
        {
            // ── NOVICE ───────────────────────────────────────────────────
            ["Novice"] = new JobData
            {
                Name = "Novice",
                MaxJobLevel = 10,
                HpJobA = 5.0,
                HpJobB = 5.0,
                SpJobA = 1.0,
                SpJobB = 1.0,
                WeaponDelay = 50.0,
            },

            // ── SWORDSMAN ────────────────────────────────────────────────
            ["Swordsman"] = new JobData
            {
                Name = "Swordsman",
                MaxJobLevel = 50,
                HpJobA = 5.0,
                HpJobB = 7.0,
                SpJobA = 1.0,
                SpJobB = 0.8,

                StrBonusTable = new Dictionary<int, int>
                {
                    { 2, 1 }, { 14, 2 }, { 33, 3 }, { 40, 4 }, { 47, 5 }, { 49, 6 }, { 50, 7 }
                },
                AgiBonusTable = new Dictionary<int, int>
                {
                    { 30, 1 }, { 46, 2 }
                },
                VitBonusTable = new Dictionary<int, int>
                {
                    { 6, 1 }, { 18, 2 }, { 38, 3 }, { 42, 4 }
                },
                DexBonusTable = new Dictionary<int, int>
                {
                    { 10, 1 }, { 22, 2 }, { 36, 3 }
                },
                LukBonusTable = new Dictionary<int, int>
                {
                    { 26, 1 }, { 44, 2 }
                },

                WeaponDelay = 45.0,
            },

            // ── Mage ─────────────────────────────────────────────────
            ["Mage"] = new JobData
            {
                Name = "Mage",
                MaxJobLevel = 50,
                HpJobA = 3.0,
                HpJobB = 3.0,
                SpJobA = 1.5,
                SpJobB = 2.0,

                AgiBonusTable = new Dictionary<int, int>
                {
                    { 18, 1 }, { 26, 2 }, {40, 3 }, { 47, 4 }
                },
                IntBonusTable = new Dictionary<int, int>
                {
                    { 2, 1 }, { 14, 2 }, { 22, 3 }, { 33, 4 }, {38, 5 }, {44, 6 }, {46, 7 }, {50, 8 }
                },
                DexBonusTable = new Dictionary<int, int>
                {
                    { 6, 1 }, { 10, 2 }, { 36, 3 }
                },
                LukBonusTable = new Dictionary<int, int>
                {
                    { 30, 1 }, { 42, 2 }, {49, 3 }
                },

                WeaponDelay = 55.0,
            },

            // ── ARCHER ───────────────────────────────────────────────────
            ["Archer"] = new JobData
            {
                Name = "Archer",
                MaxJobLevel = 50,
                HpJobA = 4.5,
                HpJobB = 5.5,
                SpJobA = 1.0,
                SpJobB = 1.0,


                StrBonusTable = new Dictionary<int, int>
                {
                    { 6, 1 }, { 38, 2 }, { 40, 3 }
                },
                AgiBonusTable = new Dictionary<int, int>
                {
                    { 26, 1 }, { 33, 2 }, {49, 3 }
                },
                VitBonusTable = new Dictionary<int, int>
                {
                    { 46, 1 }
                },
                IntBonusTable = new Dictionary<int, int>
                {
                    { 10, 1 }, {47, 2 }
                },
                DexBonusTable = new Dictionary<int, int>
                {
                    { 2, 1 }, { 14, 2 }, { 18, 3 }, {30, 4}, {36, 5 }, {42, 6}, {50, 7}
                },
                LukBonusTable = new Dictionary<int, int>
                {
                    { 22, 1 }, { 44, 2 }
                },

                WeaponDelay = 42.0,
            },

            // ── THIEF ────────────────────────────────────────────────────
            ["Thief"] = new JobData
            {
                Name = "Thief",
                MaxJobLevel = 50,
                HpJobA = 4.0,
                HpJobB = 5.0,
                SpJobA = 1.0,
                SpJobB = 1.2,

                StrBonusTable = new Dictionary<int, int>
                {
                    { 6, 1 }, { 30, 2 }, { 38, 3 }, { 47, 4}
                },
                AgiBonusTable = new Dictionary<int, int>
                {
                    { 2, 1 }, { 33, 2 }, {36, 3 }, {50, 4 }
                },
                VitBonusTable = new Dictionary<int, int>
                {
                    { 14, 1 }, {44, 2 }
                },
                IntBonusTable = new Dictionary<int, int>
                {
                    { 18, 1 }
                },
                DexBonusTable = new Dictionary<int, int>
                {
                    { 10, 1 }, { 22, 2 }, { 42, 3 }, {49, 4}
                },
                LukBonusTable = new Dictionary<int, int>
                {
                    { 26, 1 }, { 40, 2 }, {46, 3 }
                },
                WeaponDelay = 40.0,
            },

            // ── ACOLYTE ──────────────────────────────────────────────────
            ["Acolyte"] = new JobData
            {
                Name = "Acolyte",
                MaxJobLevel = 50,
                HpJobA = 4.0,
                HpJobB = 5.0,
                SpJobA = 1.2,
                SpJobB = 1.5,

                StrBonusTable = new Dictionary<int, int>
                {
                    { 26, 1 }, { 42, 2 }, { 49, 3 }
                },
                AgiBonusTable = new Dictionary<int, int>
                {
                    { 22, 1 }, { 40, 2 }
                },
                VitBonusTable = new Dictionary<int, int>
                {
                    { 6, 1 }, { 30, 2 }, { 44, 3}
                },
                IntBonusTable = new Dictionary<int, int>
                {
                    { 10, 1 }, { 33, 2 }, { 46, 3 }
                },
                DexBonusTable = new Dictionary<int, int>
                {
                    { 14, 1 }, { 36, 2 }, { 46, 3 }
                },
                LukBonusTable = new Dictionary<int, int>
                {
                    { 2, 1 }, { 18, 2 }, { 38, 3 }, { 50, 4 }
                },

                WeaponDelay = 52.0,
            },

            // ── MERCHANT ─────────────────────────────────────────────────
            ["Merchant"] = new JobData
            {
                Name = "Merchant",
                MaxJobLevel = 50,
                HpJobA = 4.5,
                HpJobB = 6.0,
                SpJobA = 1.0,
                SpJobB = 1.0,

                StrBonusTable = new Dictionary<int, int>
                {
                    { 10, 1 }, { 22, 2 }, { 40, 3 }, { 44, 4 }, { 49, 5 }
                },
                AgiBonusTable = new Dictionary<int, int>
                {
                    { 33, 1 }
                },
                VitBonusTable = new Dictionary<int, int>
                {
                    { 2, 1 }, { 18, 2 }, { 30, 3 }, { 47, 4 }
                },
                IntBonusTable = new Dictionary<int, int>
                {
                    { 26, 1 }
                },
                DexBonusTable = new Dictionary<int, int>
                {
                    { 6, 1 }, { 14, 2 }, { 38, 3 }, { 42, 4 }, { 50, 5 }
                },
                LukBonusTable = new Dictionary<int, int>
                {
                    { 36, 1 }, { 46, 2 }
                },

                WeaponDelay = 48.0,
            },
        };

        public static JobData Get(string jobName)
        {
            return _jobs.TryGetValue(jobName, out var job) ? job : _jobs["Novice"];
        }

        public static IEnumerable<string> GetAllJobNames()
        {
            return _jobs.Keys;
        }

        public static bool IsValidJobLevel(string jobName, int jobLevel)
        {
            var job = Get(jobName);
            return jobLevel >= 1 && jobLevel <= job.MaxJobLevel;
        }
    }
}
