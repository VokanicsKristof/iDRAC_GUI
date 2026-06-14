using IDRAC_IPMI.Actions;
using System.Windows;
using System.Windows.Controls;

namespace IDRAC_IPMI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewModel SensorView = new ViewModel();
        public UpdateVisuals UpdateVis;
        public SensorActions sensorActions = new SensorActions();

        public MainWindow()
        {
            InitializeComponent();

            UpdateVis = new UpdateVisuals(SensorView);

            // Warning! Before you uncomment this, make sure that the ip, username and password textbox all have the correct default value
            //Loaded += async (_, _) =>
            //{
            //    await sensorActions.LoadSensors(SensorGrid, IpValue.Text, Username.Text, Password.Text, LoadingOverlay, UpdateVis, SensorView.Fans);
               // VentillationInfo.Visibility = Visibility.Collapsed;
            //};

            DataContext = SensorView;

            SensorView.Fans = new System.Collections.ObjectModel.ObservableCollection<DTOs.FanInfo>();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await sensorActions.LoadSensors(SensorGrid, IpValue.Text, Username.Text, Password.Text, LoadingOverlay, UpdateVis, SensorView.Fans);
            VentillationInfo.Visibility = Visibility.Collapsed;
            FanData.Visibility = Visibility.Visible;
        }

        private async void UpdateFanMode(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((ComboBox)sender).SelectedItem;
            if (((ComboBoxItem)selectedItem).Content == null)
            {
                MessageBox.Show("No fan mode selected!", "Select fan mode!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            switch(((ComboBoxItem)selectedItem).Content)
            {
                case "Automatic":
                    await sensorActions.UpdateFanMode(IpValue.Text, Username.Text, Password.Text, FanModesEnum.Automatic);
                    break;
                case "Manual":
                    await sensorActions.UpdateFanMode(IpValue.Text, Username.Text, Password.Text, FanModesEnum.Manual);
                    break;
                default:
                    await sensorActions.UpdateFanMode(IpValue.Text, Username.Text, Password.Text, FanModesEnum.Manual);
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


                await sensorActions.UpdateFanSpeed(IpValue.Text, Username.Text, Password.Text, speed);
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
    }
}