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

                // 1. Process via Service
                var results = _service.UpdateStat(message.Stat, message.NewValue);

                // 2. Serialize and Encode
                string json = JsonConvert.SerializeObject(results);
                string safeJson = HttpUtility.JavaScriptStringEncode(json);

                // 3. Dispatch to UI
                await wb1.CoreWebView2.ExecuteScriptAsync($"CharacterUI.render('{safeJson}')");
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
