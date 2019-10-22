using System;

namespace Lab6
{
    class LogManager
    {
        DateTime startTime;
        MainWindow PresentationLayer;
        public LogManager(MainWindow presentationLayer, SimulationManager sim)
        {
            startTime = DateTime.Now;
            PresentationLayer = presentationLayer;
            presentationLayer.PatronsListbox.ItemsSource = sim.GetBou
        }


    }
}
