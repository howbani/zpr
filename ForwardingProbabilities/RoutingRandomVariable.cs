using ProZoneRouting.Computations;
using ProZoneRouting.Modules;
using System;
using System.Windows;

namespace ProZoneRouting.ForwardingProbabilities
{
    /*
     To understance this code, please refere to the manscript section 4.
         */
    public class RoutingRandomVariable
    {


        public long PID { get; set; } // PACKET SEQUENCE ID
        public int Sid  { get { return SourceNode.ID; } } // the sourc ID
        public int Fid { get { return ForwardNode.ID;  } } // FORWARD NODE ID. relay node: ID, at first it is the source node.
        public int Nid { get { return NextHopNode.ID; } } // NEXT HOP ID
        public double ZoneWidthControl { get; set; } // the width of routing zone.

        /// <summary>
        /// MULTIPLICATIONS
        /// </summary>
        public double RoutingVariable
        {
            get
            {
                return TransDistanceProb * DirectionProb * PerpendicularDistanceProb * EnergyProb;
            }
        }

        /// <summary>
        /// Routing distriotion
        /// </summary>
        public double RoutingVariableProb { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double EnergyProb { get; set; }
        /// <summary>
        ///  The transmission distance probability distribution denoted by (d_i ) ̃=(d ̃_(i,0),d ̃_(i,1),d ̃_(i,2)…d ̃_(i,m) ),〖0≤d ̃〗_(i,0)≤1,  implies ∑_(j=0)^m▒d ̃_(i,j) =1 and if d ̅_(i,j)>d ̅_(i,x) then d ̃_(i,j)
        ///  d ̃_(i,x) On other words, the smallest transmission distance has a greater probability value〖 d ̃〗_(i,j). 
        /// </summary> /// <summary>
        /// A. Direction Random Distribution
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public double TransDistanceProb { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double DirectionProb { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double PerpendicularDistanceProb { get; set; }
       


        /// <summary>
        /// Directions of n_i to 〖 ∀n〗_j∈N_i  (0≤j≤m) are expressed as random variable
        /// 〖 θ〗_i=(θ_(i,0),θ_(i,1),θ_(i,2)…θ_(i,m) ),
        /// where m=|〖 N〗_i | (i.e., the number of nodes in〖 N〗_i). 
        /// For example, in Figure 2(b), 〖 θ〗_9=(θ_9,5,θ_9,6,θ_9,7,θ_9,8,θ_9,10 ). 
        /// To simplify the calculations, all values in the random variable 〖 θ〗_i are normalized such that 1≤θ ̅_(i,j)≤2 as shown in (9).  The smaller the direction θ_(i,j) the more straight the routing path will be and the minimum number of hops will be. The normalized direction values in 〖 θ〗_i are denoted by θ ̅_i=(θ ̅_(i,0),θ ̅_(i,1),θ ̅_(i,2),…θ ̅_(i,m) )  ∀ 〖 n〗_0,〖 n〗_1,〖 n〗_2…〖 n〗_m  ∈〖 N〗_i. Based on the normalized direction vector θ ̅_i, we define a direction probability distribution, dented by (θ_i ) ̃=(θ ̃_(i,0),θ ̃_(i,1),θ ̃_(i,2)…θ ̃_(i,m) )  ∀ 〖 n〗_0,〖 n〗_1,〖 n〗_2…〖 n〗_m  ∈〖 N〗_i by the probability function shown in (10).  Direction probability distribution implies that 〖∀n〗_j∈N_i, the smaller the normalized direction value( θ) ̅_(i,j), the greater the probability θ ̃_(i,j), and vice versa. It is clear that ∑_(j=0)^m▒θ ̃_(i,j) =1. 
        /// θ
        ///  direction toward the sink.
        ///  dot production of two vectores, find the theta.
        ///  andgle between two vectors.
        ///  a · b = ax * bx + ay * by
        /// </summary>
        public double NormalizedDirection
        {
            get
            {
                Point pi = ForwardNode.CenterLocation;
                Point pj =NextHopNode.CenterLocation;
                Point pb =Parameters.PublicParamerters.SinkNode.CenterLocation;
                double axb = (pj.X - pi.X) * (pb.X - pi.X) + (pj.Y - pi.Y) * (pb.Y - pi.Y);
                double disMul = Dij * Dib;
                double t = axb / disMul;
                double f = (1 - t) / 2;
                return f+1;
            }
        }
       


        /// <summary>
        /// the remian energy of J. normalized between 1~2.
        /// </summary>
        public double NormalizedEnergy { get { return 1 + (NextHopNode.ResidualEnergyPercentage / 100); } }
    
       

        /// <summary>
        /// near to the forward node: Normalize to avoid 0.0
        /// Transimission distance:
        /// </summary>
        public double NormalizedTransDistance
        {
            get
            {
                double nj = (Dij / (2 * r));
                return nj+1;
            }
        }
      
       

        /// <summary>
        ///  close to the line from the source to the base station: normalize to avoid 0.0
        /// </summary>
        public double NormalizePerpendicularDistance
        { // 
            get
            {
                double h = (Hj / ((2 * r) + Hi));
                return h+1;
            }
        }

      
     
     
         
        public string RoutingRangeString { get { return RoutingProbRange[0].ToString("0.000") + "-" + RoutingProbRange[1].ToString("0.000"); } }


        /// <summary>
        /// perpendicular distance:shortest distance from a pj(point) to any point on a fixed Line in Euclidean geometry.
        /// If the line passes through two points P1=(x1,y1) and P2=(x2,y2) then the distance of (x0,y0) from the line is:
        /// https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
        /// j: the node to be recived the packet.
        /// </summary>
        public double Hj
        {
            get
            {
                Point pj = NextHopNode.CenterLocation;
                Point pb = Parameters.PublicParamerters.SinkNode.CenterLocation;
                Point ps = SourceNode.CenterLocation;
                double past = ((pb.Y - ps.Y) * pj.X) - ((pb.X - ps.X) * pj.Y) + (pb.X * ps.Y) - (pb.Y * ps.X);
                past = Math.Sqrt(Math.Pow(past, 2));
                double sbDis = Operations.DistanceBetweenTwoPoints(ps, pb);
                return past / sbDis;
            }
        }

        /// <summary>
        /// perpendicular distance:shortest distance from a pi(point) to any point on a fixed Line in Euclidean geometry.
        /// If the line passes through two points P1=(x1,y1) and P2=(x2,y2) then the distance of (x0,y0) from the line is:
        /// https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
        /// the current forward node.
        /// </summary>
        public double Hi  
        {
            get
            {
                Point pi = ForwardNode.CenterLocation;
                Point ps = SourceNode.CenterLocation;
                Point pb = Parameters.PublicParamerters.SinkNode.CenterLocation;
                double past = ((pb.Y - ps.Y) * pi.X) - ((pb.X - ps.X) * pi.Y) + (pb.X * ps.Y) - (pb.Y * ps.X);
                past = Math.Sqrt(Math.Pow(past, 2));
                double dis = Operations.DistanceBetweenTwoPoints(ps, pb);
                return past / dis;
            }
        }
      
       

        public double Dij { get; set; } // Distance from source to next hop
        public double Dib { get; set; } // from source to sink
        public double Djb { get; set; } // from next hop to the sink

        public double[] RoutingProbRange = new double[2];




        public double r { get; set; }
        public Sensor SourceNode { get; set; } // s // the source node of this packet 
        public Sensor ForwardNode { get; set; } //i // the current forward. ( current relay).
        public Sensor NextHopNode { get; set; } //j //  the node to be the netx hop node

      


    }
}
