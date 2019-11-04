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
        double patronSpeed;
        double simulationSpeed;
        ConcurrentBag<Glass> carrying;
        public string Name { get; private set; }
        public Patron(string name, Establishment establishment, CancellationToken ct)
        {
            Name = name;
            currentState = State.WalkingToBar;
            carrying = new ConcurrentBag<Glass>();
            patronSpeed = establishment.PatronSpeed;
            simulationSpeed = establishment.SimulationSpeed;
            Simulate(establishment, ct);
        }
        void Simulate(Establishment establishment, CancellationToken ct)
        {
            Log($"{Name} entered the bar");
            Task.Run(() =>
            {
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
        void WaitingForChair(Establishment establishment)
        {
            Patron patron = this;
            Log($"{this.Name} looking for a available chair");
            while (!CheckForEmptyChair(establishment) || !establishment.Table.IsFirstInQueue(this))
            {
                Thread.Sleep(SpeedModifier(300));
            }
            establishment.Table.GetFirstChairFromCondition(true).Available = false;
            if (establishment.Table.TryDequeue(this))
            {
                currentState = State.DrinkingBeer;
                return;
            }
        }
        void WaitingForBeer(Establishment establishment)
        {
            while (!establishment.Bar.CheckBarTopForBeer() || !establishment.Bar.CheckIfFirstInBarQueue(this))
            {
                Thread.Sleep(SpeedModifier(100));
            }
            carrying.Add(establishment.Bar.TakeGlassFromBarTop());
            establishment.Bar.RemovePatronFromBarQueue(this);
            currentState = State.WalkingToTable;
        }
        void DrinkingBeer(Establishment establishment)
        {
            Glass glass;
            Log($"{this.Name} sits down and drinks a beer");
            Thread.Sleep(SpeedModifier(random.Next(20000, 30000)));
            if(carrying.TryTake(out glass))
            {
                establishment.Table.PutGlassOnTable(glass);
            }
            currentState = State.LeavingEstablishment;
        }
        void LeavingEstablishment(Establishment establishment)
        {
            establishment.Table.GetFirstChairFromCondition(false).Available = true;
            Log($"{this.Name} finished the beer and left the pub");
            establishment.RemovePatron(this);
            currentState = State.LeftPub;
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
        int SpeedModifier(int StartTime)
        {
            return (int)((StartTime / patronSpeed) / simulationSpeed);
        }
    }
}
