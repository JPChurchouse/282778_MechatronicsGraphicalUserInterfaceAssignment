using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SCARA_GUI
{
    public partial class MainWindow : Window
    {
        private void WindowResized(object sender, EventArgs e) 
        { 
            UpdateFontSize(); 
        }

        private void menu_OpenFile_Clicked(object sender, EventArgs args)
        {
            OpenLogFile();
        }

        private void menu_Vis_Clicked(object sender, EventArgs e)
        {
            UpdateFontSize();
        }

        private void menu_Out_Clicked(object sender, EventArgs e)
        {
            text_OuputLog.Text = string.Empty;
        }

        private void menu_Advanced_Clicked(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }


        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!SERIALPORT.IsOpen) ScanAndConnect();
            else Disconnect();
        }
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            SendData("STOP");
        }
        private void btn_Move_Click(object sender, RoutedEventArgs e)
        {
            SendData($"MOVE,{txt_MoveX.Text},{txt_MoveY.Text},{txt_MoveW.Text}");
        }
        private void btn_Piston_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Gripper_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Home_Click(object sender, RoutedEventArgs e)
        {
            SendData("HOME");
        }
        private void LogBox_DoubleClicked(object sender, MouseButtonEventArgs e) 
        { 
            OpenLogFile(); 
        }

    }
}
