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
        public delegate void BartenderEvent();
        public event BartenderEvent WaitingForPatronEvent, WaitingForCleanGlassEvent, PouringBeerEvent, LeavingWorkEvent;

        public Bartender(Establishment est)
        {
            CurrentState = State.WaitingForPatron;
        }
        public void Simulate(Establishment est)
        {
            Task.Run(() =>
            {
                do
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForPatron:
                            WaitingForPatron(est.Bar);
                            break;
                        case State.WaitingForCleanGlass:
                            WaitingForCleanGlass(est.Bar);
                            break;
                        case State.PouringBeer:
                            PouringBeer(est.Bar);
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
        bool CheckBarQueue(Bar bar)
        {
            if (bar.BarQueue.Count > 0)
                return true;
            
            return false;
        }
        bool CheckBarShelf(Bar bar)
        {
            if (bar.Shelf.Count > 0)
                return true;
            
            return false;
        }

        void WaitingForPatron(Bar bar)
        {
            if (!CheckBarQueue(bar))
            {
                WaitingForPatronEvent();
            }
            
            while (!CheckBarQueue(bar))
            {
                Thread.Sleep(300);
            }
            
            if (CheckBarQueue(bar))
            {
                if (CheckBarShelf(bar))
                {
                    CurrentState = State.PouringBeer;
                } else
                {
                    CurrentState = State.WaitingForCleanGlass;
                }
            }
        }
        void PouringBeer(Bar bar)
        {
            Glass glass;
            if(bar.Shelf.TryTake(out glass))
            {
                PouringBeerEvent();
                bar.BarTop.Add(glass);
                CurrentState = State.WaitingForPatron;
            } else
            {
                CurrentState = State.WaitingForCleanGlass;
            }
            Thread.Sleep(3000);
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