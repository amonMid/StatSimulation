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

                        // Update the job
                        _service.UpdateJob(jobName);

                        // Reset stats in backend
                        _service.CurrentCharacter.Str = 1;
                        _service.CurrentCharacter.Agi = 1;
                        _service.CurrentCharacter.Vit = 1;
                        _service.CurrentCharacter.Int = 1;
                        _service.CurrentCharacter.Dex = 1;
                        _service.CurrentCharacter.Luk = 1;
                        _service.CurrentCharacter.JobLevel = 1;
                        _service.CurrentCharacter.BaseLevel = 1;


                        // IMPORTANT: Generate the result object here
                        results = Calculator.CalculateAll(_service.CurrentCharacter);

                        // Update the physical Inputs (Base Level, Job Level dropdowns)
                        UpdateUIStats();
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

        // Update the UI inputs based on the current character data
        private void UpdateUIStats()
        {
            var job = JobRegistry.Get(_service.CurrentCharacter.Job);

            string jsCode = $@"
            (function() {{

                isInternalUpdate = true;

                try {{
                // 1. Update Primary Stats
                const statMapping = {{
                    'str-input': {_service.CurrentCharacter.Str},
                    'agi-input': {_service.CurrentCharacter.Agi},
                    'vit-input': {_service.CurrentCharacter.Vit},
                    'int-input': {_service.CurrentCharacter.Int},
                    'dex-input': {_service.CurrentCharacter.Dex},
                    'luk-input': {_service.CurrentCharacter.Luk}
                }};

                for (const [id, val] of Object.entries(statMapping)) {{
                    const el = document.getElementById(id);
                    if (el) el.value = val;
                }}

                // „Ÿ„Ÿ„Ÿ NEW: Reset Bonus Labels „Ÿ„Ÿ„Ÿ
                // This assumes your labels have IDs like 'str-bonus', etc.
                const stats = ['str', 'agi', 'vit', 'int', 'dex', 'luk'];
                stats.forEach(s => {{
                    const bonusEl = document.getElementById(s + '-bonus');
                    if (bonusEl) bonusEl.innerText = '+0';
                }});

                // 2. Update Levels
                const baseLvInput = document.querySelector('[data-stat-input=""BASELV""]');
                if (baseLvInput) baseLvInput.value = {_service.CurrentCharacter.BaseLevel};

                // 3. Update Job Level
                if (typeof populateJobLevels === 'function') {{
                    populateJobLevels({job.MaxJobLevel});
                    const jobLvEl = document.querySelector('[data-stat-input=""JOBLV""]');
                    if (jobLvEl) jobLvEl.value = {_service.CurrentCharacter.JobLevel};
                }}

                // 4. Trigger UI Refresh
                if (baseLvInput) {{
                    baseLvInput.dispatchEvent(new Event('change'));
                }}

                // Update Job Level
                    if (typeof populateJobLevels === 'function') {{
                        populateJobLevels({job.MaxJobLevel});
                        const jobLvEl = document.querySelector('[data-stat-input=""JOBLV""]');
                        if (jobLvEl) jobLvEl.value = {_service.CurrentCharacter.JobLevel};
                    }}

                    // Update Weapon (If needed, reset to Hand on class change)
                    const weaponEl = document.getElementById('weaponSelect');
                    if (weaponEl) weaponEl.value = 'hand';

                }} finally {{
                    // „Ÿ„Ÿ UNLOCK: Allow user interaction again
                    // Delay slightly to ensure events finish bubbling
                    setTimeout(() => {{ isInternalUpdate = false; }}, 50);
                }}
            }})();";

            wb1.ExecuteScriptAsync(jsCode);
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
