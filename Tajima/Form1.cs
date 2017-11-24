using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Tajima
{
    public partial class Form1 : Form
    {

        private ConcurrentBag<string> results;

        public Form1()
        {
            InitializeComponent();
            results = new ConcurrentBag<string>();
        }

        private void ExamineButton_Click(object sender, EventArgs e)
        {
            
            // Abre el dialogo de selección de archivo
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                string[] directoriesInDirectory = Directory.GetDirectories(path);

                Parallel.ForEach(directoriesInDirectory, (directoryPath) =>
                {
                    string  simulationResultsPath = Directory.GetFiles(directoryPath, "*.xml")[0];

                    StringBuilder result = new StringBuilder();
                    //result.Append(simulationResultsPath); result.Append(";");
                    int counter = 0;
                    string line; 
                    StreamReader file = new StreamReader(simulationResultsPath);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Contains("Tajima's"))
                            counter++;

                        if(counter == 7) // The line of interest is the 7th repetition
                        {

                            MatchCollection matches = Regex.Matches(line, @"-*[0-9,\.]+");
                            
                            foreach (Match match in matches)
                            {
                                //Console.WriteLine(match);
                                result.Append(match); result.Append(";");
                            }

                            break;
                        }                               
                    }

                    file.Close();
                    results.Add((result.ToString()));
                 });

                File.WriteAllLines("ResultadosCondensados.csv", results.ToArray());





            }
        }
    }
}
