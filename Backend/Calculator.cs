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
            //double reduction = totalAgi * 0.004 + totalDex * 0.001;
            //double delay = weapondelay * (1.0 - reduction);
            //double finalAspd = 200.0 - Math.Floor(delay);
            //finalAspd = Math.Min(finalAspd, MAX_ASPD);
            //res.Aspd = Math.Floor(finalAspd).ToString();

            // --- ASPD CALCULATION (RMS Style) ---
            // Get BTBA (e.g., 1.0 for Novice Hand)
            // double btba = job.GetBTBA(charData.EquippedWeapon);

            // // Calculate WD (Weapon Delay) 
            // double wd = 500.0 * btba;

            // // Stat Reductions [WD * AGI / 25] and [WD * DEX / 100]
            // double effectiveAgi = Math.Max(0, totalAgi - 1);
            // double effectiveDex = Math.Max(0, totalDex - 1);

            // double agiRed = Math.Floor((wd * effectiveAgi) / 25.0);
            // double dexRed = Math.Floor((wd * effectiveDex) / 100.0);

            // // RMS floors the result of (WD - StatReductions) / 10.0
            // double totalReductions = agiRed + dexRed;
            // double finalDelay = Math.Floor((wd - totalReductions) / 10.0);

            // // Formula: 200 - FinalDelay * (1 - SM)
            // double sm = 0.0; // Potion/Skill bonus
            // double aspdResult = 200.0 - (finalDelay * (1.0 - sm));

            // // Cap and Output
            // res.Aspd = Math.Floor(Math.Min(aspdResult, MAX_ASPD)).ToString();

            // ── ASPD ─────────────────────────────────────────────────
            // double btba = job.GetBTBA(charData.EquippedWeapon);
            // double wd = 50.0 * btba;

            // double agiReduction = Math.Floor((wd * totalAgi) / 25.0);
            // double dexReduction = Math.Floor((wd * totalDex) / 100.0);
            // double totalReduction = agiReduction + dexReduction;

            // double delayAfterStats = (wd - totalReduction) / 10.0;
            // double aspd = 200.0 - delayAfterStats;

            // aspd = Math.Min(aspd, MAX_ASPD);

            // res.Aspd = Math.Floor(aspd).ToString();

            // double baseAspd = job.GetWeaponDelay(charData.EquippedWeapon);

            // // Add stat bonuses
            // // AGI bonus: approximately +0.4 to +0.5 ASPD per AGI point
            // // DEX bonus: approximately +0.1 to +0.2 ASPD per DEX point
            // double agiBonus = (totalAgi - 1) * 0.5;  // -1 because base stat is 1
            // double dexBonus = (totalDex - 1) * 0.15;

            // double finalAspd = baseAspd + agiBonus + dexBonus;
            // finalAspd = Math.Min(finalAspd, MAX_ASPD);

            // res.Aspd = Math.Floor(finalAspd).ToString();

            //         double aMotion = job.WeaponDelays.TryGetValue(charData.EquippedWeapon, out double delay)
            // ? delay
            // : 150.0;



            //         aMotion -= aMotion * ((4.0 * totalAgi) + totalDex) / 1000.0;

            //         double internalAspd = (2000.0 - aMotion) / 10.0;

            //         // CONVERSION TO RATEMYSERVER DISPLAY FORMAT
            //         // Based on observation: their display is roughly (internal - 44) for low stats
            //         // This varies by weapon/job but this is an approximation
            //         double displayAspd = internalAspd - 44.0;

            //         displayAspd = Math.Min(displayAspd, MAX_ASPD - 44.0);

            //         res.Aspd = Math.Floor(displayAspd).ToString();

            //// 1. Define Magician Base Stats
            //double baseAspd = 151.0; // Magician Unarmed Base
            //int penalty = 0;         // "Hand" has 0 penalty

            //// 2. Calculate the "Weapon Delay" (aMotion) from the base
            //// Formula: aMotion = 2000 - (BaseASPD - Penalty) * 10
            //double aMotion = 2000.0 - (baseAspd - penalty) * 10.0;

            //// 3. Apply Stat Reduction (The 4:1 AGI/DEX Ratio)
            //// aMotion -= aMotion * (4 * AGI + DEX) / 1000
            //aMotion -= aMotion * ((4.0 * totalAgi) + totalDex) / 1000.0;

            //// 4. Convert back to ASPD
            //// Formula: (2000 - aMotion) / 10
            //double finalAspd = (2000.0 - aMotion) / 10.0;

            //// 5. Result
            //// finalAspd will be 157.14 -> Floored to 157
            //res.Aspd = Math.Floor(finalAspd).ToString();

            res.Aspd = UpdateAspd(charData, job, res);

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
            int hpRegenValue = CalculateHPRegen(totalMaxHp, totalVit);

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

        private static int CalculateMaxHP(int baseLevel, int vit, JobData job)
        {

            // Calculate the BASE_HP (The sum of growth across levels)
            // Formula: 35 + (BaseLevel * JobB) + Sigma(JobA * i)
            double baseHpSum = 35.0 + (baseLevel * job.HpJobB);
            for (int i = 2; i <= baseLevel; i++)
            {
                baseHpSum += (int)Math.Round(job.HpJobA * i, MidpointRounding.AwayFromZero);
            }

            // Apply VIT and Trans Mod (MAX_HP = floor( BASE_HP * (1 + VIT * 0.01) * TRANS_MOD ))
            //double transMod = isTrans ? 1.25 : 1.0;
            double vitMod = 1.0 + (vit * 0.01);

            int currentMaxHp = (int)Math.Floor(baseHpSum * vitMod);

            // Multiplicative Modifiers (MAX_HP = floor( MAX_HP * (1 + HP_MOD_B * 0.01) ))
            // We floor again after applying multiplicative mods like Matyr Card
            //if (hpModB != 0)
            //{
            //    currentMaxHp = (int)Math.Floor(currentMaxHp * (1.0 + hpModB * 0.01));
            //}

            return currentMaxHp;
        }

        private static int CalculateHPRegen(int totalMaxHp, int totalVit)
        {
            // Every character starts with a base of 1 HPR
            int baseValue = 1;

            // Calculate the part based on Max HP
            // JS: Math.floor(MAX_HP / 200)
            int hpPart = totalMaxHp / 200;

            // Calculate the part based on VIT
            // JS: Math.floor(VIT / 5)
            int vitPart = totalVit / 5;

            // Sum them and ensure a minimum of 1
            // JS: Math.max( 1, floor(HP/200) + floor(VIT/5) )
            int baseHpr = Math.Max(1, hpPart + vitPart);

            // Note: If add HPR_MOD (like Increase HP Recovery skill) later:
            // baseHpr = (int)Math.Floor(baseHpr * (1 + hprMod * 0.01));

            return baseHpr + baseValue;
        }

        // ── SP CALCULATION ────────────────────────────────────────────────────
        // Scaled by INT in CalculateAll:  TotalSP = BaseSP × (1 + INT × 0.01)
        private static int CalculateBaseSP(int baseLevel, int intel, JobData job)
        {
            // Calculate BASE_SP
            // Using your pseudo-code: 10 + (Level * Job_Multiplier)
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
                // Add more ranged types if you add them later:
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

        public static string UpdateAspd(CharacterData charData, JobData job, CalculationResult res)
        {
            // 1. Get total stats (Base + Job Bonus)
            int totalAgi = charData.Agi + job.GetStatBonus(job.AgiBonusTable, charData.JobLevel);
            int totalDex = charData.Dex + job.GetStatBonus(job.DexBonusTable, charData.JobLevel);

            // 2. Get the specific delay for the weapon (e.g., 500 for Hand)
            double aMotion = job.GetWeaponDelay(charData.EquippedWeapon);

            // 3. Apply the 4:1 AGI/DEX Reduction
            // This is the "0.4% per AGI" and "0.1% per DEX" logic
            aMotion -= aMotion * ((4.0 * totalAgi) + totalDex) / 1000.0;

            // 4. Convert to final ASPD
            // Standard Formula: (2000 - aMotion) / 10
            double finalAspd = (2000.0 - aMotion) / 10.0;

            // 5. Apply Pre-Renewal Cap (190)
            finalAspd = Math.Min(finalAspd, 190.0);

            // 6. Floor and Save
            return Math.Floor(finalAspd).ToString();
        }

    }
}
