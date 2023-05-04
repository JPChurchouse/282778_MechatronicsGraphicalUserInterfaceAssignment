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

            txt_PistonInactive.Text     = Settings.Default.air_UP;
            txt_PistonActive.Text       = Settings.Default.air_DOWN;
            txt_PistonDelay.Text        = Settings.Default.air_DELAY_P;

            txt_GripperInactive.Text    = Settings.Default.air_OPEN;
            txt_GripperActive.Text      = Settings.Default.air_CLOSE;
            txt_GripperDelay.Text       = Settings.Default.air_DELAY_G;
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.air_UP         = txt_PistonInactive.Text;
            Settings.Default.air_DOWN       = txt_PistonActive.Text;
            Settings.Default.air_DELAY_P    = txt_PistonDelay.Text;

            Settings.Default.air_OPEN       = txt_GripperInactive.Text;
            Settings.Default.air_CLOSE      = txt_GripperActive.Text;
            Settings.Default.air_DELAY_G    = txt_GripperDelay.Text;

            Settings.Default.Save();
        }
    }
}
