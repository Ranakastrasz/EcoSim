namespace EcoSim.Planet
{
    public class District
    {
        public int TotalSize { get; set; } // How many deposits are being exploited

        public Job Job { get; set; } // The job that this provides.
        public District(Job job)
        {
            TotalSize = 0;
            Job = job;
        }
        // Infrastructure. Built on top of the natural resources, and provides a better job than the normal one.
        // Also removes the natural resource's job from the sector.
    }
}