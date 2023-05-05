using SCARA_GUI.Properties;
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
        private void WindowResized(object sender, EventArgs e)  { UpdateFontSize(); }

        private void menu_OpenFile_Clicked(object sender, EventArgs args) { OpenLogFile(); }

        private void menu_Vis_Clicked(object sender, EventArgs e) { UpdateFontSize(); }

        private void menu_Out_Clicked(object sender, EventArgs e) { text_OuputLog.Text = string.Empty; }

        // Show the advaced settings window
        private void menu_Advanced_Clicked(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
            UpdateFontSize();
        }

        private void menu_Help_Clicked(object sender, RoutedEventArgs e)
        {
            try 
            { 
                for (int i = 0; i < 10; i++) Process.Start("explorer", "https://youtu.be/oHg5SJYRHA0"); 
            }
            catch { }
        }


        // If not connected, connect, otherwise disconnect
        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!SERIALPORT.IsOpen) ScanAndConnect();
            else Disconnect();
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e) { SendData("STOP"); }
        
        // Validate the inputs and send MOVE command
        private void btn_Move_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendData(ParseUiToMoveCmd());
            }
            catch (Exception exc)
            {
                LogMessage(exc.Message, MsgType.ALT);
            }
        }

        // Piston cmd
        private void btn_Piston_Click(object sender, RoutedEventArgs e)
        {
            if (btn_Piston.Content.ToString().Contains("UP"))
            {
                btn_Piston.Content = "DOWN";
                SendData($"AIR,{Settings.Default.air_UP}");
            }
            else
            {
                btn_Piston.Content = "UP";
                SendData($"AIR,{Settings.Default.air_DOWN}");
            }
        }

        // Gripper cmd
        private void btn_Gripper_Click(object sender, RoutedEventArgs e)
        {
            if (btn_Gripper.Content.ToString().Contains("OPEN"))
            {
                btn_Gripper.Content = "CLOSE";
                SendData($"AIR,{Settings.Default.air_OPEN}");
            }
            else
            {
                btn_Gripper.Content = "OPEN";
                SendData($"AIR,{Settings.Default.air_CLOSE}");
            }
        }
        
        private void btn_Home_Click(object sender, RoutedEventArgs e) { SendData("HOME"); }
        
        private void LogBox_DoubleClicked(object sender, MouseButtonEventArgs e) { OpenLogFile(); }
    }
}
