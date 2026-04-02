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

                switch (message.Type?.ToUpper())
                {
                    case "CLASS_CHANGE":
                        _service.UpdateJob(message.ClassName ?? "Novice");
                        results = Calculator.CalculateAll(_service.CurrentCharacter);
                        UpdateUIStats();
                        break;

                    case "JOB_LEVEL_CHANGE":
                        int jlv = message.Value > 0 ? message.Value : message.NewValue;
                        results = _service.UpdateStat("JOBLV", jlv);
                        break;

                    case "SKILL_CHANGE":
                        _service.UpdateSkill(message.Stat, (int)message.Value);
                        results = Calculator.CalculateAll(_service.CurrentCharacter);
                        UpdateUIStats();
                        break;
                    case "SKILL_TREE_BACK":
                        // Sync job class if it changed in skill tree
                        if (!string.IsNullOrEmpty(message.Job))
                        {
                            _service.UpdateJob(message.Job);
                        }
                        // Full sync from skill tree ? job level + all skill levels
                        if (message.JobLevel > 0)
                        {
                            _service.CurrentCharacter.JobLevel = message.JobLevel;
                        }
                        if (message.SkillLevels != null)
                        {
                            _service.CurrentCharacter.SkillLevels = new Dictionary<string, int>(message.SkillLevels);
                        }
                        else
                        {
                            _service.CurrentCharacter.SkillLevels.Clear();
                        }
                        results = Calculator.CalculateAll(_service.CurrentCharacter);
                        // Push updated job level back to index.html UI
                        UpdateUIStats();
                        break;

                    case "SKILL_UPDATE":
                        // Incremental update ? single skill change + auto-allocated job level
                        if (message.JobLevel > 0)
                        {
                            _service.CurrentCharacter.JobLevel = message.JobLevel;
                        }
                        if (!string.IsNullOrEmpty(message.SkillId))
                        {
                            _service.UpdateSkill(message.SkillId, message.SkillLevel);
                        }
                        results = Calculator.CalculateAll(_service.CurrentCharacter);
                        break;

                    case "JOB_CHANGE":
                        // Carousel class switch from skill tree
                        if (!string.IsNullOrEmpty(message.Job))
                        {
                            _service.UpdateJob(message.Job);
                            _service.CurrentCharacter.SkillLevels.Clear();
                            _service.CurrentCharacter.JobLevel = 1;
                            results = Calculator.CalculateAll(_service.CurrentCharacter);
                            UpdateUIStats();
                        }
                        break;
                    case "SKILL_RESET":
                        _service.CurrentCharacter.SkillLevels.Clear();
                        results = Calculator.CalculateAll(_service.CurrentCharacter);
                        UpdateUIStats();
                        break;

                    case "WEAPON_CHANGE":
                        string weaponStr = message.Weapon ?? "Hand";
                        _service.CurrentCharacter.EquippedWeapon = ParseWeaponType(weaponStr);
                        results = Calculator.CalculateAll(_service.CurrentCharacter);
                        break;

                    case "PAGE_READY":
                        SyncFrontendState();
                        break;

                    case "STAT_CHANGE":
                    default:
                        if (string.IsNullOrEmpty(message.Stat))
                        {
                            results = Calculator.CalculateAll(_service.CurrentCharacter);
                            break;
                        }

                        int oldValue = GetCurrentStatValue(message.Stat);
                        int newValue = message.NewValue > 0 ? message.NewValue : message.Value;
                        int safeValue = Math.Max(1, newValue);
                        results = _service.UpdateStat(message.Stat, safeValue);

                        if (results != null && results.IsOverspent)
                        {
                            _service.UpdateStat(message.Stat, oldValue);
                            results = Calculator.CalculateAll(_service.CurrentCharacter);
                        }
                        break;
                }

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
                Debug.WriteLine($"[JSON Error]: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Bridge Error]: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void UpdateUIStats()
        {
            var job = JobRegistry.Get(_service.CurrentCharacter.Job);

            string jsCode = $@"
            (function() {{
                window.isInternalUpdate = true;
                try {{
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

                    const stats = ['str', 'agi', 'vit', 'int', 'dex', 'luk'];
                    stats.forEach(s => {{
                        const bonusEl = document.getElementById(s + '-bonus');
                        if (bonusEl) bonusEl.innerText = '+0';
                    }});

                    const baseLvInput = document.querySelector('[data-stat-input=""BASELV""]');
                    if (baseLvInput) baseLvInput.value = {_service.CurrentCharacter.BaseLevel};

                    if (typeof populateJobLevels === 'function') {{
                        populateJobLevels({job.MaxJobLevel});
                        const jobLvEl = document.querySelector('[data-stat-input=""JOBLV""]');
                        if (jobLvEl) jobLvEl.value = {_service.CurrentCharacter.JobLevel};
                    }}

                    if (baseLvInput) baseLvInput.dispatchEvent(new Event('change'));

                    const weaponEl = document.getElementById('weaponSelect');
                    if (weaponEl) {{
                        // Don't reset if same weapon type
                    }}
                }} finally {{
                    setTimeout(() => {{ window.isInternalUpdate = false; }}, 50);
                }}
            }})();";

            wb1.ExecuteScriptAsync(jsCode);
        }

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

        private WeaponType ParseWeaponType(string weaponStr)
        {
            string normalized = weaponStr.ToLower().Replace(" ", "_").Replace("-", "_").Replace("&", "and");
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

        private async void SyncFrontendState()
        {
            //if (wb1.CoreWebView2 == null) return;
            //var results = Calculator.CalculateAll(_service.CurrentCharacter);
            //string json = JsonConvert.SerializeObject(results);
            //string skillJson = JsonConvert.SerializeObject(new { 
            //    Job = _service.CurrentCharacter.Job, 
            //    JobLv = _service.CurrentCharacter.JobLevel,
            //    SkillLevels = _service.CurrentCharacter.SkillLevels,
            //    Weapon = StringifyWeaponType(_service.CurrentCharacter.EquippedWeapon)
            //});
            //string safeJson = System.Web.HttpUtility.JavaScriptStringEncode(json);

            //await wb1.CoreWebView2.ExecuteScriptAsync($"if(window.CharacterUI) CharacterUI.render(\"{safeJson}\");");
            //await wb1.CoreWebView2.ExecuteScriptAsync($"if(window.CharacterUI) CharacterUI.syncInputs(\"{safeJson}\");");
            //await wb1.CoreWebView2.ExecuteScriptAsync($"if(window.syncFromBackend) window.syncFromBackend({skillJson});");
            var skillLevels = JsonConvert.SerializeObject(_service.CurrentCharacter.SkillLevels ?? new Dictionary<string, int>());

            string script = $@"
                if (typeof window.syncFromBackend === 'function') {{
                    window.syncFromBackend({{
                        Job: '{_service.CurrentCharacter.Job}',
                        JobLv: {_service.CurrentCharacter.JobLevel},
                        SkillLevels: {skillLevels}
                    }});
                }}
            ";
            await wb1.CoreWebView2.ExecuteScriptAsync(script);
        }

        private string StringifyWeaponType(WeaponType weapon)
        {
            return weapon switch
            {
                WeaponType.Hand => "hand",
                WeaponType.Dagger => "dagger",
                WeaponType.OnehandedSword => "one-handed_sword",
                WeaponType.TwohandedSword => "two-handed_sword",
                WeaponType.OnehandedSpear => "one-handed_spear",
                WeaponType.TwohandedSpear => "two-handed_spear",
                WeaponType.OnehandedAxe => "one-handed_axe",
                WeaponType.TwohandedAxe => "two-handed_axe",
                WeaponType.OnehandedMace => "one-handed_mace",
                WeaponType.TwohandedMace => "two-handed_mace",
                WeaponType.RodStaff => "rod_and_staff",
                WeaponType.TwohandedStaff => "two-handed_staff",
                WeaponType.Bow => "bow",
                _ => "hand"
            };
        }
    }
}
