using System;
using System.Collections.Generic;

namespace Lab6
{
    public class LogManager
    {
        DateTime startTime;
        static MainWindow view;
        Bouncer bouncer;
        Bartender bartender;
        Waitress waitress;
        List<string> waitressLogMessages;
        List<string> bartenderLogMessages;
        List<string> patronLogMessages;
        public LogManager(MainWindow view, SimulationManager sim)
        {
            InitLogManager(view, sim);
        }
        void InitLogManager(MainWindow view, SimulationManager simulationManager)
        {
            startTime = DateTime.UtcNow;
            LogManager.view = view;
            bouncer = simulationManager.GetBouncer();
            waitress = simulationManager.GetWaitress();
            bartender = simulationManager.GetBartender();
            waitressLogMessages = new List<string>();
            bartenderLogMessages = new List<string>();
            patronLogMessages = new List<string>();
            view.WaitressListbox.ItemsSource = waitressLogMessages;
            view.BartenderListbox.ItemsSource = bartenderLogMessages;
            view.PatronsListbox.ItemsSource = patronLogMessages;
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
            Print(GetTime, view, view.WaitressListbox, waitressLogMessages, s);
        }
        void OnBouncerLogEvent(string s)
        {
            Print(GetTime, view, view.PatronsListbox, patronLogMessages, s);
        }
        void OnBartenderLogEvent(string s)
        {
            Print(GetTime, view, view.BartenderListbox, bartenderLogMessages, s);
        }
        void OnPatronLogEvent(string message)
        {
            Print(GetTime, view, view.PatronsListbox, patronLogMessages, message);
        }   
        void Print(Func<DateTime,TimeSpan>func, MainWindow view, System.Windows.Controls.ListBox listBox,List<string> list,string message)
        {
            var timeStamp = func(DateTime.UtcNow);
            view.Dispatcher.Invoke(() => 
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
