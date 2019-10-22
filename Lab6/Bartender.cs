using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bartender
    {
        public enum State { WaitingForPatron, WaitingForCleanGlass, PouringBeer, LeavingWork }
        public State CurrentState { get; set; }
        
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
                            WaitingForPatron();
                            break;
                        case State.WaitingForCleanGlass:
                            WaitingForCleanGlass();
                            break;
                        case State.PouringBeer:
                            PouringBeer();
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
            if (CheckBarCue(bar))
            {
                CurrentState = State.WaitingForCleanGlass;
                return;
            }
            if (!CheckBarCue(bar))
            {
                // Log Waiting for Patron
            }
            while (!CheckBarCue(bar))
            {
                Thread.Sleep(3000);
            }

        }
        void PouringBeer(Bar bar)
        {
            
        }
        void WaitingForCleanGlass(Bar bar)
        {
            if (CheckBarShelf(bar))
            {
                CurrentState = State.PouringBeer;
                return;
            }
            if (!CheckBarShelf(bar))
            {
                // Log Waiting for glass
            }
            while (!CheckBarShelf(bar))
            {
                Thread.Sleep(3000);
            }
        }
        void LeavingWork()
        {
            
        }
    }
}