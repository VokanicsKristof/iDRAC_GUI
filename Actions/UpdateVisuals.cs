using IDRAC_IPMI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IDRAC_IPMI.Actions
{
    public class UpdateVisuals
    {
        private readonly ViewModel _vm;

        public UpdateVisuals(ViewModel vm)
        {
            _vm = vm;
        }

        public void UpdateDashboard(List<SensorInfo> sensors)
        {
            sensors.OrderBy(d => d.SensorName);

            var inlet = sensors
                .FirstOrDefault(x => x.SensorName == "Inlet Temp");

            var exhaust = sensors
                .FirstOrDefault(x => x.SensorName == "Exhaust Temp");

            var power = sensors
                .FirstOrDefault(x => x.SensorName == "Pwr Consumption");

            var CPUs = sensors.FindAll(x => x.SensorName == "Temp");

            if (inlet != null)
                _vm.InletTempText = $"{inlet.SensorValue} °C";

            if (exhaust != null)
                _vm.ExhaustTempText = $"{exhaust.SensorValue} °C";

            if (power != null)
                _vm.PowerText = $"{power.SensorValue} W";

            _vm.CPU1Text = $"{CPUs[0].SensorValue} °C";
            
            if (CPUs.Count == 2)
                _vm.CPU2Text = $"{CPUs[1].SensorValue} °C";

            _vm.LastUpdated = "Trigger";

            bool hasWarning =
                sensors.Any(x =>
                    !string.Equals(x.SensorStatus,
                        "ok",
                        StringComparison.OrdinalIgnoreCase) &&
                    x.SensorStatus != "na");

            _vm.HealthText = hasWarning
                ? "Warning"
                : "Healthy";
        }
    }
}
