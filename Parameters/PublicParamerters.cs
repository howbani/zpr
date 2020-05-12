using ProZoneRouting.Lifetime;
using ProZoneRouting.Modules;
using System;
using System.Collections.Generic;

namespace ProZoneRouting.Parameters
{
   
   
    public static class PublicParamerters
    {
        public static long Rounds { get; set; } // how many rounds.
        public static List<DeadNodesRecord> DeadNodeList = new List<DeadNodesRecord>();
        public static long PackeTSequenceID { get; set; }
        public static bool IsNetworkDied { get; set; }
        public static double Density { get; set; } // average number of neighbores (stander deiviation)
        public static string NetworkName { get; set; }
        public static Sensor SinkNode { get; set; }
        public static double BatteryIntialEnergy = 0.5; //J 0.5
        public static double RoutingDataLength = 1024; // bit
        public static double ControlDataLength = 521; // bit
        public static double MultiplyBy = 1; // uniform(x), here MultiplyBy=x; u can change x=100 or any.
        public static double E_elec = 50; // unit: (nJ/bit) //Energy dissipation to run the radio
        public static double Efs = 0.01;// unit( nJ/bit/m^2 ) //Free space model of transmitter amplifier
        public static double Emp = 0.0000013; // unit( nJ/bit/m^4) //Multi-path model of transmitter amplifier
        public static double SensingFeildArea
        {
            get; set;
        }
        public static double CommunicationRangeRadius { get; set; } // the value of db. R.
        public static double TransmissionRate= 2 * 1000000;////2Mbps 100 × 10^6 bit/s , //https://en.wikipedia.org/wiki/Transmission_time
        public static double SpeedOfLight = 299792458;//https://en.wikipedia.org/wiki/Speed_of_light

        public static  double ThresholdDistance  //Distance threshold ( unit m) 
        {
            get { return Math.Sqrt(Efs / Emp); }
        }

        /// <summary>
        /// node that the comunication range = radius *2
        /// </summary>
        public static double RoutingZoneWidth 
        {
            get
            {
                double comRange = CommunicationRangeRadius * 2;
                if (comRange > Density)
                {
                    return (CommunicationRangeRadius * Math.Sqrt(2 * CommunicationRangeRadius)) / Density;
                }
                else
                {
                    System.Windows.MessageBox.Show("DENSITY>COM RANGE!!!");
                    return comRange;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static int ControlsRange
        {
            get { return Convert.ToInt16(Math.Sqrt(Density) * Math.Sqrt(CommunicationRangeRadius)); }
        }


        /// <summary>
        /// the runnunin time of simulator. in SEC
        /// </summary>
        public static int SimulationTime
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public static double MaxRandomContols 
        {
            get { return (ControlsRange / 3); } // we have three parts:
        }


        // lifetime paramerts:
        public static int NOS { get; set; } // NUMBER OF RANDOM SELECTED SOURCES
        public static int NOP { get; set; } // NUMBER OF PACKETS TO BE SEND.

    }
}
