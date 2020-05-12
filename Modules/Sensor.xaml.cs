using ProZoneRouting.Computations;
using ProZoneRouting.Energy;
using ProZoneRouting.Logs;
using ProZoneRouting.ForwardingProbabilities;
using ProZoneRouting.DataPacket;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ProZoneRouting.Parameters;
using ProZoneRouting.ui;
using ProZoneRouting.Properties;
#pragma warning disable CS0105 // The using directive for 'ProZoneRouting.ForwardingProbabilities' appeared previously in this namespace
#pragma warning restore CS0105 // The using directive for 'ProZoneRouting.ForwardingProbabilities' appeared previously in this namespace

namespace ProZoneRouting.Modules
{

    /// <summary>
    /// Interaction logic for Node.xaml
    /// </summary>
    public partial class Sensor : UserControl
    {
       
        public MainWindow MainWindow { get; set; }

        public static double SR { get; set; } //the radios of SENSING range.
        public double SensingRangeRadius { get { return SR; } }
        public static double CR { get; set; }  // the radios of COMUNICATION range. double OF SENSING RANGE
        public double ComunicationRangeRadius { get { return CR; } }

        public double BatteryIntialEnergy; // jouls // value will not be changed
        private double _ResidualEnergy; //// jouls this value will be changed according to useage of battery

        // 
        public double ResidualEnergy // jouls this value will be changed according to useage of battery
        {
           get { return _ResidualEnergy; }
          set
            {
                _ResidualEnergy = value;
                Prog_batteryCapacityNotation.Value = _ResidualEnergy;
            }
        } //@unit(JOULS);


        // the nodes are sordted acording to
        public List<Datapacket> PacketsList = new List<Datapacket>();// for source nodes, the generated. for the sink is the packets that recived.
        public List<SensorRoutingLog> Logs = new List<SensorRoutingLog>();
       // public List<RelativityRow> RelativityRows = new List<RelativityRow>(); // add them:
        public List<RoutingRandomVariable> HistoricalRelativityRows = new List<RoutingRandomVariable>(); // for record, u can remove it.
        public List<Sensor> NeighboreNodes { get; set; } // overlapping sensors: called vector in grouping algorithm.
        FirstOrderRadioModel EnergyModel = new FirstOrderRadioModel();
        public double RoutingDataLength; // @ UNIT bit. ==1KB

     

