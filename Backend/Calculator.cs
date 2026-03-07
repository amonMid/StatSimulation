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
            int strAtk = totalStr + (int)Math.Pow(totalStr / 10, 2);
            int dexMeleeBonus = totalDex / 5;
            int lukAtkBonus = totalLuk / 5;
            res.Atk = $"{strAtk + dexMeleeBonus + lukAtkBonus}";

            // --- WEIGHT LIMIT ---
            // Formula: Base Job Weight + (Base STR * 30)
            // Note: Usually uses charData.Str (Base) rather than totalStr
            res.MaxWeight = charData.Weight + (charData.Str * 30);

            // ── ASPD ─────────────────────────────────────────────────────
            double reduction = totalAgi * 0.004 + totalDex * 0.001;
            double delay = job.WeaponDelay * (1.0 - reduction);
            double finalAspd = 200.0 - Math.Floor(delay);
            finalAspd = Math.Min(finalAspd, MAX_ASPD);
            res.Aspd = Math.Floor(finalAspd).ToString();

            // --- AGI: FLEE ---
            // Formula:  BaseLevel + AGI + 10 (base bonus)
            // LUK does NOT contribute to Flee — only to Perfect Dodge & Crit resist
            res.Flee = $"{charData.BaseLevel + totalAgi + BASE_FLEE}";

            // --- DEF --- 
            // VIT-based soft DEF ≈ floor(VIT × 0.8)
            // Shown as "EquipDef + VitSoftDef" (equip portion is 0 here)
            int vitSoftDef = (int)Math.Floor(totalVit * 0.8);
            if (vitSoftDef <= 0) vitSoftDef = 1;
            res.Def = $"0 + {vitSoftDef}";

            // --- MDEF ---
            // INT-based soft MDEF = 1 per INT
            // VIT also grants: floor(VIT / 2) soft MDEF
            int vitMdefBonus = totalVit / 2;
            res.Mdef = $"0 + {totalInt + vitMdefBonus}";

            // --- HP CALCULATIONS ---
            // Uses the accurate job-growth loop; VIT scales the result
            int totalMaxHp = CalculateMaxHP(charData.BaseLevel, totalVit, job);
            res.MaxHp = totalMaxHp.ToString();

            //int finalMaxHp = CalculateMaxHP(charData.BaseLevel, charData.Str);
            //res.MaxHp = finalMaxHp.ToString();

            // HpRegen
            // Formula:  floor(MaxHP / 200) + floor(VIT / 5)   every 6 seconds
            int hpRegenValue = (totalMaxHp / 200) + (totalVit / 5);
            res.HpRegen = $"{hpRegenValue} per 6s";

            // --- SP CALCULATIONS ---
            // Formula:  BaseSP × (1 + INT × 0.01)
            int baseSp = CalculateBaseSP(charData.BaseLevel, job);
            int totalMaxSp = (int)(baseSp * (1 + totalInt * 0.01));
            res.MaxSp = totalMaxSp.ToString();

            // --- SP Natural Regen ---
            // Formula:  floor(MaxSP / 100) + floor(INT / 6) + 1   every 8 seconds
            // Bonus: every 100 SP grants +1 regen (already embedded in MaxSP / 100)
            int spRegenValue = (totalMaxSp / 100) + (totalInt / 6) + 1;
            res.SpRegen = $"{spRegenValue} per 8s";

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
            res.Hit = $"{BASE_HIT + charData.BaseLevel + totalDex}";

            // --- CAST TIME ---
            // 150 DEX = instant cast; each DEX = −(1/150) of cast time
            double castReduction = (totalDex / 150.0) * 100.0;
            res.CastTime = totalDex >= 150 ? "Instant" : $"{Math.Floor(castReduction)}%";

            // --- LUK ---
            // Crit = [LUK * 0.3] + 1
            res.Crit = $"{(int)(totalLuk * 0.3) + 1}";

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
            //// These constants would ideally come from a Job-specific config file
            //// Using standard "Novice" constants as an example:
            //double hpJobA = 5.0;  // The growth factor per level
            //double hpJobB = 5.0;  // The base multiplier

            //// Initial calculation: 35 + (BaseLevel * JobB)
            //double runningHp = 35 + (baseLevel * hpJobB);

            //for (int i = 2; i <= baseLevel; i++)
            //{
            //    runningHp += Math.Round(hpJobA * i);
            //}

            //// Total HP = [Base HP * (1 + VIT * 0.01)]
            //int totalMaxHp = (int)(runningHp * (1 + (vit * 0.01)));

            //return totalMaxHp;
            double baseHp = 35.0 + (baseLevel * job.HpJobB);
            for (int i = 2; i <= baseLevel; i++)
                baseHp += Math.Round(job.HpJobA * i);


            return (int)(baseHp * (1.0 + vit * 0.01));
        }

        // ── SP CALCULATION ────────────────────────────────────────────────────
        // Scaled by INT in CalculateAll:  TotalSP = BaseSP × (1 + INT × 0.01)
        private static int CalculateBaseSP(int baseLevel, JobData job)
        {
            //return 10 + (baseLevel * 10);
            double baseSp = 10.0 + (baseLevel * job.SpJobB);
            for (int i = 2; i <= baseLevel; i++)
                baseSp += Math.Round(job.SpJobA * i);
            return (int)baseSp;
        }
    }
}
