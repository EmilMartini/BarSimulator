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
        public delegate void BartenderEvent(string s);
        public event BartenderEvent Log;

        public Bartender(Establishment est) // behöver ine ta in Establishment för att sätta currentstate
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
                Log("is waiting for a patron");
            }
            while (!CheckBarQueue(bar))
            {
                Thread.Sleep(3000);
            }
            if (CheckBarQueue(bar))// ska tas bort
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
                Log("is pouring beer");
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
                Log("is waiting for a clean glass");
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
            Log("is leaving work");
            Thread.Sleep(5000);
            
        }
    }
}