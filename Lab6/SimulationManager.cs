using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lab6
{
    public class SimulationManager
    {
        public List<Patron> CurrentPatrons { get; private set; }
        public Bouncer bouncer { get; private set; }
        public Bartender bartender { get; private set; }
        public Waitress waitress { get; private set; }
        public LogManager logManager { get; private set; }
        public Establishment establishment { get; private set; }
        MainWindow window { get; set; }
        public SimulationManager(SimulationState stateToRun)
        {
            establishment = GetEstablishment(stateToRun);
            window = (MainWindow)App.Current.MainWindow;
            bouncer = new Bouncer(establishment, this);
            bartender = new Bartender(establishment);
            waitress = new Waitress(establishment.Table, establishment.Bar);
            CurrentPatrons = new List<Patron>();
            logManager = new LogManager(window, this);
            logManager.SubscribeToEvents(this);
            StartSimulation(this);
        }
        private void StartSimulation(SimulationManager simulationManager)
        {
            bouncer.Simulate(simulationManager);
            bartender.Simulate(simulationManager.establishment);
            waitress.Simulate(establishment.Table, establishment.Bar);
        }
        private Establishment GetEstablishment(SimulationState state)
        {
            switch (state)
            {
                case SimulationState.Default:
                    return new Establishment(8, 9, 120, 1);
            }
            return null;
        }
        public Bartender GetBartender()
        {
            return bartender;
        }
        public Bouncer GetBouncer()
        {
            return bouncer;
        }
        public Waitress GetWaitress()
        {
            return waitress;
        }
    }
}