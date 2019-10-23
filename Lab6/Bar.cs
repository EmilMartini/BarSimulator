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
        public ConcurrentBag<Glass> Shelf { get; set; }
        public ConcurrentBag<Glass> BarTop { get; set; }
        public ConcurrentQueue<Patron> BarQueue { get; set; }
        public Bar(Establishment establishment)
        {
            Shelf = new ConcurrentBag<Glass>();
            BarTop = new ConcurrentBag<Glass>();
            BarQueue = new ConcurrentQueue<Patron>();
            FillShelf(establishment);
        }
        private void FillShelf(Establishment establishment)
        {
            for (int i = 0; i < establishment.MaxGlasses; i++)
            {
                var glass = new Glass();
                glass.CurrentState = Glass.State.Clean;
                Shelf.Add(glass);
            }
        }
    }
}
