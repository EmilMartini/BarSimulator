using System;
using System.Linq;
using System.Windows.Controls;

namespace Lab6
{
    class DisplayController
    {
        Control[] settingControls;
        Control[] simulationControls;
        MainWindow view;

        public DisplayController(MainWindow view)
        {
            this.view = view;
            settingControls = new Control[]
            {
                view.SimulationStateDropDown,
                view.SettingsLabel,
                view.StartSimButton,
                view.SimulationSpeedLabel,
                view.speedModifierTextbox,
                view.maxChairsLabel,
                view.maxGlassesLabel,
                view.patronsPerEntryLabel,
                view.OpenForLabel,
                view.patronSimSpeedLabel,
                view.waitressSimSpeedLabel,
            };
            simulationControls = new Control[]
            {
                view.BartenderLabel,
                view.BartenderListbox,
                view.WaitressLabel,
                view.WaitressListbox,
                view.PatronsLabel,
                view.PatronsListbox,
                view.SimulationSpeedLabelInfo,
                view.PatronsInPubLabel,
                view.CleanGlassesLabel,
                view.FreeChairsLabel,
                view.TimeToCloseLabel,
                view.closeSimButton,
                view.BarIsOpenLabel,
            };
        }

        public void Start()
        {
            DisplaySettings();
            view.SimulationStateDropDown.ItemsSource = Enum.GetValues(typeof(SimulationState)).Cast<SimulationState>();
            view.SimulationStateDropDown.SelectedIndex = 0;
        }
        public void DisplaySimulation()
        {
            for (int i = 0; i < simulationControls.Length; i++)
            {
                simulationControls[i].Visibility = System.Windows.Visibility.Visible;
            }
            for (int i = 0; i < settingControls.Length; i++)
            {
                settingControls[i].Visibility = System.Windows.Visibility.Hidden;
            }
        }
        public void DisplaySettings()
        {
            for (int i = 0; i < simulationControls.Length; i++)
            {
                simulationControls[i].Visibility = System.Windows.Visibility.Hidden;
            }
            for (int i = 0; i < settingControls.Length; i++)
            {
                settingControls[i].Visibility = System.Windows.Visibility.Visible;
            }
        }
        public void PrintLabelInfo(int maxChairs, int maxGlass, TimeSpan timeSpan, int patronsPerEntry, double patronSimSpeed, double waitressSimSpeed)
        {
            view.maxChairsLabel.Content = $"Max Chairs: {maxChairs}";
            view.maxGlassesLabel.Content = $"Max Glasses: {maxGlass}";
            view.OpenForLabel.Content = $"Open for: {timeSpan.ToString(@"mm\:ss")}";
            view.patronsPerEntryLabel.Content = $"Patrons per entry: {patronsPerEntry}";
            view.patronSimSpeedLabel.Content = $"Patron simulation speed: {patronSimSpeed}";
            view.waitressSimSpeedLabel.Content = $"Waitress simulation speed: {waitressSimSpeed}";
        }
    }
}
