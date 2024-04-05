using System.Diagnostics;
namespace Project3
{
    /// <summary>
    /// container for main program
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// container for main program entry point 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            // File path for easy changing based off of where file is
            //string filePath = @"C:\Users\mgrac\OneDrive\Desktop\ConfigFile1.ini";
            string filePath = @"C:\Users\xarsk\source\repos\Project3\ConfigFile1.ini";

            //filepath for csv
            //string csvFilePath = @"C:\Users\mgrac\OneDrive\Desktop\csvfolder\simulation.csv";
            string csvFilePath = @"C:\Users\xarsk\downloads\simulation.csv";

            //filepath for csv of where different people are during each hour
            //mainly for debugging and seeing how people move 
            //string wherePeopleAreList = @"C:\Users\mgrac\OneDrive\Desktop\csvfolder\PeopleList.csv";
            string wherePeopleAreList = @"C:\Users\xarsk\downloads\PeopleList.csv";

            //Variables used for CSV file metrics
            int day = 0;
            //used for finding who has come into contact with the disease the most
            int highestInfectionCount = 0;
            Person mostInfectedPerson = null;
            //used for finding who has spread the disease the most
            int highestSpreadCountNum = 0;
            Person highestSpreadCount = null;
            int aliveCount = 0;
            int deadCount = 0;
            int infectedCount = 0;
            int QuarantinedCount = 0;

            //load in configuration information 
            Configuration config = new Configuration();
            config.LoadConfiguration(filePath);

            //make empty list of locations to add to for passable parameter to Create the locations (specifically for GeneratePeople())
            List<Location> locations = new List<Location> { };
            //With current method for location creation, Infected location must always be *created first*
            //Create two locations and a list of all locations 
            Location location1 = Location.CreateInfectedLocation("location 1", config);
            locations.Add(location1);
            Location location2 = Location.CreateLocation("location 2", config, locations);
            locations.Add(location2);

            //variables used for final report at the end of the simulation
            int totalPeople = location1.people.Count + location2.people.Count;
            int totalDeaths = 0;
            int infectedCountPercent = 0;
            int infectedThisHour = 0;
            //starts at one because there will always be one person infected to start with
            int totalInfected = 1;
            int totalSimulationHours = 1;
            List<int> infectedPerHour = new List<int>();
            List<int> infectionsPerPerson = new List<int>();
            List<double> percentInfectedPerHour = new List<double>();

            //The locations are neighbors
            location1.neighbors.Add(location2);
            location2.neighbors.Add(location1);

            List<Person> peopleToMove = new List<Person>();

