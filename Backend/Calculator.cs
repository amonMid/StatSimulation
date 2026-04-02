using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatSimulation.Backend
{
    public static class Calculator
    {
        // Constants 
        // Centralised so a job system can override these later
        private const int BASE_HIT = 175;   // All characters start with 175 HIT
        private const int BASE_FLEE = 10;    // Base flee bonus before stats
        private const int MAX_ASPD = 190;

        public static CalculationResult CalculateAll(CharacterData charData)
        {
            var res = new CalculationResult();
            var job = JobRegistry.Get(charData.Job);

            // ── Get job bonuses based on current job level ───────────────
            int bonusStr = job.GetStatBonus(job.StrBonusTable, charData.JobLevel);
            int bonusAgi = job.GetStatBonus(job.AgiBonusTable, charData.JobLevel);
            int bonusVit = job.GetStatBonus(job.VitBonusTable, charData.JobLevel);
            int bonusInt = job.GetStatBonus(job.IntBonusTable, charData.JobLevel);
            int bonusDex = job.GetStatBonus(job.DexBonusTable, charData.JobLevel);
            int bonusLuk = job.GetStatBonus(job.LukBonusTable, charData.JobLevel);



            // ── Skill Bonuses ─────────────────────────────────────────────
            int skillStr = 0, skillAgi = 0, skillVit = 0, skillInt = 0, skillDex = 0, skillLuk = 0;

            if (charData.SkillLevels.TryGetValue("owl_eye", out int owlEyeLv))
            {
                skillDex += owlEyeLv;
                res.SkillBonuses.Add($"Owl's Eye [Lv {owlEyeLv}]: DEX +{owlEyeLv}");
            }

            // ── Apply bonuses to stats ────────────────────────────────────
            int totalStr = charData.Str + bonusStr + skillStr;
            int totalAgi = charData.Agi + bonusAgi + skillAgi;
            int totalVit = charData.Vit + bonusVit + skillVit;
            int totalInt = charData.Int + bonusInt + skillInt;
            int totalDex = charData.Dex + bonusDex + skillDex;
            int totalLuk = charData.Luk + bonusLuk + skillLuk;

            // Formula: STR + (STR/10)^2 + (DEX/5) + (LUK/5)
            // ── ATK ──────────────────────────────────────────────────────
            int batk;
            int temp_dex;
            bool isRangedWeapon = IsRangedWeapon(charData.EquippedWeapon);

            if (isRangedWeapon)
            {
                // Ranged weapons: DEX is primary stat, STR is secondary
                batk = totalDex;
                temp_dex = totalStr;
            }
            else
            {
                // Melee weapons: STR is primary stat, DEX is secondary
                batk = totalStr;
                temp_dex = totalDex;
            }

            // (primary_stat / 10)^2
            int dstr = batk / 10;
            batk += dstr * dstr;

            // Add secondary stat bonus and LUK bonus
            batk += temp_dex / 5 + totalLuk / 5;

            // ── Skill Bonuses: Masteries ──────────────────────────────────
            int masteryBonus = 0;
            if (charData.EquippedWeapon == WeaponType.OnehandedSword && charData.SkillLevels.TryGetValue("sword_mastery", out int smLv))
            {
                masteryBonus = smLv * 4;
            }
            else if (charData.EquippedWeapon == WeaponType.TwohandedSword && charData.SkillLevels.TryGetValue("two_mastery", out int tmLv))
            {
                masteryBonus = tmLv * 4;
            }

            res.Atk = $"{batk} + {masteryBonus}";

            // --- WEIGHT LIMIT ---
            // Formula: Base Job Weight + (Base STR * 30)
            int weightSkillBonus = 0;
            if (charData.SkillLevels.TryGetValue("enlarge_weight", out int ewLv))
            {
                weightSkillBonus = ewLv * 200;
            }
            res.MaxWeight = 2000 + job.Weight + (charData.Str * 30) + weightSkillBonus;

            // ── ASPD ─────────────────────────────────────────────────────
            res.Aspd = UpdateAspd(charData, job, res, totalAgi, totalDex, false);

            // --- AGI: FLEE ---
            // Formula:  BaseLevel + AGI + 10 (base bonus)
            int fleeSkillBonus = 0;
            if (charData.SkillLevels.TryGetValue("improve_dodge", out int idLv))
            {
                fleeSkillBonus = idLv * 3;
            }
            res.Flee = $"{charData.BaseLevel + totalAgi + fleeSkillBonus} + {(totalLuk + 10) * 10 / 100}";

            // --- DEF --- 
            // Hard DEF (equipment)
            int hardDef = 0;  // No equipment in calculator

            // Soft DEF (VIT-based)
            int softDef = (int)Math.Floor(totalVit * 0.5);
            int vitLinear = (int)Math.Floor(totalVit * 0.3);
            int vitQuadratic = (int)Math.Floor((totalVit * totalVit) / 150.0) - 1;
            softDef += Math.Max(vitLinear, vitQuadratic);

            res.Def = $"{hardDef} + {totalVit}";

            // --- MDEF ---
            // INT-based soft MDEF = 1 per INT
            // VIT also grants: floor(VIT / 2) soft MDEF
            int vitMdefBonus = totalVit / 2;
            res.Mdef = $"0 + {totalInt}";

            // --- HP CALCULATIONS ---
            // Uses the accurate job-growth loop; VIT scales the result
            int totalMaxHp = CalculateMaxHP(charData.BaseLevel, totalVit, job);
            res.MaxHp = totalMaxHp.ToString();

            //int finalMaxHp = CalculateMaxHP(charData.BaseLevel, charData.Str);
            //res.MaxHp = finalMaxHp.ToString();

            // HpRegen
            // Formula:  floor(MaxHP / 200) + floor(VIT / 5)   every 6 seconds
            //int baseHpr = Math.Max(1, totalMaxHp / 200);
            int hpRegenValue = (int)CalculateHPRegen(totalMaxHp, totalVit);

            //res.HpRegen = $"{hpRegenValue} per 6s standing (per 3s sitting)";
            // Increase HP Recovery skill bonus
            if (charData.SkillLevels.TryGetValue("hp_recovery", out int hpRecLv))
            {
                int flatBonus = hpRecLv * 5;
                double percentBonus = hpRecLv * 0.002; // 0.2% per level
                int percentRegen = (int)(totalMaxHp * percentBonus);
                int totalSkillRegen = flatBonus + percentRegen;

                res.SkillBonuses.Add(
                    $"Increase HP Recovery [Lv {hpRecLv}]: +{flatBonus} + {percentRegen} (from MaxHP) per 10s idle"
                );

                res.HpRegen = $"{hpRegenValue} per 6s standing (per 3s sitting) + {totalSkillRegen} per 10s (idle)";
            }
            else
            {
                res.HpRegen = $"{hpRegenValue} per 6s standing (per 3s sitting)";
            }
            // --- SP CALCULATIONS ---
            // Formula:  BaseSP × (1 + INT × 0.01)
            int baseSp = CalculateBaseSP(charData.BaseLevel, totalInt, job);
            res.MaxSp = baseSp.ToString();

            // --- SP Natural Regen ---
            // Formula:  floor(MaxSP / 100) + floor(INT / 6) + 1   every 8 seconds
            // Bonus: every 100 SP grants +1 regen (already embedded in MaxSP / 100)
            int spRegenValue = (baseSp / 100) + (totalInt / 6) + 1;
            //res.SpRegen = $"{spRegenValue} per 8s standing (per 4s sitting)";
            int sRecoveryBonus = 0;
            double sRecoveryPercent = 0;

            if (charData.SkillLevels.TryGetValue("sp_recovery", out int spRecLv))
            {
                // Flat bonus: 3 * level
                sRecoveryBonus = 3 * spRecLv;

                // Percent bonus: 0.2% * level
                sRecoveryPercent = 0.002 * spRecLv;

                int percentRegen = (int)(baseSp * sRecoveryPercent);

                int totalSkillRegen = sRecoveryBonus + percentRegen;

                res.SkillBonuses.Add(
                    $"Increase SP Recovery [Lv {spRecLv}]: +{sRecoveryBonus} + {percentRegen} (from MaxSP)"
                );

                // Final SP Regen (separate display)
                res.SpRegen = $"{spRegenValue} per 8s  standing (per 4s sitting) + {totalSkillRegen} per 10s (idle)";
            }
            else
            {
                res.SpRegen = $"{spRegenValue} per 8s standing (per 4s sitting)";
            }

            // --- RECOVERY ITEM EFFECTIVENESS ---
            // HP items: Base × (1 + VIT × 0.02)  → +2% per VIT
            // SP items: Base × (1 + INT × 0.01)  → +1% per INT
            res.HpRecoveryItem = $"+{totalVit * 2}%";
            res.SpRecoveryItem = $"+{totalInt * 1}%";

            // --- INT ---
            // Min MATK = INT + floor(INT/7)^2
            // Max MATK = INT + floor(INT/5)^2
            int minMatk = totalInt + (int)Math.Pow(totalInt / 7, 2);
            int maxMatk = totalInt + (int)Math.Pow(totalInt / 5, 2);
            res.Matk = $"{minMatk} ~ {maxMatk}";

            // --- DEX ---
            // Formula:  175 (base) + BaseLevel + DEX
            res.Hit = $"{charData.BaseLevel + totalDex}";

            // --- CAST TIME ---
            // 150 DEX = instant cast; each DEX = −(1/150) of cast time
            double castReduction = (totalDex / 150.0) * 100.0;
            res.CastTime = totalDex >= 150 ? "Instant" : $"{Math.Floor(castReduction)}%";

            // --- LUK ---
            res.Crit = $"{(int)(totalLuk * 3 + 10) * 10 / 100}";

            // Perfect Dodge = [LUK * 0.1] + 1 (Base perfect dodge is usually 1)
            res.PerfectDodge = $"{(totalLuk * 0.1) + 1:F1}";

            // --- POINTS LOGIC ---
            int totalPointsGained = GetTotalPointsForLevel(charData.BaseLevel);
            int totalPointsSpent = GetStatInvestmentCost(charData.Str) +
                                   GetStatInvestmentCost(charData.Agi) +
                                   GetStatInvestmentCost(charData.Vit) +
                                   GetStatInvestmentCost(charData.Int) +
                                   GetStatInvestmentCost(charData.Dex) +
                                   GetStatInvestmentCost(charData.Luk);

            // Flag if the user has exceeded their budget
            res.StatusPoints = totalPointsGained - totalPointsSpent;
            res.IsOverspent = res.StatusPoints < 0;

            // ── NEXT STAT COSTS ───────────────────────────────────────────
            res.NextStrCost = ((charData.Str - 1) / 10) + 2;
            res.NextAgiCost = ((charData.Agi - 1) / 10) + 2;
            res.NextVitCost = ((charData.Vit - 1) / 10) + 2;
            res.NextIntCost = ((charData.Int - 1) / 10) + 2;
            res.NextDexCost = ((charData.Dex - 1) / 10) + 2;
            res.NextLukCost = ((charData.Luk - 1) / 10) + 2;

            // ── PASS THROUGH VALUES ───────────────────────────────────────
            res.Str = charData.Str;
            res.Agi = charData.Agi;
            res.Vit = charData.Vit;
            res.Int = charData.Int;
            res.Dex = charData.Dex;
            res.Luk = charData.Luk;
            res.BaseLv = charData.BaseLevel;
            res.JobLv = charData.JobLevel;

            // ── JOB BONUSES (for UI display) ─────────────────────────────
            // ── Skill Bonuses: Other ──────────────────────────────────────
            if (charData.SkillLevels.TryGetValue("enlarge_weight", out int ewLv2))
                res.SkillBonuses.Add($"Increase Weight [Lv {ewLv2}]: +{ewLv2 * 200} Max Weight");

            if (charData.SkillLevels.TryGetValue("improve_dodge", out int idLv2))
                res.SkillBonuses.Add($"Improve Dodge [Lv {idLv2}]: +{idLv2 * 3} FLEE");

            if (charData.SkillLevels.TryGetValue("sword_mastery", out int smLv2) && charData.EquippedWeapon == WeaponType.OnehandedSword)
                res.SkillBonuses.Add($"Sword Mastery [Lv {smLv2}]: +{smLv2 * 4} ATK");

            if (charData.SkillLevels.TryGetValue("two_mastery", out int tmLv2) && charData.EquippedWeapon == WeaponType.TwohandedSword)
                res.SkillBonuses.Add($"Two-Handed Mastery [Lv {tmLv2}]: +{tmLv2 * 4} ATK");

            // ── PASS THROUGH VALUES ───────────────────────────────────────
            res.Str = charData.Str;
            res.Agi = charData.Agi;
            res.Vit = charData.Vit;
            res.Int = charData.Int;
            res.Dex = charData.Dex;
            res.Luk = charData.Luk;
            res.BaseLv = charData.BaseLevel;
            res.JobLv = charData.JobLevel;

            // Pure Job Bonuses
            res.JobBonusStr = bonusStr;
            res.JobBonusAgi = bonusAgi;
            res.JobBonusVit = bonusVit;
            res.JobBonusInt = bonusInt;
            res.JobBonusDex = bonusDex;
            res.JobBonusLuk = bonusLuk;

            // Total Bonuses (Skill + Job)
            res.BonusStr = bonusStr + skillStr;
            res.BonusAgi = bonusAgi + skillAgi;
            res.BonusVit = bonusVit + skillVit;
            res.BonusInt = bonusInt + skillInt;
            res.BonusDex = bonusDex + skillDex;
            res.BonusLuk =  bonusLuk + skillLuk;

            return res;
        }

        public static int GetTotalPointsForLevel(int level)
        {
            int total = 48;
            for (int i = 1; i < level; i++) total += (i / 5) + 3;
            return total;
        }

        public static int GetStatInvestmentCost(int targetValue)
        {
            int spent = 0;
            for (int i = 1; i < targetValue; i++) spent += ((i - 1) / 10) + 2;
            return spent;
        }

        private static int CalculateMaxHP(int baseLevel, int totalVit, JobData job, bool isTrans = false)
        {
            decimal hpJobA = (decimal)job.HpJobA;
            decimal hpJobB = (decimal)job.HpJobB;

            decimal baseHp = 35 + (baseLevel * hpJobB);

            for (int i = 2; i <= baseLevel; i++)
            {
                baseHp += Math.Round(hpJobA * i, MidpointRounding.AwayFromZero);
            }

            double vitMultiplier = 1.0 + (totalVit * 0.01);
            double transMod = isTrans ? 1.25 : 1.0;

            return (int)Math.Floor((double)baseHp * vitMultiplier * transMod);
        }

        private static double CalculateHPRegen(int totalMaxHp, int totalVit, double hprMod = 0)
        {
            // Base: floor(MaxHP / 200)
            int baseHpr = totalMaxHp / 200;

            // VIT Bonus: floor(VIT / 5)
            int vitBonus = totalVit / 5;

            // Combine them AND add the base 1
            // The official formula is (floor(MaxHP/200) + floor(VIT/5) + 1)
            int combinedBase = baseHpr + vitBonus + 1;

            // Ensure it's at least 1 (though the +1 handles this)
            combinedBase = Math.Max(1, combinedBase);

            // Apply Modifiers: floor( CombinedBase * (1 + hprMod/100) )
            double modifierScale = 1.0 + (hprMod * 0.01);

            // Using a small epsilon to handle double precision issues
            int finalHpr = (int)Math.Floor((combinedBase * modifierScale) + 0.00001);

            return finalHpr;

        }

        // ── SP CALCULATION ────────────────────────────────────────────────────
        // Scaled by INT in CalculateAll:  TotalSP = BaseSP × (1 + INT × 0.01)
        private static int CalculateBaseSP(int baseLevel, int intel, JobData job)
        {
            // Calculate BASE_SP
            // 10 + (Level * Job_Multiplier)
            double baseSp = 10.0 + (baseLevel * job.SpJobB);

            // Apply INT Bonus
            // Formula: floor( BASE_SP * (1 + INT * 0.01) )
            double intMod = 1.0 + (intel * 0.01);
            int maxSp = (int)Math.Floor(baseSp * intMod);

            return maxSp;
        }

        // HELPER: Check if weapon is ranged type
        private static bool IsRangedWeapon(WeaponType weapon)
        {
            return weapon switch
            {
                WeaponType.Bow => true,
                // Add more ranged types
                // WeaponType.Musical => true,
                // WeaponType.Whip => true,
                // WeaponType.Revolver => true,
                // WeaponType.Rifle => true,
                // WeaponType.Gatling => true,
                // WeaponType.Shotgun => true,
                // WeaponType.Grenade => true,
                _ => false
            };
        }

        public static string UpdateAspd(CharacterData charData, JobData job, CalculationResult res, int totalAgi, int totalDex, bool showDebug = false)
        {
            //int totalAgi = charData.Agi + job.GetStatBonus(job.AgiBonusTable, charData.JobLevel);
            //int totalDex = charData.Dex + job.GetStatBonus(job.DexBonusTable, charData.JobLevel);

            double btba = job.GetBTBA(charData.EquippedWeapon);
            double wd = 50.0 * btba;

            double agiReduction = Math.Floor((wd * totalAgi) / 25.0);
            double dexReduction = Math.Floor((wd * totalDex) / 100.0);
            double totalReduction = agiReduction + dexReduction;

            double delayAfterStats = (wd - totalReduction) / 10.0;
            double internalAspd = 200.0 - delayAfterStats;

            double offset = 45.0 + ((btba - 1.0) * 45.0);
            double displayAspd = internalAspd - offset;
            displayAspd = Math.Min(displayAspd, 190.0);

            if (showDebug)
            {
                System.Diagnostics.Debug.WriteLine($"[ASPD] Weapon: {charData.EquippedWeapon}, BTBA: {btba}");
                System.Diagnostics.Debug.WriteLine($"[ASPD] Internal: {internalAspd:F1}, Offset: {offset:F1}");
                System.Diagnostics.Debug.WriteLine($"[ASPD] Display: {displayAspd:F1} → {Math.Floor(displayAspd)}");
            }

            return Math.Floor(displayAspd).ToString();
        }

    }
}
