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

namespace SCARA_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitLog();
        }

        // Initalise logging
        private void InitLog()
        {
            string timenow = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File($"logs/{timenow}.log")
                .CreateLogger();
            Log.Information("Started at time: " + timenow);
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
        public void UpdateUser(string msg)
        {
            /*
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate () { UpdateBox(msg); }));
                return;
            }*/
            Log.Information(msg);
            //textBox_Updates.Text = msg + "\r\n" + textBox_Updates.Text;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
