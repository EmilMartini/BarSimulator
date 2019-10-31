using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Waitress
    {
        enum State { WaitingForDirtyGlass, PickingUpGlass, CleaningGlass, LeavingWork, ShelfingGlass, LeftWork }
        State CurrentState;
        public event Action<string> Log;
        List<Glass> carryingGlasses;
        double waitressSpeed;
        double simulationSpeed;
        public Waitress(Establishment establishment)
        {
            CurrentState = State.WaitingForDirtyGlass;
            carryingGlasses = new List<Glass>(); 
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
        void WaitingForDirtyGlass(Establishment establishment)
        {
            if (TimeToGoHome(establishment))
            {
                CurrentState = State.LeavingWork;
                return;
            }
            if (establishment.Table.NumberOfGlasses() == 0) 
            {
                Log("is waiting for dirty glasses");
            }
            while (establishment.Table.NumberOfGlasses() == 0)
            {
                if (TimeToGoHome(establishment))
                {
                    CurrentState = State.LeavingWork;
                    return;
                }
                Thread.Sleep(SpeedModifier(300));
            }
            CurrentState = State.PickingUpGlass;
        }
        bool TimeToGoHome(Establishment establishment)
        {
            if (!establishment.IsOpen && establishment.CurrentPatrons.Count < 1)
            {
                return true;
            }
            return false;
        }
        void PickingUpGlass(Table table)
        {
            Log("is picking up glasses");
            Thread.Sleep(SpeedModifier(10000));
            carryingGlasses.AddRange(table.RemoveGlasses());
            CurrentState = State.CleaningGlass;
        }
        void WashingGlass()
        {
            Log($"washing {carryingGlasses.Count} glasses");
            Thread.Sleep(SpeedModifier(15000));
            foreach (var glass in carryingGlasses)
            {
                glass.CurrentState = Glass.State.Clean;
            }
            CurrentState = State.ShelfingGlass;
        }
        void ShelfingGlass(Bar bar)
        {
            Log("shelfing washed glasses");
            foreach (var glass in carryingGlasses)
            {
                bar.AddGlassToShelf(glass);
            }
            carryingGlasses.RemoveRange(0, carryingGlasses.Count);
            CurrentState = State.WaitingForDirtyGlass;
        }
        void LeavingWork()
        {
            Log("has left the pub");
            CurrentState = State.LeftWork;
        }
        int SpeedModifier(int normalSpeed)
        {
            return (int)((normalSpeed / waitressSpeed) / simulationSpeed);
        }
    }
}