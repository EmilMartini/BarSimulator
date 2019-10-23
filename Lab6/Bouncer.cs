using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bouncer
    {
        public delegate void LogEvent(string s);
        public event LogEvent Log;

        Random random = new Random();
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
        double simulationSpeed;
        enum State { Waiting, Working, LeavingWork, StopBouncer}
        State currentState { get; set; }

        public void Simulate(Establishment establishment, CancellationToken ct)
        {
            simulationSpeed = establishment.SimulationSpeed;  
            currentState = State.Working;
            Task.Run(() =>
            {
                while(currentState != State.StopBouncer && !ct.IsCancellationRequested)
                {
                    switch (currentState)
                    {
                        case State.Waiting:
                            Wait();
                            break;
                        case State.Working:
                            Work(establishment, ct);
                            break;
                        case State.LeavingWork:
                            LeavingWork();
                            break;
                    }
                }
                Console.WriteLine("Stopping Bouncer Thread");
            });
        }
        private void LeavingWork()
        {
            Log("Bouncer has left the pub.");
            currentState = State.StopBouncer;
        }
        private void Work(Establishment establishment, CancellationToken ct)
        {
            if (!establishment.IsOpen)
            {
                currentState = State.LeavingWork;
                return;
            }

            for (int i = 0; i < establishment.PatronsPerEntry; i++)
            {
                establishment.TotalPatrons++;
                Patron patron = new Patron(patronNames[random.Next(0, patronNames.Length - 1)], establishment, ct);
                establishment.CurrentPatrons.Insert(0, patron);
                Log($"{patron.Name} enters the pub");
            }
            currentState = State.Waiting;
        }
        private void Wait()
        {
            Thread.Sleep(SpeedModifier(random.Next(8000, 10000)));
            currentState = State.Working;
        }
        private int SpeedModifier(int StartTime)
        {
            return (int)(StartTime / simulationSpeed);
        }
    }
}