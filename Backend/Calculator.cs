using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatSimulation.Backend
{
    public static class Calculator
    {
        public static CalculationResult CalculateAll(CharacterData charData)
        {
            var res = new CalculationResult();

            // --- STR & LUK & DEX MELEE ATK ---
            // Formula: STR + (STR/10)^2 + (DEX/5) + (LUK/5)
            int strBonusAtk = charData.Str + (int)Math.Pow(charData.Str / 10, 2);
            int dexMeleeBonus = charData.Dex / 5;
            int lukMeleeBonus = charData.Luk / 5;
            res.Atk = $"{strBonusAtk + dexMeleeBonus + lukMeleeBonus}";

            // --- AGI & DEX (ASPD) ---
            // Every 1 AGI = -0.4% delay, Every 1 DEX = -0.1% delay
            // (Placeholder logic as ASPD depends on Weapon/Job)
            double baseAspd = 150.0;

            // Formula: BaseASPD + [(200 - BaseASPD) * (AGI + (DEX/4)) / 250]
            double statBonus = charData.Agi + (charData.Dex / 4.0);
            double finalAspd = baseAspd + (200.0 - baseAspd) * statBonus / 250.0;

            // Cap it at 190 (Standard game limit)
            if (finalAspd > 190) finalAspd = 190;

            res.Aspd = Math.Floor(finalAspd).ToString();

            // --- AGI: FLEE ---
            // Formula: Base Level + AGI + LUK/5 (Simplified)
            res.Flee = $"{charData.BaseLevel + charData.Agi + (charData.Luk / 5)}";

            // --- VIT ---
            res.Def = $"0 + {charData.Vit}";
            res.HpRegen = $"{(1000 / 200) + (charData.Vit / 5)} per 6s";

            // --- HP CALCULATIONS ---
            // Formula: Total HP = Base HP + [Base HP * (VIT * 0.01)]
            int baseHp = charData.BaseLevel * 50; // Example Base HP
            int bonusHp = (int)(baseHp * (charData.Vit * 0.01));
            int totalMaxHp = baseHp + bonusHp;
            res.MaxHp = totalMaxHp.ToString();

            //int finalMaxHp = CalculateMaxHP(charData.BaseLevel, charData.Str);
            //res.MaxHp = finalMaxHp.ToString();

            // Formula: HP Natural Regen = [Max HP / 200] + [VIT / 5]
            // Note: Only triggers every 5 VIT for the second part
            int hpRegenValue = (totalMaxHp / 200) + (charData.Vit / 5);
            res.HpRegen = $"{hpRegenValue} per 6s";

            // --- SP CALCULATIONS ---
            // Formula: Total SP = Base SP + [Base SP * (INT * 0.01)]
            int baseSp = charData.BaseLevel * 10; // Example Base SP
            int bonusSp = (int)(baseSp * (charData.Int * 0.01));
            int totalMaxSp = baseSp + bonusSp;
            res.MaxSp = totalMaxSp.ToString();

            // Formula: SP Natural Regen = [Max SP / 100] + [INT / 6] + 1
            int spRegenValue = (totalMaxSp / 100) + (charData.Int / 6) + 1;
            res.SpRegen = $"{spRegenValue} per 8s";

            // --- RECOVERY ITEM EFFECTIVENESS ---
            // HP Items: +2% per VIT | SP Items: +1% per INT
            double hpItemBonus = charData.Vit * 2;
            double spItemBonus = charData.Int * 1;
            res.HpRecoveryItem = $"+{hpItemBonus}%";
            res.SpRecoveryItem = $"+{spItemBonus}%";

            // --- INT ---
            int minMatk = charData.Int + (int)Math.Pow(charData.Int / 7, 2);
            int maxMatk = charData.Int + (int)Math.Pow(charData.Int / 5, 2);
            res.Matk = $"{minMatk} ~ {maxMatk}";
            res.Mdef = $"0 + {charData.Int}";

            // --- DEX ---
            // Hit = Base Level + DEX
            res.Hit = $"{charData.BaseLevel + charData.Dex}";

            // Cast Time = (DEX / 150) as a percentage of reduction
            double castReduction = (charData.Dex / 150.0) * 100;
            res.CastTime = charData.Dex >= 150 ? "Instant" : $"{Math.Floor(castReduction)}%";

            // --- LUK ---
            // Crit = [LUK * 0.3] + 1
            res.Crit = $"{(int)(charData.Luk * 0.3) + 1}";

            // Perfect Dodge = [LUK * 0.1] + 1 (Base perfect dodge is usually 1)
            res.PerfectDodge = $"{(charData.Luk * 0.1) + 1:F1}";

            // --- POINTS LOGIC ---
            int totalPointsGained = GetTotalPointsForLevel(charData.BaseLevel);
            int totalPointsSpent = GetStatInvestmentCost(charData.Str) +
                                   GetStatInvestmentCost(charData.Agi) +
                                   GetStatInvestmentCost(charData.Vit) +
                                   GetStatInvestmentCost(charData.Int) +
                                   GetStatInvestmentCost(charData.Dex) +
                                   GetStatInvestmentCost(charData.Luk);

            res.StatusPoints = totalPointsGained - totalPointsSpent;

            // --- NEXT COSTS ---
            res.NextStrCost = ((charData.Str - 1) / 10) + 2;
            res.NextAgiCost = ((charData.Agi - 1) / 10) + 2;
            res.NextVitCost = ((charData.Vit - 1) / 10) + 2;
            res.NextIntCost = ((charData.Int - 1) / 10) + 2;
            res.NextDexCost = ((charData.Dex - 1) / 10) + 2;
            res.NextLukCost = ((charData.Luk - 1) / 10) + 2;

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

        private static int CalculateMaxHP(int baseLevel, int vit)
        {


            // These constants would ideally come from a Job-specific config file
            // Using standard "Novice" constants as an example:
            double hpJobA = 5.0;  // The growth factor per level
            double hpJobB = 5.0;  // The base multiplier

            // Initial calculation: 35 + (BaseLevel * JobB)
            double runningHp = 35 + (baseLevel * hpJobB);

            // Cumulative growth loop: adding JobA * level for every level from 2 upwards
            for (int i = 2; i <= baseLevel; i++)
            {
                runningHp += Math.Round(hpJobA * i);
            }

            // Total HP = [Base HP * (1 + VIT * 0.01)]
            int totalMaxHp = (int)(runningHp * (1 + (vit * 0.01)));

            return totalMaxHp;
        }
    }
}
