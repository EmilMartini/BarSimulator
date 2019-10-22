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
        Patron patron;
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
            Patron.WaitingForBeerEvent += OnWaitingForBeer;
            Patron.WaitingForChairEvent += OnWaitingForChair;
            Patron.WalkingToBarEvent += OnWalkingToBar;
            Patron.WalkingToChairEvent += OnWalkingToChair;
            bartender.WaitingForPatronEvent += OnWaitingForPatron;
            bartender.WaitingForCleanGlassEvent += OnWaitingForCleanGlass;
            bartender.PouringBeerEvent += OnPouringBeer;
            bartender.LeavingWorkEvent += OnLeavingWork;
        }

        private void OnLeavingWork()
        {
            var dif = GetTime(DateTime.Now);
            PresentationLayer.Dispatcher.Invoke(() =>
            {
                BartenderLogMessages.Add($"{dif.Minutes}:{dif.Seconds} Bartender leaving establishment.");
                PresentationLayer.BartenderListbox.Items.Refresh();
            });
        }

        private void OnPouringBeer()
        {
            var dif = GetTime(DateTime.Now);
            PresentationLayer.Dispatcher.Invoke(() =>
            {
                BartenderLogMessages.Add($"{dif.Minutes}:{dif.Seconds} Bartender pouring beer.");
                PresentationLayer.BartenderListbox.Items.Refresh();
            });
        }

        private void OnWaitingForCleanGlass()
        {
            var dif = GetTime(DateTime.Now);
            PresentationLayer.Dispatcher.Invoke(() =>
            {
                BartenderLogMessages.Add($"{dif.Minutes}:{dif.Seconds} Waiting for clean glass.");
                PresentationLayer.BartenderListbox.Items.Refresh();
            });
        }

        private void OnWaitingForPatron()
        {
            var dif = GetTime(DateTime.Now);
            PresentationLayer.Dispatcher.Invoke(() =>
            {
                BartenderLogMessages.Add($"{dif.Minutes}:{dif.Seconds} Bartender waits for a patron.");
                PresentationLayer.BartenderListbox.Items.Refresh();
            });
        }

        private void OnWalkingToChair(Patron p)
        {
            var dif = GetTime(DateTime.Now);
            PresentationLayer.Dispatcher.Invoke(() =>
            {
                PatronLogMessages.Add($"{dif.Minutes}:{dif.Seconds} {p.Name} walks to a chair.");
                PresentationLayer.PatronsListbox.Items.Refresh();
            });
        }

        private void OnWalkingToBar(Patron p)
        {
            var dif = GetTime(DateTime.Now);
            PresentationLayer.Dispatcher.Invoke(() =>
            {
                PatronLogMessages.Add($"{dif.Minutes}:{dif.Seconds} {p.Name} walks to the bar.");
                PresentationLayer.PatronsListbox.Items.Refresh();
            });
        }

        private void OnWaitingForChair(Patron p)
        {
            var dif = GetTime(DateTime.Now);
            PresentationLayer.Dispatcher.Invoke(() =>
            {
                PatronLogMessages.Add($"{dif.Minutes}:{dif.Seconds} {p.Name} is waiting for a chair.");
                PresentationLayer.PatronsListbox.Items.Refresh();
            });
        }

        private void OnWaitingForBeer(Patron p)
        {
            var dif = GetTime(DateTime.Now);
            PresentationLayer.Dispatcher.Invoke(() =>
            {
                PatronLogMessages.Add($"{dif.Minutes}:{dif.Seconds} {p.Name} is waiting for a beer.");
                PresentationLayer.PatronsListbox.Items.Refresh();
            });
        }

        void OnBouncerWork(Patron p)
        {
            var dif = GetTime(DateTime.Now);  
            PresentationLayer.Dispatcher.Invoke(()=> 
            {
                PatronLogMessages.Add($"{dif.Minutes}:{dif.Seconds} {p.Name} entered the the bar.");
                PresentationLayer.PatronsListbox.Items.Refresh();
            });
        }

        private TimeSpan GetTime(DateTime now)
        {
            TimeSpan dif;
            DateTime timeStamp = DateTime.UtcNow;
            return dif = timeStamp - startTime;
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
