using System;
using System.Collections.Generic;
using System.Windows.Threading;

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
        DispatcherTimer dispatcherTimer { get; set; }
        DateTime TimeToClose { get; set; }
        public SimulationManager(SimulationState stateToRun)
        {
            dispatcherTimer = new DispatcherTimer();
            establishment = GetEstablishment(stateToRun);
            window = (MainWindow)App.Current.MainWindow;
            bouncer = new Bouncer(establishment, this);
            bartender = new Bartender(establishment);
            waitress = new Waitress(establishment.Table, establishment.Bar);
            CurrentPatrons = new List<Patron>();
            logManager = new LogManager(window, this);
            logManager.SubscribeToEvents(this);
            StartSimulation(this);
            InitUITimer();
        }
        private void StartSimulation(SimulationManager simulationManager)
        {
            bouncer.Simulate(simulationManager);
            bartender.Simulate(simulationManager.establishment);
            waitress.Simulate(establishment.Table, establishment.Bar);
            TimeToClose = DateTime.Now + establishment.TimeToClose;
        }
        private Establishment GetEstablishment(SimulationState state)
        {
            switch (state)
            {
                case SimulationState.Default:
                    return new Establishment(8, 9, new TimeSpan(0,1,20), 1);
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
            window.PatronsInPubLabel.Content = $"Patrons in bar {CurrentPatrons.Count} (Max patrons)";
            window.CleanGlassesLabel.Content = $"Number of clean glasses {establishment.Bar.Shelf.Count}";

            foreach (var chair in establishment.Table.ChairsAroundTable)
            {
                if (chair.Available)
                {
                    availableChairs++;
                }
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