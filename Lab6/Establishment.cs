﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Lab6
{
    public class Establishment
    {
        public int MaxGlasses { get; private set; }
        public int MaxChairs { get; private set; }
        public double SimulationSpeed { get; private set; }
        public double WaitressSpeed { get; private set; }
        public double PatronSpeed { get; private set; }
        public int PatronsPerEntry { get; private set; }
        public bool IsOpen { get; set; }
        public TimeSpan TimeToClose { get; private set; }
        public Table Table { get; private set; }
        public Bar Bar { get; private set; }
        public List<Patron> CurrentPatrons { get; private set; }
        public int TotalPatrons { get; set; }

        public Establishment(int maxGlasses, int maxChairs, TimeSpan timeToClose,int patronsPerEntry, double simulationSpeed, double patronSpeed, int waitressSpeed)
        {
            MaxGlasses = maxGlasses;
            MaxChairs = maxChairs;
            TimeToClose = timeToClose;
            SimulationSpeed = simulationSpeed;
            WaitressSpeed = waitressSpeed;
            PatronSpeed = patronSpeed;
            PatronsPerEntry = patronsPerEntry;
            Table = new Table(this);
            IsOpen = true; 
            Bar = new Bar(this);
            CurrentPatrons = new List<Patron>();
        }
    }
}
