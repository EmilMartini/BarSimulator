using System;

namespace Lab6
{
    class Patron
    {
        public string Name { get; private set; }
        public Patron(string name)
        {
            Name = name;
            Simulate();
        }

        void Simulate()
        {
            //Task.Run();
            throw new NotImplementedException();
        }
    }
}
