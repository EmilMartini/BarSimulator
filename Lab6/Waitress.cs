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
        public State CurrentState { get; set; }
        public Waitress(Table table, Bar bar)
        {
            CurrentState = State.WalkingToTable;
            carryingGlasses = new ConcurrentBag<Glass>();
            Simulate(table, bar);
        }
        private void Simulate(Table table, Bar bar)
        {
            Task.Run(() =>
            {
                do
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForDirtyGlass:
                            WaitingForDirtyGlass(table);
                            break;
                        case State.PickingUpGlass:
                            PickingUpGlass(table);
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
                        case State.LeavingWork:
                            LeavingWork();
                            break;
                        case State.ShelfingGlass:
                            ShelfingGlass(bar);
                            break;
                        default:
                            break;
                    }
                } while (CurrentState != State.LeavingWork);
            });
        }
        void ShelfingGlass(Bar bar)
        {
            foreach (var glass in carryingGlasses)
            {
                bar.Shelf.Add(glass);
                carryingGlasses = new ConcurrentBag<Glass>(carryingGlasses.Except(new[] { glass }));
            }
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
            //LeavingWorkEvent(this);
            Thread.Sleep(5000);
            //Log Has left the establishment
        }
        void CleaningGlass()
        {
            // CleaningGlassEvent
            Thread.Sleep(15000);
            foreach (var glass in carryingGlasses) // gör med lambda sedan
            {
                glass.CurrentState = Glass.State.Clean;
            }
            CurrentState = State.ShelfingGlass;
        }
        void WalkingToTable()
        {
            // WalkingToChairEvent(this);
            Thread.Sleep(5000);
            CurrentState = State.WaitingForDirtyGlass;
        }
        void WalkingToSink()
        {
            // WalkingToChairEvent(this);
            Thread.Sleep(5000);
            CurrentState = State.CleaningGlass;
        }
        void PickingUpGlass(Table table)
        {
            //PickingUpGlassEvent
            foreach (var glass in table.GlassesOnTable)
            {
                Thread.Sleep(10000);
                table.GlassesOnTable = new ConcurrentBag<Glass>(table.GlassesOnTable.Except(new[] { glass }));
                carryingGlasses.Add(glass);
            }
            CurrentState = State.WalkingToSink;
        }
        void WaitingForDirtyGlass(Table table)
        {
            if (!CheckTableForDirtyGlass(table))
            {
                //WaitingForChairEvent(this);
            }
            while (!CheckTableForDirtyGlass(table))
            {
                Thread.Sleep(3000);
            }
            foreach (var chair in table.ChairsAroundTable)
            {
                if (CheckTableForDirtyGlass(table))
                {
                    chair.Available = false;
                    CurrentState = State.PickingUpGlass;
                    return;
                }
            }
        }
    }
}