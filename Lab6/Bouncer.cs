﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bouncer
    {
        enum State { Waiting, Working, LeavingWork, LeftWork}
        State currentState;
        public event Action<string> Log;
        Random random = new Random();
        List<string> patronNames = new List<string>()
        {
            "Emma",
            "Olivia",
            "Ava",
            "Isabella",
            "Sophia",
            "Charlotte",
            "Mia",
            "Amelia",
            "Harper",
            "Evelyn",
            "Abigail",
            "Emily",
            "Elizabeth",
            "Mila",
            "Ella",
            "Avery",
            "Sofia",
            "Camila",
            "Aria",
            "Scarlett",
            "Victoria",
            "Madison",
            "Luna",
            "Grace",
            "Chloe",
            "Liam",
            "Noah",
            "William",
            "James",
            "Oliver",
            "Benjamin",
            "Elijah",
            "Lucas",
            "Mason",
            "Logan",
            "Alexander",
            "Ethan",
            "Jacob",
            "Michael",
            "Daniel",
            "Henry",
            "Jackson",
            "Sebastian",
            "Aiden",
            "Matthew",
            "Samuel",
            "David",
            "Joseph",
            "Carter",
            "Owen"
            };
        bool busArrived;
        DateTime busTimer;
        double simulationSpeed;
        double bouncerSpeed;
        int patronsPerEntry;
        public Bouncer(Establishment establishment)
        {
            bouncerSpeed = establishment.BouncerSpeed;
            simulationSpeed = establishment.SimulationSpeed;
            patronsPerEntry = establishment.PatronsPerEntry;
            if (establishment.IsBusloadState)
            {
                busTimer = DateTime.Now + new TimeSpan(0, 0, 20);
            }
        }
        public void Simulate(Establishment establishment, CancellationToken ct)
        {
            Task.Run(() =>
            {
                while(currentState != State.LeftWork && !ct.IsCancellationRequested)
                {
                    switch (currentState)
                    {
                        case State.Waiting:
                            Wait(ct, establishment);
                            break;
                        case State.Working:
                            Work(establishment, ct);
                            break;
                        case State.LeavingWork:
                            LeavingWork();
                            break;
                    }
                }
            });
        }
        void Wait(CancellationToken ct, Establishment establishment)
        {
            var timeToSleep = CalculateTimeToSleep(3000, 10001);
            while((DateTime.Now < timeToSleep) && !ct.IsCancellationRequested && establishment.IsOpen)
            {
                Thread.Sleep(10);
                if (!establishment.IsBusloadState || busArrived)
                {
                    continue;
                }
                if (busArrived && patronsPerEntry != establishment.PatronsPerEntry)
                {
                    patronsPerEntry = establishment.PatronsPerEntry;
                }
                if (!busArrived)
                {
                    if (DateTime.Now < busTimer)
                    {
                        continue; 
                    } 
                    else
                    {
                        patronsPerEntry = 15;
                        Log("Bus arrived");
                        busArrived = true;
                        break;
                    }
                }
            }
            currentState = State.Working;
        }
        void Work(Establishment establishment, CancellationToken ct)
        {
            if (!establishment.IsOpen)
            {
                currentState = State.LeavingWork;
                return;
            }
            for (int i = 0; i < patronsPerEntry; i++)
            {
                Patron patron = new Patron(patronNames[random.Next(0, patronNames.Count - 1)], establishment, ct);
                establishment.AddPatron(patron);
            }
            currentState = State.Waiting;
        }
        void LeavingWork()
        {
            Log("Bouncer has left the pub.");
            currentState = State.LeftWork;
        }
        DateTime CalculateTimeToSleep(int minRange, int maxRange)
        {
            int timeToSleepInMs = SpeedModifier(random.Next(minRange, maxRange));
            return DateTime.Now + new TimeSpan(0, 0, 0, 0,timeToSleepInMs);
        }
        int SpeedModifier(int StartTime)
        {
            return (int)((StartTime / bouncerSpeed) / simulationSpeed);
        }
    }
}