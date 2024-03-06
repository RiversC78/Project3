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

    }
}
