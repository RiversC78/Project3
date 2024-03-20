/*The Configuration class reads in data from a file that determines the parameters of the
simulation. From the configuration file, the user should be able to adjust the following:
 The mean population size of a location at the start of the simulation.
 The standard deviation of the population size of a location at the start of the simulation.
 The percent chance of the disease spreading to another person at the same location.
 The percent chance of the disease flagging a person as dead.
 The number of hours that the disease last.
 The number of hours that a quarantine last.
 The mean percent chance of a person entering quarantine.
 The standard deviation of the percent chance of a person entering quarantine.
 How long the simulation should last.
 The percent chance that a person will travel each hour of the simulation.
(Note: From the configurable settings above, you may extrapolate additional requirements.
 */
namespace Project3
{
    /// <summary>
    /// Reads in data from a file that determines the parameneters of the simulation.
    /// </summary>
    public class Configuration
    {
        //Properties
        //Mean population size of a location at the start of the simulation
        public int MeanPopulationSize { get; set; }
        //Standard Deviation of the population size of a location at the start of the simulation
        public int StDevPopulationSize { get; set; }
        //Percent chance of a disease spreading to another person
        public double SpreadChance { get; set; }
        //Percent chance of a disease flagging someone as dead
        public double DeathChance { get; set; }
        //The number of hours the disease lasts
        public int DiseaseHours { get; set; }
        //The number of hours quarantine lasts
        public int QuarantineHours { get; set; }
        //Mean percent chance someone enters quarantine
        public double MeanQuarantineChance { get; set; }
        //Standard Deviation of the percent chance of a person entering quarantine
        public double StDevQuarantineChance { get; set; }
        //How long the simulation lasts
        public int SimulationHours { get; set; }
        //Percent chance a person will travel each hour of the simulation
        public double TravelChance { get; set; }

        //Method to load configuration from a file
        public void LoadConfiguration(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                MeanPopulationSize = int.Parse(lines[0]);
                StDevPopulationSize = int.Parse(lines[1]);
                SpreadChance = double.Parse(lines[2]);
                DeathChance = double.Parse(lines[3]);
                DiseaseHours = int.Parse(lines[4]);
                QuarantineHours = int.Parse(lines[5]);
                MeanQuarantineChance = double.Parse(lines[6]);
                StDevQuarantineChance = double.Parse(lines[7]);
                SimulationHours = int.Parse(lines[8]);
                TravelChance = double.Parse(lines[9]);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to load configuration");
            }
        }
    }
}
