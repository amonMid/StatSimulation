using Newtonsoft.Json;
using StatSimulation.Backend;
using System.Diagnostics;
using System.Web;

namespace StatSimulation
{
    public partial class Main : Form
    {
        private readonly CharacterService _service = new CharacterService();
        private CharacterData charData => _service.CurrentCharacter;

        public Main()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await wb1.EnsureCoreWebView2Async(null);

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string frontendPath = Path.Combine(baseDir, "Frontend");

            wb1.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "app.local",
                frontendPath,
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow
            );

            wb1.CoreWebView2.Navigate("https://app.local/index.html");
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
                        if (string.IsNullOrEmpty(message.Stat))
                        {
                            results = Calculator.CalculateAll(_service.CurrentCharacter);
                            break;
                        }

                        // Capture the "Old Value" before applying the change
                        // This allows us to revert if the user overspends points
                        int oldValue = GetCurrentStatValue(message.Stat);

                        // Enforce minimum of 1 (handles empty/0 strings from JS)
                        int newValue = message.NewValue > 0 ? message.NewValue : message.Value;
                        int safeValue = Math.Max(1, newValue);

                        // Apply the change and calculate
                        results = _service.UpdateStat(message.Stat, safeValue);

                        // Check if this change made the user go negative
                        if (results != null && results.IsOverspent)
                        {
                            // Revert the backend data so it doesn't stay "broken"
                            _service.UpdateStat(message.Stat, oldValue);

                            // Recalculate one more time with the old value to send a "safe" state back to UI
                            results = Calculator.CalculateAll(_service.CurrentCharacter);

                            // Optional: Debug log
                            Debug.WriteLine($"[Overspent] Reverted {message.Stat} from {safeValue} to {oldValue}");
                        }
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

        // Get the current stat value
        private int GetCurrentStatValue(string statName)
        {
            return statName.ToUpper() switch
            {
                "STR" => charData.Str,
                "AGI" => charData.Agi,
                "VIT" => charData.Vit,
                "INT" => charData.Int,
                "DEX" => charData.Dex,
                "LUK" => charData.Luk,
                "BASELV" => charData.BaseLevel,
                "JOBLV" => charData.JobLevel,
                _ => 1
            };
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
