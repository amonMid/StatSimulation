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

            // Always allow Job Level or Base Level changes (they usually don't cost points)
            //if (testResult.StatusPoints >= 0 || value < oldValue ||
            //    statName.ToUpper() == "BASELV" || statName.ToUpper() == "JOBLV")
            //{
            //    ApplyValue(CurrentCharacter, statName, value);
            //    return testResult;
            //}

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

            // If invalid, return the calculation of the LAST GOOD state
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
        private int GetCurrentValue(string stat) => stat.ToUpper() switch
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

        private void ApplyValue(CharacterData data, string stat, int val)
        {
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
            // Update the job class string in your character data
            CurrentCharacter.Job = newJob;

            // Most RO-based systems reset Job LV or stats on change
            // If you don't want to reset, just skip this line:
            CurrentCharacter.JobLevel = 1;

            // Re-run all calculations because HP/SP multipliers depend on Job
            return Calculator.CalculateAll(CurrentCharacter);
        }
        public void Reset() => CurrentCharacter = new CharacterData();
    }
}
