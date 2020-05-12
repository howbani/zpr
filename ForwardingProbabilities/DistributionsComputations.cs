using ProZoneRouting.Computations;
using ProZoneRouting.Modules;
using ProZoneRouting.Parameters;
using ProZoneRouting.Properties;
using ProZoneRouting.ForwardingProbabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProZoneRouting.ForwardingProbabilities
{

    public class DistributionsComputations
    {

       

        public class RelativityCompar : IComparer<RoutingRandomVariable>
        {

            public int Compare(RoutingRandomVariable y, RoutingRandomVariable x)
            {
                return x.RoutingVariable.CompareTo(y.RoutingVariable);
            }
        }

        /// <summary>
        ///  PowerThetaij should be the maximal.
        ///  PowerHij should be smaller than PowerThetaij
        ///  PowerNij should be smaller than (PowerThetaij+PowerHij)/2
        ///  PowerRj= the main of all.
        /// </summary>

        /// <summary>
        /// i current node.
        /// when i has the data packet it make the dicient which is the nesx step based on these two things.
        /// </summary>
        /// <param name="SourceNode"></param>
        /// <param name="SinkNode"></param>
        /// <param name="forwardNode"></param>
        /// <returns></returns>
        public List<RoutingRandomVariable> ComputeDistributions(Sensor SourceNode, Sensor SinkNode, Sensor forwardNode,double EnergyDistCnt, double TransDistanceDistCnt, double DirectionDistCnt, double PrepDistanceDistCnt)
        {

            List<Point> Zone = SourceNode.MyRoutingZone;
            Parallelogram rect = new Computations.Parallelogram();
            if (Zone != null)
            {
                rect.P1 = Zone[0];
                rect.P2 = Zone[1];
                rect.P3 = Zone[2];
                rect.P4 = Zone[3];
            }

            Geomtric Geo = new Computations.Geomtric();
            List<RoutingRandomVariable> distributions = new List<RoutingRandomVariable>(); 
            if (SinkNode == null) { return null; }
            else
            if (SinkNode.ID != forwardNode.ID) // for all node but sink node
            {
                List<Sensor> N = forwardNode.NeighboreNodes;
                if (N != null)
                {
                    if (N.Count > 0)
                    {
                        // sum:
                        double EnergySum = 0; // energy
                        double DirectionSum = 0; // direction
                        double PreDistanceSum = 0; // prependiculare distance
                        double TransDistanceSum = 0; // transmission distance
                        foreach (Sensor nextHop in N)
                        {
                            if (nextHop.ResidualEnergy > 0)
                            {
                                // if next hop is wthin the zone of the source node then:
                                Point point = nextHop.CenterLocation;
                                if (Geo.PointTestParallelogram(rect, point))
                                {
                                    double Dij = Operations.DistanceBetweenTwoSensors(forwardNode, nextHop);// i and j
                                    double Djb = Operations.DistanceBetweenTwoSensors(nextHop, SinkNode); // j and b
                                    double Dib = Operations.DistanceBetweenTwoSensors(forwardNode, SinkNode); // i and b

                                    RoutingRandomVariable variable = new RoutingRandomVariable();
                                    variable.PID = PublicParamerters.PackeTSequenceID;
                                    variable.ForwardNode = forwardNode;
                                    variable.SourceNode = SourceNode;
                                    variable.NextHopNode = nextHop;
                                    variable.Dij = Dij;
                                    variable.Dib = Dib;
                                    variable.Djb = Djb;
                                    variable.r = forwardNode.ComunicationRangeRadius;
                                    variable.ZoneWidthControl = Settings.Default.ZoneWidthCnt;

                                    // sum's:
                                    EnergySum += Math.Exp(Math.Pow(variable.NormalizedEnergy, EnergyDistCnt));
                                    TransDistanceSum += (1 / (Math.Exp(Math.Pow(variable.NormalizedTransDistance, TransDistanceDistCnt))));
                                    DirectionSum += (1 / (Math.Exp(Math.Pow(variable.NormalizedDirection, DirectionDistCnt))));
                                    PreDistanceSum += (1 / (Math.Exp(Math.Pow(variable.NormalizePerpendicularDistance, PrepDistanceDistCnt))));

                                    distributions.Add(variable); // keep for each node.
                                }
                            }
                        }


                        double RoutingvariablSum = 0;
                        int k = 0;
                        foreach (RoutingRandomVariable dist in distributions)
                        {
                            // propablity distrubution:
                            dist.EnergyProb = (Math.Exp(Math.Pow(dist.NormalizedEnergy, EnergyDistCnt))) / EnergySum;
                            dist.TransDistanceProb = (1 / (Math.Exp(Math.Pow(dist.NormalizedTransDistance, TransDistanceDistCnt)))) / TransDistanceSum;
                            dist.DirectionProb = (1 / (Math.Exp(Math.Pow(dist.NormalizedDirection, DirectionDistCnt)))) / DirectionSum; // propablity for lemda.
                            dist.PerpendicularDistanceProb = (1 / (Math.Exp(Math.Pow(dist.NormalizePerpendicularDistance, PrepDistanceDistCnt)))) / PreDistanceSum;
                            RoutingvariablSum += dist.RoutingVariable;
                            k++;
                        }

                        distributions.Sort(new RelativityCompar());// sort
                        //- likehool probablity.
                        double LikehoodcumlativeMin = 0;
                        double LikeacumlativeMax = 0;
                        int f = 0;  
                        foreach (RoutingRandomVariable randvar in distributions)
                        {
                            randvar.RoutingVariableProb = randvar.RoutingVariable / RoutingvariablSum;
                            // set ranges:
                            if (f == 0)
                            {
                                LikehoodcumlativeMin = 0;
                                LikeacumlativeMax = randvar.RoutingVariableProb * PublicParamerters.MultiplyBy;
                                randvar.RoutingProbRange[0] = LikehoodcumlativeMin;// min
                                randvar.RoutingProbRange[1] = LikeacumlativeMax; // max
                            }
                            else
                            {
                                // get:
                                LikehoodcumlativeMin += (distributions[f - 1].RoutingVariableProb * PublicParamerters.MultiplyBy);
                                LikeacumlativeMax += (distributions[f].RoutingVariableProb * PublicParamerters.MultiplyBy);
                                randvar.RoutingProbRange[0] = LikehoodcumlativeMin;// min
                                randvar.RoutingProbRange[1] = LikeacumlativeMax; // max
                            }
                            f++;
                        } // end lik hoode
                    }

                     
                }
            }
            return distributions;
        }

    }
}



   

