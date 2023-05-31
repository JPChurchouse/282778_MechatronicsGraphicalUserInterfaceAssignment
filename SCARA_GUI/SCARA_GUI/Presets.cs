using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;


namespace SCARA_GUI
{
    internal class UserPresets
    {
        private static string file_Positions = Environment.CurrentDirectory + "\\Positions.json";
        private static string dire_Sequences = Environment.CurrentDirectory + "\\Sequences";

        public static List<Position> list_Positions = new List<Position>();

        public static void SavePositionsList()
        {
            string json = JsonConvert.SerializeObject(list_Positions);
            File.WriteAllText(file_Positions, json);
        }

        public static void ReadPositionsList()
        {
            string json = File.ReadAllText(file_Positions);
            list_Positions = JsonConvert.DeserializeObject<List<Position>>(json);
        }
        public static void OpenPresetsFile()
        {
            Console.WriteLine(file_Positions);
            try { Process.Start("explorer.exe", file_Positions); }
            catch { }
        }

        public struct Position
        {
            private string Name;
            private int W;
            private int X;
            private int Y;

            public Position(string name, int w, int x, int z)
            {
                Name = name;
                W = w;
                X = x;
                Y = z;
            }

            public string name() { return Name; }
            public string cmd() { return $"MOVE,{X},{Y},{W}"; }
        }
    }

    public partial class MainWindow : Window
    {
        private void LaunchPresets()
        {
            UserPresets.ReadPositionsList();
            PresetsWindow win = new PresetsWindow();
            if (win.ShowDialog().Value)
            {
                string res = win.result;
                if (res == null || res == "") return;
                SendData(res);
            }
        }
        private void NewPreset()
        {
            var p = new UserPresets.Position(
                "NewPreset",
                pose.Read(Pose.Axis.X),
                pose.Read(Pose.Axis.Y),
                pose.Read(Pose.Axis.W)
                );
            UserPresets.list_Positions.Add(p);
            UserPresets.SavePositionsList();
        }
        
        private void OpenPresetsFile()
        {
            UserPresets.OpenPresetsFile();
        }
    }
}
