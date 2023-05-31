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
using System.Windows.Shapes;

namespace SCARA_GUI
{
    /// <summary>
    /// Interaction logic for PresetsWindow.xaml
    /// </summary>
    public partial class PresetsWindow : Window
    {
        public string result;
        public PresetsWindow()
        {
            InitializeComponent();
            PopulateListViewer();
        }

        List <string> presets = new List<string>();
        private void PopulateListViewer()
        {
            presets.Clear();
            foreach (UserPresets.Position pos in UserPresets.list_Positions)
            {
                presets.Add(pos.name());
            }
            lst_PresetViewer.ItemsSource = presets;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Execute_Click(object sender, RoutedEventArgs e)
        {
            Execute();
        }
        private void Execute()
        {
            int sel = lst_PresetViewer.SelectedIndex;
            result = UserPresets.list_Positions[sel].cmd();
            this.Close();
        }

        private void lst_Viewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Execute();
        }
    }
}
