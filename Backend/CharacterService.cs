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
            // Update the model
            switch (statName.ToUpper())
            {
                case "STR": CurrentCharacter.Str = value; break;
                case "AGI": CurrentCharacter.Agi = value; break;
                case "VIT": CurrentCharacter.Vit = value; break;
                case "INT": CurrentCharacter.Int = value; break;
                case "DEX": CurrentCharacter.Dex = value; break;
                case "LUK": CurrentCharacter.Luk = value; break;
                case "BASELV": CurrentCharacter.BaseLevel = value; break;
            }

            // Return the full recalculated state
            return Calculator.CalculateAll(CurrentCharacter);
        }
        public void Reset() => CurrentCharacter = new CharacterData();
    }
}
