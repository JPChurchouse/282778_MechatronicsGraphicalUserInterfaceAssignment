using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Serilog;
using System.IO.Ports;
using System.IO;
using System.Diagnostics;
using SCARA_GUI.Properties;

namespace SCARA_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Init the UI
        public MainWindow()
        {
            InitializeComponent();
            
            lbl_DeviceStatus.Content = "Initalising...";
            InitLog();
            InitSerial();

            menu_Outputs_Alert.IsChecked        = Settings.Default.out_Alrt;
            menu_Outputs_Receive.IsChecked      = Settings.Default.out_Rx;
            menu_Outputs_Transmit.IsChecked     = Settings.Default.out_Tx;
            menu_Outputs_System.IsChecked       = Settings.Default.out_Sys;

            this.Width = Settings.Default.window_Width;
            this.Height = Settings.Default.window_Height;

            UpdateFontSize();
            UpdateUiConnectionStatus();

            Log.Information("Ready");
        }

        // Confirm closure, save settings
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (SERIALPORT.IsOpen)
                {
                    if (MessageBox.Show("The device is still connected, are you sure you want to close the programme?",
                        "Close programme?", MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                lbl_DeviceStatus.Content = "Closing Down...";

                Disconnect();

                Settings.Default.out_Alrt = menu_Outputs_Alert.IsChecked;
                Settings.Default.out_Rx = menu_Outputs_Receive.IsChecked;
                Settings.Default.out_Tx = menu_Outputs_Transmit.IsChecked;
                Settings.Default.out_Sys = menu_Outputs_System.IsChecked;

                Settings.Default.window_Width = (int)this.Width;
                Settings.Default.window_Height = (int)this.Height;

                Settings.Default.Save();
                Log.Information("Settings saved");

                Log.CloseAndFlush();
            });
        }

        // Update the status panel's elements to match current connection status
        private void UpdateUiConnectionStatus()
        {
            this.Dispatcher.Invoke(() =>
            {
                bool open = SERIALPORT.IsOpen;
                panel_Inputs.IsEnabled = open;
                btn_Connect.Content = open ? "DISCONNECT" : "CONNECT";
                btn_Connect.IsEnabled = true;
                this.Cursor = null;
                lbl_ConnectionStatus.Content = open ? $"Connected on {SERIALPORT.PortName}" : "Disconnected";
                lbl_DeviceStatus.Content = open ? "Ready": "Offline";
                menu_Outputs.IsEnabled = !open;
            });
        }
        
        // Update the font size for every element in the UI
        public void UpdateFontSize()
        {
            this.Dispatcher.Invoke(() =>
            {
                // Calculate the new font size
                int s = ( (int)this.Width/8 + (int)this.Height*4 ) / 100;
                Log.Debug($"Resize H: {this.Height} W: {this.Width} S: {s}");

                btn_Connect.FontSize = s;
                lbl_ConnectionStatus.FontSize = s;
                lbl_DeviceStatus.FontSize = s;
                lbl_MoveW.FontSize = s;
                lbl_MoveX.FontSize = s;
                lbl_MoveY.FontSize = s;
                txt_MoveW.FontSize = s;
                txt_MoveX.FontSize = s;
                txt_MoveY.FontSize = s;
                btn_MoveExecute.FontSize = s;
                lbl_Piston.FontSize = s;
                lbl_Gripper.FontSize = s;
                btn_Piston.FontSize = s;
                btn_Gripper.FontSize = s;
                btn_Home.FontSize = s;

                int count_extras = 0;
                if (Settings.Default.vis_ID)
                {
                    count_extras++;
                    lbl_ID.FontSize = s;
                    btn_ID.FontSize = s;
                    grid_ID.Visibility = Visibility.Visible;
                }
                else grid_ID.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_Wait)
                {
                    count_extras++;
                    btn_Wait.FontSize = s;
                    txt_Wait.FontSize = s;
                    lbl_Wait.FontSize = s;
                    grid_Wait.Visibility = Visibility.Visible;
                }
                else grid_Wait.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_SOf)
                {
                    count_extras++;
                    btn_SOffset.FontSize = s;
                    txt_SOffset.FontSize = s;
                    lbl_SOffset.FontSize = s;
                    grid_SOffset.Visibility = Visibility.Visible;
                }
                else grid_SOffset.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_ROf)
                {
                    count_extras++;
                    btn_ROffset.FontSize = s;
                    lbl_ROffset.FontSize = s;
                    grid_ROffset.Visibility = Visibility.Visible;
                }
                else grid_ROffset.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_Prox)
                {
                    count_extras++;
                    btn_Prox.FontSize = s;
                    lbl_Prox.FontSize = s;
                    grid_Prox.Visibility = Visibility.Visible;
                }
                else grid_Prox.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_Spdst)
                {
                    count_extras++;
                    btn_Speedset.FontSize = s;
                    lbl_Speedset.FontSize = s;
                    txt_Speedset.FontSize = s;
                    grid_Speedset.Visibility = Visibility.Visible;
                }
                else grid_Speedset.Visibility = Visibility.Collapsed;

                text_OuputLog.FontSize = s * 2 / 3;

                btn_EmergencyStop.FontSize = s * 3;

                img_Placeholder.Visibility = count_extras == 0 ? Visibility.Visible : Visibility.Collapsed;
                bor_Extras.Visibility = count_extras != 0 ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        // MOVE validation function
        private bool Validate(string s, int min, int max)
        {
            if (Int32.TryParse(s, out int i))
            {
                if (i <= max && i >= min) return true;
            }
            return false;
        }

        // Generate MOVE cmd
        private string ParseUiToMoveCmd()
        {
            string issues = "";
            if (!Validate(txt_MoveW.Text, Settings.Default.min_W, Settings.Default.max_W)) issues += "W";
            if (!Validate(txt_MoveX.Text, Settings.Default.min_X, Settings.Default.max_X)) issues += "X";
            if (!Validate(txt_MoveY.Text, Settings.Default.min_Y, Settings.Default.max_Y)) issues += "Y";
        
            // All valid
            if (issues == "") return $"MOVE,{txt_MoveX.Text},{txt_MoveY.Text},{txt_MoveW.Text}";
            
            // Invalid
            throw new Exception($"MOVE invalid: {issues}");
        }
    }
}
