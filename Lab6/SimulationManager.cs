using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Linq;

namespace Lab6
{
    public class SimulationManager
    {
        public List<Patron> CurrentPatrons { get; private set; }
        public Establishment establishment { get; private set; }
        public Bouncer bouncer { get; private set; }
        public Bartender bartender { get; private set; }
        public Waitress waitress { get; private set; }
        public LogManager logManager { get; private set; }
        MainWindow window { get; set; }
        DispatcherTimer dispatcherTimer { get; set; }
        DateTime TimeToClose { get; set; }
        public SimulationManager(SimulationState stateToRun, double simulationSpeed)
        {
            dispatcherTimer = new DispatcherTimer();
            establishment = GetEstablishment(stateToRun, simulationSpeed);
            window = (MainWindow)App.Current.MainWindow;
            bouncer = new Bouncer();
            bartender = new Bartender(establishment);
            waitress = new Waitress(establishment);
            
            logManager = new LogManager(window, this);
            logManager.SubscribeToEvents(this);
            StartSimulation(establishment);
            InitUITimer();
        }
        private void StartSimulation(Establishment establishment)
        {
            bouncer.Simulate(establishment);
            bartender.Simulate(establishment);
            waitress.Simulate(establishment);
            TimeToClose = DateTime.Now + establishment.TimeToClose;
        }
        private Establishment GetEstablishment(SimulationState state, double simulationSpeed)
        {
            switch (state)
            {
                case SimulationState.Default:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0),1, simulationSpeed, 1,1);
                case SimulationState.TwentyGlassThreeChairs:
                    return new Establishment(20, 3, new TimeSpan(0, 2, 0),1, simulationSpeed, 1,1);
                case SimulationState.TwentyChairsThreeGlass:
                    return new Establishment(3, 20, new TimeSpan(0, 2, 0),1, simulationSpeed, 1,1);
                case SimulationState.PatronsSlowMode:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0),1, simulationSpeed, 0.5, 1);
                case SimulationState.WaitressBoostMode:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0),1, simulationSpeed, 1, 2);
                case SimulationState.BarOpenForFiveMins:
                    break;
                case SimulationState.CouplesNight:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0), 2, simulationSpeed, 1, 1);
                    break;
                case SimulationState.BusLoad:
                    break;
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
        private void InitUITimer()
        {
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0,0,0,0,50);
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            int availableChairs = 0;
            window.PatronsInPubLabel.Content = $"Patrons in bar {establishment.CurrentPatrons.Count} (Max patrons)";
            window.CleanGlassesLabel.Content = $"Number of clean glasses {establishment.Bar.Shelf.Count}";

            foreach (var chair in establishment.Table.ChairsAroundTable)
            {
                if (chair.Available)
                    availableChairs++;
            }
            window.FreeChairsLabel.Content = $"Number of available chairs {availableChairs}";
            window.TimeToCloseLabel.Content = $"Time left until closing {GetElapsedTime(DateTime.Now)}";
        }

        private int GetElapsedTime(DateTime now)
        {
            var calculation = (int)(TimeToClose - now).TotalSeconds;

            if (calculation <= 0)
            {
                establishment.IsOpen = false;
                return 0;
            }
            else
            {
                return calculation;
            }
        }
    }
}