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

            menu_Visibility_ID.IsChecked        = Settings.Default.vis_ID;
            menu_Visibility_Prox.IsChecked      = Settings.Default.vis_Prox;
            menu_Visibility_ROffset.IsChecked   = Settings.Default.vis_ROf;
            menu_Visibility_SOffset.IsChecked   = Settings.Default.vis_SOf;
            menu_Visibility_Speedset.IsChecked  = Settings.Default.vis_Spdst;
            menu_Visibility_Wait.IsChecked      = Settings.Default.vis_Wait;

            this.Width = Settings.Default.window_Width;
            this.Height = Settings.Default.window_Height;

            UpdateFontSize();
            UpdateUiConnectionStatus();

            Log.Information("Ready");
        }

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

                Settings.Default.vis_ID = menu_Visibility_ID.IsChecked;
                Settings.Default.vis_Prox = menu_Visibility_Prox.IsChecked;
                Settings.Default.vis_ROf = menu_Visibility_ROffset.IsChecked;
                Settings.Default.vis_SOf = menu_Visibility_SOffset.IsChecked;
                Settings.Default.vis_Spdst = menu_Visibility_Speedset.IsChecked;
                Settings.Default.vis_Wait = menu_Visibility_Wait.IsChecked;

                Settings.Default.window_Width = (int)this.Width;
                Settings.Default.window_Height = (int)this.Height;

                Settings.Default.Save();
                Log.Information("Settings saved");

                Log.CloseAndFlush();
            });
        }

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
        private void UpdateFontSize()
        {
            this.Dispatcher.Invoke(() =>
            {
                Log.Debug($"rezize height: {this.Height} and width: {this.Width}");
                int s = ( (int)this.Width/8 + (int)this.Height*4 ) / 100;
                Log.Debug($"Size: {s}");

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
                if (menu_Visibility_ID.IsChecked)
                {
                    count_extras++;
                    lbl_ID.FontSize = s;
                    btn_ID.FontSize = s;
                    grid_ID.Visibility = Visibility.Visible;
                }
                else grid_ID.Visibility = Visibility.Collapsed;

                if (menu_Visibility_Wait.IsChecked)
                {
                    count_extras++;
                    btn_Wait.FontSize = s;
                    txt_Wait.FontSize = s;
                    lbl_Wait.FontSize = s;
                    grid_Wait.Visibility = Visibility.Visible;
                }
                else grid_Wait.Visibility = Visibility.Collapsed;

                if (menu_Visibility_SOffset.IsChecked)
                {
                    count_extras++;
                    btn_SOffset.FontSize = s;
                    txt_SOffset.FontSize = s;
                    lbl_SOffset.FontSize = s;
                    grid_SOffset.Visibility = Visibility.Visible;
                }
                else grid_SOffset.Visibility = Visibility.Collapsed;

                if (menu_Visibility_ROffset.IsChecked)
                {
                    count_extras++;
                    btn_ROffset.FontSize = s;
                    lbl_ROffset.FontSize = s;
                    grid_ROffset.Visibility = Visibility.Visible;
                }
                else grid_ROffset.Visibility = Visibility.Collapsed;

                if (menu_Visibility_Prox.IsChecked)
                {
                    count_extras++;
                    btn_Prox.FontSize = s;
                    lbl_Prox.FontSize = s;
                    grid_Prox.Visibility = Visibility.Visible;
                }
                else grid_Prox.Visibility = Visibility.Collapsed;

                if (menu_Visibility_Speedset.IsChecked)
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
    }
}
