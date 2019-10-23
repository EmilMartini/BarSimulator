using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        Waitress waitress;
        List<string> WaitressLogMessages = new List<string>();
        List<string> BartenderLogMessages = new List<string>();
        List<string> PatronLogMessages = new List<string>();

        public LogManager(MainWindow presentationLayer, SimulationManager sim)
        {
            PresentationLayer = presentationLayer;
            InitLogManager(PresentationLayer, sim);
        }
        private void InitLogManager(MainWindow presentationLayer, SimulationManager sim)
        {
            startTime = DateTime.UtcNow;
            PresentationLayer = presentationLayer;
            bouncer = sim.GetBouncer();
            waitress = sim.GetWaitress();
            bartender = sim.GetBartender();
            presentationLayer.WaitressListbox.ItemsSource = WaitressLogMessages;
            presentationLayer.BartenderListbox.ItemsSource = BartenderLogMessages;
            presentationLayer.PatronsListbox.ItemsSource = PatronLogMessages;
        }

        public void SubscribeToEvents(SimulationManager sim)
        {
            bouncer.Enter += OnBouncerWork;
            Patron.WaitingForBeerEvent += OnWaitingForBeer;
            Patron.WaitingForChairEvent += OnWaitingForChair;
            Patron.WalkingToBarEvent += OnWalkingToBar;
            Patron.WalkingToChairEvent += OnWalkingToChair;
            Patron.DrinkingBeerEvent += OnDrinkingBeer;
            bartender.WaitingForPatronEvent += OnWaitingForPatron;
            bartender.WaitingForCleanGlassEvent += OnWaitingForCleanGlass;
            bartender.PouringBeerEvent += OnPouringBeer;
            bartender.LeavingWorkEvent += OnBartenderLeavingWork;
            waitress.LeavingWorkEvent += OnWaitressLeavingWork;
            waitress.PickingUpGlassEvent += OnPickingUpGlass;
            waitress.CleaningGlassEvent += OnCleaningGlass;
            waitress.ShelfingGlassEvent += OnShelfingGlass;
            waitress.WaitingForDirtyGlassEvent += OnWaitingForDirtyGlass;
            waitress.WalkingToSinkEvent += OnWalkingToSink;
            waitress.WalkingToTableEvent += OnWalkingToTable;
        }

        private void OnCleaningGlass()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, "Waitress", WaitressLogMessages, "is cleaning the glasses.");
        }
        private void OnWalkingToTable()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, "Waitress", WaitressLogMessages, "is walking to the table.");
        }
        private void OnWalkingToSink()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, "Waitress", WaitressLogMessages, "is walking to sink.");
        }
        private void OnWaitingForDirtyGlass()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, "Waitress", WaitressLogMessages, "is waiting for a dirty glass.");
        }
        private void OnShelfingGlass()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, "Waitress", WaitressLogMessages, "is shelfing glasses.");
        }
        private void OnPickingUpGlass()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, "Waitress", WaitressLogMessages, "is picking up glasses.");
        }
        private void OnWaitressLeavingWork()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, "Waitress", WaitressLogMessages, "is leaving work.");
        }
        private void OnBartenderLeavingWork()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.BartenderListbox, "Bartender", BartenderLogMessages, "is leaving work.");
        }
        private void OnPouringBeer()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.BartenderListbox, "Bartender", BartenderLogMessages, "is waiting puring beer.");
        }
        private void OnWaitingForCleanGlass()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.BartenderListbox, "Bartender", BartenderLogMessages, "is waiting for a clean glass.");
        }
        private void OnWaitingForPatron()
        {
            Print(GetTime, PresentationLayer, PresentationLayer.BartenderListbox, "Bartender", BartenderLogMessages, "is waiting for a patron.");
        }
        private void OnWalkingToChair(Patron p)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, p.Name, PatronLogMessages, "is walking to a chair.");
        }
        private void OnWalkingToBar(Patron p)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, p.Name, PatronLogMessages, "is walking to the bar.");
        }
        private void OnWaitingForChair(Patron p)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, p.Name, PatronLogMessages, "is waiting for a chair.");
        }
        private void OnWaitingForBeer(Patron p)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, p.Name, PatronLogMessages, "is waiting for a beer.");
        }
        private void OnDrinkingBeer(Patron p)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, p.Name, PatronLogMessages, "is drinking a beer.");
        }
        private void OnBouncerWork(Patron p)
        {
            Print(GetTime, PresentationLayer,PresentationLayer.PatronsListbox, p.Name, PatronLogMessages, "entered the pub.");
        }
        
        private void Print(Func<DateTime,TimeSpan>func, MainWindow PresentationLayer, System.Windows.Controls.ListBox listBox,string name,List<string> list,string message)
        {
            Task.Run(()=> 
            {
                var timeStamp = func(DateTime.UtcNow);
                PresentationLayer.Dispatcher.Invoke(() => 
                {
                    list.Insert(0,$"{timeStamp.Minutes}:{timeStamp.Seconds} {name} {message}");
                    listBox.Items.Refresh();
                });
            });
        }
        private TimeSpan GetTime(DateTime now)
        {
            TimeSpan dif;
            DateTime timeStamp = now;
            return dif = timeStamp - startTime;
        }
    }
}