        public int NumberofPacketsGeneratedByMe = 0;
        
        
        public int ID { get; set; }
        public Sensor(int nodeID)
        {
            InitializeComponent();
            RoutingDataLength = PublicParamerters.RoutingDataLength;
            BatteryIntialEnergy = PublicParamerters.BatteryIntialEnergy; // the value will not be change
            
            ResidualEnergy = BatteryIntialEnergy;// joules. intializing.
            Prog_batteryCapacityNotation.Value = BatteryIntialEnergy;
            Prog_batteryCapacityNotation.Maximum = BatteryIntialEnergy;
            lbl_Sensing_ID.Content = nodeID;
            ID = nodeID;

            //:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Prog_batteryCapacityNotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
            double val = ResidualEnergyPercentage;
            if (val <=0)
            {

                // dead certificate:
                Lifetime.DeadNodesRecord recod = new Lifetime.DeadNodesRecord();
                recod.DeadAfterPackets = PublicParamerters.PackeTSequenceID;
                recod.DeadOrder = PublicParamerters.DeadNodeList.Count + 1;
                recod.Rounds = PublicParamerters.Rounds + 1;
                recod.DeadNodeID = ID;
                recod.NOS = PublicParamerters.NOS;
                recod.NOP = PublicParamerters.NOP;
                recod.RoutingZone = Settings.Default.ZoneWidthCnt;
                PublicParamerters.DeadNodeList.Add(recod);

                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col0));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col0));
            }
            if (val >= 1 && val <= 9)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col1_9));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col1_9));
            }

            if (val >= 10 && val <= 19)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col10_19));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col10_19));
            }

            if (val >= 20 && val <= 29)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col20_29));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col20_29));
            }

            // full:
            if (val >= 30 && val <= 39)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col30_39));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col30_39));
            }
            // full:
            if (val >= 40 && val <= 49)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col40_49));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col40_49));
            }
            // full:
            if (val >= 50 && val <= 59)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col50_59));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col50_59));
            }
            // full:
            if (val >= 60 && val <= 69)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col60_69));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col60_69));
            }
            // full:
            if (val >= 70 && val <= 79)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col70_79));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col70_79));
            }
            // full:
            if (val >= 80 && val <= 89)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col80_89));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col80_89));
            }
            // full:
            if (val >= 90 && val <= 100)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col90_100));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Coloring.col90_100));
            }
           
        }

        public double ResidualEnergyPercentage
        {
            get { return (ResidualEnergy / BatteryIntialEnergy) * 100; }
        }
        /// <summary>
        /// set the com range range!
        /// r is the raduis or com range.
        /// </summary>
        public double VisualizedRadius
        {
            get { return Ellipse_Sensing_range.Width / 2; }
            set
            {
                // sensing range:
                Ellipse_Sensing_range.Height = value * 2; // heigh= sen rad*2;
                Ellipse_Sensing_range.Width = value * 2; // Width= sen rad*2;
                SR = VisualizedRadius;
                CR = SR * 2; // comunication rad= sensing rad *2;

                // device:
                Device_Sensor.Width = value * 4; // device = sen rad*4;
                Device_Sensor.Height = value * 4;
                // communication range
                Ellipse_Communication_range.Height = value * 4; // com rang= sen rad *4;
                Ellipse_Communication_range.Width = value * 4;

                // battery:
                Prog_batteryCapacityNotation.Width = 8;
                Prog_batteryCapacityNotation.Height = 2;
            }

            /*
            get { return Ellipse_com_rang.Width/2; }
            set
            {
                Device_Sensor.Width = value * 2;
                Device_Sensor.Height = value * 2;
                Ellipse_com_rang.Height = value * 2;
                Ellipse_com_rang.Width = value * 2;
                SR = ComnicationRaduis;
            }*/
        }


        /// <summary>
        /// Real postion of object.
        /// </summary>
        public Point Position
        {
            get
            {
                double x = Device_Sensor.Margin.Left;
                double y = Device_Sensor.Margin.Top;
                Point p = new Point(x, y);
                return p;
            }
            set
            {
                Point p = value;
                Device_Sensor.Margin = new Thickness(p.X, p.Y, 0, 0);
            }
        }

        /*
        /// <summary>
        /// center location of node.
        /// </summary>
        public Point CenterLocation 
        {
            get
            {
                double x = Device_Sensor.Margin.Left;
                double y = Device_Sensor.Margin.Top;
                Point p = new Point(x+ComnicationRaduis, y+ComnicationRaduis);
                return p;
            }
        }*/


        /// <summary>
        /// center location of node.
        /// </summary>
        public Point CenterLocation
        {
            get
            {
                double x = Device_Sensor.Margin.Left;
                double y = Device_Sensor.Margin.Top;
                Point p = new Point(x + CR, y + CR);
                return p;
            }
        }


        //////////////////////////////Zone//////////////////////////
        /// <summary>
        /// The zone is defined after selecting the the sink.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public List<Point> MyRoutingZone
        {
            get
            {
                if (PublicParamerters.SinkNode != null)
                {
                    if (this != null)
                    {
                        // no need to calculate for sink.
                        if (this.ID != PublicParamerters.SinkNode.ID)
                        {
                            Point p1, p2;
                            p1 = CenterLocation;// the source Location
                            p2 = PublicParamerters.SinkNode.CenterLocation;// sink location
                            List<Point> reL = new List<Point>();

                            double x1 = p1.X;
                            double x2 = p2.X;
                            double y1 = p1.Y;
                            double y2 = p2.Y;
                            double Halfw = Settings.Default.ZoneWidthCnt / 2;
                            double DistanceBSS = Operations.DistanceBetweenTwoPoints(p1, p2);
                            double DelateY = Halfw * ((x2 - x1) / DistanceBSS);
                            double DeltaX = Halfw * ((y1 - y2) / DistanceBSS);

                            Point p3 = new Point(x1 + DeltaX, y1 + DelateY);
                            Point p4 = new Point(x1 - DeltaX, y1 - DelateY);
                            Point p5 = new Point(x2 + DeltaX, y2 + DelateY);
                            Point p6 = new Point(x2 - DeltaX, y2 - DelateY);

                            reL.Add(p3);
                            reL.Add(p4);
                            reL.Add(p5);
                            reL.Add(p6);

                            return reL;
                        }
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }
        }



        public bool isZoneDrawn = false;
        public List<MyLine> MyZoneLines = new List<MyLine>();
        public void DrawMyZone()
        {
            if (isZoneDrawn ==false)
            {
                List<Point> zone = MyRoutingZone;
                if (zone != null)
                {
                    if (zone.Count == 4)
                    {
                        Point p3 = zone[0];
                        Point p4 = zone[1];
                        Point p5 = zone[2];
                        Point p6 = zone[3];

                        MyLine l34 = new MyLine(p3, p4, MainWindow.Canvas_SensingFeild);
                        MyLine l56 = new MyLine(p5, p6, MainWindow.Canvas_SensingFeild);
                        MyLine l35 = new MyLine(p3, p5, MainWindow.Canvas_SensingFeild);
                        MyLine l46 = new MyLine(p4, p6, MainWindow.Canvas_SensingFeild);
                        isZoneDrawn = true;
                        MyZoneLines.Add(l34);
                        MyZoneLines.Add(l56);
                        MyZoneLines.Add(l35);
                        MyZoneLines.Add(l46);
                    }
                }
            }
        }

        public void UndrawMyZone()
        {
            if (isZoneDrawn == true)
            {
                if (MyZoneLines.Count > 0)
                {
                    foreach (MyLine myl in MyZoneLines)
                    {
                        MainWindow.Canvas_SensingFeild.Children.Remove(myl.GetMyPath());
                    }
                    MyZoneLines.Clear();
                    isZoneDrawn = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {

            Ellipse_Communication_range.Visibility = Visibility.Visible;
            DrawMyZone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Ellipse_Communication_range.Visibility = Visibility.Hidden;
            UndrawMyZone();
        }

        /***********************END ZONE***********************/

       /*
        bool StartMove = false;
        private void Device_Sensor_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point P = e.GetPosition(MainWindow.Canvas_SensingFeild);
                P.X = P.X - ComnicationRaduis;
                P.Y = P.Y - ComnicationRaduis;
                this.Position = P;
                StartMove = true;
            }
        }

        private void Device_Sensor_MouseMove(object sender, MouseEventArgs e)
        {
            if (StartMove)
            {
                System.Windows.Point P = e.GetPosition(MainWindow.Canvas_SensingFeild);
                P.X = P.X - ComnicationRaduis;
                P.Y = P.Y - ComnicationRaduis;
                this.Position = P;
            }
        }
        */
        bool StartMove = false;
        private void Device_Sensor_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point P = e.GetPosition(MainWindow.Canvas_SensingFeild);
                P.X = P.X - CR;
                P.Y = P.Y - CR;
                this.Position = P;
                StartMove = true;
            }
        }

        private void Device_Sensor_MouseMove(object sender, MouseEventArgs e)
        {
            if (StartMove)
            {
                System.Windows.Point P = e.GetPosition(MainWindow.Canvas_SensingFeild);
                P.X = P.X - CR;
                P.Y = P.Y - CR;
                this.Position = P;
            }
        }

        private void Device_Sensor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StartMove = false;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

        /// <summary>
        /// generate and sent
        /// </summary>
        /// <returns></returns>
        public void GeneratePacketAndSent(bool isRandom,double EnergyDistCnt, double TransDistanceDistCnt, double DirectionDistCnt, double PrepDistanceDistCnt)
        {
            PublicParamerters.PackeTSequenceID++;
            DistributionsComputations relatives = new DistributionsComputations();
            List< RoutingRandomVariable> RelativityRows = relatives.ComputeDistributions(this, PublicParamerters.SinkNode, this, EnergyDistCnt, TransDistanceDistCnt, DirectionDistCnt, PrepDistanceDistCnt);  

            if (RelativityRows != null)
            {
                SeletNextHopRandomlly randomGenrator = new SeletNextHopRandomlly();
                RoutingRandomVariable nextHop = randomGenrator.SelectNextHop(RelativityRows);
                if (nextHop != null)
                {
                    if (RelativityRows.Count > 0)
                    {
                        Datapacket datap = new Datapacket(this, nextHop.NextHopNode);
                        
                        datap.ForwardingRandomNumber = randomGenrator.RandomeNumber;
                        datap.SourceNode = this;
                        datap.SourceNodeID = ID;
                        datap.Path = this.ID.ToString(); // start
                        datap.Distance = Operations.DistanceBetweenTwoSensors(PublicParamerters.SinkNode, this);

                        datap.IsRandomControls = isRandom;
                        datap.EnergyDistCnt = EnergyDistCnt;
                        datap.TransDistanceDistCnt = TransDistanceDistCnt;
                        datap.DirectionDistCnt = DirectionDistCnt;
                        datap.PrepDistanceDistCnt = PrepDistanceDistCnt;
                        datap.RoutingZoneWidthCnt = Settings.Default.ZoneWidthCnt;
                        NumberofPacketsGeneratedByMe++;
                        datap.RoutingProbabilityForPath = nextHop.RoutingVariableProb;

                      
                        datap.PacektSequentID = PublicParamerters.PackeTSequenceID;
                        SendData(this, nextHop.NextHopNode, datap);// send the data:
                        if (Settings.Default.ShowRoutigZone)
                        {
                            DrawMyZone();
                        }
                    }

                    this.lbl_Sensing_ID.Foreground = Brushes.Red;
                   // this.lbl_Sensing_ID.FontWeight = FontWeights.Bold;
                   
                    //: historial record:
                    if (Settings.Default.KeepLogs)
                    {
                        HistoricalRelativityRows.AddRange(RelativityRows);
                    }
                }
            }
        }


        public void GeneratePacketAndSent(int numOfPackets, bool isRandom, double en, double dis, double dir, double pre)
        {
            for (int j = 1; j <= numOfPackets; j++)
            {
               
                GeneratePacketAndSent(isRandom, en, dis, dir, pre);
            }
        }

      
        /// <summary>
        ///  select this node as a source and let it 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btn_send_packet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lbl_title = sender as Label;
            switch(lbl_title.Name)
            {
                case "btn_send_1_packet":
                    {
                        GeneratePacketAndSent(false,Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        break;
                    }
                case "btn_send_10_packet":
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                        break;
                    }

                case "btn_send_100_packet":
                    {
                        for (int j = 1; j <= 100; j++)
                        {
                            GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                        break;
                    }

                case "btn_send_300_packet":
                    {
                        for (int j = 1; j <= 300; j++)
                        {
                            GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                        break;
                    }

                case "btn_send_1000_packet":
                    {
                        for (int j = 1; j <= 1000; j++)
                        {
                            GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                        break;
                    }

                case "btn_send_5000_packet":
                    {
                        for (int j = 1; j <= 5000; j++)
                        {
                            GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                        break;
                    }

                case "_rbtn_send_1_packet":
                    {
                        List<double> Controls = RandomControls.Generate4Controls();
                        GeneratePacketAndSent(true, Controls[0], Controls[1], Controls[2], Controls[3]); 

                        break;
                    }
                case "_rbtn_send_10_packet":
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            List<double> Controls = RandomControls.Generate4Controls();
                            GeneratePacketAndSent(true, Controls[0], Controls[1], Controls[2], Controls[3]);
                        }
                        break;
                    }

                case "_rbtn_send_100_packet":
                    {
                        for (int j = 1; j <= 100; j++)
                        {
                            List<double> Controls = RandomControls.Generate4Controls();
                            GeneratePacketAndSent(true, Controls[0], Controls[1], Controls[2], Controls[3]);
                        }
                        break;
                    }
                case "_rbtn_send_300_packet":
                    {
                        for (int j = 1; j <= 300; j++)
                        {
                            List<double> Controls = RandomControls.Generate4Controls();
                            GeneratePacketAndSent(true, Controls[0], Controls[1], Controls[2], Controls[3]);
                        }
                        break;
                    }
                case "_rbtn_send_1000_packet":
                    {
                        for (int j = 1; j <= 1000; j++)
                        {
                            List<double> Controls = RandomControls.Generate4Controls();
                            GeneratePacketAndSent(true, Controls[0], Controls[1], Controls[2], Controls[3]);
                        }
                        break;
                    }
            }
        }


        /// <summary>
        /// send data
        /// </summary>
        public void SendData(Sensor FromSensor, Sensor ToSensor, Datapacket datap)
        {
            //   Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            //  {
            if (ToSensor != null || FromSensor != null)
            {
                if (ToSensor.ID != FromSensor.ID)
                {
                    if (FromSensor.ResidualEnergy > 0)
                    {

                        SensorRoutingLog log = new SensorRoutingLog();
                        log.IsSend = true;
                        log.NodeID = this.ID;
                        log.Operation = "To:" + ToSensor.ID;
                        log.Time = DateTime.Now;
                        log.Distance_M = Operations.DistanceBetweenTwoSensors(FromSensor, ToSensor);
                        log.UsedEnergy_Nanojoule = EnergyModel.Transmit(RoutingDataLength, log.Distance_M);
                        //

                        // set the remain battery Energy:
                        double remainEnergy = ResidualEnergy - log.UsedEnergy_Joule;
                        FromSensor.ResidualEnergy = remainEnergy;
                        log.RemaimBatteryEnergy_Joule = ResidualEnergy;
                        log.PID = datap.PacektSequentID;
                        //
                        log.DirectionDistCnt = datap.DirectionDistCnt;
                        log.EnergyDistCnt = datap.EnergyDistCnt;
                        log.PrepDistanceDistCnt = datap.PrepDistanceDistCnt;
                        log.TransDistanceDistCnt = datap.TransDistanceDistCnt;
                        log.RoutingZoneWidthCnt = datap.RoutingZoneWidthCnt;
                        // add the path:457430817
                        datap.UsedEnergy_Joule += log.UsedEnergy_Joule;
                        //

                        if (Settings.Default.KeepLogs)
                        {
                            log.RelaySequence = datap.Hops + 1;
                            log.ForwardingRandomNumber = datap.ForwardingRandomNumber;
                            FromSensor.Logs.Add(log); // keep logs for each node.
                            MainWindow.Canvas_SensingFeild.Children.Add(datap); // add the lines to the boad.
                        }


                        // ToSensor ReceiveData
                        ToSensor.ReceiveData(FromSensor, ToSensor, datap);
                    }
                    else
                    {
                        PublicParamerters.IsNetworkDied = true;
                        
                        this.Ellipse_Communication_range.Fill = Brushes.Brown; // die out node.                                             // MessageBox.Show("DeadNODE!");
                    }
                }
            }
            else
            {

            }
            //  });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FromSensor"></param>
        /// <param name="ToSensor"></param>
        /// <param name="datap"></param>
        public void ReceiveData(Sensor FromSensor, Sensor ToSensor, Datapacket datap)
        {
            if (ToSensor != null || FromSensor != null)
            {
                if (ToSensor.ID != FromSensor.ID)
                {
                    if (ToSensor.ResidualEnergy > 0)
                    {
                        SensorRoutingLog log = new SensorRoutingLog();
                        log.IsReceive = true;
                        log.NodeID = this.ID;
                        log.Operation = "From:" + FromSensor.ID;
                        log.Time = DateTime.Now;
                        log.Distance_M = Operations.DistanceBetweenTwoSensors(ToSensor, FromSensor);
                        log.UsedEnergy_Nanojoule = EnergyModel.Receive(RoutingDataLength);
                        log.PID = datap.PacektSequentID;
                        // set the remain battery Energy:
                        if (ToSensor.ID !=PublicParamerters.SinkNode.ID)
                        {
                            double remainEnergy = ResidualEnergy - log.UsedEnergy_Joule;
                            ToSensor.ResidualEnergy = remainEnergy;
                            log.RemaimBatteryEnergy_Joule = ResidualEnergy;
                        }
                     
                        // routing distance:
                        datap.Path += ">" + ToSensor.ID;
                        datap.RoutingDistance += log.Distance_M;
                        datap.Hops += 1;
                        datap.UsedEnergy_Joule += log.UsedEnergy_Joule;
                        datap.Delay += DelayModel.DelayModel.Delay(FromSensor, ToSensor);

                        // random：
                        if (datap.IsRandomControls)
                        {
                            // forward: with random random controls:
                            // calculate adil relatives:
                            Sensor RelayNode = ToSensor;
                            DistributionsComputations relatives = new DistributionsComputations();
                            List<double> Controls = RandomControls.Generate4Controls();
                            List<RoutingRandomVariable> RelativityRows = relatives.ComputeDistributions(datap.SourceNode, PublicParamerters.SinkNode, RelayNode, Controls[0], Controls[1], Controls[2], Controls[3]);
                            SeletNextHopRandomlly randomGenrator = new SeletNextHopRandomlly();
                            RoutingRandomVariable nextHop = randomGenrator.SelectNextHop(RelativityRows);
                            if (nextHop != null) // not the sink
                            {
                                //: historial record:
                                if (Settings.Default.KeepLogs)
                                {
                                    // log.ForwardingRandomNumber = datap.ForwardingRandomNumber;
                                    ToSensor.Logs.Add(log); // add the log
                                    HistoricalRelativityRows.AddRange(RelativityRows); // add the log
                                }
                                // forward:
                                Datapacket forwardPacket;
                                if (RelativityRows.Count > 0)
                                {
                                    Sensor NextHopNode = nextHop.NextHopNode;
                                    forwardPacket = new Datapacket(RelayNode, NextHopNode);
                                    forwardPacket.ForwardingRandomNumber = randomGenrator.RandomeNumber;
                                    forwardPacket.SourceNodeID = datap.SourceNodeID;
                                    forwardPacket.Distance = datap.Distance;
                                    forwardPacket.RoutingDistance = datap.RoutingDistance;
                                    forwardPacket.Path = datap.Path;
                                    forwardPacket.Hops = datap.Hops;
                                    forwardPacket.UsedEnergy_Joule = datap.UsedEnergy_Joule;
                                    forwardPacket.Delay = datap.Delay;
                                    forwardPacket.SourceNode = datap.SourceNode;
                                    forwardPacket.IsRandomControls = true;
                                    forwardPacket.RoutingZoneWidthCnt = datap.RoutingZoneWidthCnt;
                                    forwardPacket.EnergyDistCnt = Controls[0];
                                    forwardPacket.TransDistanceDistCnt = Controls[1];
                                    forwardPacket.DirectionDistCnt = Controls[2];
                                    forwardPacket.PrepDistanceDistCnt = Controls[3];
                                    forwardPacket.RoutingProbabilityForPath = datap.RoutingProbabilityForPath * nextHop.RoutingVariableProb;
                                    forwardPacket.PacektSequentID = datap.PacektSequentID;
                                    RelayNode.SendData(RelayNode, NextHopNode, forwardPacket);
                                }
                            }
                            else // this sink:
                            {
                                if (RelayNode.ID == PublicParamerters.SinkNode.ID)
                                {
                                    RelayNode.PacketsList.Add(datap);
                                    
                                }
                            }
                        }
                        else
                        {
                            // forward: without random controls:
                            // calculate adil relatives:
                            Sensor RelayNode = ToSensor;
                            DistributionsComputations relatives = new DistributionsComputations();
                            List<RoutingRandomVariable> RelativityRows = relatives.ComputeDistributions(datap.SourceNode, PublicParamerters.SinkNode, RelayNode, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                            SeletNextHopRandomlly randomGenrator = new SeletNextHopRandomlly();
                            RoutingRandomVariable nextHop = randomGenrator.SelectNextHop(RelativityRows);
                            if (nextHop != null) // not the sink
                            {
                                //: historial record:
                                if (Settings.Default.KeepLogs)
                                {
                                    // log.ForwardingRandomNumber = datap.ForwardingRandomNumber;
                                    ToSensor.Logs.Add(log); // add the log
                                    HistoricalRelativityRows.AddRange(RelativityRows); // add the log
                                }
                                // forward:
                                Datapacket forwardPacket;
                                if (RelativityRows.Count > 0)
                                {
                                    Sensor NextHopNode = nextHop.NextHopNode;
                                    forwardPacket = new Datapacket(RelayNode, NextHopNode);
                                    forwardPacket.ForwardingRandomNumber = randomGenrator.RandomeNumber;
                                    forwardPacket.SourceNodeID = datap.SourceNodeID;
                                    forwardPacket.Distance = datap.Distance;
                                    forwardPacket.RoutingDistance = datap.RoutingDistance;
                                    forwardPacket.Path = datap.Path;
                                    forwardPacket.Hops = datap.Hops;
                                    forwardPacket.UsedEnergy_Joule = datap.UsedEnergy_Joule;
                                    forwardPacket.Delay = datap.Delay;
                                    forwardPacket.SourceNode = datap.SourceNode;
                                    forwardPacket.RoutingZoneWidthCnt = datap.RoutingZoneWidthCnt;
                                    forwardPacket.IsRandomControls = false;
                                    forwardPacket.PrepDistanceDistCnt = datap.PrepDistanceDistCnt;
                                    forwardPacket.TransDistanceDistCnt = datap.TransDistanceDistCnt;
                                    forwardPacket.EnergyDistCnt = datap.EnergyDistCnt;
                                    forwardPacket.DirectionDistCnt = datap.DirectionDistCnt;
                                    forwardPacket.RoutingProbabilityForPath = datap.RoutingProbabilityForPath * nextHop.RoutingVariableProb;
                                    forwardPacket.PacektSequentID = datap.PacektSequentID;
                                    RelayNode.SendData(RelayNode, NextHopNode, forwardPacket);
                                }
                            }
                            else // this sink:
                            {
                                if (RelayNode.ID == PublicParamerters.SinkNode.ID)
                                {
                                    RelayNode.PacketsList.Add(datap);
                                    

                                }
                            }
                        } 

                    }
                    else
                    {
                        this.Ellipse_Communication_range.Fill = Brushes.Brown;// die out node
                                                                        // MessageBox.Show("DeadNODE!");
                    }
                }
            }
        }

        private void btn_show_relativyt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(HistoricalRelativityRows.Count>0)
            {
                UiShowRelativityForAnode re = new ui.UiShowRelativityForAnode();
                re.dg_relative_list.ItemsSource = HistoricalRelativityRows;
                re.Show();
            }
        }

        private void lbl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ToolTip = new Label() { Content = ResidualEnergyPercentage.ToString("00.00") + "%" };
        }

        private void btn_show_routing_log_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Logs.Count>0)
            {
                UiShowRelativityForAnode re = new ui.UiShowRelativityForAnode();
                re.dg_relative_list.ItemsSource = Logs;
                re.Show();
            }
        }

        private void btn_draw_random_numbers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<KeyValuePair<int, double>> rands = new List<KeyValuePair<int, double>>();
            int index = 0;
            foreach (SensorRoutingLog log in Logs )
            {
                if(log.IsSend)
                {
                    index++;
                    rands.Add(new KeyValuePair<int, double>(index, log.ForwardingRandomNumber));
                }
            }
            UiRandomNumberGeneration wndsow = new ui.UiRandomNumberGeneration();
            wndsow.chart_x.DataContext = rands;
            wndsow.Show();
        }

       
    }
}
