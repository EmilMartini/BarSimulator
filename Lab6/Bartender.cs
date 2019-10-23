using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bartender
    {
        public enum State { WaitingForPatron, WaitingForCleanGlass, PouringBeer, LeavingWork , LeftWork}
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
                while(CurrentState != State.LeftWork)
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForPatron:
                            WaitingForPatron(est);
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
                }
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

        void WaitingForPatron(Establishment establishment)
        {
            if (establishment.CurrentPatrons.Count <= 0 && !establishment.IsOpen)
            {
                CurrentState = State.LeavingWork;
                return;
            }
            if (!CheckBarQueue(establishment.Bar))
            {
                Log("is waiting for a patron");
            }
            while (!CheckBarQueue(establishment.Bar))
            {
                if(establishment.CurrentPatrons.Count <= 0 && !establishment.IsOpen)
                {
                    CurrentState = State.LeavingWork;
                    return;
                }
                Thread.Sleep(3000);
            }
            if (CheckBarQueue(establishment.Bar))// ska tas bort
            {
                if (CheckBarShelf(establishment.Bar))
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
            CurrentState = State.LeftWork;
            Log("has left the pub.");
        }
    }
}