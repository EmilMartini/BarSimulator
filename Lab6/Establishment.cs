using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Lab6
{
    public class Establishment
    {

        int totalPatrons;
        public int MaxGlasses { get; private set; }
        public int MaxChairs { get; private set; }
        public double SimulationSpeed { get; private set; }
        public double WaitressSpeed { get; private set; }
        public double PatronSpeed { get; private set; }
        public double BouncerSpeed { get; private set; }
        public int PatronsPerEntry { get; private set; }
        public bool IsOpen { get; set; }
        public TimeSpan TimeToClose { get; private set; }
        public Table Table { get; private set; }
        public Bar Bar { get; private set; }
        public List<Patron> CurrentPatrons { get; private set; }
        public bool IsBusloadState { get; private set; }

        public Establishment(int maxGlasses, int maxChairs, TimeSpan timeToClose,int patronsPerEntry, double simulationSpeed, double patronSpeed, double waitressSpeed, double bouncerSpeed, bool isBusload)
        {
            MaxGlasses = maxGlasses;
            MaxChairs = maxChairs;
            TimeToClose = CalculateTimeToClose(timeToClose, simulationSpeed);
            SimulationSpeed = simulationSpeed;
            WaitressSpeed = waitressSpeed;
            BouncerSpeed = bouncerSpeed;
            PatronSpeed = patronSpeed;
            PatronsPerEntry = patronsPerEntry;
            IsBusloadState = isBusload;
            Table = new Table(this);
            IsOpen = true; 
            Bar = new Bar(this);
            CurrentPatrons = new List<Patron>();
        }

        TimeSpan CalculateTimeToClose(TimeSpan input, double simulationSpeed)
        {
            return TimeSpan.FromSeconds(input.TotalSeconds / simulationSpeed);
        }

        public void SetChairAvailable(Patron patron)
        {
            var chair = Table.GetFirstChairFromCondition(false);
            chair.Available = true;
        }

        public void AddPatron(Patron patron)
        {
            CurrentPatrons.Insert(0, patron);
            totalPatrons++;
        }
        public int TotalNumberOfPatrons()
        {
            return totalPatrons;
        }
    }
}
