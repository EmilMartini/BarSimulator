namespace Lab6
{
    public class Chair
    {
        bool Available { get;  set; }

        public Chair()
        {
            this.Available = true;
        }

        public bool IsAvailable()
        {
            if (this.Available)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetAvailable()
        {
            if(!this.Available)
            {
                this.Available = true;
            }
        }

        public void SetToTaken()
        {
            if (this.Available)
            {
                this.Available = false;
            }
        }
    }
}