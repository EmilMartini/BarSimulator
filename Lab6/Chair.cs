namespace Lab6
{
    public class Chair
    {
        bool available;
        public Chair()
        {
            available = true;
        }
        public bool IsAvailable()
        {
            if (available)
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
            available = availability;
        }
    }
}