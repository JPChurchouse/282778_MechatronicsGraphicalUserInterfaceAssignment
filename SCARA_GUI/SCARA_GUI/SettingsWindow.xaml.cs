using SCARA_GUI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCARA_GUI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            SettingsToUi();
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateAndSave();
        }
        private void Confirm_Click(object sender, EventArgs e)
        {
            ValidateAndSave();
        }

        private void ValidateAndSave()
        {
            // Piston
            //txt_PistonInactive.Text = Settings.Default.air_UP;
            //txt_PistonActive.Text = Settings.Default.air_DOWN;
            
            if (!Validate(txt_PistonDelay.Text,0,10000))
                txt_PistonDelay.Text = Settings.Default.air_DELAY_P;

            // Gripper
            //txt_GripperInactive.Text = Settings.Default.air_OPEN;
            //txt_GripperActive.Text = Settings.Default.air_CLOSE;

            if (!Validate(txt_GripperDelay.Text, 0, 10000))
                txt_GripperDelay.Text = Settings.Default.air_DELAY_G;

            // Maxes
            if (!Validate(txt_MaxW.Text, 0, 200))
                txt_MaxW.Text = Settings.Default.max_W;

            if (!Validate(txt_MaxX.Text, 0, 200))
                txt_MaxX.Text = Settings.Default.max_X;

            if (!Validate(txt_MaxY.Text, 0, 200))
                txt_MaxY.Text = Settings.Default.max_Y;

            // Mins
            if (!Validate(txt_MinW.Text, -200, 0))
                txt_MinW.Text = Settings.Default.min_W;

            if (!Validate(txt_MinX.Text, -200, 0))
                txt_MinX.Text = Settings.Default.min_X;

            if (!Validate(txt_MinY.Text, -200, 0))
                txt_MinY.Text = Settings.Default.min_Y;


            UiToSettings();
        }

        private void SettingsToUi()
        {
            txt_PistonInactive.Text = Settings.Default.air_UP;
            txt_PistonActive.Text = Settings.Default.air_DOWN;
            txt_PistonDelay.Text = Settings.Default.air_DELAY_P;

            txt_GripperInactive.Text = Settings.Default.air_OPEN;
            txt_GripperActive.Text = Settings.Default.air_CLOSE;
            txt_GripperDelay.Text = Settings.Default.air_DELAY_G;

            txt_MaxW.Text = Settings.Default.max_W;
            txt_MaxX.Text = Settings.Default.max_X;
            txt_MaxY.Text = Settings.Default.max_Y;

            txt_MinW.Text = Settings.Default.min_W;
            txt_MinX.Text = Settings.Default.min_X;
            txt_MinY.Text = Settings.Default.min_Y;
        }

        private void UiToSettings() 
        {
            Settings.Default.air_UP = txt_PistonInactive.Text;
            Settings.Default.air_DOWN = txt_PistonActive.Text;
            Settings.Default.air_DELAY_P = txt_PistonDelay.Text;

            Settings.Default.air_OPEN = txt_GripperInactive.Text;
            Settings.Default.air_CLOSE = txt_GripperActive.Text;
            Settings.Default.air_DELAY_G = txt_GripperDelay.Text;

            Settings.Default.max_W = txt_MaxW.Text;
            Settings.Default.max_X = txt_MaxX.Text;
            Settings.Default.max_Y = txt_MaxY.Text;

            Settings.Default.min_W = txt_MinW.Text;
            Settings.Default.min_X = txt_MinX.Text;
            Settings.Default.min_Y = txt_MinY.Text;

            Settings.Default.Save();
        }
    
        private bool Validate(string s, int min, int max)
        {
            try
            {
                // Test for null
                if (s == null) throw new ArgumentNullException ();

                // Test for NaN
                if (Int32.TryParse(s, out int i))
                {
                    // Test for OutOfBounds
                    if (i > max || i < min) throw new ArgumentOutOfRangeException();
                    
                    // Validated
                    return true;
                }
                else throw new FormatException();
            }
            catch
            {
                return false;
            }
        }
    }
}
