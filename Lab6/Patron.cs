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
        public delegate void PatronEvent(string s);
        public static event PatronEvent Log;
        Random random = new Random();
        State CurrentState { get; set; }
        public string Name { get; private set; }
        double PatronSpeed { get; set; }
        double SimulationSpeed { get; set; }
        ConcurrentBag<Glass> Holding { get; set; }
        public Patron(string name, Establishment establishment, CancellationToken ct)
        {
            Name = name;
            CurrentState = State.WalkingToBar;
            Holding = new ConcurrentBag<Glass>();
            PatronSpeed = establishment.PatronSpeed;
            SimulationSpeed = establishment.SimulationSpeed;
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
            establishment.Table.EnqueuePatron(this);
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
            if (establishment.Bar.BarTop.Count > 0)
            {
                return true;
            }
            return false;
        }
        bool CheckForEmptyChair(Establishment establishment)
        {
            var chair = establishment.Table.GetFirstChairFromCondition(true);
            if(chair != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        void DrinkingBeer(Establishment establishment)
        {
            Log($"{this.Name} sits down and drinks a beer");
            Thread.Sleep(SpeedModifier(random.Next(20000, 30000)));
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
            while (!CheckForEmptyChair(establishment) || !establishment.Table.IsFirstInQueue(this))
            {
                Thread.Sleep(SpeedModifier(300));
            }

            var chair = establishment.Table.GetFirstChairFromCondition(true);
            if(chair != null)
            {
                chair.SetAvailability(false);
                if (establishment.Table.TryDequeue(this))
                {
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
            Patron patron;
            Glass glass = establishment.Bar.BarTop.ElementAt(0);
            establishment.Bar.BarTop = new ConcurrentBag<Glass>(establishment.Bar.BarTop.Except(new[] { glass }));
            establishment.Bar.BarQueue.TryDequeue(out patron);
            Holding.Add(glass);

            
            CurrentState = State.WalkingToTable;
        }
        void LeavingEstablishment(Establishment establishment)
        {
            var chair = establishment.Table.GetFirstChairFromCondition(false);
            if(chair != null)
            {
                chair.SetAvailability(true);
            }
            Log($"{this.Name} finished the beer and left the pub");
            CurrentState = State.LeftPub;
        }
        int SpeedModifier(int StartTime)
        {
            return (int)((StartTime / PatronSpeed) / SimulationSpeed);
        }
    }
}
