using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Lab6
{
    public class LogManager
    {
        DateTime startTime;
        static MainWindow PresentationLayer;
        SimulationManager simulationManager;
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
            simulationManager = sim;
            bouncer = sim.GetBouncer();
            waitress = sim.GetWaitress();
            bartender = sim.GetBartender();
            presentationLayer.WaitressListbox.ItemsSource = WaitressLogMessages;
            presentationLayer.BartenderListbox.ItemsSource = BartenderLogMessages;
            presentationLayer.PatronsListbox.ItemsSource = PatronLogMessages;
        }

        public void SubscribeToEvents(SimulationManager sim)
        {
            bouncer.Log += OnBouncerLogEvent;
            Patron.Log += OnPatronLogEvent;
            Patron.UpdatePatronCount += OnPatronUpdateCount;
            bartender.Log += OnBartenderLogEvent;
            waitress.Log += OnWaitressLogEvent;
        }

        private void OnPatronUpdateCount(Patron p, string s)
        {
            PresentationLayer.Dispatcher.Invoke(()=> 
            {
                PresentationLayer.PatronsInPubLabel.Content = $"Patrons in bar {simulationManager.CurrentPatrons.Count + 1} (Max patrons)";
            });
        }

        private void OnWaitressLogEvent(string s)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, "Waitress", WaitressLogMessages, s);
        }

        private void OnBouncerLogEvent(Patron p, string s)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, p.Name, PatronLogMessages, s);
        }

        private void OnBartenderLogEvent(string s)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.BartenderListbox, "Bartender", BartenderLogMessages, s);
        }

        private void OnPatronLogEvent(Patron p, string message)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, p.Name, PatronLogMessages, message);
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
