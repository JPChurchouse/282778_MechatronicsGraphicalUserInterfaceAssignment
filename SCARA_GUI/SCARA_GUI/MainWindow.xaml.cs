using SCARA_GUI.Properties;
using Serilog;
using System.Windows;

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

            Ui_UpdateFontSize();
            Ui_UpdateConnectionStatus();
            Ui_UpdateMoveParams();

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
        private void Ui_UpdateConnectionStatus()
        {
            this.Dispatcher.Invoke(() =>
            {
                bool open = SERIALPORT.IsOpen;

                // Unexpected disconnect flag, device thinks its online, but connection is closed
                bool unex_disc = lbl_DeviceStatus.Content.ToString().Contains("Available") && !open;

                btn_Connect.Content = open ? "DISCONNECT" : "CONNECT";
                lbl_ConnectionStatus.Content = open ? $"Connected on {SERIALPORT.PortName}" : "Disconnected";
                lbl_DeviceStatus.Content = open ? "System Online": "Offline";
                menu_Outputs.IsEnabled = !open;
                menu_Presets_Save.IsEnabled = open;

                Ui_SetControlsEnabled(open);

                if (unex_disc) LogMessage("Unexpected disconnection",MsgType.ALT);
            });
        }
        
        // Set the enabled state of the controls
        private void Ui_SetControlsEnabled(bool enabled = false)
        {
            this.Dispatcher.Invoke(() =>
            {
                Log.Debug($"Set enabled: {enabled}");
                panel_Inputs.IsEnabled = enabled;
                btn_Connect.IsEnabled = true;

                this.Cursor = null;
            });
        }
        
        // Update the font size for every element in the UI
        public void Ui_UpdateFontSize()
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
                btn_Preset.FontSize = s;
                btn_MoveWJogDown.FontSize = s;
                btn_MoveWJogUp.FontSize = s;
                btn_MoveXJogDown.FontSize = s;
                btn_MoveXJogUp.FontSize = s;
                btn_MoveYJogDown.FontSize = s;
                btn_MoveYJogUp.FontSize = s;

                int count_extras = 0;
                if (Settings.Default.vis_ID)
                {
                    count_extras++;
                    btn_ID.FontSize = s;
                    btn_ID.Visibility = Visibility.Visible;
                }
                else btn_ID.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_Wait)
                {
                    count_extras++;
                    btn_Wait.FontSize = s;
                    txt_Wait.FontSize = s;
                    grid_Wait.Visibility = Visibility.Visible;
                }
                else grid_Wait.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_SOf)
                {
                    count_extras++;
                    btn_SOffset.FontSize = s;
                    txt_SOffset.FontSize = s;
                    grid_SOffset.Visibility = Visibility.Visible;
                }
                else grid_SOffset.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_ROf)
                {
                    count_extras++;
                    btn_ROffset.FontSize = s;
                    btn_ROffset.Visibility = Visibility.Visible;
                }
                else btn_ROffset.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_Prox)
                {
                    count_extras++;
                    btn_Prox.FontSize = s;
                    btn_Prox.Visibility = Visibility.Visible;
                }
                else btn_Prox.Visibility = Visibility.Collapsed;

                if (Settings.Default.vis_Spdst)
                {
                    count_extras++;
                    btn_SpeedSet.FontSize = s;
                    txt_SpeedSet.FontSize = s;
                    btn_AccelSet.FontSize = s;
                    txt_AccelSet.FontSize = s;
                    grid_Speedset.Visibility = Visibility.Visible;
                }
                else grid_Speedset.Visibility = Visibility.Collapsed;

                text_OuputLog.FontSize = s * 2 / 3;

                btn_EmergencyStop.FontSize = s * 3;

                img_Placeholder.Visibility = count_extras == 0 ? Visibility.Visible : Visibility.Collapsed;
                bor_Extras.Visibility = count_extras != 0 ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        public void Ui_UpdateMoveParams()
        {
            int W = pose.Read(Pose.Axis.W);
            txt_MoveW.Text = W.ToString();
            sld_MoveW.Maximum = Settings.Default.max_W;
            sld_MoveW.Minimum = Settings.Default.min_W;
            sld_MoveW.Value = W;

            int X = pose.Read(Pose.Axis.X);
            txt_MoveX.Text = X.ToString();
            sld_MoveX.Maximum = Settings.Default.max_X;
            sld_MoveX.Minimum = Settings.Default.min_X;
            sld_MoveX.Value = X;

            int Y = pose.Read(Pose.Axis.Y);
            txt_MoveY.Text = Y.ToString();
            sld_MoveY.Maximum = Settings.Default.max_Y;
            sld_MoveY.Minimum = Settings.Default.min_Y;
            sld_MoveY.Value = Y;
        }

        
    }
}
