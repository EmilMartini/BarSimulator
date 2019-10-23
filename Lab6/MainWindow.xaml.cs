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
            BartenderButton.Visibility = Visibility.Hidden;
            BartenderLabel.Visibility = Visibility.Hidden;
            BartenderListbox.Visibility = Visibility.Hidden;

            WaitressButton.Visibility = Visibility.Hidden;
            WaitressLabel.Visibility = Visibility.Hidden;
            WaitressListbox.Visibility = Visibility.Hidden;

            PatronsButton.Visibility = Visibility.Hidden;
            PatronsLabel.Visibility = Visibility.Hidden;
            PatronsListbox.Visibility = Visibility.Hidden;

            OpenCLoseButton.Visibility = Visibility.Hidden;

            PatronsInPubLabel.Visibility = Visibility.Hidden;
            CleanGlassesLabel.Visibility = Visibility.Hidden;
            FreeChairsLabel.Visibility = Visibility.Hidden;
            TimeToCloseLabel.Visibility = Visibility.Hidden;
            StopAllButton.Visibility = Visibility.Hidden;

            SettingsLabel.Visibility = Visibility.Visible;
            SimulationStateDropDown.Visibility = Visibility.Visible;
            StartSimButton.Visibility = Visibility.Visible;
        }

        private void DisplaySimulation()
        {
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

            PatronsInPubLabel.Visibility = Visibility.Visible;
            CleanGlassesLabel.Visibility = Visibility.Visible;
            FreeChairsLabel.Visibility = Visibility.Visible;
            TimeToCloseLabel.Visibility = Visibility.Visible;
            StopAllButton.Visibility = Visibility.Visible;

            SettingsLabel.Visibility = Visibility.Hidden;
            SimulationStateDropDown.Visibility = Visibility.Hidden;
            StartSimButton.Visibility = Visibility.Hidden;
        }
        private void ComboBox_Initialized(object sender, EventArgs e)
        {
            SimulationStateDropDown.ItemsSource = Enum.GetValues(typeof(SimulationState)).Cast<SimulationState>(); 
        }
        private void StartSimButton_Click(object sender, RoutedEventArgs e)
        {
            DisplaySimulation();
            SimulationManager Manager = new SimulationManager((SimulationState)SimulationStateDropDown.SelectedItem);
        }
    }
}
