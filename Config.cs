using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ha_overlay
{
    internal class Config
    {
        public string HAUrl { get; set; } = "http://homeassistant.local:8123";
        public bool AlwaysOnTop { get; set; } = false;
        public string Token { get; set; } = null;
        public float FontSize { get; set; } = 14;
        public List<Sensor> Sensors { get; set; }
    }
}
