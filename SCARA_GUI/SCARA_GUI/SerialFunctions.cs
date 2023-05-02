using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Management;

namespace SCARA_GUI
{
    public partial class MainWindow : Window
    {
        private readonly SerialPort serialport = new SerialPort();

        // Initalise serial ports
        private void InitPort(SerialPort port)
        {
            port.RtsEnable = true;
            port.DtrEnable = true;
            port.ReadTimeout = 500;
            port.WriteTimeout = 500;
            port.NewLine = "\n";
            port.BaudRate = 115200;
            port.PortName = "COM0";
        }

        // Scan for ports and connect to them
        private void ScanAndConnect(SerialPort port)
        {
            bool port_connected = port.IsOpen;

            // If they're both connected, return
            if (port_connected) return;

            // Update labels
            UpdateUser($"Setting up Serial Ports...");

            // Update UI availibility for this function
            //button_Scan.Enabled = false;
            //this.Cursor = !control_connected ? Cursors.WaitCursor : Cursors.Default;


            // Connect to ports
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_PnPEntity WHERE Caption LIKE '% (COM%'");
                ManagementObjectCollection devices = searcher.Get();

                if (devices.Count == 0) return;

                foreach (var dev in devices)
                {
                    string caption = dev.GetPropertyValue("Caption").ToString();
                    Log.Debug($"Found device: {caption}");

                    // Work on controller micro
                    // If not connected and port is an Uno or Due
                    if (caption.Contains("Arduino Uno") || caption.Contains("Arduino Due")
                        || caption.Contains("CH340"))
                    {
                        port.PortName = ParsePortInfo(caption);
                        UpdateUser("Attempting to open Serial Port");
                        try
                        {
                            port.Open();
                            UpdateUser("Controller openned");
                        }
                        catch
                        {
                            UpdateUser("Controller FAILED to open");
                        }
                        port_connected = port.IsOpen;
                    }
                }
            }
            catch (Exception ex) { Log.Error(ex.Message); }

            if (port_connected)
            {
                // Show warning
                WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
                player.URL = "alarm.mp3";
                player.controls.play();

                UpdateUser("CRITICAL WARNING - SCARA ACTIVE");
                MessageBox.Show(
                    "The SCARA is about to become active. Press OK when the area is safe.",
                    "⚠️ CRITICAL WARNING ⚠️");

                player.close();

                // Home all axies
                SendData(port, "ECHO,1");
                //MoveToReady();

                //this.AcceptButton = button_AutoManual;
            }

            //this.Cursor = Cursors.Default;
        }

        // Disconnect all ports
        private void Disconnect(SerialPort port)
        {
            if (port.IsOpen)
            {
                UpdateUser($"Closing Serial Port");
                SendData(port, "STOP");
                port.Close();
            }
        }
        // Process sending data
        private void SendData(SerialPort port, string data)
        {
            if (!port.IsOpen) return;
            if (data == null || data == "") return;
            //UpdateBox($"TX_{PortString(port)}: {data}");
            port.WriteLine(data);
        }

        private string ParsePortInfo(string info)
        {
            return info.Substring(info.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);
        }
    }
}
