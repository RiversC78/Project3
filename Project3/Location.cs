/*The Location class represents a population or community center. It has the following properties:
 string id – A location’s unique identifier to assist with debugging and analysis.
 ICollection<Person> people – A collection of people currently at the location.
 ICollection<Location> neighbors – A collection of nodes connected to the current
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
        public string Id { get; set; }
        public ICollection<Person> people { get; set; }
        public ICollection<Location> neighbors { get; set; }

        //Location constructor
        public Location(string id)
        {
            Id = id;
            people = new List<Person>();
            neighbors = new List<Location>();
        }
        /// <summary>
        /// Method to spread disease
        /// </summary>
        /// <param name="spreadChance"></param>
        public void SpreadDisease(double spreadChance)
        {
            //for all people created
            foreach (Person person in people)
            {
                if (person.IsInfected && !person.IsDead)
                {
                    foreach (Person otherPerson in people)
                    {
                        if (otherPerson != person && !otherPerson.IsInfected && !otherPerson.IsDead)
                        {
                            double randomValue = new Random().NextDouble();
                            if (randomValue > spreadChance)
                            {
                                otherPerson.IsInfected = true;
                                otherPerson.InfectionCount++;
                                person.InfectionSpreadCount++;
                            }
                        }
                    }
                }
            }
        }//end SpreadDisease

        /// <summary>
        /// Method to continue travel for persons after quarantine
        /// </summary>
        public void TravelAfterQuarantine()
        {
            foreach (Person person in people)
            {
                if (person.IsQuarantined)
                {
                    Random rand = new Random();
                    double randomValue = rand.Next(1,101);

                    if (randomValue < person.QuarantineChance)
                    {
                        person.IsQuarantined = false;
                    }
                }
            }
        }//end TravelAfterQuarantine
        /// <summary>
        /// Method to move people around from locations 
        /// </summary>
        public void MovePeople()
        {
            foreach (Person person in people)
            {

                if (!person.IsDead && !person.IsQuarantined)
                {
                    foreach (var neighbor in neighbors)
                    {
                        Random rand = new Random();
                        double randomValue = rand.Next(1, 101);
                        if (randomValue < person.TravelChance)
                        {
                            neighbor.people.Add(person);
                            people.Remove(person);
                            break;
                        }
                    }
                }
            }
        }//end MovePeople
    }//end class
}//end namespace

