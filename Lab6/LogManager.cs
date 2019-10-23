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
            bartender.Log += OnBartenderLogEvent;
            waitress.Log += OnWaitressLogEvent;
        }
        private void OnWaitressLogEvent(string s)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, WaitressLogMessages, s);
        }
        private void OnBouncerLogEvent(string s)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, PatronLogMessages, s);
        }
        private void OnBartenderLogEvent(string s)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.BartenderListbox, BartenderLogMessages, s);
        }
        private void OnPatronLogEvent(string message)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, PatronLogMessages, message);
        }   
        private void Print(Func<DateTime,TimeSpan>func, MainWindow PresentationLayer, System.Windows.Controls.ListBox listBox,List<string> list,string message)
        {
            var timeStamp = func(DateTime.UtcNow);
            PresentationLayer.Dispatcher.Invoke(() => 
            {
                list.Insert(0,$"{timeStamp.Minutes}:{timeStamp.Seconds} {message}");
                listBox.Items.Refresh();
            });
        }
        private TimeSpan GetTime(DateTime now)
        {
            return now - startTime;
        }
    }
}
