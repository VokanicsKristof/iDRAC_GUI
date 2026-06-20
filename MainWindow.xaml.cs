using IDRAC_IPMI.Actions;
using IDRAC_IPMI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;

namespace IDRAC_IPMI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel SensorView = new MainViewModel();
        public UpdateVisuals UpdateVis;
        public SensorActions sensorActions;

        public MainWindow()
        {
            InitializeComponent();

            UpdateVis = new UpdateVisuals(SensorView.GeneralViewModel);

            // Warning! Before you uncomment this, make sure that the ip, username and password viewmodel property all have the correct default value
            //Loaded += async (_, _) =>
            //{
               // await sensorActions.LoadSensors(SensorGrid, LoadingOverlay, UpdateVis, SensorView.Fans);
               // VentillationInfo.Visibility = Visibility.Collapsed;
            //};

            DataContext = SensorView;

            SensorView.GeneralViewModel.Fans = new System.Collections.ObjectModel.ObservableCollection<DTOs.FanInfo>();
            SensorView.GeneralViewModel.Credentials = new DTOs.Credentials();

            sensorActions = new SensorActions(SensorView.GeneralViewModel.Credentials);
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await sensorActions.LoadSensors(SensorGrid, LoadingOverlay, UpdateVis, SensorView.GeneralViewModel.Fans);
            VentillationInfo.Visibility = Visibility.Collapsed;
            FanData.Visibility = Visibility.Visible;

            string chassisPowerState = await sensorActions.GetChassisPowerStateAsync();
            ChassisStatus.Visibility = Visibility.Visible;
            ChassisStatus.Text = chassisPowerState;

            if (chassisPowerState.ToLower().Contains("on"))
            {
                ChassisStatus.Foreground = Brushes.Green;
            }
            else
            {
                ChassisStatus.Foreground = Brushes.Red;
            }
        }

        private async void UpdateFanMode(object sender, SelectionChangedEventArgs e)
        {
            if (sensorActions == null) return;

            var selectedItem = ((ComboBox)sender).SelectedItem;
            if (((ComboBoxItem)selectedItem).Content == null)
            {
                MessageBox.Show("No fan mode selected!", "Select fan mode!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            switch(((ComboBoxItem)selectedItem).Content)
            {
                case "Automatic":
                    await sensorActions.UpdateFanModeAsync(FanModesEnum.Automatic);
                    break;
                case "Manual":
                    await sensorActions.UpdateFanModeAsync(FanModesEnum.Manual);
                    break;
                default:
                    await sensorActions.UpdateFanModeAsync(FanModesEnum.Manual);
                    break;
            }
        }

        private CancellationTokenSource? _fanSpeedCts;

        private async void UpdateFanSpeed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _fanSpeedCts?.Cancel();
            int speed = (int)e.NewValue;

            if(FanSpeedLabel != null)
            {
                FanSpeedLabel.Text = speed + "%";
            }

            _fanSpeedCts = new CancellationTokenSource();
            var token = _fanSpeedCts.Token;

            try
            {
                await Task.Delay(500, token);


                await sensorActions.UpdateFanSpeedAsync(speed);
            }
            catch (TaskCanceledException)
            {
                // user moved slider again within delay time
            }
        }

        private void Btn_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Fullscreen(object sender, RoutedEventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Normal:
                    this.WindowState = WindowState.Maximized;
                    break;
                case WindowState.Maximized:
                    this.WindowState = WindowState.Normal;
                    break;
            }
        }

        private void Btn_Minimized(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private async void RefreshPowerInfo(object sender, RoutedEventArgs e)
        {
            var result = await sensorActions.GetPowerInfoAsync();

            SensorView.PowerViewModel.PowerInfos.Clear();

            foreach (var info in result)
            {
                SensorView.PowerViewModel.PowerInfos.Add(info);
            }
        }

        private async void PowerOption(object sender, RoutedEventArgs e)
        {
            string id = ((Button)sender).Name;

            MessageBoxResult result = MessageBox.Show("Are you sure want to do the following command?\n\n" + id, "Alert!", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                switch (id.ToLower())
                {
                    case "poweron":
                        await sensorActions.UpdatePowerStateAsync(PowerOptionEnum.PowerOn);
                        break;
                    case "gracefulshutdown":
                        await sensorActions.UpdatePowerStateAsync(PowerOptionEnum.GracefulShutdown);
                        break;
                    case "forceoff":
                        await sensorActions.UpdatePowerStateAsync(PowerOptionEnum.ForceOff);
                        break;
                    case "reboot":
                        await sensorActions.UpdatePowerStateAsync(PowerOptionEnum.Reboot);
                        break;
                    default:
                        MessageBox.Show("Action aborted!\n\n" + id + "\n\nUnknown command!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
                MessageBox.Show("Action completed!\n\n" + id, "Alert!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if(result == MessageBoxResult.No)
            {
                MessageBox.Show("Action aborted!\n\n" + id, "Alert!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}