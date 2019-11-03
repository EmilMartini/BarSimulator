using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Waitress
    {
        enum State { WaitingForDirtyGlass, PickingUpGlass, CleaningGlass, LeavingWork, ShelfingGlass, LeftWork }
        State currentState;
        public event Action<string> Log;
        List<Glass> carryingGlasses;
        double waitressSpeed;
        double simulationSpeed;
        public Waitress(Establishment establishment)
        {
            currentState = State.WaitingForDirtyGlass;
            carryingGlasses = new List<Glass>(); 
            waitressSpeed = establishment.WaitressSpeed;
            simulationSpeed = establishment.SimulationSpeed;
        }
        public void Simulate(Establishment establishment, CancellationToken ct)
        {
            Task.Run(() =>
            {
                while (currentState != State.LeftWork && !ct.IsCancellationRequested) 
                {
                    switch (currentState)
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
                currentState = State.LeavingWork;
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
                    currentState = State.LeavingWork;
                    return;
                }
                Thread.Sleep(SpeedModifier(300));
            }
            currentState = State.PickingUpGlass;
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
            currentState = State.CleaningGlass;
        }
        void WashingGlass()
        {
            Log($"washing {carryingGlasses.Count} glasses");
            Thread.Sleep(SpeedModifier(15000));
            foreach (var glass in carryingGlasses)
            {
                glass.CurrentState = Glass.State.Clean;
            }
            currentState = State.ShelfingGlass;
        }
        void ShelfingGlass(Bar bar)
        {
            Log("shelfing washed glasses");
            foreach (var glass in carryingGlasses)
            {
                bar.AddGlassToShelf(glass);
            }
            carryingGlasses.RemoveRange(0, carryingGlasses.Count);
            currentState = State.WaitingForDirtyGlass;
        }
        void LeavingWork()
        {
            Log("has left the pub");
            currentState = State.LeftWork;
        }
        int SpeedModifier(int normalSpeed)
        {
            return (int)((normalSpeed / waitressSpeed) / simulationSpeed);
        }
    }
}