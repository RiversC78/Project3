namespace Project3
{
    public class MathOperations
    {
        /// <summary>
        /// Generates numbers using a normal distribution
        /// </summary>
        /// <returns></returns>
        public static double RandomGaussian()
        {
            Random rand = new Random();
            double u1 = 1.0 - rand.NextDouble(); // Uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // Box-Muller transform
            return randStdNormal;
        }
    }
    // Extension method to generate Gaussian random numbers
    public static class RandomExtensions
    {
        /// <summary>
        /// Method to generate Gaussian random numbers
        /// </summary>
        /// <param name="rand">random object reference</param>
        /// <param name="mean">mean from config file</param>
        /// <param name="stdDev">standard deviation from the config file</param>
        /// <returns></returns>
        public static double NextGaussian(this Random rand, double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble(); // Uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); // Box-Muller transform
            double randNormal = mean + stdDev * randStdNormal; // Apply the transform to get normal distribution
            return randNormal;
        }
    }
}
