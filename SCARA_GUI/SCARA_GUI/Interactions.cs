using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SCARA_GUI
{
    public partial class MainWindow : Window
    {
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Move_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Piston_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Gripper_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Home_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LogBox_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            string dire = Environment.CurrentDirectory;
            try
            {
                Process.Start("explorer.exe", $"/select, {dire}\\{LogFile}");
            }
            catch { }
        }

    }
}
