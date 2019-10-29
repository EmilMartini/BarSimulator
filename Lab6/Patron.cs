using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Patron
    { 
        public enum State { WaitingForChair, WaitingForBeer, DrinkingBeer, LeavingEstablishment, LeftPub, WalkingToBar, WalkingToTable }
        public static event Action<string> Log;
        Random random = new Random();

        double patronSpeed { get; set; }
        double simulationSpeed { get; set; }
        public string Name { get; private set; }
        State CurrentState { get; set; }
        ConcurrentBag<Glass> Holding { get; set; }

        public Patron(string name, Establishment establishment, CancellationToken ct)
        {
            Name = name;
            CurrentState = State.WalkingToBar;
            Holding = new ConcurrentBag<Glass>();
            patronSpeed = establishment.PatronSpeed;
            simulationSpeed = establishment.SimulationSpeed;
            Simulate(establishment, ct);
        }
        void Simulate(Establishment establishment, CancellationToken ct)
        {
            Task.Run(() =>
            {
                Log($"{Name} entered the bar");
                while (CurrentState != State.LeftPub && !ct.IsCancellationRequested)
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
                        case State.LeavingEstablishment:
                            LeavingEstablishment(establishment);
                            RemovePatron(this, establishment);
                            break;
                        case State.WalkingToBar:
                            WalkingToBar(establishment);
                            break;
                        case State.WalkingToTable:
                            WalkingToTable(establishment);
                            break;
                        default:
                            break;
                    }
                }
            });
        }
        void WalkingToTable(Establishment establishment)
        {
            Thread.Sleep(SpeedModifier(4000));
            establishment.Table.ChairQueue.Enqueue(this);
            CurrentState = State.WaitingForChair;
        }
        void WalkingToBar(Establishment establishment)
        {
            Thread.Sleep(SpeedModifier(1000));
            establishment.Bar.BarQueue.Enqueue(this);
            CurrentState = State.WaitingForBeer;
        }
        void RemovePatron(Patron patron, Establishment establishment)
        {
            establishment.CurrentPatrons.Remove(patron);
        }
        bool CheckBarTopForBeer(Establishment establishment)
        {
            if(establishment.Bar.BarTop.Count > 0)
                return true;

            return false;
        }
        bool CheckForEmptyChair(Establishment establishment)
        {
            foreach (var chair in establishment.Table.ChairsAroundTable)
            {
                if (chair.Available)
                    return true;
            }
            return false;
        }
        void DrinkingBeer(Establishment establishment)
        {
            Log($"{this.Name} sits down and drinks a beer");
            Thread.Sleep(SpeedModifier(random.Next(10000, 20000)));
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
            Patron patron = this;
            Log($"{this.Name} looking for a available chair");
            while (!CheckForEmptyChair(establishment) || establishment.Table.ChairQueue.First() != this)
            {
                if (establishment.Table.ChairQueue.First() == this) // testrad
                    Log($"{this.Name} First in line");

                if (establishment.Table.ChairQueue.First() != this)
                    Log($"{this.Name} not first in line");
                
                Thread.Sleep(SpeedModifier(3000));
            }
            foreach (var chair in establishment.Table.ChairsAroundTable)
            {
                if (chair.Available)
                {
                    chair.Available = false;
                    establishment.Table.ChairQueue.TryDequeue(out patron);
                    CurrentState = State.DrinkingBeer;
                    return;
                }
            }
        }
        void WaitingForBeer(Establishment establishment)
        {
            while (!CheckBarTopForBeer(establishment) || establishment.Bar.BarQueue.First() != this)
            {
                Thread.Sleep(SpeedModifier(300));
            }
            if (CheckBarTopForBeer(establishment))
            {
                Glass glass = establishment.Bar.BarTop.ElementAt(0);
                Holding.Add(glass);
                establishment.Bar.BarTop = new ConcurrentBag<Glass>(establishment.Bar.BarTop.Except(new[] { glass }));
                establishment.Bar.BarQueue = new ConcurrentQueue<Patron>(establishment.Bar.BarQueue.Except(new[] { this }));
                CurrentState = State.WalkingToTable;
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
            Log($"{this.Name} finished the beer and left the pub");
            CurrentState = State.LeftPub;
        }
        int SpeedModifier(int StartTime)
        {
            return (int)((StartTime / patronSpeed) / simulationSpeed);
        }
    }
}
