using System;
using System.Collections.Generic;

namespace Lab6
{
    public class LogManager
    {
        DateTime startTime;
        static MainWindow PresentationLayer;
        Bouncer bouncer;
        Bartender bartender;
        Waitress waitress;
        List<string> WaitressLogMessages;
        List<string> BartenderLogMessages;
        List<string> PatronLogMessages;
        public LogManager(MainWindow presentationLayer, SimulationManager sim)
        {
            PresentationLayer = presentationLayer;
            InitLogManager(PresentationLayer, sim);
        }
        void InitLogManager(MainWindow presentationLayer, SimulationManager sim)
        {
            startTime = DateTime.UtcNow;
            PresentationLayer = presentationLayer;
            bouncer = sim.GetBouncer();
            waitress = sim.GetWaitress();
            bartender = sim.GetBartender();
            WaitressLogMessages = new List<string>();
            BartenderLogMessages = new List<string>();
            PatronLogMessages = new List<string>();
            presentationLayer.WaitressListbox.ItemsSource = WaitressLogMessages;
            presentationLayer.BartenderListbox.ItemsSource = BartenderLogMessages;
            presentationLayer.PatronsListbox.ItemsSource = PatronLogMessages;
            SubscribeToEvents();
        }
        void SubscribeToEvents()
        {
            bouncer.Log += OnBouncerLogEvent;
            Patron.Log += OnPatronLogEvent;
            bartender.Log += OnBartenderLogEvent;
            waitress.Log += OnWaitressLogEvent;
        }
        void OnWaitressLogEvent(string s)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.WaitressListbox, WaitressLogMessages, s);
        }
        void OnBouncerLogEvent(string s)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, PatronLogMessages, s);
        }
        void OnBartenderLogEvent(string s)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.BartenderListbox, BartenderLogMessages, s);
        }
        void OnPatronLogEvent(string message)
        {
            Print(GetTime, PresentationLayer, PresentationLayer.PatronsListbox, PatronLogMessages, message);
        }   
        void Print(Func<DateTime,TimeSpan>func, MainWindow PresentationLayer, System.Windows.Controls.ListBox listBox,List<string> list,string message)
        {
            var timeStamp = func(DateTime.UtcNow);
            PresentationLayer.Dispatcher.Invoke(() => 
            {
                list.Insert(0, $"{timeStamp.ToString(@"mm\:ss")} {message}");
                listBox.Items.Refresh();
            });
        }
        TimeSpan GetTime(DateTime now)
        {
            return now - startTime;
        }
    }
}
