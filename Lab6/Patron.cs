using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    class Patron
    {
        public enum State { WaitingForChair, WaitingForBeer, DrinkingBeer, WalkingToBar, LeavingEstablishment }
        public string Name { get; private set; }
        public State CurrentState { get; set; }
        public ConcurrentBag<Glass> Holding { get; set; }
        public Patron(string name, Table table)
        {
            Name = name;
            CurrentState = State.WalkingToBar;
            Simulate(table);
        }
        void Simulate(Table table)
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
                            WaitingForBeer();
                            break;
                        case State.DrinkingBeer:
                            DrinkingBeer(table);
                            break;
                        case State.WalkingToBar:
                            WalkingToBar();
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
        void WaitingForBeer()
        {

        }
        void LeavingEstablishment()
        {

        }
        void WalkingToBar()
        {

        }
    }
}
