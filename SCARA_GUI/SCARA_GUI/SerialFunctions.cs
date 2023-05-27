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
using SCARA_GUI.Properties;

namespace SCARA_GUI
{
    public partial class MainWindow : Window
    {
        // The global SerialPort object
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

        // Serial Port error handler
        private void SERIALPORT_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            LogMessage("Serial Port Error", MsgType.ALT);
            Ui_UpdateConnectionStatus();
        }

        // Serial Port RX handler
        private void SERIALPORT_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (SERIALPORT.BytesToRead == 0) return;
            
                string data = SERIALPORT.ReadLine();
                data = data.Replace("\r", "");
                data = data.Replace("\n", "");
                Log.Information($"Received: \"{data}\"");

                if (data.Contains("RECEIVED")) 
                {
                    LockoutEnd();
                    return;
                }// Don't show the user the ECHO rx cmds

                LogMessage(data, MsgType.RXD);
            }
            catch (Exception exc)
            {
                Log.Debug($"Failed to read serial: {exc.ToString()}");
            }
        }

        // Scan for the right device and connect to it
        private void ScanAndConnect()
        {
            this.Dispatcher.Invoke(() =>
            {
                bool port_connected = SERIALPORT.IsOpen;

                // If connected, return
                if (port_connected) return;

                // Update labels
                LogMessage("Setting up Serial Port", MsgType.SYS);
                lbl_DeviceStatus.Content = "Connecting...";
                btn_Connect.Content = "Connecting...";

                // Update UI availibility for this function
                btn_Connect.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                // Apply user settings
                SERIALPORT.ReadTimeout = Settings.Default.ser_Tim;
                SERIALPORT.WriteTimeout = Settings.Default.ser_Tim;
                SERIALPORT.BaudRate = Settings.Default.ser_Baud;

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
                        if (port_connected) break;
                    }
                }
                catch (Exception ex) { Log.Error(ex.Message); }

                if (port_connected)
                {
                    Log.Debug($"Baudrate: {SERIALPORT.BaudRate}");

                    
                    // Play alarm
                    WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
                    if (Settings.Default.alarm) 
                    {
                        player.URL = "alarm.mp3";
                        player.controls.play();
                    }

                    // Show warning
                    LogMessage("WARNING - SCARA ACTIVE", MsgType.ALT);
                    MessageBox.Show(
                        "The SCARA is about to become active. Press \"OK\" to proceed when the area is safe.",
                        "⚠️ WARNING ⚠️");

                    player.close();

                    // Home all axies
                    SendData("ECHO,1");
                    SendData("CLEAR");
                    SendData("HOME");
                }
                else
                {
                    LogMessage("Unable to connect to device", MsgType.SYS);
                }

                Ui_UpdateConnectionStatus();
                this.Cursor = null;
            });
        }

        // Disconnect Serial Port
        private async void Disconnect()
        {
            try
            {
                if (SERIALPORT.IsOpen)
                {
                    LogMessage("Closing Serial Port", MsgType.SYS);

                    SendData("STOP");
                    await Task.Delay(100);
                    SendData("CLEAR");
                    await Task.Delay(100);

                    SERIALPORT.Close();
                }
                else
                {
                    LogMessage("Serial Port already closed", MsgType.SYS);
                }
            }
            catch (Exception exc)
            {
                Log.Error(exc.Message);
            }
            finally
            {
                await Task.Delay(500);
                Ui_UpdateConnectionStatus();
            }
        }

        // Process sending data on the Serial Port
        private void SendData(string data)
        {
            if (!SERIALPORT.IsOpen)
            {
                LogMessage("Connection error", MsgType.ALT);
            }

            else if (data == null || data == "")
            {
                LogMessage("Not content to send", MsgType.ALT);
            }

            else 
            { 
                LogMessage($"Sending: {data}", MsgType.TXD);
                try
                {
                    SERIALPORT.WriteLine(data);
                    LockoutStart();
                }
                catch (Exception ex)
                {
                    LogMessage("Unable to send command", MsgType.ALT);
                    Log.Error(ex.ToString());
                }
            }

            Ui_UpdateConnectionStatus();
        }

        // Convert the COM Port message to a "COM x" string
        private string ParsePortInfo(string info) { return info.Substring(info.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty); }


        // LOCKOUT
        System.Timers.Timer timer_lockout = new System.Timers.Timer();
        private void LockoutStart()
        {
            long tim = Settings.Default.lockout;
            if (tim <= 0) return;

            Ui_SetControlsEnabled(false);
            timer_lockout.Interval = tim;
            timer_lockout.Elapsed += timer_lockout_elapsed;
            timer_lockout.Start();
        }
        private void LockoutEnd()
        {
            timer_lockout.Stop();
            Ui_SetControlsEnabled(true);
        }

        private void timer_lockout_elapsed(object sender, System.Timers.ElapsedEventArgs e) { LockoutEnd(); }
    }
}
