using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Lab6
{
    class LogManager
    {
        DateTime startTime;
        static MainWindow PresentationLayer;
        Bouncer bouncer;
        Bartender bartender;
        Waiter waiter;
        List<string> WaitressLogMessages = new List<string>();
        List<string> BouncerLogMessages = new List<string>();
        List<string> BartenderLogMessages = new List<string>();
        List<string> PatronLogMessages = new List<string>();

        public LogManager(MainWindow presentationLayer, SimulationManager sim)
        {
            PresentationLayer = presentationLayer;
            InitLogManager(PresentationLayer, sim);
            RefreshAll();
        }

        public void SubscribeToEvents(SimulationManager sim)
        {
            bouncer.Enter += OnBouncerWork;
            bartender.WaitingForPatronEvent += OnWaitingForPatron;
            bartender.WaitingForCleanGlassEvent += OnWaitingForCleanGlass;
            bartender.PouringBeerEvent += OnPouringBeer;
            bartender.LeavingWorkEvent += OnLeavingWork;
            bartender.HasLeftWorkEvent += OnHasLeftWorkEvent;
        }

        private void OnHasLeftWorkEvent()
        {
            throw new NotImplementedException();
        }

        void OnLeavingWork()
        {
            throw new NotImplementedException();
        }

        void OnPouringBeer()
        {
            throw new NotImplementedException();
        }

        void OnWaitingForCleanGlass()
        {
            throw new NotImplementedException();
        }

        void OnWaitingForPatron()
        {
            throw new NotImplementedException();
        }

        void OnBouncerWork(Patron patron)
        {
            DateTime timeStamp = DateTime.UtcNow;
            TimeSpan dif = timeStamp - startTime;
            
            PresentationLayer.Dispatcher.Invoke(()=> 
            {
                PatronLogMessages.Add($"{dif.Minutes}:{dif.Seconds} {patron.Name} entered the the bar.");
                PresentationLayer.PatronsListbox.Items.Refresh();
            });
        }

        private void RefreshAll()
        {
            PresentationLayer.PatronsListbox.Items.Refresh();
            PresentationLayer.BartenderListbox.Items.Refresh();
            PresentationLayer.WaitressListbox.Items.Refresh();
            PresentationLayer.PatronsListbox.Items.Refresh();
        }
        private void InitLogManager(MainWindow presentationLayer, SimulationManager sim)
        {
            startTime = DateTime.UtcNow;
            PresentationLayer = presentationLayer;
            bouncer = sim.GetBouncer();
            waiter = sim.GetWaiter();
            bartender = sim.GetBartender();
            presentationLayer.WaitressListbox.ItemsSource = WaitressLogMessages;
            presentationLayer.BouncerListbox.ItemsSource = BouncerLogMessages;
            presentationLayer.BartenderListbox.ItemsSource = BartenderLogMessages;
            presentationLayer.PatronsListbox.ItemsSource = PatronLogMessages;
        }
    }
}
