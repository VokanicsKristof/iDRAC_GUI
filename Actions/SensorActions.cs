using IDRAC_IPMI.DTOs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace IDRAC_IPMI.Actions
{
    public class SensorActions
    {
        public string IPMI_Path = @"C:\ipmitool\ipmitool.exe";
        private Credentials _credentials;

        public SensorActions(Credentials credentials)
        {
            _credentials = credentials;
        }

        public async Task LoadSensors(DataGrid SensorGrid, Grid LoadingOverlay, UpdateVisuals updateVis, ObservableCollection<FanInfo> fans)
        {
            LoadingOverlay.Visibility = Visibility.Visible;

            var output = await GetSensorsAsync();

            var sensors = ParseSensors(output).OrderBy(d => d.SensorName).ToList();

            SensorGrid.ItemsSource = sensors;

            updateVis.UpdateDashboard(sensors);

            LoadingOverlay.Visibility = Visibility.Collapsed;

            fans.Clear();

            foreach (var sensor in sensors.FindAll(d => d.MeasurementUnit.ToLower() == "rpm" && d.SensorName.ToLower().Contains("fan")))
            {
                fans.Add(new FanInfo(sensor));
            }
        }

        public async Task<string> GetSensorsAsync()
        {
            var args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} sensor";

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

        public async Task<string> GetChassisPowerStateAsync()
        {
            var args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} chassis power status";

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

        public async Task UpdateFanModeAsync(FanModesEnum fanMode)
        {
            var args = "";

            switch (fanMode)
            {
                case FanModesEnum.Automatic:
                    args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} raw 0x30 0x30 0x01 0x01";
                    break;
                case FanModesEnum.Manual:
                    args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} raw 0x30 0x30 0x01 0x00";
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

        public async Task<string> UpdateFanSpeedAsync(int amount)
        {
            var args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} raw 0x30 0x30 0x02 0xff 0x{amount.ToString("X")}";

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

        public async Task<List<PowerInfo>> GetPowerInfoAsync()
        {
            if (!ipmiExist())
            {
                MessageBox.Show($"IPMI not found!\nPlease place it to: {IPMI_Path}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            List<PowerInfo> powerInfo = new List<PowerInfo>();
            
            var args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} delloem powermonitor";

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

            List<string> lines = output.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();

            for(int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Trim().StartsWith("Statistic"))
                {
                    string statName = lines[i++].Split(":")[1].Trim();
                    string statStartDate = lines[i].Remove(0, lines[i++].IndexOf(":") + 1).Trim();
                    string statEndDate = lines[i].Remove(0, lines[i++].IndexOf(":") + 1).Trim();
                    string statReading = lines[i].Split(":")[1].Trim();
                    powerInfo.Add(new PowerInfo(statName, statStartDate, statEndDate, statReading));
                }
            }

            return powerInfo;
        }

        public async Task UpdatePowerStateAsync(PowerOptionEnum powerOption)
        {
            var args = "";

            switch (powerOption)
            {
                case PowerOptionEnum.PowerOn:
                    args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} chassis power on";
                    break;
                case PowerOptionEnum.GracefulShutdown:
                    args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} chassis power soft";
                    break;
                case PowerOptionEnum.ForceOff:
                    args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} chassis power off";
                    break;
                case PowerOptionEnum.Reboot:
                    args = $"-C 17 -H {_credentials.Ip} -I lanplus -U {_credentials.Username} -P {_credentials.Password} chassis power cycle";
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