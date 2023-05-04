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
            try
            {
                Validate(txt_MoveW.Text, Settings.Default.min_W, Settings.Default.max_W, "W");
                Validate(txt_MoveX.Text, Settings.Default.min_X, Settings.Default.max_X, "X");
                Validate(txt_MoveY.Text, Settings.Default.min_Y, Settings.Default.max_Y, "Y");
            }
            catch (Exception exc)
            {
                LogMessage($"Invalid MOVE command: {exc.Message}",MsgType.ALT);
                return;
            }
            SendData($"MOVE,{txt_MoveX.Text},{txt_MoveY.Text},{txt_MoveW.Text}");
        }
        private void Validate(string s, int min, int max, string ID)
        {
            if (Int32.TryParse(s, out int i))
            {
                if (i > max || i < min) throw new Exception(ID);
            }
            else throw new Exception(ID);
        }

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
