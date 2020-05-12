using ProZoneRouting.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ProZoneRouting.ui
{
    public class SensoreBasicsDetails
        {
            public string  NodeID{get;set;}
            public string RealLocation { get; set; } // real location
            public string CenterLocation { get; set; } // center location
            public string OverlappingNodes { get; set; }
           


        }

    /// <summary>
    /// Interaction logic for UIshowSensorsLocations.xaml
    /// </summary>
    public partial class UIshowSensorsLocations : Window
    {
       
        private string FindOverlappingNodesString(Sensor node)
        {
            string str = "";
            if (node.NeighboreNodes != null)
            {
                foreach (Sensor _node in node.NeighboreNodes)
                {
                    str += _node.ID.ToString()+",";
                }
            }
            return str;
        }

        


        

        public UIshowSensorsLocations(List<Sensor> SensorsNodes)
        {
            InitializeComponent();

            List<SensoreBasicsDetails> NodesLocationsList = new List<SensoreBasicsDetails>();
            foreach(Sensor node in SensorsNodes)
            {
                SensoreBasicsDetails Sensorinfo = new SensoreBasicsDetails();
               
                Sensorinfo.NodeID =node.lbl_Sensing_ID.Content.ToString();
                Sensorinfo.RealLocation = node.Position.X + "," + node.Position.Y;
                Sensorinfo.CenterLocation = (node.Position.X + node.VisualizedRadius) + "," + (node.Position.Y + node.VisualizedRadius);
                Sensorinfo.OverlappingNodes = FindOverlappingNodesString(node);

                NodesLocationsList.Add(Sensorinfo);
                
            }

            dg_locations.ItemsSource = NodesLocationsList;
        }
    }
}
