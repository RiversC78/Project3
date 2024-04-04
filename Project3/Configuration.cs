
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
using System.Text.RegularExpressions;

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
        //Percent chance of a disease spreading to another person value between 0-100
        public double SpreadChance { get; set; }
        //Percent chance of a disease flagging someone as dead value between 0-100 as a percentage
        public double DeathChance { get; set; }
        //The number of hours the disease lasts
        public int DiseaseHours { get; set; }
        //The number of hours quarantine lasts
        public int QuarantineHours { get; set; }
        //Mean percent chance someone enters quarantine value between 0-100 as a percentage
        public double MeanQuarantineChance { get; set; }
        //Standard Deviation of the percent chance of a person entering quarantine value between 0-100 as a percentage
        public double StDevQuarantineChance { get; set; }
        //How long the simulation lasts
        public int SimulationHours { get; set; }
        //Percent chance a person will travel each hour of the simulation value between 0-100 as a percentage
        public double TravelChance { get; set; }

        /// <summary>
        /// method to load information from a config file
        /// </summary>
        /// <param name="filePath">filepath to config file</param>
        /// <exception cref="ArgumentException"></exception>
        public void LoadConfiguration(string filePath)
        {
            try
            {
                //check if file exists
                if (File.Exists(filePath))
                {
                    //create a line count for the file 
                    var lineCount = 0;
                    using (var reader = File.OpenText(filePath))
                    {
                        while (reader.ReadLine() != null)
                        {
                            lineCount++;
                        }
                    }
                    //read all the lines in the file and set equal to properties 
                    using (var reader = File.OpenText(filePath))
                    {

                        for (int i = 1; i <= lineCount; i++)
                        {
                            //skip lines that start with "[" as those are section headers in a .ini file
                            string line = reader.ReadLine();
                            if (Regex.IsMatch(line, @"^\["))
                            {
                                continue;
                            }
                            else if (Regex.IsMatch(line, @"^\s*$"))
                            {
                                continue;
                            }

                            //set DeathChance
                            else if (Regex.IsMatch(line, @"chanceDiseaseKills"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();

                                //DeathChance = int.Parse(propertySet);

                                DeathChance = double.Parse(propertySet);

                            }
                            //set DiseaseHours
                            else if (Regex.IsMatch(line, @"hoursDiseaseLasts"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                DiseaseHours = int.Parse(propertySet);
                            }
                            //set QuarantineHours
                            else if (Regex.IsMatch(line, @"hoursQuarantineLasts"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                QuarantineHours = int.Parse(propertySet);
                            }
                            //set MeanQuarantineChance
                            else if (Regex.IsMatch(line, @"meanChanceQuarantine"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                MeanQuarantineChance = double.Parse(propertySet);
                            }
                            //set StDevQuarantineChance
                            else if (Regex.IsMatch(line, @"standDevChanceQuarantine"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                StDevQuarantineChance = double.Parse(propertySet);
                            }
                            //set Simulation Hours 
                            else if (Regex.IsMatch(line, @"durationOfSimHours"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                SimulationHours = int.Parse(propertySet);
                            }
                            //set TravelChance
                            else if (Regex.IsMatch(line, @"chanceOfTravel"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                TravelChance = double.Parse(propertySet);
                            }
                            //set SpreadChance
                            else if (Regex.IsMatch(line, @"chanceDiseaseSpread"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                SpreadChance = double.Parse(propertySet);
                            }
                            //set MeanPopulationSize
                            else if (Regex.IsMatch(line, @"meanPopSize"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                MeanPopulationSize = int.Parse(propertySet);
                            }
                            //set StDevPopulationSize
                            else if (Regex.IsMatch(line, @"standDevPopSize"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                StDevPopulationSize = (int)double.Parse(propertySet);
                            }
                        }
                    }
                }
                else { throw new ArgumentException("No file found"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to load configuration");
            }

        }//end LoadConfiguration

        /// <summary>
        /// Generates people based on configuration settings
        /// </summary>
        /// <param name="peopleCount">number of people to generate</param>
        /// <param name="config">configuration file attributes</param>
        /// <returns>list of people created</returns>
        public List<Person> GeneratePeople(int peopleCount, Configuration config, int peopleAlreadyExisting = 0)
        {
            //Create a list of people
            List<Person> people = new List<Person>();

            Random rand = new Random();

            for (int i = 0; i < peopleCount; i++)
            {
                string id = $"Person_{i + peopleAlreadyExisting}";
                //People may begin travelling between hours 0-23
                int travelStartTime = rand.Next(0, 24);
                //travelEndTime can only be after travelStartTime
                int travelEndTime = rand.Next(travelStartTime + 1, 25);
                //Everyone starts out not infected
                bool isInfected = false;
                int infectionCount = 0;
                int infectionSpreadCount = 0;
                //Everyone is alive at generation
                bool isDead = false;
                //No one is quarantined at generation
                bool isQuarantined = false;
                //Quarantine chance is taken from the configuration
                double quarantineChance = GenerateQuarantineChance();
                double travelChance = TravelChance;



                //Create person object
                Person person = new Person(id, travelStartTime, travelEndTime, isInfected,
                                            infectionCount, infectionSpreadCount, isDead,
                                            isQuarantined, quarantineChance, travelChance, 0, 0, config);


                //Generated people are added to a list
                people.Add(person);
            }
            return people;
        }//end GeneratePeople

        /// <summary>
        /// Creates the chance that someone will quarantine or not with statistics
        /// </summary>
        /// <returns></returns>
        private double GenerateQuarantineChance()
        {
            //Generate quarantine chance based on a normal random distribution
            Random rand = new Random();
            //Mean and stardard deviation of quarantine chance are taken from configuration
            double mean = MeanQuarantineChance;
            double stdDev = StDevQuarantineChance;
            //Chance clamped between 0 and 100, to be used like a percentage
            double chance = rand.NextGaussian(mean, stdDev);
            //Chance clamped between 0 and 100, to be used like a percentage
            //TODO instructions say to clamp between 0 and 1, checking with teacher if this method is fine or if I need to change it
            return chance; 
        }//end GenerateQuarantineChance
    }
}//end namespace
