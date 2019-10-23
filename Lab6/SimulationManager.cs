using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Linq;

namespace Lab6
{
    public class SimulationManager
    {
        Establishment establishment { get; set; }
        Bouncer bouncer { get; set; }
        Bartender bartender { get; set; }
        Waitress waitress { get; set; }
        LogManager logManager { get; set; }
        MainWindow window { get; set; }
        DispatcherTimer dispatcherTimer { get; set; }
        DateTime timeToClose { get; set; }
        public SimulationManager(SimulationState stateToRun, double simulationSpeed)
        {
            dispatcherTimer = new DispatcherTimer();
            establishment = GetEstablishment(stateToRun, simulationSpeed);
            window = (MainWindow)App.Current.MainWindow;
            bouncer = new Bouncer();
            bartender = new Bartender(establishment);
            waitress = new Waitress(establishment);
            logManager = new LogManager(window, this);
        }
        public void StartSimulation()
        {
            bouncer.Simulate(establishment);
            bartender.Simulate(establishment);
            waitress.Simulate(establishment);
            timeToClose = DateTime.Now + establishment.TimeToClose;
            window.SimulationSpeedLabelInfo.Content = $"Simulation speed: {establishment.SimulationSpeed}";
            InitUITimer();
        }
        Establishment GetEstablishment(SimulationState state, double simulationSpeed)
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
                    return new Establishment(8, 9, new TimeSpan(0, 5, 0), 1, simulationSpeed, 1, 1);
                case SimulationState.CouplesNight:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0), 2, simulationSpeed, 1, 1);
                case SimulationState.BusLoad: // någon fancy lösning
                    break;
                case SimulationState.CrazyState:
                    return new Establishment(100, 100, new TimeSpan(1, 0, 0), 2, 5, 0.2, 2);
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
        void InitUITimer()
        {
            dispatcherTimer.Tick += TimerTick;
            dispatcherTimer.Interval = new TimeSpan(0,0,0,0,25);
            dispatcherTimer.Start();
        }
        void TimerTick(object sender, EventArgs e)
        {
            int availableChairs = 0;
            window.PatronsInPubLabel.Content = $"Patrons in bar: {establishment.CurrentPatrons.Count}";
            window.CleanGlassesLabel.Content = $"Number of clean glasses: {establishment.Bar.Shelf.Count}";

            foreach (var chair in establishment.Table.ChairsAroundTable)
            {
                if (chair.Available)
                    availableChairs++;
            }
            window.FreeChairsLabel.Content = $"Number of available chairs: {availableChairs}";
            window.TimeToCloseLabel.Content = $"Time left until closing: {GetElapsedTime(DateTime.Now)}";
        }
        int GetElapsedTime(DateTime now)
        {
            var calculation = (int)(timeToClose - now).TotalSeconds;
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