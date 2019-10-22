using System;
using System.Linq;
using System.Windows;

namespace Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void DisplayComponents()
        {
            BouncerButton.Visibility = Visibility.Visible;
            BouncerLabel.Visibility = Visibility.Visible;
            BouncerListbox.Visibility = Visibility.Visible;

            BartenderButton.Visibility = Visibility.Visible;
            BartenderLabel.Visibility = Visibility.Visible;
            BartenderListbox.Visibility = Visibility.Visible;

            WaitressButton.Visibility = Visibility.Visible;
            WaitressLabel.Visibility = Visibility.Visible;
            WaitressListbox.Visibility = Visibility.Visible;

            PatronsButton.Visibility = Visibility.Visible;
            PatronsLabel.Visibility = Visibility.Visible;
            PatronsListbox.Visibility = Visibility.Visible;

            OpenCLoseButton.Visibility = Visibility.Visible;

            GuestsInBarLabel.Visibility = Visibility.Visible;
            CleanGlassesLabel.Visibility = Visibility.Visible;
            FreeChairsLabel.Visibility = Visibility.Visible;
            TimeToCloseLabel.Visibility = Visibility.Visible;
            StopAllButton.Visibility = Visibility.Visible;

            SettingsLabel.Visibility = Visibility.Hidden;
            SimulationStateDropDown.Visibility = Visibility.Hidden;
            StartSimButton.Visibility = Visibility.Hidden;
        }
        public MainWindow GetWindow()
        {
            return this;
        }
        private void ComboBox_Initialized(object sender, EventArgs e)
        {
            SimulationStateDropDown.ItemsSource = Enum.GetValues(typeof(SimulationState)).Cast<SimulationState>(); 
        }
        private void StartSimButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayComponents();
            SimulationManager Manager = new SimulationManager((SimulationState)SimulationStateDropDown.SelectedItem);
        }
    }
}
