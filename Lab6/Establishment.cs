using System;
using System.Collections.Concurrent;

namespace Lab6
{
    class Establishment
    {
        public int MaxGlasses { get; private set; }
        public int MaxChairs { get; private set; }
        public int MaxPatrons { get; private set; }
        public int SimulationSpeed { get; private set; }
        public bool IsOpen { get; set; }

        public Establishment(int maxGlasses, int maxChairs, int maxPatrons, int simulationSpeed)
        {
            MaxGlasses = maxGlasses;
            MaxChairs = maxChairs;
            MaxPatrons = maxPatrons;
            SimulationSpeed = simulationSpeed;
            IsOpen = true;
        }
    }
}
