using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_03
{
    /// <summary>
    /// The person class represents a single person in the simulation.
    /// </summary>
    public class Person
    {
        //A person's unique identifier
        public string Id { get; set; }
        //The time when a person starts traveling
        public int TravelStartTime { get; set; }
        //The time when a person stops traveling
        public int TravelEndTime { get; set; }
        //Bool to indicate if someone is infected or not
        public bool IsInfected { get; set; }
        //Indicates how many times a person has been infected
        public int InfectionCount { get; set; }
        //Indicates how many times someone has spread the disease to someone else
        public int InfectionSpreadCount { get; set; }
        //Bool to indicate someone is dead
        public bool IsDead { get; set; }
        //Bool to indicate if a person is quarantined -- they will not travel to other locations and will not spread disease, but will resume travel once recovered. 
        public bool IsQuarantined { get; set; }
        //Percentage likelihood a person is to quaratine once infected 
        double QuarantineChance { get; set; }


        //Constructor
        public Person(string id, int travelStartTime, int travelEndTime, bool isInfected, int infectionCount, int infectionSpreadCount, bool isDead, bool isQuarantined, double quarantineChance)
        {
            Id = id;
            TravelStartTime = travelStartTime;
            TravelEndTime = travelEndTime;
            IsInfected = isInfected;
            InfectionCount = infectionCount;
            InfectionSpreadCount = infectionSpreadCount;
            IsDead = isDead;
            IsQuarantined = isQuarantined;
            QuarantineChance = quarantineChance;
        }

        
    }
}
