using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Lab6
{
    public class Establishment
    {
        public int MaxGlasses { get; private set; }
        public int MaxChairs { get; private set; }
        public double SimulationSpeed { get; private set; }
        public double WaitressSpeed { get; set; }
        public double PatronSpeed { get; set; }
        public int PatronsPerEntry { get; set; }
        public bool IsOpen { get; set; }
        public TimeSpan TimeToClose { get; set; }
        public Table Table { get; set; }
        public Bar Bar { get; set; }
        public List<Patron> CurrentPatrons { get; private set; }
        public Establishment(int maxGlasses, int maxChairs, TimeSpan timeToClose,int patronsToEntry, double simulationSpeed, double patronSpeed, int waitressSpeed)
        {
            MaxGlasses = maxGlasses;
            MaxChairs = maxChairs;
            TimeToClose = timeToClose;
            SimulationSpeed = simulationSpeed;
            WaitressSpeed = waitressSpeed;
            PatronSpeed = patronSpeed;
            PatronsPerEntry = patronsToEntry;
            Table = new Table(this);
            IsOpen = true; 
            Bar = new Bar(this);
            Table.InitTable();
            CurrentPatrons = new List<Patron>();
        }
    }
}
