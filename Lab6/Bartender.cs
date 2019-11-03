using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bartender
    {
        enum State { WaitingForPatron, WaitingForCleanGlass, PouringBeer, LeavingWork , LeftWork}
        State currentState;
        public event Action<string> Log;
        double simulationSpeed;
        public Bartender(Establishment establishment)
        {
            currentState = State.WaitingForPatron;
            simulationSpeed = establishment.SimulationSpeed;
        }
        public void Simulate(Establishment establishment, CancellationToken ct)
        {
            Task.Run(() =>
            {
                while(currentState != State.LeftWork && !ct.IsCancellationRequested)
                {
                    switch (currentState)
                    {
                        case State.WaitingForPatron:
                            WaitingForPatron(establishment);
                            break;
                        case State.WaitingForCleanGlass:
                            WaitingForCleanGlass(establishment.Bar);
                            break;
                        case State.PouringBeer:
                            PouringBeer(establishment.Bar);
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
            return (int)(StartTime / simulationSpeed);
        }
        void WaitingForPatron(Establishment establishment)
        {
            if (TimeToGoHome(establishment))
            {
                currentState = State.LeavingWork;
                return;
            }
            Thread.Sleep(SpeedModifier(300));
            if (!establishment.Bar.CheckBarQueue())
            {
                Log("waiting for a patron");
                while (!establishment.Bar.CheckBarQueue())
                {
                    if(TimeToGoHome(establishment))
                    {
                        currentState = State.LeavingWork;
                        return;
                    }
                    Thread.Sleep(SpeedModifier(300));
                }
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
        bool TimeToGoHome(Establishment establishment)
        {
            if (!establishment.IsOpen && establishment.NumberOfCurrentPatrons() < 1)
            {
                return true;
            }
            return false;
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
                glass.CurrentState = Glass.State.Full;
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
                while (!bar.CheckBarShelfForGlass())
                {
                    Thread.Sleep(SpeedModifier(300));
                }
            }
            else 
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