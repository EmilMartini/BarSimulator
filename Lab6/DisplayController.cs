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
    }
}
