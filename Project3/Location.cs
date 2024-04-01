
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
    public class Location
    {
        public string Id { get; set; }
        public ICollection<Person> people { get; set; }
        public ICollection<Location> neighbors { get; set; }

        public Location(string id)
        {
            Id = id;
            people = new List<Person>();
            neighbors = new List<Location>();

        }

        public void SpreadDisease(double spreadChance, Person currentPerson)
        {
            Random random = new Random();
            foreach (Person otherPerson in people)
            {
                if (currentPerson == otherPerson)
                {
                    continue;
                }

                int chance = random.Next(0, 100);
                if (chance > spreadChance && !otherPerson.IsQuarantined)
                {
                    otherPerson.IsInfected= true;
                    otherPerson.InfectionCount++;
                    currentPerson.InfectionSpreadCount++;

                    //Determines if the newly infected person will quarantine
                    chance = random.Next(0, 100);
                    if (otherPerson.QuarantineChance < chance)
                    {
                        otherPerson.IsQuarantined= true;
                    }
                }
            }
        }

        //Moves a person to a neighboring location
        public void MovePeople(Person person)
        {
            Random random = new Random();
            if (random.Next(0, 100) > person.TravelChance)
            {
                int pickNeighbor = new Random().Next(neighbors.Count());
                Location chosenNeighbor = neighbors.ElementAt(pickNeighbor);

                chosenNeighbor.people.Add(person);
                people.Remove(person);
            }
        }

    }
}
