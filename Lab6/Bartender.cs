using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bartender
    {
        public enum State { WaitingForPatron, WaitingForCleanGlass, PouringBeer, LeavingWork }
        public State CurrentState { get; set; }
        public delegate void BartenderAction();
        public event BartenderAction WaitingForPatronEvent, WaitingForCleanGlassEvent, PouringBeerEvent, LeavingWorkEvent, HasLeftWorkEvent;
        public Bartender(Establishment est)
        {

        }
        void Simulate(Bar bar)
        {
            Task.Run(() =>
            {
                do
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForPatron:
                            WaitingForPatron(bar);
                            break;
                        case State.WaitingForCleanGlass:
                            WaitingForCleanGlass(bar);
                            break;
                        case State.PouringBeer:
                            PouringBeer(bar);
                            break;
                        case State.LeavingWork:
                            LeavingWork();
                            break;
                        default:
                            break;
                    }


                } while (CurrentState != State.LeavingWork);
            });
        }
        bool CheckBarCue(Bar bar)
        {
            if (bar.BarCue != null)
            {
                return true;
            }
            return false;
        }
        bool CheckBarShelf(Bar bar)
        {
            if (bar.Shelf != null)
            {
                return true;
            }
            return false;
        }
        void WaitingForPatron(Bar bar)
        {
            if (!CheckBarCue(bar))
            {
                WaitingForPatronEvent();
            }
            while (!CheckBarCue(bar))
            {
                Thread.Sleep(3000);
            }
            if (CheckBarCue(bar))
            {
                CurrentState = State.WaitingForCleanGlass;
            }

        }
        void PouringBeer(Bar bar)
        {
            PouringBeerEvent();
            foreach (var glass in bar.Shelf)
            {
                glass.CurrentState = Glass.State.Full;
                bar.BarTop.Add(glass);
                bar.Shelf = new ConcurrentBag<Glass>(bar.Shelf.Except(new[] { glass }));
                return;
            }

        }
        void WaitingForCleanGlass(Bar bar)
        {
            if (!CheckBarShelf(bar))
            {
                WaitingForCleanGlassEvent();
            }
            while (!CheckBarShelf(bar))
            {
                Thread.Sleep(3000);
            }
            if (CheckBarShelf(bar))
            {
                CurrentState = State.PouringBeer;
            }
        }
        void LeavingWork()
        {
            LeavingWorkEvent();
            Thread.Sleep(5000);
            HasLeftWorkEvent();
        }
    }
}