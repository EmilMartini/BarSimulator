using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bouncer
    {
        public delegate void LogEvent(string s);
        public event LogEvent Log;

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
        enum State { Waiting, Working, LeavingWork, StopBouncer}
        State currentState { get; set; }
        public Bouncer(Establishment est, SimulationManager sim)
        {
        }

        public void Simulate(SimulationManager sim)
        {
            currentState = State.Working;
            Task.Run(() =>
            {
                while(currentState != State.StopBouncer)
                {
                    switch (currentState)
                    {
                        case State.Waiting:
                            Wait();
                            break;
                        case State.Working:
                            Work(sim);
                            break;
                        case State.LeavingWork:
                            LeavingWork();
                            break;
                    }
                }
            });
        }

        private void LeavingWork()
        {
            Log("Bouncer is leaving the pub.");
            currentState = State.StopBouncer;
        }

        private void Work(SimulationManager sim)
        {
            if (!sim.establishment.IsOpen)
            {
                currentState = State.LeavingWork;
                return;
            }

            Patron patron = new Patron(patronNames[rnd.Next(0, patronNames.Length - 1)], sim.establishment, sim);
            sim.CurrentPatrons.Insert(0, patron);
            Log($"{patron.Name} enters the pub");
            currentState = State.Waiting;
        }
        private void Wait()
        {
            Thread.Sleep(rnd.Next(8000, 10000));
            currentState = State.Working;
        }
    }
}