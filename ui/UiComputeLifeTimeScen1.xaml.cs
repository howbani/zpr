using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ProZoneRouting.Modules;
using ProZoneRouting.Parameters;
using System;
using ProZoneRouting.Properties;
using ProZoneRouting.Lifetime;

namespace ProZoneRouting.ui
{
    /// <summary>
    /// Interaction logic for UiComputeLifeTimeScen1.xaml
    /// </summary>
    public partial class UiComputeLifeTimeScen1 : Window
    {
        List<Sensor> myNetWork;
        public UiComputeLifeTimeScen1(List<Sensor> _network) 
        {
            InitializeComponent();

            myNetWork = _network;
            for (int i=1;i< myNetWork.Count;i++) 
            {
                com_nos.Items.Add(new ComboBoxItem() { Content=i.ToString() });
                com_nop.Items.Add(new ComboBoxItem() { Content = i.ToString() });
                com_NOD.Items.Add(new ComboBoxItem() { Content = i.ToString() });
                com_num_of_deadNodes.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            }

            com_nos.Text = "5";
            com_nop.Text = "50";
            com_NOD.Text = "1";
            com_num_of_deadNodes.Text = "1";
        }

        private void btn_compute_life_time_Click(object sender, RoutedEventArgs e)
        {
           
            Settings.Default.KeepLogs = false;
            int NOD = Convert.ToInt16(com_NOD.Text);
            int NOS = Convert.ToInt16(com_nos.Text);
            int NOP = Convert.ToInt16(com_nop.Text);
            PublicParamerters.NOS = NOS;
            PublicParamerters.NOP = NOP;
            int round = 0;
            while (PublicParamerters.DeadNodeList.Count < NOD)
            {
                NetworkLifeTime Ran = new NetworkLifeTime();
                Ran.RandimSelect(myNetWork, NOS, NOP);
                round++;
                PublicParamerters.Rounds = round;
                PublicParamerters.SinkNode.PacketsList.Clear();
            }

            UiNetworkLifetimeReport xx = new UiNetworkLifetimeReport();
            xx.Title = "Lifetime report";
            xx.dg_grid.ItemsSource = PublicParamerters.DeadNodeList;
            xx.Show();

            PublicParamerters.NOS = 0;
            PublicParamerters.NOP = 0;

            this.Close();

        }


        private void btn_from_all_Click(object sender, RoutedEventArgs e)
        {

            Settings.Default.KeepLogs = false;
            int NOD = Convert.ToInt16(com_num_of_deadNodes.Text);
            PublicParamerters.NOS = myNetWork.Count;
            PublicParamerters.NOP = 1;
            int round = 0;
            while (PublicParamerters.DeadNodeList.Count < NOD)
            {
                NetworkLifeTime Ran = new NetworkLifeTime();
                Ran.FromAllNodes(myNetWork);
                round++;
                PublicParamerters.Rounds = round;
                PublicParamerters.SinkNode.PacketsList.Clear();
            }

            UiNetworkLifetimeReport xx = new UiNetworkLifetimeReport();
            xx.Title = "Lifetime report";
            xx.dg_grid.ItemsSource = PublicParamerters.DeadNodeList;
            xx.Show();

            PublicParamerters.NOS = 0;
            PublicParamerters.NOP = 0;

            this.Close();
        }
    }
}
