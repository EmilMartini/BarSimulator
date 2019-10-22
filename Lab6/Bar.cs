using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    class Bar
    {
        public ConcurrentBag<Glass> Shelf { get; private set; }
        public ConcurrentBag<Glass> BarTop { get; set; }
        public Bar(Establishment est)
        {
            Shelf = new ConcurrentBag<Glass>();
            BarTop = new ConcurrentBag<Glass>();
        }
    }
}
