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
        State currentState;
        public string Name { get; private set; }
        double patronSpeed;
        double simulationSpeed;
        ConcurrentBag<Glass> holding;
        public Patron(string name, Establishment establishment, CancellationToken ct)
        {
            Name = name;
            currentState = State.WalkingToBar;
            holding = new ConcurrentBag<Glass>();
            patronSpeed = establishment.PatronSpeed;
            simulationSpeed = establishment.SimulationSpeed;
            Simulate(establishment, ct);
        }
        void Simulate(Establishment establishment, CancellationToken ct)
        {
            Task.Run(() =>
            {
                Log($"{Name} entered the bar");
                while (currentState != State.LeftPub && !ct.IsCancellationRequested)
                {
                    switch (currentState)
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
            currentState = State.WaitingForChair;
        }
        void WalkingToBar(Establishment establishment)
        {
            Thread.Sleep(SpeedModifier(1000));
            establishment.Bar.AddPatronToBarQueue(this);
            currentState = State.WaitingForBeer;
        }
        void RemovePatron(Patron patron, Establishment establishment)
        {
            establishment.CurrentPatrons.Remove(patron);
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
            foreach (var glass in holding)
            {
                glass.CurrentState = Glass.State.Dirty;
                establishment.Table.PutGlassOnTable(glass);
                holding = new ConcurrentBag<Glass>(holding.Except(new[] { glass }));
            }
            currentState = State.LeavingEstablishment;
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
                    currentState = State.DrinkingBeer;
                    return;
                }
            }
        }
        void WaitingForBeer(Establishment establishment)
        {
            while (!establishment.Bar.CheckBarTopForBeer() || establishment.Bar.CheckIfFirstInBarQueue(this))
            {
                Thread.Sleep(SpeedModifier(300));
            }
            establishment.Bar.RemovePatronFromBarQueue(this);
            Holding.Add(establishment.Bar.TakeGlassFromBarTop());
            currentState = State.WalkingToTable;
        }
        void LeavingEstablishment(Establishment establishment)
        {
            var chair = establishment.Table.GetFirstChairFromCondition(false);
            if(chair != null)
            {
                chair.SetAvailability(true);
            }
            Log($"{this.Name} finished the beer and left the pub");
            currentState = State.LeftPub;
        }
        int SpeedModifier(int StartTime)
        {
            return (int)((StartTime / patronSpeed) / simulationSpeed);
        }
    }
}
