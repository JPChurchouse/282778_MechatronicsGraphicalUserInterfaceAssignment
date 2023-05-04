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
        private SerialPort SERIALPORT = new SerialPort();

        // Initalise serial ports
        private void InitSerial()
        {
            SERIALPORT.RtsEnable = true;
            SERIALPORT.DtrEnable = true;
            SERIALPORT.ReadTimeout = 500;
            SERIALPORT.WriteTimeout = 500;
            SERIALPORT.NewLine = "\n";
            SERIALPORT.BaudRate = 115200;
            SERIALPORT.PortName = "COM0";
            SERIALPORT.DataReceived += SERIALPORT_DataReceived;
            SERIALPORT.ErrorReceived += SERIALPORT_ErrorReceived;
        }

        private void SERIALPORT_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            UpdateUiConnectionStatus();
        }

        private void SERIALPORT_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (SERIALPORT.BytesToRead == 0) return;
            
            string data = SERIALPORT.ReadLine();
            data = data.Replace("\r", "");
            data = data.Replace("\n", "");
            Log.Information($"Received: \"{data}\"");

            LogMessage(data, MsgType.RXD);
        }

        // Scan for ports and connect to them
        private void ScanAndConnect()
        {
            this.Dispatcher.Invoke(() =>
            {
                bool port_connected = SERIALPORT.IsOpen;

                // If connected, return
                if (port_connected) return;

                // Update labels
                LogMessage("Setting up Serial Port", MsgType.SYS);

                // Update UI availibility for this function
                btn_Connect.IsEnabled = false;
                this.Cursor = Cursors.Wait;


                // Connect to ports
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_PnPEntity WHERE Caption LIKE '% (COM%'");
                    ManagementObjectCollection devices = searcher.Get();

                    if (devices.Count == 0) throw new Exception("No devices found");

                    foreach (var dev in devices)
                    {
                        string caption = dev.GetPropertyValue("Caption").ToString();
                        Log.Debug($"Found device: {caption}");

                        // If not connected and port is an Uno or Due
                        if (caption.Contains("Arduino Uno") || caption.Contains("Arduino Due") || caption.Contains("CH340"))
                        {
                            SERIALPORT.PortName = ParsePortInfo(caption);

                            LogMessage("Attempting to open Serial Port", MsgType.SYS);
                            try { SERIALPORT.Open(); }
                            catch { }

                            port_connected = SERIALPORT.IsOpen;
                            LogMessage($"Connection {(port_connected ? "OPEN" : "FAILED")}", MsgType.SYS);
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

                    LogMessage("WARNING - SCARA ACTIVE", MsgType.ALT);
                    MessageBox.Show(
                        "The SCARA is about to become active. Press \"OK\" to proceed when the area is safe.",
                        "⚠️ WARNING ⚠️");

                    player.close();

                    // Home all axies
                    SendData("ECHO,1");
                    SendData("HOME");

                    //this.AcceptButton = button_AutoManual;
                }

                UpdateUiConnectionStatus();
                this.Cursor = null;
            });
        }

        // Disconnect all ports
        private void Disconnect()
        {
            if (SERIALPORT.IsOpen)
            {
                LogMessage("Closing Serial Port", MsgType.SYS);
                SendData("STOP");
                SERIALPORT.Close();
            }
            else
            {
                LogMessage("Serial Port already closed", MsgType.SYS);
            }
            UpdateUiConnectionStatus();
        }

        // Process sending data
        private void SendData(string data)
        {
            if (!SERIALPORT.IsOpen)
            {
                LogMessage("Not connected", MsgType.ALT);
                UpdateUiConnectionStatus();
                return;
            }
            if (data == null || data == "")
            {
                LogMessage("Not content to send", MsgType.SYS);
                UpdateUiConnectionStatus();
                return;
            }
            
            LogMessage($"Sending: {data}", MsgType.TXD);
            try
            {
                SERIALPORT.WriteLine(data);
            }
            catch (Exception ex)
            {
                LogMessage("Unable to send command", MsgType.ALT);
                Log.Error(ex.ToString());
            }
        }

        private string ParsePortInfo(string info)
        {
            return info.Substring(info.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);
        }
    }
}
