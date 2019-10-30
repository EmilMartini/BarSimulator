namespace Lab6
{
    public class Chair
    {
        bool Available { get;  set; }

        public Chair()
        {
            Available = true;
        }

        public bool IsAvailable()
        {
            if (Available)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetAvailability(bool availability)
        {
            Available = availability;
        }
    }
}