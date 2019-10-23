using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Patron
    { 
        public enum State { WaitingForChair, WaitingForBeer, DrinkingBeer, WalkingToBar, WalkingToChair, LeavingEstablishment }
        public delegate void PatronEvent(Patron p);
        public static event PatronEvent WaitingForBeerEvent, WaitingForChairEvent, WalkingToBarEvent, WalkingToChairEvent, DrinkingBeerEvent, LeavingEstablishmentEvent;

        public string Name { get; private set; }
        public State CurrentState { get; set; }
        public ConcurrentBag<Glass> Holding { get; set; }
        public Patron(string name, Establishment establishment)
        {
            Name = name;
            CurrentState = State.WalkingToBar;
            Holding = new ConcurrentBag<Glass>();
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
                            WaitingForChair(establishment.Table);
                            break;
                        case State.WaitingForBeer:
                            WaitingForBeer(establishment.Bar);
                            break;
                        case State.DrinkingBeer:
                            DrinkingBeer(establishment.Table);
                            break;
                        case State.WalkingToBar:
                            WalkingToBar(establishment.Bar);
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
            if(bar.BarTop.Count > 0)
            {
                return true;
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
            DrinkingBeerEvent(this);
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
                WaitingForChairEvent(this);
            }
            while (!CheckForEmptyChair(table))
            {
                Thread.Sleep(3000);
            }
            foreach (var chair in table.ChairsAroundTable)
            {
                if (CheckForEmptyChair(table))
                {
                    chair.Available = false;
                    CurrentState = State.DrinkingBeer;
                    return;
                }
            }

        }
        void WaitingForBeer(Bar bar)
        {
            if (!CheckBarTopForBeer(bar))
            {
                WaitingForBeerEvent(this);
            }
            while (!CheckBarTopForBeer(bar))
            {
                Thread.Sleep(3000);
            }
            if (CheckBarTopForBeer(bar)) // och först i kön
            {
                Glass glass = bar.BarTop.ElementAt(0);
                Holding.Add(glass);
                bar.BarTop = new ConcurrentBag<Glass>(bar.BarTop.Except(new[] { glass }));
                bar.BarQueue = new ConcurrentQueue<Patron>(bar.BarQueue.Except(new[] { this }));
                CurrentState = State.WalkingToChair;
            }
            
        }
        void LeavingEstablishment()
        {
            LeavingEstablishmentEvent(this);
            Thread.Sleep(5000);
            //Log Has left the establishment
        }
        void WalkingToBar(Bar bar)
        {
            WalkingToBarEvent(this);
            bar.BarQueue.Enqueue(this);
            CurrentState = State.WaitingForBeer;
        }
        void WalkingToChair()
        {
            WalkingToChairEvent(this);
            Thread.Sleep(5000);
            CurrentState = State.WaitingForChair;
        }
    }
}
