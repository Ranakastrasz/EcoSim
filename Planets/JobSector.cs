namespace EcoSim.Planet
{
    public class JobSector
    {
        public Job Job { get; set; }
        public int JobSlots { get; set; }
        public int Workers { get; set; } // How many workers are currently assigned to this job

        public JobSector(Job job)
        {
            Job = job;
            JobSlots = 0;
            Workers = 0;
        }
        // Update, Tells the workers to do the job, and somehow sends the resource changes to the somewhere else.
    }
}