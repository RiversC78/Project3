namespace Project3
{
    internal class Program
    {
        //Variable for the StreamWriter

        static void Main(string[] args)
        {

            // File path for easy changing based off of where file is
            string filePath = @"C:\Users\mgrac\OneDrive\Desktop\ConfigFile1.ini";
            //string filePath = @"C:\Users\xarsk\source\repos\Project3\ConfigFile1.ini";

            //filepath for csv
            string csvFilePath = @"C:\Users\mgrac\OneDrive\Desktop\csvfolder\simulation.csv";

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

            //Create two locations
            Location location1 = CreateInfectedLocation("location 1", config);
            Location location2 = CreateLocation("location 2", config);

            //used for final report at the end of the simulation
            int totalPeople = location1.people.Count + location2.people.Count;

            //The locations are neighbors
            location1.neighbors.Add(location2);
            location2.neighbors.Add(location1);

            List<Location> locations = new List<Location> { location1, location2 };

            int totalSimulationMinutes = 0;
            int currentTime = 0;

            List<Person> peopleToMove = new List<Person>();


            //Runs the simulation until the total simulation time reaches the configuration's total time
            while (totalSimulationMinutes < config.SimulationMinutes)
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
                            }

                            if (person.IsQuarantined)
                            {
                                QuarantinedCount++;
                            }
                        }
                    }
                    //Resets the list of people to move
                    peopleToMove.Clear();


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
                            writer.WriteLine($"  -  Person who has contacted the most disease: Person {mostInfectedPerson.Id}");
                        }
                        else
                        {
                            writer.WriteLine($"  -  Person who has contacted the most disease: no one.");
                        }
                        if (highestSpreadCount != null)
                        {
                            writer.WriteLine($"  -  Person who has spread the most disease: Person {highestSpreadCount.Id}");
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

                    //Increments the total simulation minutes by an hour
                    totalSimulationMinutes += 60;



                    //When the total simulation minutes reaches the time specified in the configuration, it ends
                    if (totalSimulationMinutes >= config.SimulationMinutes)
                    {
                        break;
                    }
                }
            }
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
            Console.WriteLine($"Total run time: {totalSimulationMinutes} minutes");
            //TODO: Still needs total infected over the course of the simulation
            Console.WriteLine($"Total infected: ");
            Console.WriteLine($"Total deaths: ");
            //TODO: still need percent infected
            Console.WriteLine($"Percent infected: ");
            Console.WriteLine($"Percent dead: ");
            //TODO: still need percent infected on average
            Console.WriteLine($"Percent infected on average: ");
            //TODO: Still need this number
            Console.WriteLine($"Average number of people an infected person spread to: ");
            //TODO: still need this number
            Console.WriteLine($"Maximum number of people an infected person spread to: ");

            foreach (Location location in locations)
            {
                Console.WriteLine($" ---- Location ");
                Console.WriteLine($"Average population size: ");
                Console.WriteLine($"Average percent of people sick with disease: ");
                Console.WriteLine($"Average percent of people in quarantine: ");
            }


            File.WriteAllText(csvFilePath, string.Empty);
        }
        //End of Main

        //Creates a location with a mean and standard deviation of people
        public static Location CreateLocation(string locationId, Configuration config)
        {
            // Generate population size for the location based on mean and standard deviation
            int populationSize = (int)Math.Round(config.MeanPopulationSize + (config.StDevPopulationSize * RandomGaussian()));

            // Generate people for the location
            List<Person> people = config.GeneratePeople(populationSize, config);

            // Create the location
            Location location = new Location(locationId);

            foreach (Person person in people)
            {
                location.people.Add(person);
            }

            return location;
        }

        //Creates a location that has an infected person
        public static Location CreateInfectedLocation(string locationId, Configuration config)
        {
            //Generate population size for the location based on mean and standard deviation
            int populationSize = (int)Math.Round(config.MeanPopulationSize + (config.StDevPopulationSize * RandomGaussian()));

            //Generate people for the location
            List<Person> people = config.GeneratePeople(populationSize, config);

            //Select a person to be patient zero
            PatientZero(people);

            // Create the location
            Location location = new Location(locationId);

            foreach (Person person in people)
            {
                location.people.Add(person);
            }

            return location;
        }
        //Randomly picks a person to have the disease
        public static void PatientZero(List<Person> people)
        {
            Random random = new Random();
            int patientZero = random.Next(0, people.Count);
            people[patientZero].IsInfected = true;
        }


        //Generates numbers using a normal distribution
        public static double RandomGaussian()
        {
            Random rand = new Random();
            double u1 = 1.0 - rand.NextDouble(); // Uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // Box-Muller transform
            return randStdNormal;
        }
    }
}