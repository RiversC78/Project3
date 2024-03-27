
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

        //put in person? :(
        //Method to spread the disease.
        //***TO DO: add chance to quarantine***
        public void SpreadDisease(double spreadChance)
        {
            //for all people created
            foreach (Person person in people)

                //Iterates through each pair of people
                for (int i = 0; i < people.Count; i++)
                {
                    for (int j = i + 1; j < people.Count; j++)
                    {
                        Person person1 = people.ElementAt(i);
                        Person person2 = people.ElementAt(j);

                        Random random = new Random();

                        double chance = random.NextDouble();

                        //Checks for the chance to spread disease
                        if (person1.CanSpread() && chance > spreadChance && !person2.IsQuarantined)
                        {
                            person2.IsInfected = true;
                            person2.InfectionCount++;
                            person1.InfectionSpreadCount++;

                            //Determines if the infected person will quarantine
                            chance = random.NextDouble();
                            if (person2.QuarantineChance < chance)
                            {
                                person2.IsQuarantined = true;
                            }
                        }
                        //Checks if person2 spreads to person1
                        else if (person2.CanSpread() && chance > spreadChance && !person1.IsQuarantined)
                        {
                            person1.IsInfected = true;
                            person1.InfectionCount++;
                            person2.InfectionSpreadCount++;

                            //Determines if the infected person will quarantine
                            chance = random.NextDouble();
                            if (person1.QuarantineChance < chance)
                            {
                                person1.IsQuarantined = true;
                            }
                        }
                    }
                }
        }

        //Method to continue to travel after quarantine.
        public void TravelAfterQuarantine()
        {
            foreach (Person person in people)
            {
                if (person.IsQuarantined)
                {
                    Random rand = new Random();
                    double randomValue = rand.Next(1, 101);

                    if (randomValue < person.QuarantineChance)
                    {
                        person.IsQuarantined = false;
                    }
                }
            }
        }

        //Moves a person to a neighbor location
        public void MovePeople()
        {
            foreach (Person person in people)
            {
                double chance = new Random().NextDouble();

                if (chance > person.TravelChance)
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

                    //Pick a neighbor location for someone to move to
                    int pickNeighbor = new Random().Next(neighbors.Count());
                    Location chosenNeighbor = neighbors.ElementAt(pickNeighbor);

                    //Move that person to their new location
                    chosenNeighbor.people.Add(person);
                    people.Remove(person);
                }
            }
        }
    }
}