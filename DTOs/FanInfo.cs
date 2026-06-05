using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDRAC_IPMI.DTOs
{
    public class FanInfo
    {
        public string Name { get; set; }

        private string _RPM = "";
        public string RPM
        {
            get => _RPM;
            set
            {
                string val = value.Split(".")[0];
                _RPM = val;
            }
        }

        private string _CriticalLow = "";
        public string CriticalLow
        {
            get => _CriticalLow;
            set
            {
                string val = value.Split(".")[0];
                _CriticalLow = val;
            }
        }

        public bool IsHealthy => int.Parse(RPM) >= int.Parse(CriticalLow);

        public FanInfo(SensorInfo sensorInfo)
        {
            Name = sensorInfo.SensorName;
            RPM = sensorInfo.SensorValue;
            CriticalLow = sensorInfo.CriticalMinimum;
        }
    }
}
