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
        ConcurrentBag<Glass> Shelf { get; set; }
        ConcurrentBag<Glass> BarTop { get; set; }
        ConcurrentQueue<Patron> BarQueue { get; set; }
        public Bar(Establishment establishment)
        {
            Shelf = new ConcurrentBag<Glass>();
            BarTop = new ConcurrentBag<Glass>();
            BarQueue = new ConcurrentQueue<Patron>();
            FillShelf(establishment);
        }
        void FillShelf(Establishment establishment)
        {
            for (int i = 0; i < establishment.MaxGlasses; i++)
            {
                var glass = new Glass();
                glass.CurrentState = Glass.State.Clean;
                Shelf.Add(glass);
            }
        }
        public bool CheckBarShelfForGlass()
        {
            if (Shelf.Count > 0)
            {
                return true;
            }
            return false;
        }
        public int GetNumberOfGlassesInBarShelf()
        {
            return Shelf.Count();
        }
        public Glass GetGlassFromShelf()
        {
            Glass glass;
            Shelf.TryTake(out glass);
            return glass;
        }
        public void AddGlassToShelf(Glass glass)
        {
            Shelf.Add(glass);
        }
        public void AddGlassToBarTop(Glass glass)
        {
            BarTop.Add(glass);
        }
        public Glass TakeGlassFromBarTop()
        {
            Glass glass = BarTop.ElementAt(0);
            BarTop = new ConcurrentBag<Glass>(BarTop.Except(new[] { glass }));
            return glass;
        }
        public bool CheckBarTopForBeer()
        {
            if (BarTop.Count > 0)
            {
                return true;
            }
            return false;
        }
        public bool CheckIfFirstInBarQueue(Patron patron)
        {
            if (BarQueue.First() == patron)
            {
                return true;
            }
            return false;
        }
        public void AddPatronToBarQueue(Patron patron)
        {
            BarQueue.Enqueue(patron);
        }
        public void RemovePatronFromBarQueue(Patron patron)
        {
            BarQueue.TryDequeue(out patron);
        }
        public bool CheckBarQueue()
        {
            if (BarQueue.Count > 0)
            {
                return true;
            }
            return false;
        }
        public Patron GetFirstInBarQueue()
        {
            return BarQueue.First();
        }
    }
}
