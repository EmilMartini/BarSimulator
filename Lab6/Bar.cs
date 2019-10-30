using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bar
    {
        ConcurrentBag<Glass> shelf;
        ConcurrentBag<Glass> barTop;
        ConcurrentQueue<Patron> barQueue;
        public Bar(Establishment establishment)
        {
            shelf = new ConcurrentBag<Glass>();
            barTop = new ConcurrentBag<Glass>();
            barQueue = new ConcurrentQueue<Patron>();
            FillShelf(establishment);
        }
        void FillShelf(Establishment establishment)
        {
            for (int i = 0; i < establishment.MaxGlasses; i++)
            {
                var glass = new Glass();
                glass.CurrentState = Glass.State.Clean;
                shelf.Add(glass);
            }
        }
        public bool CheckBarShelfForGlass()
        {
            if (shelf.Count > 0)
            {
                return true;
            }
            return false;
        }
        public int GetNumberOfGlassesInBarShelf()
        {
            return shelf.Count();
        }
        public Glass GetGlassFromShelf()
        {
            Glass glass;
            shelf.TryTake(out glass);
            return glass;
        }
        public void AddGlassToShelf(Glass glass)
        {
            shelf.Add(glass);
        }
        public void AddGlassToBarTop(Glass glass)
        {
            barTop.Add(glass);
        }
        public Glass TakeGlassFromBarTop()
        {
            Glass glass = barTop.ElementAt(0);
            barTop = new ConcurrentBag<Glass>(barTop.Except(new[] { glass }));
            return glass;
        }
        public bool CheckBarTopForBeer()
        {
            if (barTop.Count > 0)
            {
                return true;
            }
            return false;
        }
        public bool CheckIfFirstInBarQueue(Patron patron)
        {
            if (barQueue.First() == patron)
            {
                return true;
            }
            return false;
        }
        public void AddPatronToBarQueue(Patron patron)
        {
            barQueue.Enqueue(patron);
        }
        public void RemovePatronFromBarQueue(Patron patron)
        {
            barQueue.TryDequeue(out patron);
        }
        public bool CheckBarQueue()
        {
            if (barQueue.Count > 0)
            {
                return true;
            }
            return false;
        }
        public Patron GetFirstInBarQueue()
        {
            return barQueue.First();
        }
    }
}
