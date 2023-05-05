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
            int changes = 0;
            // Piston
            //txt_PistonInactive.Text = Settings.Default.air_UP;
            //txt_PistonActive.Text = Settings.Default.air_DOWN;
            
            if (!Validate(txt_PistonDelay.Text, 0, 10000))
            {
                txt_PistonDelay.Text = Settings.Default.air_DELAY_P.ToString();
                changes++;
            }

            // Gripper
            //txt_GripperInactive.Text = Settings.Default.air_OPEN;
            //txt_GripperActive.Text = Settings.Default.air_CLOSE;

            if (!Validate(txt_GripperDelay.Text, 0, 10000))
            {
                txt_GripperDelay.Text = Settings.Default.air_DELAY_G.ToString();
                changes++;
            }

            // Maxes
            if (!Validate(txt_MaxW.Text, 0, 200))
            {
                txt_MaxW.Text = Settings.Default.max_W.ToString();
                changes++;
            }

            if (!Validate(txt_MaxX.Text, 0, 200))
            {
                txt_MaxX.Text = Settings.Default.max_X.ToString();
                changes++;
            }

            if (!Validate(txt_MaxY.Text, 0, 200))
            {
                txt_MaxY.Text = Settings.Default.max_Y.ToString();
                changes++;
            }

            // Mins
            if (!Validate(txt_MinW.Text, -200, 0))
            {
                txt_MinW.Text = Settings.Default.min_W.ToString();
                changes++;
            }

            if (!Validate(txt_MinX.Text, -200, 0))
            {
                txt_MinX.Text = Settings.Default.min_X.ToString();
                changes++;
            }

            if (!Validate(txt_MinY.Text, -200, 0))
            {
                txt_MinY.Text = Settings.Default.min_Y.ToString();
                changes++;
            }


            if (MessageBox.Show(
                (changes > 0 ? $"{changes} settings were outside of parameters and reverted.\n":"") +
                "Are you sure you want to save these settings?\n" +
                "The manufacturer takes no responsibility for any dammages as a result of changed settings.",
                "Save settings?",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel)
                != MessageBoxResult.Yes)
            {
                return;
            }
            UiToSettings();
        }

        private void SettingsToUi()
        {
            // Piston
            txt_PistonInactive.Text = Settings.Default.air_UP;
            txt_PistonActive.Text = Settings.Default.air_DOWN;
            txt_PistonDelay.Text = Settings.Default.air_DELAY_P.ToString();

            // Gripper
            txt_GripperInactive.Text = Settings.Default.air_OPEN;
            txt_GripperActive.Text = Settings.Default.air_CLOSE;
            txt_GripperDelay.Text = Settings.Default.air_DELAY_G.ToString();

            // Max
            txt_MaxW.Text = Settings.Default.max_W.ToString();
            txt_MaxX.Text = Settings.Default.max_X.ToString();
            txt_MaxY.Text = Settings.Default.max_Y.ToString();

            // Min
            txt_MinW.Text = Settings.Default.min_W.ToString();
            txt_MinX.Text = Settings.Default.min_X.ToString();
            txt_MinY.Text = Settings.Default.min_Y.ToString();

            // Visibililty
            txt_ID.IsChecked    = Settings.Default.vis_ID;
            txt_Prox.IsChecked  = Settings.Default.vis_Prox;
            txt_ROf.IsChecked   = Settings.Default.vis_ROf;
            txt_SOf.IsChecked   = Settings.Default.vis_SOf;
            txt_Spd.IsChecked   = Settings.Default.vis_Spdst;
            txt_Wait.IsChecked  = Settings.Default.vis_Wait;
        }

        private void UiToSettings() 
        {
            try
            {
                // Piston
                Settings.Default.air_UP = txt_PistonInactive.Text;
                Settings.Default.air_DOWN = txt_PistonActive.Text;
                Settings.Default.air_DELAY_P = StoI(txt_PistonDelay.Text);

                // Gripper
                Settings.Default.air_OPEN = txt_GripperInactive.Text;
                Settings.Default.air_CLOSE = txt_GripperActive.Text;
                Settings.Default.air_DELAY_G = StoI(txt_GripperDelay.Text);

                // Max
                Settings.Default.max_W = StoI(txt_MaxW.Text);
                Settings.Default.max_X = StoI(txt_MaxX.Text);
                Settings.Default.max_Y = StoI(txt_MaxY.Text);

                // Min
                Settings.Default.min_W = StoI(txt_MinW.Text);
                Settings.Default.min_X = StoI(txt_MinX.Text);
                Settings.Default.min_Y = StoI(txt_MinY.Text);

                // Visibility
                Settings.Default.vis_ID     = (bool)txt_ID.IsChecked;
                Settings.Default.vis_Prox   = (bool)txt_Prox.IsChecked;
                Settings.Default.vis_ROf    = (bool)txt_ROf.IsChecked;
                Settings.Default.vis_SOf    = (bool)txt_SOf.IsChecked;
                Settings.Default.vis_Spdst  = (bool)txt_Spd.IsChecked;
                Settings.Default.vis_Wait   = (bool)txt_Wait.IsChecked;
            }
            catch 
            {
                MessageBox.Show("Error", "Unable to save one or more settings!");
            }

            Settings.Default.Save();
        }

        private int StoI(string s)
        {
            if (Int32.TryParse(s, out int i)) return i;
            else throw new Exception("Couldn't convert");
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
