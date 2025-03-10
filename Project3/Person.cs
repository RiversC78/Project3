﻿/*The Person class represents a single person that exist within the simulation. Each person should
have at least the following attributes:
string Id - A person’s unique identifier to assist with debugging and analysis.
int TravelStartTime – The time of day on a 24-hour clock when this person may start
travelling to other locations.
o Must be less than “TravelEndTime”
int TravelEndTime – The time of day on a 24-hour clock when this person may stop
travelling to other locations.
o Must be greater than “TravelStartTime”
bool IsInfected – A Boolean flag to indicate whether a person is currently infected.
int InfectionCount – An indicator of how many times a person has been infected.
int InfectionSpreadCount – An indicator of how many times a person has spread the
disease to another person.
bool IsDead – A Boolean flag to indicate that the person is no longer alive, and therefore
can no longer travel between locations, and can no longer catch or spread the disease.
bool IsQuarantined – A Boolean flag to indicate whether a person is quarantined,
meaning that they will not travel to other locations and will not spread the disease, but will
resume travel once recovered from the disease.
double QuarantineChance – A percentage of the likelihood this person is to quarantine
themselves once infected.
o Must be clamped between 0 and 1.
o This should be generated by the configuration file at runtime.
o The configuration file will provide a mean and a standard deviation.
o This value must be randomly generated based on a normal random distribution as
defined by the configuration file.
*/

namespace Project3
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
        public double QuarantineChance { get; set; }
        public double TravelChance { get; set; }
        //Remaining time in quarantine
        public int QuarantineStartTime { get; set; }

        //Track how long someone has been infected
        public int InfectionTime { get; set; }
        //If someone is immune or not
        public bool Immunity { get; set; }
        //Track how long someone has been in quarantine
        public int QuarantineTime { get; set; }

        private readonly Configuration config;


        //Constructor
        public Person(string id, int travelStartTime, int travelEndTime, bool isInfected, int infectionCount, int infectionSpreadCount, bool isDead, bool isQuarantined, double quarantineChance, double travelChance, int infectionTime, int quarantineTime, bool immunity, Configuration config)
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
            TravelChance = travelChance;
            InfectionTime = infectionTime;
            QuarantineTime = quarantineTime;
            Immunity = immunity; 

            this.config = config;
        }//end Person constructor

        /// <summary>
        /// Determines if a person will move
        /// </summary>
        /// <param name="currentHour">returns current hour of the day in the sim</param>
        /// <returns>bool on if person will move or not</returns>
        public bool ShouldMove(int currentHour)
        {
            return !IsQuarantined && !IsDead && currentHour >= TravelStartTime && currentHour <= TravelEndTime;
        }//end ShouldMove

        /// <summary>
        /// Updates attributes, gives people a chance to die, exit quarantine after some time, and recover from disease after a certain amount of time if they're not dead
        /// </summary>
        public void Update(Person person)
        {
            //Determines if someone dies
            if (person.IsInfected)
            {
                Random random = new Random();
                double deathChance = random.Next(0, 101);
                if (deathChance < config.DeathChance)
                {
                    person.IsDead = true;
                    person.IsInfected = false;
                    person.IsQuarantined = false;
                }
            }

            //Determines if someone leaves quarantine
            if (person.IsQuarantined)
            {
                person.QuarantineTime++;
                if (person.QuarantineTime >= config.QuarantineHours)
                {
                    person.IsQuarantined = false;
                    person.QuarantineTime = 0;
                }
            }

            //Determines if someone still has the disease
            if (person.IsInfected)
            {
                person.InfectionTime++;
                ////If they've been infected for the duration the disease lasts, they no longer are infected
                if (person.InfectionTime >= config.DiseaseHours)
                {
                    person.IsInfected = false;
                    person.InfectionTime = 0;
                }
            }
        }//end Update

        /// <summary>
        /// Determines if someone can spread the disease
        /// </summary>
        /// <returns>true if able to spread disease, false if not</returns>
        public bool CanSpread()
        {
            return !IsQuarantined && !IsDead && IsInfected;
        }//end CanSpread

        /// <summary>
        /// method to determine if everyone that is alive no longer has the disease and if it is still spreadable or if everyone infected quarantined or is dead - checks by location
        /// </summary>
        /// <param name="locations">list of all active locations</param>
        /// <returns>true if everyone is dead and false if not</returns>
        public static bool IsEveryoneDead(List<Location> locations)
        {
            //check each location
            foreach (var place in locations)
            {
                //used to test if everyone at a location is dead
                int deadAtLocation = 0;
                //if every location only has dead people return true
                int emptyLocations = 0;
                //check each person at the location
                foreach (var person in place.people)
                {
                    if (person.IsDead)
                    {
                        deadAtLocation++;
                    }
                }
                //only check for empty locations after confirming if everyone at a location is dead or not 
                if (deadAtLocation >= place.people.Count)
                {
                    emptyLocations++;
                }
                //if all locations are empty return true
                if (emptyLocations >= locations.Count)
                {
                    return true;
                }
            }
            //if not all locations are empty return false
            return false;
        }//end IsEveryoneDead

        /// <summary>
        /// Randomly picks a starter person to have the disease
        /// </summary>
        /// <param name="people">list of people to pick from for patient zero</param>
        public static void PatientZero(List<Person> people)
        {
            Random random = new Random();
            int patientZero = random.Next(0, people.Count-1);
            people[patientZero].IsInfected = true;
        }//end PatientZero

        /// <summary>
        /// checks to see if everyone with the disease is dead and it is no longer spreadable - used to end sim early if so
        /// </summary>
        /// <param name="locations">list of locations active in the sim</param>
        /// <returns>true or false if everyone with the disease is dead</returns>
        public static bool IsDiseaseDead(List<Location> locations)
        {
            //make sure if everyone in the sim is dead or not
            if (IsEveryoneDead(locations))
            {
                return true;
            }
            //create counter variable for # of people infected 
            int infectedCount = 0;
            //check by each place 
            foreach (var place in locations)
            {
                foreach (var person in place.people)
                {
                    if (person.IsInfected)
                    {
                        infectedCount++;
                    }
                }
            }
            //if there is no one currently infected, return true (people who are dead will not show infected)
            if (infectedCount == 0)
            {
                return true;
            }
            //if there is at least one person infected continue sim - including if everyone infected is quarantined or not
            else
            {
                return false;
            }
        }//end IsDiseaseDead
    }//end class
}//end namespace

