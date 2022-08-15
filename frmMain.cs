using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ha_overlay
{
    public partial class frmMain : Form
    {
        private Config config;

        public frmMain()
        {
            InitializeComponent();

            DoubleBuffered = true;

            var deserializer = new YamlDotNet.Serialization.Deserializer();
            var data = File.ReadAllText(Path.Combine(".", "config.yaml"));
            config = deserializer.Deserialize<Config>(data);

            TopMost = config.AlwaysOnTop;

            GetAll();

            StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(Location))
                {
                    Location = new Point(scrn.Bounds.Right - Width - 100, scrn.Bounds.Top);
                    return;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetAll();
        }

        private void GetAll()
        {
            var width = Width - 1;

            Controls.Clear();

            for (var i=0; i<config.Sensors.Count; i++)
            {
                var sensor = config.Sensors[i];
                var lbl = CreateLabel(width, i, config.FontSize, sensor.Color);
                var txt = GetData(sensor.Name, lbl.Text, sensor.Format, sensor.Label);
                lbl.Text = $"{txt}";
                Controls.Add(lbl);
            }

            timer.Enabled = true;
        }

        private string GetData(string sensor, string currentValue, string format, string label)
        {
            var culture = new CultureInfo("en-GB");
            try
            {
                var request = WebRequest.Create($"{config.HAUrl}/api/states/{sensor}");
                request.Headers.Add("Authorization", $"Bearer {config.Token}");
                request.ContentType = "application/json";
                var response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var data = reader.ReadToEnd();
                dynamic parsedJson = JsonConvert.DeserializeObject(data);

                var _label = label != null ? $"{label} " : "";
                var state = parsedJson["state"].ToString();
                var unitOfMeasure = parsedJson["attributes"]["unit_of_measurement"];

                if (parsedJson["attributes"]["device_class"] == "monetary")
                {
                    return $"{_label}{decimal.Parse(state, culture).ToString(format)} {unitOfMeasure}";
                }
                return $"{_label}{state} {unitOfMeasure}";
            }
            catch
            {
                return currentValue;
            }
        }

        private Label CreateLabel(int width, int i, float fontSize, string color)
        {
            var lbl = new Label();
            lbl.ForeColor = ColorTranslator.FromHtml(color);
            lbl.Font = new Font("Tahoma", fontSize);
            lbl.Height = 35;
            lbl.Width = width;
            lbl.TextAlign = ContentAlignment.MiddleRight;
            lbl.Top = 10 + 30 * i;
            return lbl;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            GetAll();
        }

        private void toolQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
