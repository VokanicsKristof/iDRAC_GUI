using IDRAC_IPMI.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
namespace IDRAC_IPMI
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string _LastUpdated = "Last updated: ";
        public string LastUpdated
        {
            get => _LastUpdated;
            set
            {
                _LastUpdated = $"Last updated: {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";
                OnPropertyChanged();
            }
        }

        private string _PowerText = "";
        public string PowerText 
        { 
            get => _PowerText; 
            set
            {
                _PowerText = value;
                OnPropertyChanged();
            }
        }
        private string _InletTempText = "";
        public string InletTempText
        {
            get => _InletTempText;
            set
            {
                _InletTempText = value;
                OnPropertyChanged();
            }
        }
        private string _ExhaustTempText = "";
        public string ExhaustTempText
        {
            get => _ExhaustTempText;
            set
            {
                _ExhaustTempText = value;
                OnPropertyChanged();
            }
        }

        private string _HealthText = "";
        public string HealthText
        {
            get => _HealthText;
            set
            {
                _HealthText = value;
                switch (value)
                {
                    case "Warning":
                        HealthColor = Brushes.OrangeRed;
                        break;
                    case "Healthy":
                        HealthColor = Brushes.Lime;
                        break;
                }
                OnPropertyChanged();
            }
        }

        private string _CPU1Text = "";
        public string CPU1Text
        {
            get => _CPU1Text;
            set
            {
                _CPU1Text = value;
                OnPropertyChanged();
            }
        }

        private string _CPU2Text = "";
        public string CPU2Text
        {
            get => _CPU2Text;
            set
            {
                _CPU2Text = value;
                OnPropertyChanged();
            }
        }

        private Brush _HealthColor;
        public Brush HealthColor 
        {
            get => _HealthColor;
            set
            {
                _HealthColor = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SensorInfo> Sensors { get; }
        public ObservableCollection<FanInfo> Fans { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
