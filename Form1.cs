using Newtonsoft.Json;
using StatSimulation.Backend;
using System.Diagnostics;
using System.Web;

namespace StatSimulation
{
    public partial class Form1 : Form
    {
        private readonly CharacterService _service = new CharacterService();
        private CharacterData charData => _service.CurrentCharacter;

        public Form1()
        {
            InitializeComponent();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDir, "Frontend", "index.html");
            wb1.Source = new Uri(fullPath);
        }


        private async void wb1_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<StatUpdateMessage>(e.WebMessageAsJson);
                if (message == null) return;

                CalculationResult results = null;

                //   Handle different message types 
                switch (message.Type?.ToUpper())
                {
                    case "CLASS_CHANGE":
                        // Use ClassName field for class changes
                        string jobName = message.ClassName ?? "Novice";
                        results = _service.UpdateJob(jobName);
                        break;

                    case "JOB_LEVEL_CHANGE":
                        // Use Value field for job level
                        int jobLevel = message.Value > 0 ? message.Value : message.NewValue;
                        results = _service.UpdateStat("JOBLV", jobLevel);
                        break;

                    case "WEAPON_CHANGE":
                        // TODO: Handle weapon changes when implemented
                        //string weapon = message.Weapon ?? "bare_hands";
                        string weaponStr = message.Weapon ?? "Hand";
                        charData.EquippedWeapon = ParseWeaponType(weaponStr);

                        // For now, just recalculate without changing anything
                        results = Calculator.CalculateAll(_service.CurrentCharacter);
                        break;

                    case "STAT_CHANGE":
                    default:
                        // „Ÿ„Ÿ CRITICAL: Check if Stat is provided „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ
                        if (string.IsNullOrEmpty(message.Stat))
                        {
                            // If no stat specified, just return current state
                            results = Calculator.CalculateAll(_service.CurrentCharacter);
                            break;
                        }

                        // Use NewValue, fallback to Value if NewValue is 0
                        int statValue = message.NewValue > 0 ? message.NewValue : message.Value;
                        results = _service.UpdateStat(message.Stat, statValue);
                        break;
                }

                // „Ÿ„Ÿ Send results back to UI „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ
                if (results != null)
                {
                    string json = JsonConvert.SerializeObject(results);
                    string safeJson = System.Web.HttpUtility.JavaScriptStringEncode(json);

                    await wb1.CoreWebView2.ExecuteScriptAsync($"CharacterUI.render('{safeJson}')");
                    await wb1.CoreWebView2.ExecuteScriptAsync($"CharacterUI.syncInputs('{safeJson}')");
                }
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JSON Error]: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Bridge Error]: {ex.Message}\n{ex.StackTrace}");
            }
        }

        // Helper method to parse weapon strings from JavaScript
        private WeaponType ParseWeaponType(string weaponStr)
        {
            // Normalize: lowercase, replace spaces/hyphens with underscores
            string normalized = weaponStr
                .ToLower()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("&", "and");  // "Rod & Staff" ¨ "rod_and_staff"

            return normalized switch
            {
                "hand" or "hands" => WeaponType.Hand,
                "dagger" => WeaponType.Dagger,
                "one_handed_sword" => WeaponType.OnehandedSword,
                "two_handed_sword" => WeaponType.TwohandedSword,
                "one_handed_axe" => WeaponType.OnehandedAxe,
                "two_handed_axe" => WeaponType.TwohandedAxe,
                "one_handed_mace" => WeaponType.OnehandedMace,
                "two_handed_mace" => WeaponType.TwohandedMace,
                "rod_and_staff" => WeaponType.RodStaff,
                "two_handed_staff" => WeaponType.TwohandedStaff,
                "one_handed_spear" => WeaponType.OnehandedSpear,
                "two_handed_spear" => WeaponType.TwohandedSpear,
                "bow" => WeaponType.Bow,
                _ => WeaponType.Hand
            };
        }
    }
}
