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
                BaseLevel = CurrentCharacter.BaseLevel,
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

            if (testResult.StatusPoints >= 0 || value < oldValue || statName.ToUpper() == "BASELV")
            {
                ApplyValue(CurrentCharacter, statName, value);
                return testResult;
            }

            // Update the model
            //switch (statName.ToUpper())
            //{
            //    case "STR": CurrentCharacter.Str = value; break;
            //    case "AGI": CurrentCharacter.Agi = value; break;
            //    case "VIT": CurrentCharacter.Vit = value; break;
            //    case "INT": CurrentCharacter.Int = value; break;
            //    case "DEX": CurrentCharacter.Dex = value; break;
            //    case "LUK": CurrentCharacter.Luk = value; break;
            //    case "BASELV": CurrentCharacter.BaseLevel = value; break;
            //}

            //// Return the full recalculated state
            //return Calculator.CalculateAll(CurrentCharacter);

            // If invalid, return the calculation of the LAST GOOD state
            return Calculator.CalculateAll(CurrentCharacter);
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
            }
        }
        public void Reset() => CurrentCharacter = new CharacterData();
    }
}
