using ProZoneRouting.DataPacket;
using ProZoneRouting.Modules;
using ProZoneRouting.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProZoneRouting.ui
{
    /// <summary>
    /// Interaction logic for UiRecievedPackertsBySink.xaml
    /// </summary>
    public partial class UiRecievedPackertsBySink : Window
    {
        public UiRecievedPackertsBySink()
        {
            InitializeComponent();
            Sensor sink= PublicParamerters.SinkNode;
            if (sink != null)
            {
                List<UnVisualizedDataPacket> recivedpackets = new List<DataPacket.UnVisualizedDataPacket>();
                foreach (Datapacket pck in sink.PacketsList)
                {
                    UnVisualizedDataPacket pp = new UnVisualizedDataPacket()
                    {
                        Distance = pck.Distance,
                        Path = pck.Path,
                        RoutingDistance = pck.RoutingDistance,
                        SID = pck.SourceNodeID,
                        Hops = pck.Hops,
                        UsedEnergy_Joule = pck.UsedEnergy_Joule,
                        Delay = pck.Delay,
                        PrepDistanceDistCnt = pck.PrepDistanceDistCnt,
                        TransDistanceDistCnt = pck.TransDistanceDistCnt,
                        EnergyDistCnt = pck.EnergyDistCnt,
                        DirectionDistCnt = pck.DirectionDistCnt,
                        AverageTransDistrancePerHop = pck.AverageTransDistrancePerHop,
                        RoutingDistanceEffiecncy = pck.RoutingDistanceEfficiency,
                        RoutingEfficiency = pck.RoutingEfficiency,
                        TransDistanceEfficiency = pck.TransDistanceEfficiency,
                        RoutingZoneWidthCnt = pck.RoutingZoneWidthCnt,
                        RoutingProbabilityForPath = pck.RoutingProbabilityForPath,
                        PID = pck.PacektSequentID
                    };
                    recivedpackets.Add(pp);
                }

                
               

                dg_packets.ItemsSource = recivedpackets;
            }
        }
    }
}
