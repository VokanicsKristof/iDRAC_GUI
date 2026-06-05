using IDRAC_IPMI.DTOs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace IDRAC_IPMI.Actions
{
    public class SensorActions
    {
        public string IPMI_Path = @"C:\ipmitool\ipmitool.exe";
        public async Task LoadSensors(DataGrid SensorGrid, string IP, string user, string password, Grid LoadingOverlay, UpdateVisuals updateVis, ObservableCollection<FanInfo> fans)
        {
            LoadingOverlay.Visibility = Visibility.Visible;

            var ipmi = new SensorActions();

            var output = await ipmi.GetSensorsAsync(IP, user, password);

            var sensors = ipmi.ParseSensors(output).OrderBy(d => d.SensorName).ToList();

            SensorGrid.ItemsSource = sensors;

            updateVis.UpdateDashboard(sensors);

            LoadingOverlay.Visibility = Visibility.Collapsed;

            fans.Clear();

            foreach (var sensor in sensors.FindAll(d => d.MeasurementUnit.ToLower() == "rpm" && d.SensorName.ToLower().Contains("fan")))
            {
                fans.Add(new FanInfo(sensor));
            }
        }

        public async Task<string> GetSensorsAsync(string ip, string user, string password)
        {
            var args = $"-C 17 -H {ip} -I lanplus -U {user} -P {password} sensor";

            if (!ipmiExist())
            {
                MessageBox.Show($"IPMI not found!\nPlease place it to: {IPMI_Path}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

            var psi = new ProcessStartInfo
            {
                FileName = IPMI_Path,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new Exception(error);

            return output;
        }

        public async Task UpdateFanMode(string ip, string user, string password, FanModesEnum fanMode)
        {
            var args = "";

            switch (fanMode)
            {
                case FanModesEnum.Automatic:
                    args = $"-C 17 -H {ip} -I lanplus -U {user} -P {password} raw 0x30 0x30 0x01 0x01";
                    break;
                case FanModesEnum.Manual:
                    args = $"-C 17 -H {ip} -I lanplus -U {user} -P {password} raw 0x30 0x30 0x01 0x00";
                    break;
            }

            if (!ipmiExist())
            {
                MessageBox.Show($"IPMI not found!\nPlease place it to: {IPMI_Path}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var psi = new ProcessStartInfo
            {
                FileName = IPMI_Path,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new Exception(error);
        }

        public async Task<string> UpdateFanSpeed(string ip, string user, string password, int amount)
        {
            var args = $"-C 17 -H {ip} -I lanplus -U {user} -P {password} raw 0x30 0x30 0x02 0xff 0x{amount.ToString("X")}";

            if (!ipmiExist())
            {
                MessageBox.Show($"IPMI not found!\nPlease place it to: {IPMI_Path}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

            var psi = new ProcessStartInfo
            {
                FileName = IPMI_Path,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new Exception(error);

            return output;
        }

        public List<SensorInfo> ParseSensors(string output)
        {
            var sensors = new List<SensorInfo>();

            foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split('|');

                if (parts.Length < 3)
                {
                    continue;
                }

                sensors.Add(new SensorInfo(line));
            }

            return sensors;
        }

        public bool ipmiExist()
        {
            return File.Exists(IPMI_Path);
        }
    }
}