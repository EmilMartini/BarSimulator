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
        ConcurrentBag<Glass> carryingGlasses { get; set; }
        double waitressSpeed { get; set; }
        double simulationSpeed { get; set; }
        State CurrentState { get; set; }
        public Waitress(Establishment establishment)
        {
            CurrentState = State.WaitingForDirtyGlass;
            carryingGlasses = new ConcurrentBag<Glass>();
            waitressSpeed = establishment.WaitressSpeed;
            simulationSpeed = establishment.SimulationSpeed;
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
            foreach (var glass in carryingGlasses)
            {
                bar.Shelf.Add(glass);
                carryingGlasses = new ConcurrentBag<Glass>(carryingGlasses.Except(new[] { glass }));
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
            return (int)((normalSpeed / waitressSpeed) / simulationSpeed);
        }
        void LeavingWork()
        {
            Log("has left the pub");
            CurrentState = State.LeftWork;
        }
        void WashingGlass()
        {
            Log("washing glasses");
            Thread.Sleep(SpeedModifier(15000));
            foreach (var glass in carryingGlasses)
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
                carryingGlasses.Add(glass);
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
        }// snygga till eventuellt
    }
}