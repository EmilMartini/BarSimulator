using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    class Patron
    {
        public enum State { WaitingForChair, WaitingForBeer, DrinkingBeer, WalkingToBar, WalkingToChair, LeavingEstablishment }
        public string Name { get; private set; }
        public State CurrentState { get; set; }
        public ConcurrentBag<Glass> Holding { get; set; }
        public Patron(string name, Table table)
        {
            Name = name;
            CurrentState = State.WalkingToBar;
        }
        void Simulate(Table table, Bar bar)
        {
            Task.Run(() =>
            {
                do
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForChair:
                            WaitingForChair(table);
                            break;
                        case State.WaitingForBeer:
                            WaitingForBeer(bar);
                            break;
                        case State.DrinkingBeer:
                            DrikingBeer();
                            break;
                        case State.WalkingToBar:
                            WalkingToBar();
                            break;
                        case State.WalkingToChair:
                            WalkingToChair();
                            break;
                        case State.LeavingEstablishment:
                            LeavingEstablishment();
                            break;
                        default:
                            break;
                    }
                } while (CurrentState != State.LeavingEstablishment);
            });
        }
        bool CheckBarTopForBeer(Bar bar)
        {
            foreach (var beer in bar.BarTop)
            {
                if (bar.BarTop != null)
                {
                    return true;
                }
            }
            return false;
        }
        bool CheckForEmptyChair(Table table)
        {
            foreach (var chair in table.ChairsAroundTable)
            {
                if (chair.Available)
                {
                    return true;
                }
            }
            return false;
        }
        void DrinkingBeer(Table table)
        {
            // Skicka logg till LogManager "Name: sätter sig och dricker öl
            Thread.Sleep(15000);
            foreach (var glass in Holding) // gör med lambda sedan
            {
                glass.CurrentState = Glass.State.Dirty;
                table.GlassesOnTable.Add(glass);
                Holding = new ConcurrentBag<Glass>(Holding.Except(new[] { glass }));
            }
            CurrentState = State.LeavingEstablishment;
        }
        void WaitingForChair(Table table)
        {
            if (!CheckForEmptyChair(table))
            {
                Thread.Sleep(3000);
                // Skicka logg till LogManager "Name: väntar på en stol
                return;
            }
            foreach (var chair in table.ChairsAroundTable)
            {
                if (CheckForEmptyChair(table))
                {
                    chair.Available = false;
                    CurrentState = State.DrinkingBeer;
                }
            }

        } // saknas saker
        void WaitingForBeer(Bar bar)
        {
            if (CheckBarTopForBeer(bar)) // och först i kön
            {
                foreach (var glass in bar.BarTop)
                {
                    Holding.Add(glass);
                    bar.BarTop = new ConcurrentBag<Glass>(bar.BarTop.Except(new[] { glass }));
                    CurrentState = State.WaitingForChair;
                    return;
                }
            }
            Thread.Sleep(2000);
        }
        void LeavingEstablishment()
        {
            //Log Leaving the establishment
            Thread.Sleep(5000);
        }
        void WalkingToBar()
        {
            //Log Walking to table
            Thread.Sleep(5000);
            CurrentState = State.WaitingForBeer;
        }
        void WalkingToChair()
        {
            //Log Walking to table
            Thread.Sleep(5000);
            CurrentState = State.WaitingForChair;
        }
    }
}
