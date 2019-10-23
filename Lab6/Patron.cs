using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Patron
    { 
        public enum State { WaitingForChair, WaitingForBeer, DrinkingBeer, WalkingToBar, WalkingToChair, LeavingEstablishment, RemovePatron }
        public delegate void PatronEvent(string s);
        public static event PatronEvent Log;

        double patronSpeed;
        double simulationSpeed;
        public string Name { get; private set; }
        public State CurrentState { get; set; }
        public ConcurrentBag<Glass> Holding { get; set; }
        public Patron(string name, Establishment establishment)
        {
            Name = name;
            CurrentState = State.WalkingToBar;
            Holding = new ConcurrentBag<Glass>();
            patronSpeed = establishment.PatronSpeed;
            simulationSpeed = establishment.SimulationSpeed;
            Simulate(establishment);
        }
        void Simulate(Establishment establishment)
        {
            Task.Run(() =>
            {
                do
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForChair:
                            WaitingForChair(establishment);
                            break;
                        case State.WaitingForBeer:
                            WaitingForBeer(establishment);
                            break;
                        case State.DrinkingBeer:
                            DrinkingBeer(establishment);
                            break;
                        case State.WalkingToBar:
                            WalkingToBar(establishment);
                            break;
                        case State.WalkingToChair:
                            WalkingToChair();
                            break;
                        case State.LeavingEstablishment:
                            LeavingEstablishment(establishment);
                            break;
                        default:
                            break;
                    }
                } while (CurrentState != State.RemovePatron);
                RemovePatron(this, establishment);
            });
        }
        private void RemovePatron(Patron patron, Establishment establishment)
        {
            establishment.CurrentPatrons.Remove(patron);
        }
        bool CheckBarTopForBeer(Establishment establishment)
        {
            if(establishment.Bar.BarTop.Count > 0)
            {
                return true;
            }
            return false;
        }
        bool CheckForEmptyChair(Establishment establishment)
        {
            foreach (var chair in establishment.Table.ChairsAroundTable)
            {
                if (chair.Available)
                {
                    return true;
                }
            }
            return false;
        }
        void DrinkingBeer(Establishment establishment)
        {
            Log($"{this.Name} is drinking a beer");
            Thread.Sleep(SpeedModifier(15000));
            foreach (var glass in Holding)
            {
                glass.CurrentState = Glass.State.Dirty;
                establishment.Table.GlassesOnTable.Add(glass);
                Holding = new ConcurrentBag<Glass>(Holding.Except(new[] { glass }));
            }
            CurrentState = State.LeavingEstablishment;
        }
        void WaitingForChair(Establishment establishment) 
        {
            if (!CheckForEmptyChair(establishment))
            {
                Log($"{this.Name} is waiting for a chair");
            }
            while (!CheckForEmptyChair(establishment))
            {
                Thread.Sleep(SpeedModifier(3000));
            }
            foreach (var chair in establishment.Table.ChairsAroundTable)
            {
                if (chair.Available)
                {
                    chair.Available = false;
                    CurrentState = State.DrinkingBeer;
                    return;
                }
            }

        }
        void WaitingForBeer(Establishment establishment)
        {
            if (!CheckBarTopForBeer(establishment))
            {
                Log($"{this.Name} is waiting for a beer");
            }
            while (!CheckBarTopForBeer(establishment))
            {
                Thread.Sleep(SpeedModifier(3000));
            }
            if (CheckBarTopForBeer(establishment)) // och först i kön
            {
                Glass glass = establishment.Bar.BarTop.ElementAt(0);
                Holding.Add(glass);
                establishment.Bar.BarTop = new ConcurrentBag<Glass>(establishment.Bar.BarTop.Except(new[] { glass }));
                establishment.Bar.BarQueue = new ConcurrentQueue<Patron>(establishment.Bar.BarQueue.Except(new[] { this }));
                CurrentState = State.WalkingToChair;
            }
            
        }
        void LeavingEstablishment(Establishment establishment)
        {
            foreach (var chair in establishment.Table.ChairsAroundTable)
            {
                if (!chair.Available)
                {
                    chair.Available = true;
                    break;
                }
            }
            Log($"{this.Name} is leaving establishment");
            Thread.Sleep(SpeedModifier(5000));
            Log($"{this.Name} has left the pub");
            CurrentState = State.RemovePatron;
            
        }
        void WalkingToBar(Establishment establishment)
        {
            Log($"{this.Name} is walking to the bar.");
            Thread.Sleep(SpeedModifier(5000));
            establishment.Bar.BarQueue.Enqueue(this);
            CurrentState = State.WaitingForBeer;
        }
        void WalkingToChair()
        {
            Log($"{this.Name} is walking to a chair");
            Thread.Sleep(SpeedModifier(5000));
            CurrentState = State.WaitingForChair;
        }
        int SpeedModifier(int StartTime)
        {
            return (int)((StartTime / patronSpeed) / simulationSpeed);
        }
    }
}
