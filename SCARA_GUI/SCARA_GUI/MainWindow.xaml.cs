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

namespace SCARA_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string LogFile;
        public MainWindow()
        {
            InitializeComponent();
            InitLog();
        }

        // Initalise logging
        private void InitLog()
        {
            string timenow = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            LogFile = $"logs\\{timenow}_SCARA_GUI.log";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File(LogFile)
                .CreateLogger();
            Log.Information("This programme was developed by J. P. Churchouse");
            Log.Information("Started programme at time: " + timenow);
        }

        // Handler when the Form closes
        /*
        private void MainPage_Closing(object WriteLineer, FormClosingEventArgs e)
        {
            closing_form = true;
            Log.Debug("Closing form");
            DisconnectAll();
            Log.CloseAndFlush();
        }*/

        // Add a line of text to the outputs log
        public enum MsgType { UNK, ALT, SYS, RXD, TXD, RET}
        public void LogMessage(string msg, MsgType type = MsgType.UNK)
        {
            string info = $"<{type}> {msg}";
            Log.Information(info);

            int go = 0;

                 if (type == MsgType.ALT && menu_Outputs_Alert.IsChecked) go++;
            else if (type == MsgType.SYS && menu_Outputs_System.IsChecked) go++;
            else if (type == MsgType.RXD && menu_Outputs_Receive.IsChecked) go++;
            else if (type == MsgType.TXD && menu_Outputs_Transmit.IsChecked) go++;
            else if (type == MsgType.RET) go++;
                 
            if (go > 0) text_OuputLog.Text = info + "\r\n" + text_OuputLog.Text;
        }

        public void WindowResized(object sender, EventArgs e)
        {
            Log.Debug($"rezize height: {this.Height} and width: {this.Width}");
            int s = (int)this.Width * (int)this.Height / 50000;
            Log.Debug($"Size: {s}");
            s = 30;

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

            text_OuputLog.FontSize = s;

            btn_EmergencyStop.FontSize = s;

        }

        private void LogBox_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            string dire = Environment.CurrentDirectory;
            try 
            { 
                Process.Start("explorer.exe", $"/select, {dire}\\{LogFile}");
            }
            catch {}
        }
    }
}