            //stopwatch to time how long the sim runs in real time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //Runs the simulation until the total simulation time reaches the configuration's total time
            while (totalSimulationHours < config.SimulationHours)
            {
                //Simulates each hour of the day
                for (int hour = 0; hour < 24; hour++)
                {
                    //Before everyone has moved, spread disease
                    foreach (Location location in locations)
                    {
                        foreach (Person person in location.people)
                        {
                            //If a person isn't quarantined or dead and is infected, they may spread the disease
                            if (person.CanSpread())
                            {
                                location.SpreadDisease(config.SpreadChance, person);
                            }

                            //Checks who has contacted the disease most this hour
                            if (person.InfectionCount > highestInfectionCount)
                            {
                                highestInfectionCount = person.InfectionCount;
                                mostInfectedPerson = person;
                            }

                            //Checks who has spread the disease the most this hour
                            if (person.InfectionSpreadCount > highestSpreadCountNum)
                            {
                                highestSpreadCountNum = person.InfectionSpreadCount;
                                highestSpreadCount = person;
                            }

                            if (person.IsDead)
                            {
                                deadCount++;
                            }
                            else
                            {
                                aliveCount++;
                            }

                            if (person.IsInfected)
                            {
                                infectedCount++;
                                infectedThisHour++;
                                totalInfected++;
                                //Adds to amount someone has spread disease to a list, used for later calculation
                                infectionsPerPerson.Add(person.InfectionSpreadCount);
                            }

                            if (person.IsQuarantined)
                            {
                                QuarantinedCount++;
                            }
                        }
                        //Updates the population sizes for final report calculations
                        location.UpdatePopSize();
                        //Updates the amount of sick people at each location
                        location.UpdateSickCount();
                        //Adds to list to calculate the average of people quarantining
                        location.UpdateQuarantineCount();
                    }
                    //Adds to list to calculate the average infected per hour
                    infectedPerHour.Add(infectedThisHour);

                    //Resets the list of people to move
                    peopleToMove.Clear();

                    //Reset variable
                    infectedThisHour = 0;

                    //Data to log to CSV:
                    // The person’s information that has been infected the most
                    // The person’s information that has spread the disease the most
                    // The number of people currently not dead
                    // The number of people currently dead
                    // The number of people currently infected with the disease
                    // The number of people currently quarantined

                    //Write metrics to CSV 
                    using (StreamWriter writer = new StreamWriter(new FileStream(csvFilePath, FileMode.Append, FileAccess.Write)))
                    {
                        writer.WriteLine($"--- Day {day}  - Hour {hour} ---");
                        if (mostInfectedPerson != null)
                        {
                            writer.WriteLine($"  -  Person who has contacted the most disease: {mostInfectedPerson.Id}");
                        }
                        else
                        {
                            writer.WriteLine($"  -  Person who has contacted the most disease: no one.");
                        }
                        if (highestSpreadCount != null)
                        {
                            writer.WriteLine($"  -  Person who has spread the most disease: {highestSpreadCount.Id}");
                        }
                        else
                        {
                            writer.WriteLine($"  -  Person who has spread the most disease: no one.");
                        }
                        writer.WriteLine($"  -  People alive: {aliveCount}");
                        writer.WriteLine($"  -  People dead: {deadCount}");
                        writer.WriteLine($"  -  People infected: {infectedCount}");
                        writer.WriteLine($"  -  People Quarantined: {QuarantinedCount} \n");
                        writer.Close();
                    }//end using

                    //Write where people are at to a csv for easy debugging
                    using (StreamWriter writer = new StreamWriter(new FileStream(wherePeopleAreList, FileMode.Append, FileAccess.Write)))
                    {
                        writer.WriteLine($"--- Day {day}  - Hour {hour} ---");
                        foreach (var location in locations)
                        {
                            writer.WriteLine($"{location.Id} \n");
                            foreach (var person in location.people)
                            {
                                writer.WriteLine($"{person.Id} - Infected?: {person.IsInfected} - Quarantined?: {person.IsQuarantined} - Dead?: {person.IsDead}");
                            }
                            writer.WriteLine("-----------");
                        }
                    }//end using

                    //check where people are, update their characteristics after the disease spreads
                    foreach (Location location in locations)
                    {
                        foreach (Person person in location.people)
                        {
                            //Updates how long someone has been in quarantine
                            person.Update(person);

                            //If a person isn't quarantined or dead and the hour is in their travel time, they may move.
                            if (person.ShouldMove(hour))
                            {
                                peopleToMove.Add(person);
                            }
                        }
                    }

                    //For everyone that is supposed to move, they are moved
                    foreach (Person person in peopleToMove)
                    {
                        foreach (Location location in locations)
                        {
                            if (location.people.Contains(person))
                            {
                                location.MovePeople(person);
                                break;
                            }
                        }
                    }
                    deadCount = 0;
                    aliveCount = 0;
                    infectedCount = 0;
                    QuarantinedCount = 0;

                    //increment total sim hours as an hour has passed
                    totalSimulationHours++;

                    //When the total simulation hours reaches the time specified in the configuration, it ends
                    if (totalSimulationHours >= config.SimulationHours)
                    {
                        break;
                    }
                    //check if everyone is dead and break loop early if so - jump to final report section
                    if (Person.IsDiseaseDead(locations))
                    {
                        goto FinalReport;
                    }

                }//end hour 
                day++;
            }//end main sim while loop
            //end stopwatch time for total sim 
            stopwatch.Stop();
        //identifier for jump point if everyone is dead 
        FinalReport:
            //Finds data for end report
            foreach (Location location in locations)
            {

                foreach (Person person in location.people)
                {
                    if (person.IsDead)
                    {
                        totalDeaths++;
                    }
                    if (person.IsInfected)
                    {
                        infectedCountPercent++;
                    }
                }
            }
            //Calculates the percentage of people infected
            double infectionPercentage = (double)infectedCountPercent / totalPeople * 100;
            //Calculates the percentage of people are dead
            double deathPercent = (double)totalDeaths / totalPeople * 100;
            //Calculates the average percent of people infected each hour
            double averageInfectedPerHour = infectedPerHour.Average();
            //Calculates the average number of people an infected person spread the disease to
            double averageInfectionsPerPerson;
            //checks if the disease was spread at all or if the first person who got the disease died before spreading it
            if (infectionsPerPerson.Count == 0)
            {
                averageInfectionsPerPerson = 0;
            }
            else
            {
                averageInfectionsPerPerson = infectionsPerPerson.Average();
            }
            //Finds the maximum number of infections caused by a person
            int maxInfectionPerPerson;
            //checks if the disease was spread at all or if the first person who got the disease died before spreading it
            if (infectionsPerPerson.Count == 0)
            {
                maxInfectionPerPerson = 0;
            }
            else
            {
                maxInfectionPerPerson = infectionsPerPerson.Max();
            }

