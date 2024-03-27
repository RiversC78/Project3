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
            string filePath = @"C:\Users\xarsk\source\repos\Project3\ConfigFile1.ini";
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
                            //***TO DO: update still needs functionality***
                            person.Update();

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

                            //Data to log to CSV:
                            // The person’s information that has been infected the most
                            // The person’s information that has spread the disease the most
                            // The number of people currently not dead
                            // The number of people currently dead
                            // The number of people currently infected with the disease
                            // The number of people currently quarantined

                            string[] metrics = { };
                            foreach (string data in metrics)
                            {

                            }
                        }
                    }

                    //Increments the total simulation minutes by an hour
                    totalSimulationMinutes += 60;

                    //When the total simulation minutes reaches the time specified in the configuration, it ends
                    if (totalSimulationMinutes >= config.SimulationMinutes)
                    {
                        break;
                    }
                }
            }

            //Determines the total amount of infection spread
            int totalInfectedCount = 0;
            foreach (Location Location in locations)
            {
                foreach (Person person in Location.people)
                {
                    totalInfectedCount += person.InfectionSpreadCount;
                }
            }

            //Determines total amount of deaths
            int totalDeaths = 0;
            foreach (var Location in locations)
            {
                foreach (var person in Location.people)
                {
                    if (person.IsDead)
                    {
                        totalDeaths++;
                    }
                }
            }

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
    }//end class
}//end namespace

