using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bartender
    {
        enum State { WaitingForPatron, WaitingForCleanGlass, PouringBeer, LeavingWork , LeftWork}
        public event Action<string> Log;
        State currentState;
        double SsmulationSpeed;
        public Bartender(Establishment establishment)
        {
            currentState = State.WaitingForPatron;
            SsmulationSpeed = establishment.SimulationSpeed;
        }
        public void Simulate(Establishment est, CancellationToken ct)
        {
            Task.Run(() =>
            {
                while(currentState != State.LeftWork && !ct.IsCancellationRequested)
                {
                    switch (currentState)
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
            return (int)(StartTime / SsmulationSpeed);
        }
        void WaitingForPatron(Establishment establishment)
        {
            if (establishment.CurrentPatrons.Count <= 0 && !establishment.IsOpen)
            {
                currentState = State.LeavingWork;
                return;
            }
            if (!establishment.Bar.CheckBarQueue())
            {
                Log("waiting for a patron");
            }
            Thread.Sleep(SpeedModifier(300));
            while (!establishment.Bar.CheckBarQueue())
            {
                if(establishment.CurrentPatrons.Count <= 0 && !establishment.IsOpen)
                {
                    currentState = State.LeavingWork;
                    return;
                }
                Thread.Sleep(SpeedModifier(300));
            }
            if (establishment.Bar.CheckBarShelfForGlass())
            {
                currentState = State.PouringBeer;
            }
            else
            {
                currentState = State.WaitingForCleanGlass;
            }
        }
        void PouringBeer(Bar bar)
        {
            Glass glass;
            if(bar.CheckBarShelfForGlass())
            {
                glass = bar.GetGlassFromShelf();
                Log("fetching glass");
                Thread.Sleep(SpeedModifier(3000));
                Log($"pouring {bar.GetFirstInBarQueue().Name} a beer");
                Thread.Sleep(SpeedModifier(3000));
                bar.AddGlassToBarTop(glass);
                currentState = State.WaitingForPatron;
            } 
            else
            {
                currentState = State.WaitingForCleanGlass;
            }
        }
        void WaitingForCleanGlass(Bar bar)
        {
            if (!bar.CheckBarShelfForGlass())
            {
                Log("is waiting at the shelf for a clean glass");
            }
            while (!bar.CheckBarShelfForGlass())
            {
                Thread.Sleep(SpeedModifier(300));
            }
            if (bar.CheckBarShelfForGlass())
            {
                currentState = State.PouringBeer;
            }
        }
        void LeavingWork()
        {
            currentState = State.LeftWork;
            Log("left the pub.");
        }
    }
}