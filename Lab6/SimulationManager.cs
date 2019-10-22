using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lab6
{
    class SimulationManager
    {
        Bouncer bouncer;
        Bartender bartender;
        Waiter waiter;
        public SimulationManager(SimulationState stateToRun)
        {
            Establishment establishment = GetEstablishment(stateToRun);
            MainWindow window = new MainWindow();
            LogManager logManager = new LogManager(window, this);
            bouncer = new Bouncer(establishment);
            bartender = new Bartender(establishment);
            waiter = new Waiter(establishment.Table);
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