using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bartender
    {
        enum State { WaitingForPatron, WaitingForCleanGlass, PouringBeer, LeavingWork , LeftWork}
        public event Action<string> Log;
        State CurrentState { get; set; }
        double SimulationSpeed { get; set; }
        public Bartender(Establishment establishment)
        {
            CurrentState = State.WaitingForPatron;
            SimulationSpeed = establishment.SimulationSpeed;
        }
        public void Simulate(Establishment est, CancellationToken ct)
        {
            Task.Run(() =>
            {
                while(CurrentState != State.LeftWork && !ct.IsCancellationRequested)
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
        int SpeedModifier(int StartTime)
        {
            return (int)(StartTime / SimulationSpeed);
        }
        bool CheckBarQueue(Bar bar)
        {
            if (bar.BarQueue.Count > 0)
            {
                return true;
            }
            return false;
        }
        bool CheckBarShelf(Bar bar)
        {
            if (bar.Shelf.Count > 0)
            {
                return true;
            }
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
                Log("waiting for a patron");
            }
            while (!CheckBarQueue(establishment.Bar))
            {
                if(establishment.CurrentPatrons.Count <= 0 && !establishment.IsOpen)
                {
                    CurrentState = State.LeavingWork;
                    return;
                }
                Thread.Sleep(SpeedModifier(300));
            }
            if (CheckBarShelf(establishment.Bar))
            {
                CurrentState = State.PouringBeer;
            }
            else
            {
                CurrentState = State.WaitingForCleanGlass;
            }
            
        }
        void PouringBeer(Bar bar)
        {
            Glass glass;
            if(bar.Shelf.TryTake(out glass))
            {
                Log("fetching glass");
                Thread.Sleep(SpeedModifier(3000));
                Log($"pouring {bar.BarQueue.First().Name} a beer");
                Thread.Sleep(SpeedModifier(3000));
                bar.BarTop.Add(glass);
                CurrentState = State.WaitingForPatron;
            } 
            else
            {
                CurrentState = State.WaitingForCleanGlass;
            }
        }
        void WaitingForCleanGlass(Bar bar)
        {
            if (!CheckBarShelf(bar))
            {
                Log("is waiting at the shelf for a clean glass");
            }
            while (!CheckBarShelf(bar))
            {
                Thread.Sleep(SpeedModifier(300));
            }
            if (CheckBarShelf(bar))
            {
                CurrentState = State.PouringBeer;
            }
        }
        void LeavingWork()
        {
            CurrentState = State.LeftWork;
            Log("left the pub.");
        }
    }
}