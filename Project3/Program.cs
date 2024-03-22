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

            //locations and people made to test methods
            Location location1 = new Location("Location1");
            Location location2 = new Location("Location2");

            location1.people.Add(new Person("Person 1", 7, 22, true, 0, 0, false, false, .5, .5, 0));
            location1.people.Add(new Person("Person 2", 7, 22, false, 0, 0, false, false, .5, .5, 0));
            location1.people.Add(new Person("Person 3", 7, 22, false, 0, 0, false, false, .5, .5, 0));

            location2.people.Add(new Person("Person 4", 7, 22, false, 0, 0, false, false, .5, .5, 0));
            location2.people.Add(new Person("Person 5", 7, 22, false, 0, 0, false, false, .5, .5, 0));
            location2.people.Add(new Person("Person 6", 7, 22, false, 0, 0, false, false, .5, .5, 0));

            List<Location> locations = new List<Location> { location1, location2 };

            Queue<Person> infectedQueue = new Queue<Person>();

            for (int hour = 0; hour < 24; hour++)
            {
                foreach (Location location in locations)
                {
                    foreach (Person person in location.people)
                    {
                        if (hour >= person.TravelStartTime && hour <= person.TravelEndTime)
                        {
                            location.MovePeople();
                            location.SpreadDisease(config.SpreadChance);

                            if (person.IsInfected)
                            {
                                if (person.InfectionTime >= 2)
                                {
                                    person.IsInfected = false;
                                }
                                else
                                {
                                    person.InfectionTime++;
                                    infectedQueue.Enqueue(person);
                                }
                            }
                        }
                    }
                }


                //Determines the total amount of infection spread
                int totalInfectedCount = 0;
                foreach (var Location in locations)
                {
                    foreach (var person in Location.people)
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

                //Determines what percentage of people were infected at the end of the simulation
                int totalPeople = 0;
                int totalInfected = 0;
                double infectionPercent;
                foreach (var Location in locations)
                {
                    totalPeople += Location.people.Count;

                    foreach (var person in Location.people)
                    {
                        if (person.IsInfected)
                        {
                            totalInfected++;
                        }
                    }
                }
                //Determines the percentage of people infected
                infectionPercent = totalInfected / totalPeople * 100;

                //Determines percentage of dead people.
                double deathPercent = totalDeaths / totalPeople * 100;
            }
        }//end Main
    }//end class
}//end namespace

