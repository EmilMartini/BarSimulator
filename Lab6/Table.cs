using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Lab6
{
    public class Table
    {
        ConcurrentBag<Glass> GlassesOnTable { get; set; }
        ConcurrentBag<Chair> ChairsAroundTable { get; set; }
        ConcurrentQueue<Patron> ChairQueue { get; set; }
        public Table(Establishment establishment)
        {
            GlassesOnTable = new ConcurrentBag<Glass>();
            ChairsAroundTable = new ConcurrentBag<Chair>();
            ChairQueue = new ConcurrentQueue<Patron>();
            InitTable(establishment);
        }
        void InitTable(Establishment establishment)
        {
            for (int i = 0; i < establishment.MaxChairs; i++)
            {
                var chair = new Chair();
                ChairsAroundTable.Add(chair);
            }
        }
        public bool IsFirstInQueue(Patron patron)
        {
            if(ChairQueue.First() == patron)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Chair GetFirstChairFromCondition(bool availableCondition)
        {
            foreach (var chair in ChairsAroundTable)
            {
                if (chair.IsAvailable() == availableCondition)
                {
                    return chair;
                }
            }
            return null;
        }
        public int GetNumberOfAvailableChairs()
        {
            int numberOfChairs = 0;
            foreach (var chair in ChairsAroundTable)
            {
                if (chair.IsAvailable())
                {
                    numberOfChairs++;
                }
            }
            return numberOfChairs;
        }
        public bool TryDequeue(Patron patron)
        {
            Patron dequeuedPatron;
            if (ChairQueue.TryDequeue(out dequeuedPatron))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void EnqueuePatron(Patron patron)
        {
            ChairQueue.Enqueue(patron);
        }
    }
}
