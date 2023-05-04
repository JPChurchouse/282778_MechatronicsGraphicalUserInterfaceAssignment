using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SCARA_GUI
{
    public partial class MainWindow : Window
    {
        private string LogFile;
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

        // Add a line of text to the outputs log
        public enum MsgType { UNK, ALT, SYS, RXD, TXD, RET }
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
        public void OpenLogFile()
        {
            string dire = Environment.CurrentDirectory;
            try { Process.Start("explorer.exe", $"/select, {dire}\\{LogFile}"); }
            catch { }
        }
    }
}
