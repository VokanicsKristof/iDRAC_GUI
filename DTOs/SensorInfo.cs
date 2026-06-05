using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDRAC_IPMI.DTOs
{
    public class SensorInfo
    {
        public string SensorName { get; set; }
        public string SensorValue { get; set; }
        public string MeasurementUnit { get; set; }
        public string SensorStatus { get; set; }
        public string LowerNonRecoverable { get; set; }
        public string CriticalMinimum { get; set; }
        public string WarningMinimum { get; set; }
        public string WarningMaximum { get; set; }
        public string CriticalMaximum { get; set; }
        public string UpperNonRecoverable { get; set; }

        public SensorInfo(string line)
        {
            string[] data = line.Split('|');
            SensorName = data[0].Trim();
            SensorValue = data[1].Trim();
            MeasurementUnit = data[2].Trim();
            SensorStatus = data[3].Trim();
            LowerNonRecoverable = data[4].Trim();
            CriticalMinimum = data[5].Trim();
            WarningMinimum = data[6].Trim();
            WarningMaximum = data[7].Trim();
            CriticalMaximum = data[8].Trim();
            UpperNonRecoverable = data[9].Trim();
        }
    }
}
