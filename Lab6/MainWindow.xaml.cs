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
        SimulationManager manager; //Liten bokstav på privata fält
        DisplayController controller;
        public MainWindow()  
        {
            InitializeComponent();
            controller = new DisplayController(this);
            controller.Start();
            
            //Enkapsulera CurrentPatrons i establishement
            //Ni tar väldigt många parametrerar i konstruktorn för establishmenten, inte lättläsligt
        }
        private void StartSimButton_Click(object sender, RoutedEventArgs e)
        {
            double parsed;
            if(double.TryParse(speedModifierTextbox.Text.Replace('.',','), out parsed))
            {
                if(parsed > 0 && parsed < 11)
                {
                    controller.DisplaySimulation();
                    manager = new SimulationManager((SimulationState)SimulationStateDropDown.SelectedItem, parsed);
                    manager.StartSimulation();
                }
            }
        }
        private void SimulationStateDropDown_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (SimulationStateDropDown.SelectedItem)
            {
                case SimulationState.Default:
                    controller.PrintLabelInfo(9, 8, new TimeSpan(0, 2, 0), 1, 1, 1);
                    break;
                case SimulationState.TwentyGlassThreeChairs:
                    controller.PrintLabelInfo(3, 20, new TimeSpan(0, 2, 0), 1, 1, 1);
                    break;
                case SimulationState.TwentyChairsFiveGlass:
                    controller.PrintLabelInfo(20, 5, new TimeSpan(0, 2, 0), 1, 1, 1);
                    break;
                case SimulationState.PatronsSlowMode:
                    controller.PrintLabelInfo(9, 8, new TimeSpan(0, 2, 0), 1, 0.5, 1);
                    break;
                case SimulationState.WaitressBoostMode:
                    controller.PrintLabelInfo(9, 8, new TimeSpan(0, 2, 0), 1, 1, 2);
                    break;
                case SimulationState.BarOpenForFiveMins:
                    controller.PrintLabelInfo(9, 8, new TimeSpan(0, 5, 0), 1, 1, 1);
                    break;
                case SimulationState.CouplesNight:
                    controller.PrintLabelInfo(9, 8, new TimeSpan(0, 2, 0), 2, 1, 1);
                    break;
                case SimulationState.BusLoad:
                    controller.PrintLabelInfo(9, 8, new TimeSpan(0, 2, 0), 20, 1, 1);
                    break;
                case SimulationState.CrazyState:
                    controller.PrintLabelInfo(100, 100, new TimeSpan(0, 59, 59), 4, 0.2, 3);
                    break;
            }
        }
        private void CloseSimButton_Click(object sender, RoutedEventArgs e)
        {
            if (manager.StopSimulation())
            {
                controller.DisplaySettings();
            }
        }
    }
}