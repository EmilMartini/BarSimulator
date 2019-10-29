using System;
using System.Collections.Concurrent;

namespace Lab6
{
    public class Table
    {
        public ConcurrentBag<Glass> GlassesOnTable { get; set; }
        public BlockingCollection<Chair> ChairsAroundTable { get; private set; }
        public ConcurrentQueue<Patron> ChairQueue { get; set; }
        public Table(Establishment est)
        {
            GlassesOnTable = new ConcurrentBag<Glass>();
            ChairsAroundTable = new BlockingCollection<Chair>(est.MaxChairs);
            ChairQueue = new ConcurrentQueue<Patron>();
            InitTable();
        }
        void InitTable()
        {
            for (int i = 0; i < ChairsAroundTable.BoundedCapacity; i++)
            {
                var chair = new Chair();
                chair.Available = true;
                ChairsAroundTable.Add(chair);
            }
        }
    }
}
