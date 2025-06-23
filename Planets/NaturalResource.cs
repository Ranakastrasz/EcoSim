namespace EcoSim.Planet
{
    public class NaturalResource
    {
        // Raw resource deposits. Provides a basic natural job, or can be exploited by infrastructure.
        public string Name { get; set; }
        public int TotalDeposits { get; set; } // How many deposits exist
        public int AvailableDeposits { get; set; } // How many deposits are currently available for jobs

        //public Job Job { get; set; } // The job that this provides.
        public NaturalResource(string name, int deposits/*, Job job*/)
        {
            Name = name;
            TotalDeposits = deposits;
            AvailableDeposits = deposits;
            //Job = job;
        }
        // At most, maybe it talks to the District
    }
}