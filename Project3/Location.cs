/*The Location class represents a population or community center. It has the following properties:
string id – A location’s unique identifier to assist with debugging and analysis.
ICollection<Person> people – A collection of people currently at the location.
ICollection<Location> neighbors – A collection of nodes connected to the current
node.
Each hour of the simulation, people at the same location should have an opportunity to spread the
disease to other people at the same location. After the spreading of the disease occurs, then
people should have the opportunity to move to a neighboring location.
People that are dead do not spread the disease and do not travel ever again.
People that are quarantined do not spread the disease and do not travel, but may resume both
once the quarantine period is complete
 */
namespace Project3
{
    /// <summary>
    /// Class to hold location information about the people at a certain location and the locations directly next to itself
    /// </summary>
    public class Location
    {
        //location id 
        public string Id { get; set; }
        //list of people at location at any given time
        public ICollection<Person> people { get; set; }
        //list of all neighboring locations of a given location
        public ICollection<Location> neighbors { get; set; }
        //list of all population sizes of a location from different times
        public List<int> populationSizes { get; set; }
        //list of all total number of sick people at a location from different times
        public List<int> SickCount { get; set; }
        //list of percentages from each hour of how many people were infected at a location
        public List<double> infectedAverage { get; set; }
        //list of total number of people in quarantine at a location from different times
        public List<int> QuarantineCount { get; set; }
        //List of percentages from each hour of how many people are in quarantine at a location
        public List<double> quarantinedAverage { get; set; }

        //Location constructor
        public Location(string id)
        {
            Id = id;
            people = new List<Person>();
            neighbors = new List<Location>();
            populationSizes = new List<int>();
            SickCount = new List<int>();
            infectedAverage = new List<double>();
            QuarantineCount = new List<int>();
            quarantinedAverage = new List<double>();
        }
        /// <summary>
        /// Method to spread the disease 
        /// </summary>
        /// <param name="spreadChance">chance to spread the disease</param>
        /// <param name="currentPerson">person who may spread the disease</param>
        public void SpreadDisease(double spreadChance, Person currentPerson)
        {
            Random random = new Random();
            foreach (Person otherPerson in people)
            {
                if (currentPerson == otherPerson)
                {
                    continue;
                }

                int chance = random.Next(0, 101);
                if (chance > spreadChance && !otherPerson.IsQuarantined)
                {
                    otherPerson.IsInfected = true;
                    otherPerson.InfectionCount++;
                    currentPerson.InfectionSpreadCount++;

                    //Determines if the newly infected person will quarantine
                    chance = random.Next(0, 101);
                    if (otherPerson.QuarantineChance < chance)
                    {
                        otherPerson.IsQuarantined = true;
                    }
                }
            }
        }//end SpreadDisease
        /// <summary>
        /// moves a person to a neighboring location
        /// </summary>
        /// <param name="person">person to be moved</param>
        public void MovePeople(Person person)
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 101);
            if (randomNumber < person.TravelChance)
            {
                int pickNeighbor = random.Next(0, neighbors.Count());
                Location chosenNeighbor = neighbors.ElementAt(pickNeighbor);

                chosenNeighbor.people.Add(person);
                people.Remove(person);
            }
        }//end MovePeople

        /// <summary>
        /// Updates the population size
        /// </summary>
        public void UpdatePopSize()
        {
            populationSizes.Add(people.Count);
        }//end UpdatePopSize

        /// <summary>
        /// Averages the population sizes
        /// </summary>
        /// <returns>current population size and adds it to the list</returns>
        public double AveragePopulation()
        {
            return populationSizes.Average();
        }

        /// <summary>
        /// Stores how many infected people at a location at that hour in a list
        /// </summary>
        public void UpdateSickCount()
        {
            int sickCount = 0;
            foreach (Person person in people)
            {
                if (person.IsInfected)
                {
                    sickCount++;
                }
            }
            SickCount.Add(sickCount);
        }//end UpdateSickCount

        /// <summary>
        /// Stores how many quarantined people at a location at that hour in a list
        /// </summary>
        public void UpdateQuarantineCount()
        {
            int quarantineCount = 0;
            foreach (Person person in people)
            {
                if (person.IsQuarantined)
                {
                    quarantineCount++;
                }
            }
            QuarantineCount.Add(quarantineCount);
        }//end UpdateQuarantineCount

        /// <summary>
        /// Averages the amount of infected people at the location
        /// </summary>
        /// <returns></returns>
        public double AverageInfected()
        {
            for (int i = 0; i < SickCount.Count; i++)
            {
                double percentInfected = (double)SickCount.ElementAt(i) / populationSizes.ElementAt(i);
                infectedAverage.Add(percentInfected);
            }
            return infectedAverage.Average();

        }//end AverageInfected

        /// <summary>
        /// Averages the amoount of quarantined people at the location
        /// </summary>
        /// <returns></returns>
        public double AverageQuarantined()
        {
            for (int i = 0; i < QuarantineCount.Count; i++)
            {
                double percentQuarantined = (double)QuarantineCount.ElementAt(i) / populationSizes.ElementAt(i);
                quarantinedAverage.Add(percentQuarantined);

            }
            return quarantinedAverage.Average();
        }

        /// <summary>
        /// Creates a location that has the first infected person
        /// </summary>
        /// <param name="locationId"> location unique identifier</param>
        /// <param name="config">configuration file attributes</param>
        /// <returns>Location with patient zero</returns>
        public static Location CreateInfectedLocation(string locationId, Configuration config)
        {
            //Generate population size for the location based on mean and standard deviation
            int populationSize = (int)Math.Round(config.MeanPopulationSize + (config.StDevPopulationSize * MathOperations.RandomGaussian()));

            //Generate people for the location
            List<Person> people = config.GeneratePeople(populationSize, config);

            //Select a person to be patient zero
            Person.PatientZero(people);

            // Create the location
            Location location = new Location(locationId);

            foreach (Person person in people)
            {
                location.people.Add(person);
            }

            return location;
        }//end CreateInfectedLocation

        /// <summary>
        /// Creates a location with a mean and standard deviation of people
        /// </summary>
        /// <param name="locationId">unique location identifier</param>
        /// <param name="config">configuration file attributes</param>
        /// <returns></returns>
        public static Location CreateLocation(string locationId, Configuration config, List<Location> locationList)
        {
            //Get the number of people at all previously created locations 
            //starts at negative one to take into account person 0 with Count() method
            int numberOfCurrentPeople = -1;
            foreach (Location place in locationList)
            {
                numberOfCurrentPeople = place.people.Count();
            }
            // Generate population size for the location based on mean and standard deviation
            int populationSize = (int)Math.Round(config.MeanPopulationSize + (config.StDevPopulationSize * MathOperations.RandomGaussian()));

            // Generate people for the location
            List<Person> people = config.GeneratePeople(populationSize, config, numberOfCurrentPeople);

            // Create the location
            Location location = new Location(locationId);

            foreach (Person person in people)
            {
                location.people.Add(person);
            }

            return location;
        }//end CreateLocation
    }//end class
}//end namespace
