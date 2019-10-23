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
        SimulationManager Manager;
        public MainWindow()
        {
            InitializeComponent();
            DisplaySettingsMenu();
            SimulationStateDropDown.SelectedItem = SimulationState.Default;
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
            closeSimButton.Visibility = Visibility.Hidden;

            SettingsLabel.Visibility = Visibility.Visible;
            SimulationStateDropDown.Visibility = Visibility.Visible;
            StartSimButton.Visibility = Visibility.Visible;
            SimulationSpeedLabel.Visibility = Visibility.Visible;
            speedModifierTextbox.Visibility = Visibility.Visible;

            maxChairsLabel.Visibility = Visibility.Visible;
            maxGlassesLabel.Visibility = Visibility.Visible;
            patronsPerEntryLabel.Visibility = Visibility.Visible;
            OpenForLabel.Visibility = Visibility.Visible;
            patronSimSpeedLabel.Visibility = Visibility.Visible;
            waitressSimSpeedLabel.Visibility = Visibility.Visible;
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
            closeSimButton.Visibility = Visibility.Visible;

            SettingsLabel.Visibility = Visibility.Hidden;
            SimulationStateDropDown.Visibility = Visibility.Hidden;
            StartSimButton.Visibility = Visibility.Hidden;
            SimulationSpeedLabel.Visibility = Visibility.Hidden;
            speedModifierTextbox.Visibility = Visibility.Hidden;

            maxChairsLabel.Visibility = Visibility.Hidden;
            maxGlassesLabel.Visibility = Visibility.Hidden;
            patronsPerEntryLabel.Visibility = Visibility.Hidden;
            OpenForLabel.Visibility = Visibility.Hidden;
            patronSimSpeedLabel.Visibility = Visibility.Hidden;
            waitressSimSpeedLabel.Visibility = Visibility.Hidden;
        }
        private void ComboBox_Initialized(object sender, EventArgs e)
        {
            SimulationStateDropDown.ItemsSource = Enum.GetValues(typeof(SimulationState)).Cast<SimulationState>();
        }
        private void StartSimButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplaySimulation();
                Manager = new SimulationManager((SimulationState)SimulationStateDropDown.SelectedItem, double.Parse(speedModifierTextbox.Text));
                Manager.StartSimulation();
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void SimulationStateDropDown_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (SimulationStateDropDown.SelectedItem)
            {
                case SimulationState.Default:
                    PrintLabelInfo(9, 8, new TimeSpan(0, 1, 20), 1, 1, 1);
                    break;
                case SimulationState.TwentyGlassThreeChairs:
                    PrintLabelInfo(3, 20, new TimeSpan(0, 1, 20), 1, 1, 1);
                    break;
                case SimulationState.TwentyChairsThreeGlass:
                    PrintLabelInfo(20, 3, new TimeSpan(0, 1, 20), 1, 1, 1);
                    break;
                case SimulationState.PatronsSlowMode:
                    PrintLabelInfo(9, 8, new TimeSpan(0, 1, 20), 1, 0.5, 1);
                    break;
                case SimulationState.WaitressBoostMode:
                    PrintLabelInfo(9, 8, new TimeSpan(0, 1, 20), 1, 1, 2);
                    break;
                case SimulationState.BarOpenForFiveMins:
                    PrintLabelInfo(9, 8, new TimeSpan(0, 5, 0), 1, 1, 1);
                    break;
                case SimulationState.CouplesNight:
                    PrintLabelInfo(9, 8, new TimeSpan(0, 1, 20), 2, 1, 1);
                    break;
                case SimulationState.BusLoad:
                    PrintLabelInfo(9, 8, new TimeSpan(0, 1, 20), 20, 1, 1);
                    break;
                case SimulationState.CrazyState:
                    PrintLabelInfo(100, 100, new TimeSpan(0, 59, 59), 4, 0.2, 3);
                    break;
            }
        }
        private void PrintLabelInfo(int maxChairs, int maxGlass, TimeSpan timeSpan, int patronsPerEntry, double patronSimSpeed, double waitressSimSpeed)
        {
            maxChairsLabel.Content = $"Max Chairs: {maxChairs}";
            maxGlassesLabel.Content = $"Max Glasses: {maxGlass}";
            OpenForLabel.Content = $"Open for: {timeSpan.ToString(@"mm\:ss")}";
            patronsPerEntryLabel.Content = $"Patrons per entry: {patronsPerEntry}";
            patronSimSpeedLabel.Content = $"Patron sim-speed: {patronSimSpeed}";
            waitressSimSpeedLabel.Content = $"Waitress sim-speed: {waitressSimSpeed}";
        }
        private void CloseSimButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.StopSimulation();
            DisplaySettingsMenu();
            SimulationStateDropDown.SelectedItem = SimulationState.Default;
        }
    }
}