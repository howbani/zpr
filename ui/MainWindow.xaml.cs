using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ProZoneRouting.Modules;
using ProZoneRouting.db;
using ProZoneRouting.Computations;
using ProZoneRouting.Coverage;
using ProZoneRouting.Parameters;
using ProZoneRouting.Charts;
using ProZoneRouting.DataPacket;
using ProZoneRouting.Properties;
using System.Windows.Media;
using ProZoneRouting.Logs;
using ProZoneRouting.ui.conts;
using System.Data;
using ProZoneRouting.ExpermentsResults;
using System.Windows.Threading;
using ProZoneRouting.ForwardingProbabilities;

namespace ProZoneRouting.ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        public List<Sensor> myNetWork = new List<Sensor>();
        public Int32 stopSimlationWhen = 1000000000; // s by defult.
        public string PacketRate { get; set; }
        public DispatcherTimer TimerCounter = new DispatcherTimer();
        public DispatcherTimer RandomSelectSourceNodesTimer = new DispatcherTimer();

        bool isCoverageSelected = false;
        public static double Swith;// sensing feild width.
        public static double Sheigh;// sensing feild height.
        public static double SensingFeildArea
        {
            get
            {
                return Swith * Sheigh;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Swith = Canvas_SensingFeild.Width - 100;
            Sheigh = Canvas_SensingFeild.Height - 100;
            PublicParamerters.SensingFeildArea = SensingFeildArea;

            /*
             UiRandom xx = new RandomNumber.UiRandom();
             xx.Show();*/

            /*
            TestTringlePoint tt = new tests.TestTringlePoint();
            tt.Show();*/
            RandomSelectSourceNodesTimer.Tick += RandomSelectNodes_Tick;
            TimerCounter.Interval = TimeSpan.FromSeconds(1);
            TimerCounter.Tick += TimerCounter_Tick;
            FillColors();
        }

        private void RandomSelectNodes_Tick(object sender, EventArgs e)
        {
            // start sending after the nodes are intilized all.
            if (PublicParamerters.SimulationTime < stopSimlationWhen)
            {
                int index = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(myNetWork.Count - 2));
                if (index != PublicParamerters.SinkNode.ID)
                {
                    myNetWork[index].GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                }
            }
        }

        private void TimerCounter_Tick(object sender, EventArgs e)
        { 
            //
            if (PublicParamerters.SimulationTime <= stopSimlationWhen)
            {
                PublicParamerters.SimulationTime += 1;
                Title = "ZPR:" + PublicParamerters.SimulationTime.ToString();
            }
            else
            {
                TimerCounter.Stop();
                RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
                RandomSelectSourceNodesTimer.Stop();
                top_menu.IsEnabled = true;
            }
        }

        private void FillColors()
        {

            lvl_0.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col0));
            lvl_1_9.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col1_9));
            lvl_10_19.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col10_19));
            lvl_20_29.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col20_29));
            lvl_30_39.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col30_39));
            lvl_40_49.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col40_49));
            lvl_50_59.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col50_59));
            lvl_60_69.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col60_69));
            lvl_70_79.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col70_79));
            lvl_80_89.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col80_89));
            lvl_90_100.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col90_100));
        }


        private void BtnFile(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            switch (Header)
            {
                case "_Multiple Nodes":
                    {
                        UiAddNodes ui = new UiAddNodes();
                        ui.MainWindow = this;
                        ui.Show();
                        break;
                    }

                case "_Export Topology":
                    {
                        UiExportTopology top = new UiExportTopology(myNetWork);
                        top.Show();
                        break;
                    }

                case "_Import Topology":
                    {
                        UiImportTopology top = new UiImportTopology(this);
                        top.Show();
                        break;
                    }
            }

        }

        int rounds = 0;

        
        public void DisplaySimulationParameters(int rootNodeId, string deblpaymentMethod)
        {
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            PublicParamerters.SinkNode.Ellipse_center.Width = 12;
            PublicParamerters.SinkNode.Ellipse_center.Height = 12;

            PublicParamerters.SinkNode.lbl_Sensing_ID.Foreground =Brushes.Blue;
            PublicParamerters.SinkNode.lbl_Sensing_ID.FontWeight = FontWeights.Bold;
            lbl_sink_id.Content = rootNodeId;
            lbl_coverage.Content = deblpaymentMethod;
            lbl_network_size.Content = myNetWork.Count;
            lbl_sensing_range.Content = PublicParamerters.SinkNode.SensingRangeRadius;
            lbl_communication_range.Content = (PublicParamerters.SinkNode.ComunicationRangeRadius);
            lbl_Transmitter_Electronics.Content = PublicParamerters.E_elec;
            lbl_fes.Content = PublicParamerters.Efs;
            lbl_Transmit_Amplifier.Content = PublicParamerters.Emp;
            lbl_data_length_control.Content = PublicParamerters.ControlDataLength;
            lbl_data_length_routing.Content= PublicParamerters.RoutingDataLength;
            lbl_density.Content = PublicParamerters.Density;
            lbl_control_range.Content = PublicParamerters.ControlsRange;
            lbl_zone_width.Content = PublicParamerters.RoutingZoneWidth;
        }

        public void HideSimulationParameters()
        {
            Settings.Default.IsIntialized = false;
            rounds = 0;
            lbl_rounds.Content = "0";
            PublicParamerters.SinkNode = null;
            lbl_sink_id.Content = "nil";
            lbl_coverage.Content = "nil";
            lbl_network_size.Content = "unknown";
            lbl_sensing_range.Content = "unknown";
            lbl_communication_range.Content = "unknown";
            lbl_Transmitter_Electronics.Content = "unknown";
            lbl_fes.Content = "unknown";
            lbl_Transmit_Amplifier.Content = "unknown";
            lbl_data_length_control.Content = "unknown";
            lbl_data_length_routing.Content = "unknown";
            lbl_density.Content = "0";
            lbl_control_range.Content = "0";
            lbl_zone_width.Content = "0";
        }

        public void RandomDeplayment(int sinkIndex)
        {
            int rootNodeId = sinkIndex;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            GetOverlappingNodes overlappingNodesFinder = new GetOverlappingNodes(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
            lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;
            isCoverageSelected = true;
            PublicParamerters.Density = Density.GetDensity(myNetWork);
            DisplaySimulationParameters(rootNodeId, "Random");
            Settings.Default.ZoneWidthCnt = PublicParamerters.RoutingZoneWidth;
            Settings.Default.IsIntialized = true;

        }

        public void GridCoverag2(int sinkIndex) 
        {
            int rootNodeId = sinkIndex;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            GridCoverage GridCoverage = new GridCoverage();
            GridCoverage.GridCoverage2(Canvas_SensingFeild, myNetWork, Convert.ToInt16((Sensor.SR * 2)));
            GetOverlappingNodes overlappingNodesFinder = new GetOverlappingNodes(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
            lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;
            isCoverageSelected = true;
            PublicParamerters.Density = Density.GetDensity(myNetWork);
            DisplaySimulationParameters(rootNodeId, "grid_coverag_2");
            Settings.Default.ZoneWidthCnt = PublicParamerters.RoutingZoneWidth;
            Settings.Default.IsIntialized = true;
        }

        public void GridCoverag1(int sinkIndex) 
        {
            int rootNodeId = sinkIndex;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            GridCoverage GridCoverage = new Coverage.GridCoverage();
            GridCoverage.GridCoverage1(Canvas_SensingFeild, myNetWork, Convert.ToInt16((Sensor.SR * 2) * 0.7));
            GetOverlappingNodes overlappingNodesFinder = new GetOverlappingNodes(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
            lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;
            isCoverageSelected = true;
            PublicParamerters.Density = Density.GetDensity(myNetWork);
            DisplaySimulationParameters(rootNodeId, "grid_coverag_1");
            Settings.Default.ZoneWidthCnt = PublicParamerters.RoutingZoneWidth;
            Settings.Default.IsIntialized = true;
        }

        public void ZigzagCoverage(int sinkIndex) 
        {
            int rootNodeId = sinkIndex;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            ZizageCoverage zig = new ZizageCoverage();
            zig.coverage(Canvas_SensingFeild, myNetWork, Convert.ToInt16(2 * Sensor.SR));
            GetOverlappingNodes overlappingNodesFinder = new GetOverlappingNodes(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
            lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;
            isCoverageSelected = true;
            PublicParamerters.Density = Density.GetDensity(myNetWork);
            DisplaySimulationParameters(rootNodeId, "Zigzag");
            Settings.Default.ZoneWidthCnt = PublicParamerters.RoutingZoneWidth;
            Settings.Default.IsIntialized = true;
        }


        private void Coverage_Click(object sender, RoutedEventArgs e)
        {
            if (myNetWork.Count > 0)
            {
                string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
                lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;
                isCoverageSelected = true;

                MenuItem item = sender as MenuItem;
                string Header = item.Name.ToString();
                switch (Header)
                {
                    case "grid_coverag_1":
                        if (myNetWork.Count > 1)
                        {
                            GridCoverag1(0);
                        }
                        break;
                    case "grid_coverag_2":
                        if (myNetWork.Count > 1)
                        {
                            GridCoverag2(0);
                        }
                        break;
                    case "zigzag_coverage":
                        if (myNetWork.Count > 1)
                        {
                            ZigzagCoverage(0);
                        }
                        break;

                    case "btn_Random":
                        {
                            RandomDeplayment(0);
                        }

                        break;
                }
            }
            else
            {
                MessageBox.Show("Please imort the nodes from Db.");
            }
        }

        private void base_station_position_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            switch (Header)
            {
                case "_Top":

                    break;
                case "_Bottom":

                    break;
                case "_Right":

                    break;
                case "_Left":

                    break;
            }
        }
        private void Display_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Name.ToString();
            switch (Header)
            {
                case "_show_id":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.lbl_Sensing_ID.Visibility == Visibility.Hidden)
                        {
                            sensro.lbl_Sensing_ID.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.lbl_Sensing_ID.Visibility = Visibility.Hidden;
                        }
                    }
                    break;
                case "_show_rang":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Ellipse_Communication_range.Visibility == Visibility.Hidden)
                        {
                            sensro.Ellipse_Communication_range.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.Ellipse_Communication_range.Visibility = Visibility.Hidden;
                        }
                    }
                    break;

                case "_show_battrey":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Prog_batteryCapacityNotation.Visibility == Visibility.Hidden)
                        {
                            sensro.Prog_batteryCapacityNotation.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.Prog_batteryCapacityNotation.Visibility = Visibility.Hidden;
                        }
                    }
                    break;
            }
        }

        private void btn_other_Menu(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            switch (Header)
            {
                case "_Draw Tree":

                    break;
                case "_Print Info":
                    UIshowSensorsLocations uIlocations = new UIshowSensorsLocations(myNetWork);
                    uIlocations.Show();
                    break;
                case "_Entir Network Routing Log":
                    UiRoutingDetailsLong routingLogs = new ui.UiRoutingDetailsLong(myNetWork);
                    routingLogs.Show();
                    break;
                case "_Log For Each Sensor":

                    break;
                //_Relatives:
                case "_Node Forwarding Probability Distributions":
                    {
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Forwarding Probability Distributions For Each Node";
                        foreach (Sensor source in myNetWork)
                        {
                            if (source.HistoricalRelativityRows.Count > 0)
                            {
                                ListControl List = new conts.ListControl();
                                List.lbl_title.Content = "Node:" + source.ID;
                                List.dg_date.ItemsSource = source.HistoricalRelativityRows;
                                windsow.stack_items.Children.Add(List);
                            }
                        }
                        windsow.Show();
                        break;
                    }
                //
                case "_Expermental Results":
                    UIExpermentResults xxxiu = new UIExpermentResults();
                    xxxiu.Show();
                    break;
                case "_Probability Matrix":
                    {
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Matrix";
                        AdjecentMatrix mat = new AdjecentMatrix();
                        List<DataTable> Tables = mat.ConvertToTable(myNetWork);
                        foreach(DataTable table in Tables) 
                        {
                            ListControl List = new conts.ListControl();
                            List.lbl_title.Content = table.TableName;
                            List.dg_date.ItemsSource = table.DefaultView;
                            windsow.stack_items.Children.Add(List);
                        }
                        windsow.Show();
                    }
                    break;
                //
                case "_Packets Paths":
                    UiRecievedPackertsBySink packsInsinkList = new UiRecievedPackertsBySink();
                    packsInsinkList.Show();

                    break;
                //
                case "_Random Numbers":

                    List<KeyValuePair<int, double>> rands = new List<KeyValuePair<int, double>>();
                    int index = 0;
                    foreach (Sensor sen in myNetWork)
                    {
                        foreach (SensorRoutingLog log in sen.Logs)
                        {
                            if (log.IsSend)
                            {
                                index++;
                                rands.Add(new KeyValuePair<int, double>(index, log.ForwardingRandomNumber));
                            }
                        }
                    }

                    UiRandomNumberGeneration wndsow = new ui.UiRandomNumberGeneration();
                    wndsow.chart_x.DataContext = rands;
                    wndsow.Show();

                    break;
                case "_Nodes Load":
                    {
                        SegmaManager sgManager = new SegmaManager();
                        Sensor sink = PublicParamerters.SinkNode;
                        List<string> Paths = new List<string>();
                        if (sink != null)
                        {
                            foreach (Datapacket pck in sink.PacketsList)
                            {
                                Paths.Add(pck.Path);
                            }
                          
                        }

                        sgManager.Filter(Paths);
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Nodes Load";
                        SegmaCollection collectionx = sgManager.GetCollection;
                        foreach(SegmaSource source in collectionx.GetSourcesList)
                        {
                            source.NumberofPacketsGeneratedByMe = myNetWork[source.SourceID].NumberofPacketsGeneratedByMe;
                            ListControl List = new conts.ListControl();
                            List.lbl_title.Content = "Source:" + source.SourceID + " Pks:" + source.NumberofPacketsGeneratedByMe + " Relays:" + source.RelaysCount + " Hops:" + source.HopsSum + " Mean:" + source.Mean+ " Variance:" + source.Veriance + " E:" + source.PathsSpread;
                            List.dg_date.ItemsSource = source.GetRelayNodes;
                            windsow.stack_items.Children.Add(List);
                        }
                        windsow.Show();
                        break;
                    }
                //_Distintc Paths
                case "_Distintc Paths":
                    {
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Distinct Paths for each Source";
                        DisPathConter dip = new DisPathConter();
                       List<ClassfyPathsPerSource> classfy= dip.ClassyfyDistinctPathsPerSources();
                        foreach (ClassfyPathsPerSource source in classfy)
                        {
                            ListControl List = new conts.ListControl();
                            List.lbl_title.Content = "Source:" + source.SourceID;
                            List.dg_date.ItemsSource = source.DistinctPathsForThisSource;
                            windsow.stack_items.Children.Add(List);
                        }
                        windsow.Show();
                        break;
                    }
            }
        }

        
        private void Btn_Send_packetsFromEachNode(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {
                // not random:
                MenuItem slected = sender as MenuItem;
                int pktsNumber = Convert.ToInt16(slected.Header.ToString().Split('_')[1]);
                rounds += pktsNumber;
                lbl_rounds.Content = rounds;

                for (int i = 1; i <= pktsNumber; i++)
                {
                    foreach (Sensor sen in myNetWork)
                    {
                        if (sen.ID != PublicParamerters.SinkNode.ID)
                        {
                            sen.GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                    }
                }

                /*
                foreach (Sensor sen in myNetWork)
                {
                    if (sen.ID != PublicParamerters.SinkNode.ID)
                    {
                        for (int i = 1; i <= pktsNumber; i++)
                        {
                            sen.GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                    }
                }*/

            }
            else
            {
                MessageBox.Show("Please selete the coverage.Coverage->Random");
            }
        }

        private void BuildTheTree(object sender, RoutedEventArgs e)
        {

        }

        private void tconrol_charts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((tconrol_charts.SelectedItem as TabItem).Name == "tab_Path_Efficiency")
            {
                List<List<KeyValuePair<int, double>>> Vals = PathEfficiencyChart.BuildPathEfficiencyAndDelayChart();
                if (Vals.Count == 2)
                {
                    col_Path_Efficiency.DataContext = Vals[0];
                    col_Delay.DataContext = Vals[1];
                }
                col_EnergyConsumptionForEachNode.DataContext = EnergyConsumptionForEachNode.BuildEnergyConsumptionForEachNodeChart(myNetWork);
            }
            else if ((tconrol_charts.SelectedItem as TabItem).Name == "tab_packets_chart")
            {
                List<List<KeyValuePair<int, double>>> Vals = PathEnergyChart.BuildChartPackets();
                if (Vals.Count == 2)
                {
                    col_packs_hops.DataContext = Vals[0];
                 //   col_packs_energy.DataContext= Vals[1];
                }
            }
            //

            else if ((tconrol_charts.SelectedItem as TabItem).Name == "tab_hops_distrubitions")
            {
                List<List<KeyValuePair<int, double>>> xxx= Distrubtions.FindDistubtions();

                List<KeyValuePair<int, double>> hops = xxx[0];
                List<KeyValuePair<int, double>> energy = xxx[1];

                List<KeyValuePair<int, double>> delay = xxx[2]; 

                cols_hops_ditrubtions.DataContext = hops;
                cols_energy_distribution.DataContext = energy;

                cols_delay_distribution.DataContext = delay;
            }
        }

        public void ClearExperment()
        {
            try
            {


                Canvas_SensingFeild.Children.Clear();
                if (myNetWork != null)
                    myNetWork.Clear();
               
                isCoverageSelected = false;
               

                HideSimulationParameters();
                isOpendtab_location = false;
                col_Path_Efficiency.DataContext = null;
                col_Delay.DataContext = null;
                col_EnergyConsumptionForEachNode.DataContext = null;

                
                cols_hops_ditrubtions.DataContext = null;
                lbl_hops_dis_network_info.Content = "";
                lbl_hops_dis_network_info.Content = "";
                cols_hops_ditrubtions.DataContext = null;
                cols_energy_distribution.DataContext = null;
                cols_delay_distribution.DataContext = null;

                PublicParamerters.PackeTSequenceID = 0;
                PublicParamerters.IsNetworkDied = false;
                PublicParamerters.Density = 0;
                PublicParamerters.NetworkName = "";
                PublicParamerters.DeadNodeList.Clear();
                PublicParamerters.NOP = 0;
                PublicParamerters.NOS = 0;
                PublicParamerters.Rounds = 0;
                PublicParamerters.PackeTSequenceID = 0;
                PublicParamerters.SinkNode = null;

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }


        private void ben_clear_click(object sender, RoutedEventArgs e)
        {
            ClearExperment();
        }

        bool isOpendtab_location = false;

        public object NetworkLifeTime { get; private set; }

        private void tab_network_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((tab_network.SelectedItem as TabItem).Name == "tab_location")
            {
                if (isOpendtab_location == false)
                {
                    if (isCoverageSelected)
                    {
                        col_network.DataContext = NodesLocationsScatter.BuildNodesLocationsScatterNodeChart(myNetWork);
                        col_Neighbors.DataContext = NodesLocationsScatter.GetNieborsDist(myNetWork);
                        isOpendtab_location = true;
                    }
                }
            }
            
        }

        private void lbl_show_grid_line_x_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (col_network_X_Gird.ShowGridLines == false) col_network_X_Gird.ShowGridLines = true;
            else col_network_X_Gird.ShowGridLines = false;
        }

        private void lbl_show_grid_line_y_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (col_network_Y_Gird.ShowGridLines == false) col_network_Y_Gird.ShowGridLines = true;
            else col_network_Y_Gird.ShowGridLines = false;
        }



        private void setDisributaions_Click(object sender, RoutedEventArgs e) 
        {
            if (myNetWork.Count >0)
            {
                if (isCoverageSelected)
                {
                    UIPowers cc = new ui.UIPowers(this);
                    cc.Show();
                }
                else
                {
                    MessageBox.Show("plz select coverage. Coverage->Random");
                }
            }
            else
            {
                MessageBox.Show("please select a network: File>importe");
            }
        }


        private void _set_paramertes_Click(object sender, RoutedEventArgs e)
        {
            ben_clear_click(sender, e);

            UiSetparamertes setpa = new UiSetparamertes(this);
            this.WindowState = WindowState.Minimized;
            setpa.Show();

        }

       

        private void btn_chek_lifetime_Click(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {
                this.WindowState = WindowState.Minimized;
                for (int i=0; ;i++)
                {
                    rounds++;
                    lbl_rounds.Content = rounds;
                    if (!PublicParamerters.IsNetworkDied)
                    {
                        foreach (Sensor sen in myNetWork)
                        {
                            if (sen.ID != PublicParamerters.SinkNode.ID)
                            {
                                sen.GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt,
                                    Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please selete the coverage. Coverage->Random");
            }
        }

        private void check_show_matrix_Checked(object sender, RoutedEventArgs e)
        {

        }


       

        private void btn_lifetime_s1_Click(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {
                if (PublicParamerters.SinkNode.PacketsList.Count == 0)
                {
                    ui.UiComputeLifeTimeScen1 lifewin = new UiComputeLifeTimeScen1(myNetWork);
                    lifewin.Show();
                }
                else
                {
                    MessageBox.Show("Please clear first: File->Clear!");
                }
            }
            else
            {
                MessageBox.Show("Please selected the Coverage.Coverage->Random");
            }
        }

        private void EnCon_Scenario2_Click(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {

                if (PublicParamerters.SinkNode.PacketsList.Count == 0)
                {

                    if (myNetWork.Count >= 600)
                    {
                        DoEnergyConsmption1Experment x =
                            new ExpermentsResults.DoEnergyConsmption1Experment(myNetWork, 500);
                        List<EnergyConsmption1> dataColle = x.Perform();
                        UiShowLists win = new UiShowLists();
                        win.Title = "Scenario2";
                        ListControl ContlList = new ListControl();
                        ContlList.lbl_title.Content = "Scenario2";
                        ContlList.dg_date.ItemsSource = dataColle;
                        win.stack_items.Children.Add(ContlList);
                        win.Show();
                    }
                    else
                    {
                        MessageBox.Show("please use the network:http://staff.ustc.edu.cn/~anmande/mynets/600Nodes.xlsx");
                    }
                }
                else
                {
                    MessageBox.Show("Please clear first: File->Clear!");
                }

            }
            else
            {
                MessageBox.Show("please select the Coverage. Coverage->Random");
            }
        }

        private void EnCon_Scenario1_Click(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {
                if (PublicParamerters.SinkNode.PacketsList.Count == 0)
                {
                    ui.UiSelectNodesWidthDistance win = new UiSelectNodesWidthDistance(this);
                    win.Show();
                }
                else
                {
                    MessageBox.Show("Please clear first: File->Clear!");
                }
            }
            else
            {
                MessageBox.Show("Please selected the Coverage.Coverage->Random");
            }

        }

        public void SendPackectPerSecond(double s)
        {
            RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(s);
            RandomSelectSourceNodesTimer.Start();
            PacketRate = "1 packet per " + s + " s";
        }

        private void Btn_comuputeEnergyCon_withinTime_Click(object sender, RoutedEventArgs e)
        {

            if (Settings.Default.IsIntialized)
            {
                MessageBox.Show("File->clear and try agian.");
            }
            else
            {
                PacketRate = "";
                stopSimlationWhen = 0;
                UISetParEnerConsum con = new UISetParEnerConsum(this);
                con.Owner = this;
                con.Show();
                top_menu.IsEnabled = false;
            }
        }
    }
}


           
