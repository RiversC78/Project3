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
            Location location1 = CreateLocation("location 1", config);
            Location location2 = CreateLocation("location 2", config);


            //The locations are neighbors
            location1.neighbors.Add(location2);
            location2.neighbors.Add(location1);

            List<Location> locations = new List<Location> { location1, location2 };

            int totalSimulationMinutes = 0;

            //loop for ebery person that updates attributes
            //loop for every location thats spreads 
            int currentTime = 0;


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
                            person.Update(currentTime, config.QuarantineHours);

                            //If a person isn't quarantined or dead and the hour is in their travel time, they may move.
                            if (person.ShouldMove(hour))
                            {
                                location.MovePeople();
                            }
                            //If a person isn't quarantined or dead, they may spread the disease
                            if (person.CanSpread())
                            {
                                location.SpreadDisease(config.SpreadChance);
                            }

                            //Checks who has contacted the disease most this hour
                            if (person.InfectionCount > highestInfectionCount)
                            {
                                highestInfectionCount= person.InfectionCount;
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
                            } else
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
                        writer.WriteLine($"  -  People Quarantined: {QuarantinedCount}");
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
            Console.WriteLine(" ---- Final Report: ");
            Console.Write($"Total run time: ");

        }
        public static Location CreateLocation(string locationId, Configuration config)
        {
            // Generate population size for the location based on mean and standard deviation
            int populationSize = (int)Math.Round(config.MeanPopulationSize + (config.StDevPopulationSize * RandomGaussian()));

            // Generate people for the location
            List<Person> people = config.GeneratePeople(populationSize);

            // Create the location
            Location location = new Location(locationId);

            foreach (Person person in people)
            {
                location.people.Add(person);
            }

            return location;
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