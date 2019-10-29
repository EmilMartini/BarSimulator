using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Waitress
    {
        enum State { WaitingForDirtyGlass, PickingUpGlass, CleaningGlass, LeavingWork, ShelfingGlass, LeftWork }
        public event Action<string> Log;
        ConcurrentBag<Glass> CarryingGlasses { get; set; }
        double WaitressSpeed { get; set; }
        double SimulationSpeed { get; set; }
        State CurrentState { get; set; }
        public Waitress(Establishment establishment)
        {
            CurrentState = State.WaitingForDirtyGlass;
            CarryingGlasses = new ConcurrentBag<Glass>();
            WaitressSpeed = establishment.WaitressSpeed;
            SimulationSpeed = establishment.SimulationSpeed;
        }
        public void Simulate(Establishment establishment, CancellationToken ct)
        {
            Task.Run(() =>
            {
                while (CurrentState != State.LeftWork && !ct.IsCancellationRequested) 
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForDirtyGlass:
                            WaitingForDirtyGlass(establishment);
                            break;
                        case State.PickingUpGlass:
                            PickingUpGlass(establishment.Table);
                            break;
                        case State.CleaningGlass:
                            WashingGlass();
                            break;
                        case State.ShelfingGlass:
                            ShelfingGlass(establishment.Bar);
                            break;
                        case State.LeavingWork:
                            LeavingWork();
                            break;
                        default:
                            break;
                    }
                }
            });
        }
        void ShelfingGlass(Bar bar)
        {
            Log("shelfing washed glasses");
            foreach (var glass in CarryingGlasses)
            {
                bar.Shelf.Add(glass);
                CarryingGlasses = new ConcurrentBag<Glass>(CarryingGlasses.Except(new[] { glass }));
            }
            CurrentState = State.WaitingForDirtyGlass;
        }
        bool CheckTableForDirtyGlass(Table table)
        {
            if (table.GlassesOnTable.Count > 0)
            {
                return true;
            }
            return false;
        }
        int SpeedModifier(int normalSpeed)
        {
            return (int)((normalSpeed / WaitressSpeed) / SimulationSpeed);
        }
        void LeavingWork()
        {
            Log("has left the pub");
            CurrentState = State.LeftWork;
        }
        void WashingGlass()
        {
            Log($"washing {CarryingGlasses.Count} glasses");
            Thread.Sleep(SpeedModifier(15000));
            foreach (var glass in CarryingGlasses)
            {
                glass.CurrentState = Glass.State.Clean;
            }
            CurrentState = State.ShelfingGlass;

        }
        void PickingUpGlass(Table table)
        {
            Log("is picking up glasses");
            Thread.Sleep(SpeedModifier(10000));
            foreach (var glass in table.GlassesOnTable)
            {
                table.GlassesOnTable = new ConcurrentBag<Glass>(table.GlassesOnTable.Except(new[] { glass }));
                CarryingGlasses.Add(glass);
            }
            CurrentState = State.CleaningGlass;
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
                Thread.Sleep(SpeedModifier(300));
            }
            CurrentState = State.PickingUpGlass;
        }
    }
}