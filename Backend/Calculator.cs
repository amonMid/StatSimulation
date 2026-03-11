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


            // ── Apply bonuses to stats ────────────────────────────────────
            int totalStr = charData.Str + bonusStr;
            int totalAgi = charData.Agi + bonusAgi;
            int totalVit = charData.Vit + bonusVit;
            int totalInt = charData.Int + bonusInt;
            int totalDex = charData.Dex + bonusDex;
            int totalLuk = charData.Luk + bonusLuk;


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

            res.Atk = $"{batk} + 0";

            // --- WEIGHT LIMIT ---
            // Formula: Base Job Weight + (Base STR * 30)
            // Note: Usually uses charData.Str (Base) rather than totalStr
            res.MaxWeight = 2000 + job.Weight + (charData.Str * 30);

            // ── ASPD ─────────────────────────────────────────────────────
            res.Aspd = UpdateAspd(charData, job, res, false);

            // --- AGI: FLEE ---
            // Formula:  BaseLevel + AGI + 10 (base bonus)
            // LUK does NOT contribute to Flee — only to Perfect Dodge & Crit resist
            res.Flee = $"{charData.BaseLevel + totalAgi} + {(totalLuk + 10) * 10 / 100}";

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

            res.HpRegen = $"{hpRegenValue} per 6s standing (per 3s sitting)";

            // --- SP CALCULATIONS ---
            // Formula:  BaseSP × (1 + INT × 0.01)
            int baseSp = CalculateBaseSP(charData.BaseLevel, totalInt, job);
            res.MaxSp = baseSp.ToString();

            // --- SP Natural Regen ---
            // Formula:  floor(MaxSP / 100) + floor(INT / 6) + 1   every 8 seconds
            // Bonus: every 100 SP grants +1 regen (already embedded in MaxSP / 100)
            int spRegenValue = (baseSp / 100) + (totalInt / 6) + 1;
            res.SpRegen = $"{spRegenValue} per 8s standing (per 4s sitting)";

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
            res.BonusStr = bonusStr;
            res.BonusAgi = bonusAgi;
            res.BonusVit = bonusVit;
            res.BonusInt = bonusInt;
            res.BonusDex = bonusDex;
            res.BonusLuk = bonusLuk;

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
            //// Calculate BASE_HP
            //// Starting with 35 + (Level * JobB)
            //double baseHpSum = 35.0 + (baseLevel * job.HpJobB);

            //// Sum growth from level 2 to baseLevel
            //for (int i = 2; i <= baseLevel; i++)
            //{
            //    // Use standard Rounding for the growth increment as per pseudo-code
            //    baseHpSum += Math.Round(job.HpJobA * i, MidpointRounding.AwayFromZero);
            //}

            //// Apply VIT Modifier
            //// Formula: floor( BASE_HP * (1 + VIT * 0.01) * TRANS_MOD )
            //double transMod = isTrans ? 1.25 : 1.0;

            //int currentMaxHp = (int)Math.Floor(baseHpSum * (1.0 + vit * 0.01) * transMod);

            //return currentMaxHp;
            // Calculate the growth part only
            double growthSum = 0;
            for (int i = 2; i <= baseLevel; i++)
            {
                // Use standard Rounding for the growth increment
                growthSum += Math.Round(job.HpJobA * i, MidpointRounding.AwayFromZero);
            }

            // Define the "Base HP" as a whole number
            // 35 + (Level * JobB) + sum of rounded levels
            int baseHp = (int)(35 + (baseLevel * job.HpJobB) + growthSum);

            // Apply Multipliers to the integer baseHp
            double vitMultiplier = 1.0 + (totalVit * 0.01);
            double transMod = isTrans ? 1.25 : 1.0;

            // Final Calculation: Floor then add the +2 offset for RMS alignment
            int currentMaxHp = (int)Math.Floor(baseHp * vitMultiplier * transMod) + 2;

            return currentMaxHp;
        }

        private static double CalculateHPRegen(int totalMaxHp, int totalVit, double hprMod = 0)
        {
            // Base: 1 per 200 HP, minimum 1
            // (1768 / 200) = 8.84 -> truncated to 8 in integer division
            int baseHpr = Math.Max(1, totalMaxHp / 200);

            // Add VIT bonus: +1 for every 5 VIT
            // (50 / 5) = 10
            int vitBonus = totalVit / 5;

            // Combine them BEFORE applying modifiers
            int totalBase = baseHpr + vitBonus; // 8 + 10 = 18

            //Apply Modifiers and FLOOR the result
            double finalHpr = (baseHpr + vitBonus + 1) * (1.0 + (hprMod * 0.01));

            return (int)Math.Floor(finalHpr);

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

        public static string UpdateAspd(CharacterData charData, JobData job, CalculationResult res, bool showDebug = false)
        {
            int totalAgi = charData.Agi + job.GetStatBonus(job.AgiBonusTable, charData.JobLevel);
            int totalDex = charData.Dex + job.GetStatBonus(job.DexBonusTable, charData.JobLevel);

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
