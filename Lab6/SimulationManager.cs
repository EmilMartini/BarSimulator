using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lab6
{
    class SimulationManager
    {
        public List<Patron> CurrentPatrons { get; private set; }
        Bouncer bouncer { get; set; }
        Bartender bartender { get; set; }
        Waiter waiter { get; set; }
        LogManager logManager { get; set; }
        public SimulationManager(SimulationState stateToRun)
        {
            Establishment establishment = GetEstablishment(stateToRun);
            MainWindow window = (MainWindow)App.Current.MainWindow;
            bouncer = new Bouncer(establishment, this);
            bartender = new Bartender(establishment);
            waiter = new Waiter(establishment.Table);
            CurrentPatrons = new List<Patron>();
            logManager = new LogManager(window, this);
            logManager.SubscribeToEvents(this);

            bouncer.Simulate(establishment, this);

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
        public Waiter GetWaiter()
        {
            return waiter;
        }
    }
}