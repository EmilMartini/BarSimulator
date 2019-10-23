using System;
using System.Collections.Concurrent;

namespace Lab6
{
    public class Waitress
    {
        // public static event PatronEvent;
        public enum State { WaitingForDirtyGlass, PickingUpGlass, WalkingToSink, WalkingToTable }
        BlockingCollection<Glass> carryingGlasses;
        public Waitress(Table table)
        {
            carryingGlasses = new BlockingCollection<Glass>();
            Simulate(table);
        }

        private void Simulate(Table table)
        {

        }
    }
}