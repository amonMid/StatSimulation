using Newtonsoft.Json;
using StatSimulation.Backend;
using System.Diagnostics;
using System.Web;

namespace StatSimulation
{
    public partial class Form1 : Form
    {
        private readonly CharacterService _service = new CharacterService();
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

                // --- NEW TYPE-BASED LOGIC ---
                switch (message.Type)
                {
                    case "CLASS_CHANGE":
                        // Handle Job Class Update (e.g., "Swordsman")
                        results = _service.UpdateJob(message.ClassName);
                        break;

                    case "STAT_CHANGE":
                        // Handle Attribute Update (e.g., STR, AGI)
                        results = _service.UpdateStat(message.Stat, message.NewValue);
                        break;

                    case "JOB_LEVEL_CHANGE":
                        // Handle Level Change
                        results = _service.UpdateStat("JOBLV", message.NewValue);
                        break;

                    default:
                        // Fallback for simple stat updates if type is missing
                        results = _service.UpdateStat(message.Stat, message.NewValue);
                        break;
                }
                // Serialize and Encode
                string json = JsonConvert.SerializeObject(results);
                string safeJson = HttpUtility.JavaScriptStringEncode(json);

                // Dispatch to UI
                await wb1.CoreWebView2.ExecuteScriptAsync($"CharacterUI.render('{safeJson}')");
                // Prevents the user from typing a number that makes points negative
                await wb1.CoreWebView2.ExecuteScriptAsync($"CharacterUI.syncInputs('{safeJson}')");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Deserialization failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Bridge Error]: {ex.Message}");
            }
        }
    }
}
