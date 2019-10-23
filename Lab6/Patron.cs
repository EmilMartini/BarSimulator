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
        // Det tar en sekund att komma till baren, fyra sekunder att gå till ett bord, och mellan tio och tjugo sekunder (slumpa) att dricka ölen
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
                while (CurrentState != State.LeftPub)
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
                            WalkingToTable();
                            break;
                        default:
                            break;
                    }
                }
            });
        }
        private void WalkingToTable()
        {
            Log($"{this.Name} walks to the table");
            Thread.Sleep(SpeedModifier(4000));
            CurrentState = State.WaitingForChair;
        }
        void WalkingToBar(Establishment establishment)
        {
            //Log($"{this.Name} walks to the bar"); Kanske inte skall användas, se spec
            Thread.Sleep(SpeedModifier(1000));
            establishment.Bar.BarQueue.Enqueue(this);
            CurrentState = State.WaitingForBeer;
        }
        private void RemovePatron(Patron patron, Establishment establishment)
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
        }// lambda
        void DrinkingBeer(Establishment establishment)
        {
            Log($"{this.Name} sits down and drinks a beer");
            Thread.Sleep(SpeedModifier(random.Next(10000, 15000)));
            foreach (var glass in Holding)
            {
                glass.CurrentState = Glass.State.Dirty;
                establishment.Table.GlassesOnTable.Add(glass);
                Holding = new ConcurrentBag<Glass>(Holding.Except(new[] { glass }));
            }
            CurrentState = State.LeavingEstablishment;
        }// lambda
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

        }// snygga till eventuellt
        void WaitingForBeer(Establishment establishment)
        {
            if (!CheckBarTopForBeer(establishment))
            {
                //Log($"{this.Name} is waiting for a beer"); Kanske inte skall användas, se spec
            }
            while (!CheckBarTopForBeer(establishment))
            {
                Thread.Sleep(SpeedModifier(300));
            }
            if (CheckBarTopForBeer(establishment)) // och först i kön
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
            
        }// lambda
        int SpeedModifier(int StartTime)
        {
            return (int)((StartTime / patronSpeed) / simulationSpeed);
        }
    }
}
