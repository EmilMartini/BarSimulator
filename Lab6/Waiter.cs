using System;
using System.Collections.Concurrent;

namespace Lab6
{
    public class Waiter
    {
        BlockingCollection<Glass> carryingGlasses;
        public Waiter(Establishment est)
        {
            carryingGlasses = new BlockingCollection<Glass>(est.MaxGlasses);
            Simulate(est);
        }

        private void Simulate(Establishment est)
        {
        }
    }
}