            //Once the simulation is complete, it should generate a report with the following information:
            // How long did the simulation run
            // How many people were infected over the course of the simulation
            // How many people were died over the course of the simulation
            // What percentage of people were infected at the end of the simulation
            // What percentage of people were dead at the end of the simulation
            // What percentage of people were infected on average
            // What was the average number of people an infected person spread the disease to
            // What was the maximum number of people an infected person spread the disease to
            // The following may need to be placed into a separate heading or report:
            //      o What was the average population size at each location
            //      o What was the average percent of people sick with the disease at each location
            //      o What was the average percent of people in quarantine at each location


            Console.WriteLine(" ---- Final Report: ");
            Console.WriteLine($"Total sim run time: {totalSimulationHours} hours in simulation time");
            Console.WriteLine($"Maximum sim run time: {config.SimulationHours} hours in simulation time");
            Console.WriteLine($"Total real run time: {stopwatch.ElapsedMilliseconds} milliseconds in real time");
            Console.WriteLine($"Total infected: {totalInfected}");
            Console.WriteLine($"Total deaths: {totalDeaths}");
            Console.WriteLine($"Percent currently infected: {infectionPercentage} %");
            Console.WriteLine($"Percent dead: {deathPercent} %");
            Console.WriteLine($"Percent infected on average at a given time: {averageInfectedPerHour} %");
            Console.WriteLine($"Average number of people an infected person spread to: {averageInfectionsPerPerson}");
            Console.WriteLine($"Maximum number of people an infected person spread to: {maxInfectionPerPerson}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("--------------");
            Console.ForegroundColor = ConsoleColor.White;

            foreach (Location location in locations)
            {
                double avgPop = location.AveragePopulation();
                double avgInfected = location.AverageInfected() * 100;
                double avgQuarantined = location.AverageQuarantined() * 100;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($" ---- Location {location.Id}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Average population size: {avgPop}");
                Console.WriteLine($"Average percent of people sick with disease: {avgInfected} %");
                Console.WriteLine($"Average percent of people in quarantine: {avgQuarantined} %");
            }

            File.WriteAllText(csvFilePath, string.Empty);
            File.WriteAllText(wherePeopleAreList, string.Empty);
        }//End of Main
    }//end class
}//end namespace

