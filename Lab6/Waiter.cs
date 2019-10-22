using System;
using System.Collections.Concurrent;

namespace Lab6
{
    public class Waiter
    {
        BlockingCollection<Glass> carryingGlasses;
        public Waiter(Table table)
        {
            carryingGlasses = new BlockingCollection<Glass>();
            Simulate(table);
        }

        private void Simulate(Table table)
        {

        }
    }
}