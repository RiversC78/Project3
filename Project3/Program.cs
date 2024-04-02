﻿namespace Project3
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
            string csvFilePath = @"C:\Users\xarsk\downloads\simulation.csv"; ;

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

            //Create two locations and a list of all locations 
            Location location1 = Location.CreateInfectedLocation("location 1", config);
            Location location2 = Locaiton.CreateLocation("location 2", config);

            //variables used for final report at the end of the simulation
            int totalPeople = location1.people.Count + location2.people.Count;
            int totalDeaths = 0;
            int infectedCountPercent = 0;
            int infectedThisHour = 0;
            int totalInfected = 0;
            List<int> infectedPerHour = new List<int>();
            List<int> infectionsPerPerson = new List<int>();

            //The locations are neighbors
            location1.neighbors.Add(location2);
            location2.neighbors.Add(location1);

            List<Location> locations = new List<Location> { location1, location2 };

            int totalSimulationHours = 0;
            int totalCompletedSimDays = 0;

            List<Person> peopleToMove = new List<Person>();

            //Runs the simulation until the total simulation time reaches the configuration's total time
            while (totalSimulationHours < config.SimulationHours)
            {
                //Simulates each hour of the day
                for (int hour = 0; hour < 24; hour++)
                {
                    foreach (Location location in locations)
                    {
                        foreach (Person person in location.people)
                        {
                            //**TODO: Update still needs to be fixed. 
                            //Updates how long someone has been in quarantine

                            person.Update();

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

                    //After everyone has moved, spread disease
                    foreach (Location location in locations)
                    {
                        foreach (Person person in location.people)
                        {
                            //If a person isn't quarantined or dead, they may spread the disease
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
                    }
                    //Adds to list to calculate the average infected per hour
                    infectedPerHour.Add(infectedThisHour);

                    //Resets the list of people to move
                    peopleToMove.Clear();

                    //Reset variable
                    infectedThisHour = 0;


                    //Data to log to CSV:
                    // The person’s information that has been infected the most
                    // The person’s information that has spread the disease the most
                    // The number of people currently not dead
                    // The number of people currently dead
                    // The number of people currently infected with the disease
                    // The number of people currently quarantined

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
                    if (Person.IsEveryoneDead(locations))
                    {
                        goto FinalReport; 
                    }
                }//end hour 
                totalCompletedSimDays++; 
            }//end main sim while loop
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
            double averageInfectionsPerPerson = infectionsPerPerson.Average();
            //Finds the maximum number of infections caused by a person
            int maxInfectionPerPerson = infectionsPerPerson.Max();

            //Once the simulation is complete, it should generate a report with the following information:
            // How long did the simulation run
            // How many people were infected over the course of the simulation
            // How many people were died over the course of the simulation
            // What percentage of people were infected at the end of the simulation
            // What percentage of people were dead at the end of the simulation
            // What percentage of people were infected on average
            // What was the average number of people an infected person spread the disease to
            // What was the maximum number of people an infected person spread the disease to
            // The following may need to be placed into a separate heading or report:
            //      o What was the average population size at each location
            //      o What was the average percent of people sick with the disease at each location
            //      o What was the average percent of people in quarantine at each location


            Console.WriteLine(" ---- Final Report: ");
            Console.WriteLine($"Total run time: {totalSimulationHours} hours in simulation time");
            Console.WriteLine($"Total infected: {totalInfected}");
            Console.WriteLine($"Total deaths: {totalDeaths}");
            Console.WriteLine($"Percent infected: {infectionPercentage}");
            Console.WriteLine($"Percent dead: {deathPercent}");
            Console.WriteLine($"Percent infected on average: {averageInfectedPerHour}");
            Console.WriteLine($"Average number of people an infected person spread to: {averageInfectionsPerPerson}");
            Console.WriteLine($"Maximum number of people an infected person spread to: {maxInfectionPerPerson}");

            foreach (Location location in locations)
            {
                double avgPop = location.AveragePopulation();
                double avgInfected = location.AverageInfected();
                double avgQuarantined;

                Console.WriteLine($" ---- Location {location.Id}");
                Console.WriteLine($"Average population size: {avgPop}");
                //TODO: Fix issue with no one infected at location 2
                Console.WriteLine($"Average percent of people sick with disease: {avgInfected}");
                Console.WriteLine($"Average percent of people in quarantine: ");
            }


            File.WriteAllText(csvFilePath, string.Empty);
        }
        //End of Main

        //Randomly picks a person to have the disease
        public static void PatientZero(List<Person> people)
        {
            Random random = new Random();
            int patientZero = random.Next(0, people.Count);
            people[patientZero].IsInfected = true;
        }
    }
}
