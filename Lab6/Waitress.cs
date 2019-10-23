using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Waitress
    {
        // public static event PatronEvent;
        public enum State { WaitingForDirtyGlass, PickingUpGlass, WalkingToSink, WalkingToTable, CleaningGlass, LeavingWork, ShelfingGlass }
        ConcurrentBag<Glass> carryingGlasses;

        public delegate void WaitressEvent(string s);
        public event WaitressEvent Log;
        public State CurrentState { get; set; }
        public Waitress(Establishment establishment)
        {
            CurrentState = State.WalkingToTable;
            carryingGlasses = new ConcurrentBag<Glass>();
        }
        public void Simulate(Establishment establishment)
        {
            Task.Run(() =>
            {
                while (CurrentState != State.LeavingWork) 
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForDirtyGlass:
                            WaitingForDirtyGlass(establishment);
                            break;
                        case State.PickingUpGlass:
                            PickingUpGlass(establishment.Table);
                            break;
                        case State.WalkingToSink:
                            WalkingToSink();
                            break;
                        case State.WalkingToTable:
                            WalkingToTable();
                            break;
                        case State.CleaningGlass:
                            CleaningGlass();
                            break;
                        case State.ShelfingGlass:
                            ShelfingGlass(establishment.Bar);
                            break;
                        default:
                            break;
                    }
                } 
                LeavingWork();
            });
        }
        void ShelfingGlass(Bar bar)
        {
            foreach (var glass in carryingGlasses)
            {
                bar.Shelf.Add(glass);
                carryingGlasses = new ConcurrentBag<Glass>(carryingGlasses.Except(new[] { glass }));
            }
            CurrentState = State.WalkingToTable;
        }
        bool CheckTableForDirtyGlass(Table table)
        {
            if (table.GlassesOnTable.Count > 0)
            {
                return true;
            }
            return false;
        }
        void LeavingWork()
        {
            Log("is leaving work");
            Thread.Sleep(5000);
            Log("has left the pub");
        }
        void CleaningGlass()
        {
            //CleaningGlassEvent();
            Thread.Sleep(15000);
            foreach (var glass in carryingGlasses) // gör med lambda sedan
            {
                glass.CurrentState = Glass.State.Clean;
            }
            CurrentState = State.ShelfingGlass;
        }
        void WalkingToTable()
        {
            Log("is walking to the table");
            Thread.Sleep(5000);
            CurrentState = State.WaitingForDirtyGlass;
        }
        void WalkingToSink()
        {
            Log("is walking to the sink");
            Thread.Sleep(5000);
            CurrentState = State.CleaningGlass;
        }
        void PickingUpGlass(Table table)
        {
            Log("is picking up glasses");
            foreach (var glass in table.GlassesOnTable)
            {
                Thread.Sleep(10000);
                table.GlassesOnTable = new ConcurrentBag<Glass>(table.GlassesOnTable.Except(new[] { glass }));
                carryingGlasses.Add(glass);
            }
            CurrentState = State.WalkingToSink;
        }
        void WaitingForDirtyGlass(Establishment establishment)
        {
            if (!establishment.IsOpen && establishment.CurrentPatrons.Count < 1)
            {
                CurrentState = State.LeavingWork;
                return;
            }
            if (!CheckTableForDirtyGlass(establishment.Table))
            {
                Log("is waiting for dirty glasses");
            }
            while (!CheckTableForDirtyGlass(establishment.Table))
            {
                if (!establishment.IsOpen && establishment.CurrentPatrons.Count < 1)
                {
                    CurrentState = State.LeavingWork;
                    return;
                }
                Thread.Sleep(3000);
            }
            CurrentState = State.PickingUpGlass;
        }
    }
}