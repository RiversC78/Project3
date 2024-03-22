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
        public int SimulationMinutes { get; set; }
        //Percent chance a person will travel each hour of the simulation
        public double TravelChance { get; set; }

        //Method to load configuration from a file
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
                            if(Regex.IsMatch(line, @"^\["))
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
                            //set Simulation Minutes 
                            else if (Regex.IsMatch(line, @"durationOfSimMinutes"))
                            {
                                string[] temp = line.Split("=");
                                string propertySet = temp[1].Trim();
                                SimulationMinutes = int.Parse(propertySet);
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
        }
    }
}
