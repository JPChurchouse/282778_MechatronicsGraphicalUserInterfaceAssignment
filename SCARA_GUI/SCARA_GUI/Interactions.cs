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
        private void WindowResized(object sender, EventArgs e) { Ui_UpdateFontSize(); }

        private void menu_OpenFile_Clicked(object sender, EventArgs args) { OpenLogFile(); }

        private void menu_Vis_Clicked(object sender, EventArgs e) { Ui_UpdateFontSize(); }

        private void menu_Out_Clicked(object sender, EventArgs e) { text_OuputLog.Text = string.Empty; }

        // Show the advaced settings window
        private void menu_Advanced_Clicked(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
            Ui_UpdateFontSize();
        }

        private void menu_Help_Clicked(object sender, RoutedEventArgs e) { OpenHelpFile(); }

        // If not connected, connect, otherwise disconnect
        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!SERIALPORT.IsOpen) ScanAndConnect();
            else Disconnect();
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e) 
        { 
            Disconnect();
        }
        
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
                SendData($"AIR,{Settings.Default.air_UP},{Settings.Default.air_DELAY_P}");
            }
            else
            {
                btn_Piston.Content = "UP";
                SendData($"AIR,{Settings.Default.air_DOWN},{Settings.Default.air_DELAY_P}");
            }
        }

        // Gripper cmd
        private void btn_Gripper_Click(object sender, RoutedEventArgs e)
        {
            if (btn_Gripper.Content.ToString().Contains("OPEN"))
            {
                btn_Gripper.Content = "CLOSE";
                SendData($"AIR,{Settings.Default.air_OPEN},{Settings.Default.air_DELAY_G}");
            }
            else
            {
                btn_Gripper.Content = "OPEN";
                SendData($"AIR,{Settings.Default.air_CLOSE},{Settings.Default.air_DELAY_G}");
            }
        }
        
        private void btn_Home_Click(object sender, RoutedEventArgs e) { SendData("HOME"); }
        
        private void LogBox_DoubleClicked(object sender, MouseButtonEventArgs e) { OpenLogFile(); }

        private void btn_Wait_Click(object sender, RoutedEventArgs e)
        {
            if (Validate(txt_Wait.Text, 0, 999999999))
            {
                SendData($"WAIT,{txt_Wait.Text}");
            }
            else
            {
                LogMessage("WAIT invalid", MsgType.ALT);
            }
        }

        private void btn_ID_Click(object sender, RoutedEventArgs e) { SendData($"ID");}

        private void btn_SOffset_Click(object sender, RoutedEventArgs e)
        {
            if (Validate(txt_SOffset.Text, 0, 999999999))
            {
                SendData($"SOFFSET,{txt_SOffset.Text}");
            }
            else
            {
                LogMessage("SOFFSET invalid", MsgType.ALT);
            }
        }

        private void btn_ROffset_Click(object sender, RoutedEventArgs e) { SendData($"ROFFSET"); }

        private void btn_Prox_Click(object sender, RoutedEventArgs e) { SendData($"PROX"); }

        private void btn_Speedset_Click(object sender, RoutedEventArgs e)
        {
            if (Validate(txt_Speedset.Text, 0, 200))
            {
                SendData($"SPEEDSET,{txt_Speedset.Text},{txt_Speedset.Text}");
            }
            else
            {
                LogMessage("SPEEDSET invalid", MsgType.ALT);
            }
        }

        private void btn_JogAny_Click(object sender, RoutedEventArgs e)
        {
            Log.Debug($"dragging: {sld_MoveW.Value}");
        }


        private void sld_MoveW_Dragging(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Log.Debug($"dragging: {sld_MoveW.Value}");
        }
        private void sld_MoveX_Dragging(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Log.Debug($"dragging: {sld_MoveW.Value}");
        }
        private void sld_MoveY_Dragging(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Log.Debug($"dragging: {sld_MoveW.Value}");
        }

        private void sld_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Log.Debug($"drag completed: {sld_MoveW.Value}");
        }
    }
}
