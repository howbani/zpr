using ProZoneRouting.Parameters;
using ProZoneRouting.ForwardingProbabilities;
using System;
using System.Collections.Generic;

namespace ProZoneRouting.ForwardingProbabilities
{
    public class SeletNextHopRandomlly
    {
        private double CurrentGeneratedNumber;

        private static double RdmGenerator(double max)
        {
            return UnformRandomNumberGenerator.GetUniform(max);
        }

        /// <summary>
        /// selete next hop:
        /// the 
        /// </summary>
        /// <param name="probabilities"></param>
        /// <returns></returns>
        public RoutingRandomVariable SelectNextHop(List<RoutingRandomVariable> probabilities)
        {

            double randNumber = (RdmGenerator(PublicParamerters.MultiplyBy) * DateTime.Now.Millisecond) % PublicParamerters.MultiplyBy;
            foreach (RoutingRandomVariable row in probabilities)
            {
                if (randNumber>= row.RoutingProbRange[0] && randNumber<= row.RoutingProbRange[1])
                {
                    CurrentGeneratedNumber = randNumber;
                    return row;
                }
            }
            return null;
        }

        public double RandomeNumber { get { return CurrentGeneratedNumber; } }
    }
}
