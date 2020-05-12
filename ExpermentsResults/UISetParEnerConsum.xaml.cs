using ProZoneRouting.Properties;
using ProZoneRouting.ui;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ProZoneRouting.ExpermentsResults 
{
    /// <summary>
    /// Interaction logic for UISetParEnerConsum.xaml
    /// </summary>
    public partial class UISetParEnerConsum : Window
    {
        MainWindow _MainWindow;
        public UISetParEnerConsum(MainWindow __MainWindow_)
        {
            InitializeComponent();
            _MainWindow = __MainWindow_;

            for (int i = 60; i <= 1000; i = i + 60)
            {
                comb_simuTime.Items.Add(i);
               
            }
            comb_simuTime.Text = "300";

            comb_packet_rate.Items.Add("0.001");
            comb_packet_rate.Items.Add("0.01");
            comb_packet_rate.Items.Add("0.1");
            comb_packet_rate.Items.Add("0.5");
            for (int i = 1; i <= 5; i++)
            {
                comb_packet_rate.Items.Add(i);
            }

            comb_packet_rate.Text = "0.1";



            int conrange = 5;
            for (int i = 0; i <= conrange; i++)
            {
                if (i == conrange)
                {
                    double dc = Convert.ToDouble(i);
                    com_direction.Items.Add(dc);
                    com_energy.Items.Add(dc);
                    com_prependicular.Items.Add(dc);
                    com_transmision_distance.Items.Add(dc);
                }
                else
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        string str = i + "." + j;
                        double dc = Convert.ToDouble(str);
                        com_direction.Items.Add(dc);
                        com_energy.Items.Add(dc);
                        com_prependicular.Items.Add(dc);
                        com_transmision_distance.Items.Add(dc);

                    }
                }
            }

            // set defuals:
            com_direction.Text = Settings.Default.DirectionDistCnt.ToString();
            com_energy.Text = Settings.Default.EnergyDistCnt.ToString();
            com_prependicular.Text = Settings.Default.PrepDistanceDistCnt.ToString();
            com_transmision_distance.Text = Settings.Default.TransDistanceDistCnt.ToString();



        }


        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {



            Settings.Default.KeepLogs = Convert.ToBoolean(chk_save_logs.IsChecked);


            Settings.Default.PrepDistanceDistCnt = Convert.ToDouble(com_prependicular.Text);
            Settings.Default.TransDistanceDistCnt = Convert.ToDouble(com_transmision_distance.Text);
            Settings.Default.EnergyDistCnt = Convert.ToDouble(com_energy.Text);
            Settings.Default.DirectionDistCnt = Convert.ToDouble(com_direction.Text);

            int stime = 100000000;
            double packper = Convert.ToDouble(comb_packet_rate.Text);
            _MainWindow.stopSimlationWhen = stime;
            _MainWindow.SendPackectPerSecond(packper);
            _MainWindow.RandomDeplayment(0);

            this.Close();

        }

        

      


       
    }
}
