using System;
using System.Windows.Threading;
using System.Linq;
using System.Threading;

namespace Lab6
{
    public class SimulationManager
    {
        Establishment establishment;
        Bouncer bouncer;
        Bartender bartender;
        Waitress waitress;
        LogManager logManager;
        MainWindow window;
        DispatcherTimer timer;
        DateTime timeToClose;
        CancellationTokenSource cts;
        CancellationToken ct;

        public SimulationManager(SimulationState stateToRun, double simulationSpeed)
        {
            timer = new DispatcherTimer();
            establishment = GetEstablishment(stateToRun, simulationSpeed);
            window = (MainWindow)App.Current.MainWindow; 
            bouncer = new Bouncer(establishment);
            bartender = new Bartender(establishment);
            waitress = new Waitress(establishment);
            logManager = new LogManager(window, this); 
        }
        public void StartSimulation()
        {
            cts = new CancellationTokenSource();
            ct = cts.Token;
            bouncer.Simulate(establishment, ct);
            bartender.Simulate(establishment, ct);
            waitress.Simulate(establishment,ct);
            timeToClose = DateTime.Now + establishment.TimeToClose;
            window.SimulationSpeedLabelInfo.Content = $"Simulation speed: {establishment.SimulationSpeed}";
            InitUITimer();
        }
        public bool StopSimulation()
        {
            cts.Cancel();
            if (ct.IsCancellationRequested)
            {
                timer.Stop();
                return true;
            }
            return false;
        }
        Establishment GetEstablishment(SimulationState state, double simulationSpeed)
        {
            switch (state)
            {
                case SimulationState.Default:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0), 1, simulationSpeed, 1, 1, 1, false);
                case SimulationState.TwentyGlassThreeChairs:
                    return new Establishment(20, 3, new TimeSpan(0, 2, 0), 1, simulationSpeed, 1, 1, 1, false);
                case SimulationState.TwentyChairsFiveGlass:
                    return new Establishment(5, 20, new TimeSpan(0, 2, 0), 1, simulationSpeed, 1, 1, 1, false);
                case SimulationState.PatronsSlowMode:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0), 1, simulationSpeed, 0.5, 1, 1, false);
                case SimulationState.WaitressBoostMode:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0), 1, simulationSpeed, 1, 2, 1, false);
                case SimulationState.BarOpenForFiveMinutes:
                    return new Establishment(8, 9, new TimeSpan(0, 5, 0), 1, simulationSpeed, 1, 1, 1, false);
                case SimulationState.CouplesNight:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0), 2, simulationSpeed, 1, 1, 1, false);
                case SimulationState.BusLoad:
                    return new Establishment(8, 9, new TimeSpan(0, 2, 0), 1, simulationSpeed, 1, 1, 0.5, true);
                case SimulationState.CrazyMode:
                    return new Establishment(100, 100, new TimeSpan(1, 0, 0), 5,simulationSpeed,0.5,2,1,true);
            }
            return null;
        }
        void InitUITimer()
        {
           timer.Tick += TimerTick;
           timer.Interval = new TimeSpan(0,0,0,0,100);
           timer.Start();
        }
        void TimerTick(object sender, EventArgs e)
        {
            window.PatronsInPubLabel.Content = $"Patrons in bar: {establishment.CurrentPatrons.Count} (Total: {establishment.TotalNumberOfPatrons()})";
            window.CleanGlassesLabel.Content = $"Number of clean glasses: {establishment.Bar.GetNumberOfGlassesInBarShelf()} (Max: {establishment.MaxGlasses})";
            window.FreeChairsLabel.Content = $"Number of available chairs: {establishment.Table.GetNumberOfAvailableChairs()} (Max: {establishment.MaxChairs})";
            window.TimeToCloseLabel.Content = "Time left until closing: " + $"{GetElapsedTime(DateTime.Now).ToString(@"mm\:ss")}";
            window.BarIsOpenLabel.Content = establishment.IsOpen ? @"Bar is: Open" : $"Bar is: Closed";
        }
        TimeSpan GetElapsedTime(DateTime now)
        {
            TimeSpan calculation = timeToClose - now;
            if (calculation.TotalSeconds <= 0)
            {
                establishment.IsOpen = false;
                return new TimeSpan(0,0,0);
            }
            else
            {
                return calculation;
            }
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