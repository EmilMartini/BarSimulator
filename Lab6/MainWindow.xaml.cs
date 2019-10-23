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
            DisplaySettingsMenu();
        }

        private void DisplaySettingsMenu()
        {
            BartenderLabel.Visibility = Visibility.Hidden;
            BartenderListbox.Visibility = Visibility.Hidden;

            WaitressLabel.Visibility = Visibility.Hidden;
            WaitressListbox.Visibility = Visibility.Hidden;

            PatronsLabel.Visibility = Visibility.Hidden;
            PatronsListbox.Visibility = Visibility.Hidden;

            SimulationSpeedLabelInfo.Visibility = Visibility.Hidden;
            PatronsInPubLabel.Visibility = Visibility.Hidden;
            CleanGlassesLabel.Visibility = Visibility.Hidden;
            FreeChairsLabel.Visibility = Visibility.Hidden;
            TimeToCloseLabel.Visibility = Visibility.Hidden;

            SettingsLabel.Visibility = Visibility.Visible;
            SimulationStateDropDown.Visibility = Visibility.Visible;
            StartSimButton.Visibility = Visibility.Visible;
            SimulationSpeedLabel.Visibility = Visibility.Visible;
            speedModifierTextbox.Visibility = Visibility.Visible;
        }
        private void DisplaySimulation()
        {
            BartenderLabel.Visibility = Visibility.Visible;
            BartenderListbox.Visibility = Visibility.Visible;

            WaitressLabel.Visibility = Visibility.Visible;
            WaitressListbox.Visibility = Visibility.Visible;

            PatronsLabel.Visibility = Visibility.Visible;
            PatronsListbox.Visibility = Visibility.Visible;

            SimulationSpeedLabelInfo.Visibility = Visibility.Visible;
            PatronsInPubLabel.Visibility = Visibility.Visible;
            CleanGlassesLabel.Visibility = Visibility.Visible;
            FreeChairsLabel.Visibility = Visibility.Visible;
            TimeToCloseLabel.Visibility = Visibility.Visible;

            SettingsLabel.Visibility = Visibility.Hidden;
            SimulationStateDropDown.Visibility = Visibility.Hidden;
            StartSimButton.Visibility = Visibility.Hidden;
            SimulationSpeedLabel.Visibility = Visibility.Hidden;
            speedModifierTextbox.Visibility = Visibility.Hidden;
        }
        private void ComboBox_Initialized(object sender, EventArgs e)
        {
            SimulationStateDropDown.ItemsSource = Enum.GetValues(typeof(SimulationState)).Cast<SimulationState>();
            SimulationStateDropDown.SelectedItem = SimulationState.Default;
        }
        private void StartSimButton_Click(object sender, RoutedEventArgs e)
        {
            DisplaySimulation();
            SimulationManager Manager = new SimulationManager((SimulationState)SimulationStateDropDown.SelectedItem, double.Parse(speedModifierTextbox.Text));
        }
    }
}
