using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Lab6
{
    public class Table
    {
        ConcurrentBag<Glass> GlassesOnTable;
        ConcurrentBag<Chair> ChairsAroundTable;
        ConcurrentQueue<Patron> ChairQueue;
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
                if (chair.Available)
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
                if (chair.Available)
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
        public void PutGlassOnTable(Glass glass)
        {
            GlassesOnTable.Add(glass);
        }

        public int NumberOfGlasses()
        {
            return GlassesOnTable.Count();
        }


        //KOLLA IGENOM
        public List<Glass> RemoveGlasses()
        {
            int glassesToRemove = GlassesOnTable.Count;
            List<Glass> glassesToReturn = new List<Glass>();
            glassesToReturn.AddRange(GlassesOnTable.Take<Glass>(glassesToRemove).ToList());
            GlassesOnTable = new ConcurrentBag<Glass>(GlassesOnTable.Except(glassesToReturn));
            return glassesToReturn;
        }
    }
}
