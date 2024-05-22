using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOG_based_calculator
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string jsonFilePath = "D:\\Academic_Material\\semester 8\\HCI\\project\\EOG based calculator\\data.json";
            string jsonData = File.ReadAllText(jsonFilePath);

            var data = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jsonData);

            var classNames = new Dictionary<int, string>
        {
            { 0, "Up" },
            { 1, "Down" },
            { 2, "Right" },
            { 3, "Left" },
            { 4, "Blink" }
        };

            var form = new Form1();

            form.SimulateButtonClicksAsync(data["test"], classNames);

            Application.Run(form);
        }

      
    }
}
