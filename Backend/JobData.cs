using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatSimulation.Backend
{
    public class JobData
    {
        // Basic info
        public string Name { get; set; }
        public int MaxJobLevel { get; set; }

        //Weight
        public int Weight { get; set; }

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

        public double WeaponDelay { get; set; }
        //public int AspdOffset { get; set; } = 35;  // Default AspdOffSet

        public Dictionary<WeaponType, int> WeaponAspdOffsets { get; set; } = new Dictionary<WeaponType, int>();

        public int GetAspdOffset(WeaponType weapon)
        {
            return WeaponAspdOffsets.TryGetValue(weapon, out int offset) ? offset : 35;
        }

        //Weapon-specific delays
        public Dictionary<WeaponType, double> WeaponDelays { get; set; } = new Dictionary<WeaponType, double>();

        public double GetWeaponDelay(WeaponType weapon)
        {
            // If the weapon isn't defined for this job, return a very high delay (slow)
            return WeaponDelays.TryGetValue(weapon, out double delay) ? delay : 2000.0;
        }

        // Base Time Between Attacks

        public Dictionary<WeaponType, double> WeaponBTBAs { get; set; } = new Dictionary<WeaponType, double>();

        public double GetBTBA(WeaponType weapon)
        {
            return WeaponBTBAs.TryGetValue(weapon, out double val) ? val : 2.0; // Default slow
        }


        //Get stat bonus for a specific job level.
        // Returns the highest bonus where jobLevel >= threshold.
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
                HpJobA = 0,
                HpJobB = 5.0,
                SpJobA = 1.0,
                SpJobB = 1.0,
                Weight = 0,

                //WeaponDelay = 50.0,

                WeaponDelays = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 495 },            
                    { WeaponType.Dagger, 650 },          
                    { WeaponType.OnehandedSword, 698 },  
                    { WeaponType.OnehandedAxe, 803 },    
                    { WeaponType.OnehandedMace, 698 },   
                    { WeaponType.TwohandedMace, 698 },   
                    { WeaponType.RodStaff, 650 },        
                    { WeaponType.TwohandedStaff, 650 }   
                },

                //BTBAs value
                WeaponBTBAs = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 1.0 },
                    { WeaponType.Dagger, 1.3 },
                    { WeaponType.OnehandedSword, 1.4 },
                    { WeaponType.OnehandedAxe, 1.6 },
                    { WeaponType.OnehandedMace, 1.4 },
                    { WeaponType.TwohandedMace, 1.4 },
                    { WeaponType.RodStaff, 1.3 },
                    { WeaponType.TwohandedStaff, 1.3 }
                }
            },

            // ── SWORDSMAN ────────────────────────────────────────────────
            ["Swordsman"] = new JobData
            {
                Name = "Swordsman",
                MaxJobLevel = 50,
                HpJobA = 0.7,
                HpJobB = 5.0,
                SpJobA = 1.0,
                SpJobB = 2.0,
                Weight = 800,

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

                //WeaponDelay = 45.0,

                WeaponDelays = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 405 },            
                    { WeaponType.Dagger, 500 },          
                    { WeaponType.OnehandedSword, 555 }, 
                    { WeaponType.TwohandedSword, 608 },  
                    { WeaponType.OnehandedSpear, 650 },  
                    { WeaponType.TwohandedSpear, 700 },  
                    { WeaponType.OnehandedAxe, 700 },    
                    { WeaponType.TwohandedAxe, 755 },    
                    { WeaponType.OnehandedMace, 650 },   
                    { WeaponType.TwohandedMace, 700 }    
                },

                //BTBAs value
                WeaponBTBAs = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 0.8 },
                    { WeaponType.Dagger, 1 },
                    { WeaponType.OnehandedSword, 1.1 },
                    { WeaponType.TwohandedSword, 1.2 },
                    { WeaponType.OnehandedSpear, 1.3 },
                    { WeaponType.TwohandedSpear, 1.4 },
                    { WeaponType.OnehandedAxe, 1.4 },
                    { WeaponType.TwohandedAxe, 1.5 },
                    { WeaponType.OnehandedMace, 1.3 },
                    { WeaponType.TwohandedMace, 1.4 }
                }


            },

            // ── Mage ─────────────────────────────────────────────────
            ["Magician"] = new JobData
            {
                Name = "Magician",
                MaxJobLevel = 50,
                HpJobA = 0.3,
                HpJobB = 5.0,
                SpJobA = 1.5,
                SpJobB = 6.0,
                Weight = 200,


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


                WeaponAspdOffsets = new Dictionary<WeaponType, int>
                {
                    { WeaponType.Hand, 35 },
                    { WeaponType.Dagger, 46 },
                    { WeaponType.RodStaff,  57 },
                    { WeaponType.TwohandedStaff, 57 }
                },

                WeaponDelays = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 500 },
                    { WeaponType.Dagger, 600 },
                    { WeaponType.RodStaff, 700 },
                    { WeaponType.TwohandedStaff, 700 }
                },

                //BTBAs value
                WeaponBTBAs = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 1 },
                    { WeaponType.Dagger, 1.2 },
                    { WeaponType.RodStaff, 1.4 },
                    { WeaponType.TwohandedStaff, 1.4 }
                }
            },

            // ── ARCHER ───────────────────────────────────────────────────
            ["Archer"] = new JobData
            {
                Name = "Archer",
                MaxJobLevel = 50,
                HpJobA = 0.5,
                HpJobB = 5.0,
                SpJobA = 1.0,
                SpJobB = 2.0,
                Weight = 600,

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

                //WeaponDelay values
                WeaponDelays = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 405 },    
                    { WeaponType.Dagger, 600 },  
                    { WeaponType.Bow, 700 }      
                },

                //BTBAs value
                WeaponBTBAs = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 0.8 },
                    { WeaponType.Dagger, 1.2 },
                    { WeaponType.Bow, 1.4 }
                }
                //WeaponDelay = 42.0,
            },

            // ── THIEF ────────────────────────────────────────────────────
            ["Thief"] = new JobData
            {
                Name = "Thief",
                MaxJobLevel = 50,
                HpJobA = 0.5,
                HpJobB = 5.0,
                SpJobA = 1.0,
                SpJobB = 2.0,
                Weight = 400,

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

                //Weapon delays value
                WeaponDelays = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 405 },            
                    { WeaponType.Dagger, 500 },          
                    { WeaponType.OnehandedSword, 650 },  
                    { WeaponType.OnehandedAxe, 803 },    
                    { WeaponType.Bow, 803 },             
                },

                //BTBAs value
                WeaponBTBAs = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 0.8 },
                    { WeaponType.Dagger, 1 },
                    { WeaponType.OnehandedSword, 1.3 },
                    { WeaponType.OnehandedAxe, 1.6 },
                    { WeaponType.Bow, 1.6 }
                }

                //WeaponDelay = 40.0,
            },

            // ── ACOLYTE ──────────────────────────────────────────────────
            ["Acolyte"] = new JobData
            {
                Name = "Acolyte",
                MaxJobLevel = 50,
                HpJobA = 0.4,
                HpJobB = 5.0,
                SpJobA = 1.2,
                SpJobB = 5.0,
                Weight = 400,

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

                WeaponDelays = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 405 },            
                    { WeaponType.OnehandedMace, 600 },   
                    { WeaponType.TwohandedMace, 600 },   
                    { WeaponType.RodStaff, 600 },        
                    { WeaponType.TwohandedStaff, 600 }   
                },

                //BTBAs value
                WeaponBTBAs = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 0.8 },
                    { WeaponType.OnehandedMace, 1.2 },
                    { WeaponType.TwohandedMace, 1.2 },
                    { WeaponType.RodStaff, 1.2 },
                    { WeaponType.TwohandedStaff, 1.2 }
                }

                //WeaponDelay = 52.0,
            },

            // ── MERCHANT ─────────────────────────────────────────────────
            ["Merchant"] = new JobData
            {
                Name = "Merchant",
                MaxJobLevel = 50,
                HpJobA = 0.4,
                HpJobB = 5.0,
                SpJobA = 1.0,
                SpJobB = 3.0,
                Weight = 800,

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

                WeaponDelays = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 415 },            // Result: 164
                    { WeaponType.Dagger, 615 },          // Result: 147
                    { WeaponType.OnehandedSword, 720 },  // Result: 138
                    { WeaponType.OnehandedAxe, 720 },    // Result: 138
                    { WeaponType.TwohandedAxe, 765 },    // Result: 134
                    { WeaponType.OnehandedMace, 720 },   // Result: 138
                    { WeaponType.TwohandedMace, 720 }    // Result: 138
                },

                //BTBAs value
                WeaponBTBAs = new Dictionary<WeaponType, double>
                {
                    { WeaponType.Hand, 0.8 },
                    { WeaponType.Dagger, 1.2 },
                    { WeaponType.OnehandedSword, 1.4 },
                    { WeaponType.OnehandedAxe, 1.4 },
                    { WeaponType.TwohandedAxe, 1.5 },
                    { WeaponType.OnehandedMace, 1.4 },
                    { WeaponType.TwohandedMace, 1.4 }
                }

                //WeaponDelay = 48.0,
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
