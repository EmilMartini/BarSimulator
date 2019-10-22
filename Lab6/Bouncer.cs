using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    class Bouncer
    {
        public delegate void Entry(Patron p);
        public event Entry Enter;
        Random rnd = new Random();
        string[] patronNames = new string[]
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
        enum State { Waiting, Working, LeavingWork}
        State currentState { get; set; }
        public Bouncer(Establishment est, SimulationManager sim)
        {
        }

        public void Simulate(Establishment est, SimulationManager sim)
        {
            currentState = State.Working;
            Task.Run(() =>
            {
                do
                {
                    switch (currentState)
                    {
                        case State.Waiting:
                            Wait();
                            break;
                        case State.Working:
                            Work(est, sim);
                            break;
                        case State.LeavingWork:
                            LeavingWork();
                            break;
                    }
                } while (currentState != State.LeavingWork);
            });
        }

        private void LeavingWork()
        {
            throw new NotImplementedException();
        }

        private void Work(Establishment est, SimulationManager sim)
        {
            if (!est.IsOpen)
            {
                currentState = State.LeavingWork;
                return;
            }

            Patron patron = new Patron(patronNames[rnd.Next(0, patronNames.Length - 1)], est.Table);
            sim.CurrentPatrons.Insert(0, patron);
            Enter(patron);
            currentState = State.Waiting;
        }
        private void Wait()
        {
            Thread.Sleep(rnd.Next(8000, 10000));
            currentState = State.Working;
        }
    }
}