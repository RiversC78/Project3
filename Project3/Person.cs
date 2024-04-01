﻿
/*The Person class represents a single person that exist within the simulation. Each person should
have at least the following attributes:
 string Id - A person’s unique identifier to assist with debugging and analysis.
 int TravelStartTime – The time of day on a 24-hour clock when this person may start
travelling to other locations.
o Must be less than “TravelEndTime”
 int TravelEndTime – The time of day on a 24-hour clock when this person may stop
travelling to other locations.
o Must be greater than “TravelStartTime”
 bool IsInfected – A Boolean flag to indicate whether a person is currently infected.
 int InfectionCount – An indicator of how many times a person has been infected.
 int InfectionSpreadCount – An indicator of how many times a person has spread the
disease to another person.
 bool IsDead – A Boolean flag to indicate that the person is no longer alive, and therefore
can no longer travel between locations, and can no longer catch or spread the disease.
 bool IsQuarantined – A Boolean flag to indicate whether a person is quarantined,
meaning that they will not travel to other locations and will not spread the disease, but will
resume travel once recovered from the disease.
 double QuarantineChance – A percentage of the likelihood this person is to quarantine
themselves once infected.
o Must be clamped between 0 and 1.
o This should be generated by the configuration file at runtime.
o The configuration file will provide a mean and a standard deviation.
o This value must be randomly generated based on a normal random distribution as
defined by the configuration file.
*/

using System.Data;

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
        //public int InfectionTime { get; set; }


        //Track how long someone has been in quarantine
        public int QuarantineTime { get; set; }

        private readonly Configuration config;


        //Constructor
        public Person(string id, int travelStartTime, int travelEndTime, bool isInfected, int infectionCount, int infectionSpreadCount, bool isDead, bool isQuarantined, double quarantineChance, double travelChance, int infectionTime, int quarantineTime, Configuration config)
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
            //InfectionTime = infectionTime;
            QuarantineTime = quarantineTime;

            this.config = config;
        }

        //Determines if a person will move
        public bool ShouldMove(int currentHour)
        {
            return !IsQuarantined && !IsDead && currentHour >= TravelStartTime && currentHour <= TravelEndTime;
        }

        //Updates people's attributes
        //Chance to die
        //After a certain amount of time, people should exit quarantine
        //After a certaom amount of time, people no longer have the disease
        public void Update()
        {
            //Determines if someone dies
            if (IsInfected)
            {
                Random random = new Random();
                double deathChance = random.NextDouble();
                if (deathChance > config.DeathChance)
                {
                    IsDead = true;
                }
            }

            //Determines if someone leaves quarantine
            if (IsQuarantined)
            {
                QuarantineTime++;
                if (QuarantineTime >= config.QuarantineHours)
                {
                    IsQuarantined = false;
                    QuarantineTime = 0;
                }
            }

            //Determines if someone still has the disease
            if (IsInfected)
            {
                //InfectionTime++;
                ////If they've been infected for the duration the disease lasts, they no longer are infected
                //if (InfectionTime >= config.DiseaseHours)
                //{
                //    IsInfected = false;
                //    InfectionTime = 0;
                //}
            }
        }

        //Determines if someone can spread the disease
        public bool CanSpread()
        {
            return !IsQuarantined && !IsDead && IsInfected;
        }

    }
}
