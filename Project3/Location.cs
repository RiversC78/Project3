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

        //Method to spread the disease.
        public void SpreadDisease(double spreadChance)
        {
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
        }

        //Method to continue to travel after quarantine.
        public void TravelAfterQuarantine()
        {
            foreach (Person person in people)
            {
                if (person.IsQuarantined)
                {
                    double randomValue = new Random().NextDouble();
                    if (randomValue < person.QuarantineChance)
                    {
                        person.IsQuarantined = false;
                    }
                }
            }
        }

        public void MovePeople()
        {
            foreach (Person person in people)
            {

                if (!person.IsDead && !person.IsQuarantined)
                {
                    foreach (var neighbor in neighbors)
                    {
                        double randomValue = new Random().NextDouble();
                        if (randomValue < person.TravelChance)
                        {
                            neighbor.people.Add(person);
                            people.Remove(person);
                            break;
                        }
                    }
                }
            }
        }
    }
}
