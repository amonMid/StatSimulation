using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatSimulation.Backend
{
    public class CharacterService
    {
        public CharacterData CurrentCharacter { get; private set; } = new CharacterData();

        public CalculationResult UpdateStat(string statName, int value)
        {
            // ── GUARD: Null or empty stat name ──────────────────────────
            if (string.IsNullOrEmpty(statName))
            {
                // Return current state without changes
                return Calculator.CalculateAll(CurrentCharacter);
            }

            // Get current value to see if user is decreasing it
            int oldValue = GetCurrentValue(statName);

            // Create a "Test Clone" to check if points go negative
            var tempChar = new CharacterData
            {
                Job = CurrentCharacter.Job,
                BaseLevel = CurrentCharacter.BaseLevel,
                JobLevel = CurrentCharacter.JobLevel,
                Str = CurrentCharacter.Str,
                Agi = CurrentCharacter.Agi,
                Vit = CurrentCharacter.Vit,
                Int = CurrentCharacter.Int,
                Dex = CurrentCharacter.Dex,
                Luk = CurrentCharacter.Luk
            };

            // Apply change to clone
            ApplyValue(tempChar, statName, value);

            // Test the points
            var testResult = Calculator.CalculateAll(tempChar);

            // Check if the change resulted in negative points
            if (testResult.StatusPoints < 0)
            {
                // If it was a Level Change that caused the negative points, 
                // we force a reset of all stats to 1.
                if (statName.ToUpper() == "BASELV" || statName.ToUpper() == "JOBLV")
                {
                    ResetAttributes(CurrentCharacter); // Reset STR, AGI, etc. to 1
                    ApplyValue(CurrentCharacter, statName, value); // Apply the new level
                    return Calculator.CalculateAll(CurrentCharacter);
                }

                // If it was a regular Stat increase that caused negative points, just reject it
                return Calculator.CalculateAll(CurrentCharacter);
            }

            // If points are fine, apply the change normally
            ApplyValue(CurrentCharacter, statName, value);

            return Calculator.CalculateAll(CurrentCharacter);
        }

        // Helper to reset all attributes to 1
        private void ResetAttributes(CharacterData data)
        {
            data.Str = 1;
            data.Agi = 1;
            data.Vit = 1;
            data.Int = 1;
            data.Dex = 1;
            data.Luk = 1;
        }

        private int GetCurrentValue(string stat)
        {
            // ── GUARD: Null check ────────────────────────────────────
            if (string.IsNullOrEmpty(stat))
                return 1;

            return stat.ToUpper() switch
            {
                "STR" => CurrentCharacter.Str,
                "AGI" => CurrentCharacter.Agi,
                "VIT" => CurrentCharacter.Vit,
                "INT" => CurrentCharacter.Int,
                "DEX" => CurrentCharacter.Dex,
                "LUK" => CurrentCharacter.Luk,
                "BASELV" => CurrentCharacter.BaseLevel,
                "JOBLV" => CurrentCharacter.JobLevel,
                _ => 1
            };
        }


        //Apply the value to the Character Data
        private void ApplyValue(CharacterData data, string stat, int val)
        {
            // ── GUARD: Null check ────────────────────────────────────
            if (string.IsNullOrEmpty(stat))
                return;

            if (val < 1) val = 1; // Minimum stat is 1

            switch (stat.ToUpper())
            {
                case "STR": data.Str = val; break;
                case "AGI": data.Agi = val; break;
                case "VIT": data.Vit = val; break;
                case "INT": data.Int = val; break;
                case "DEX": data.Dex = val; break;
                case "LUK": data.Luk = val; break;
                case "BASELV": data.BaseLevel = val; break;
                case "JOBLV": data.JobLevel = val; break;
            }
        }

        public CalculationResult UpdateJob(string newJob)
        {
            // ── GUARD: Null or empty job name ───────────────────────
            if (string.IsNullOrEmpty(newJob))
                newJob = "Novice";

            if (CurrentCharacter.Job == newJob)
                return Calculator.CalculateAll(CurrentCharacter);

            // Update the job class string in your character data
            CurrentCharacter.Job = newJob;

            // Reset Job LV to 1 on class change
            CurrentCharacter.JobLevel = 1;

            // Reset Skills on class change
            CurrentCharacter.SkillLevels.Clear();

            // Re-run all calculations because HP/SP multipliers depend on Job
            return Calculator.CalculateAll(CurrentCharacter);
        }

        public CalculationResult UpdateSkill(string skillId, int level)
        {
            if (string.IsNullOrEmpty(skillId))
                return Calculator.CalculateAll(CurrentCharacter);

            if (level <= 0)
            {
                if (CurrentCharacter.SkillLevels.ContainsKey(skillId))
                    CurrentCharacter.SkillLevels.Remove(skillId);
            }
            else
            {
                CurrentCharacter.SkillLevels[skillId] = level;
            }

            return Calculator.CalculateAll(CurrentCharacter);
        }

        public void Reset() => CurrentCharacter = new CharacterData();
    }
}