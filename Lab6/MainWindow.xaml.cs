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
        SimulationManager manager;
        DisplayController controller;
        double parsedSimulationSpeed;
        public MainWindow()  
        {
            InitializeComponent();
            controller = new DisplayController(this);
            controller.Start();
        }
        void StartSimButton_Click(object sender, RoutedEventArgs e)
        {
            controller.DisplaySimulation();
            manager = new SimulationManager((SimulationState)SimulationStateDropDown.SelectedItem, parsedSimulationSpeed);
            manager.StartSimulation();
        }
        void CloseSimButton_Click(object sender, RoutedEventArgs e)
        {
            if (manager.StopSimulation())
            {
                controller.DisplaySettings();
            }
        }
        void SpeedModifierTextbox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (double.TryParse(speedModifierTextbox.Text.Replace('.', ','), out parsedSimulationSpeed) && parsedSimulationSpeed > 0 && parsedSimulationSpeed < 6)
            {
                StartSimButton.IsEnabled = true;
            }
            else
            {
                StartSimButton.IsEnabled = false;
            }
        }
    }
}