using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Lab6
{
    public class Table
    {
        ConcurrentBag<Glass> glassesOnTable;
        ConcurrentBag<Chair> chairsAroundTable;
        ConcurrentQueue<Patron> chairQueue;
        public Table(Establishment establishment)
        {
            glassesOnTable = new ConcurrentBag<Glass>();
            chairsAroundTable = new ConcurrentBag<Chair>();
            chairQueue = new ConcurrentQueue<Patron>();
            InitTable(establishment);
        }
        void InitTable(Establishment establishment)
        {
            for (int i = 0; i < establishment.MaxChairs; i++)
            {
                var chair = new Chair();
                chairsAroundTable.Add(chair);
            }
        }
        public bool IsFirstInQueue(Patron patron)
        {
            if(chairQueue.First() == patron)
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
            foreach (var chair in chairsAroundTable)
            {
                if (chair.Available == availableCondition)
                {
                    return chair;
                }
            }
            return null;
        }
        public int GetNumberOfAvailableChairs()
        {
            return chairsAroundTable.Where(o => o.Available).Count();
        }
        public bool TryDequeue(Patron patron)
        {
            Patron dequeuedPatron;
            if (chairQueue.TryDequeue(out dequeuedPatron))
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
            chairQueue.Enqueue(patron);
        }
        public void PutGlassOnTable(Glass glass)
        {
            glassesOnTable.Add(glass);
        }
        public int NumberOfGlasses()
        {
            return glassesOnTable.Count();
        }
        public List<Glass> RemoveGlasses()
        {
            int glassesToRemove = glassesOnTable.Count;
            List<Glass> glassesToReturn = new List<Glass>();
            glassesToReturn.AddRange(glassesOnTable.Take<Glass>(glassesToRemove).ToList());
            glassesOnTable = new ConcurrentBag<Glass>(glassesOnTable.Except(glassesToReturn));
            return glassesToReturn;
        }
    }
}
