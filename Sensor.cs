using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ha_overlay
{
    internal class Sensor
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Format { get; set; }
        public string Color { get; set; } = "#ff0000";
    }
}