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
        // -------------------------------------------
        // Attributes
        // -------------------------------------------

        /// <summary>
        /// The results lines are stored here.
        /// </summary>
        private ConcurrentBag<string> results;


        // -------------------------------------------
        // Methods
        // -------------------------------------------

        public Form1()
        {
            InitializeComponent();
          
        }

        /// <summary>
        /// Called when the button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExamineButton_Click(object sender, EventArgs e)
        {
            results = new ConcurrentBag<string>();

            // Opens the select directory Dialog
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                label2.Text = "carpeta " + path + " seleccionada. Ejecución en curso...";
                label2.Refresh();
                string[] directoriesInDirectory = Directory.GetDirectories(path);

                // Read the folders in parallel
                Parallel.ForEach(directoriesInDirectory, (directoryPath) =>
                {
                    // The file of interest is the only xml file in the folder.
                    string  simulationResultsPath = Directory.GetFiles(directoryPath, "*.xml")[0];
                    StringBuilder result = new StringBuilder();
                    int counter = 0;
                    string line; 
                    StreamReader file = new StreamReader(simulationResultsPath);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Contains("Tajima's"))
                            counter++;

                        if(counter == 7) // The line of interest is the 8th repetition
                        {
                            // File line example:             
                            //           Tajima's D    -0.35637    -0.31302    -0.33470     0.03065 
                            // This regex matchs all numbers on the line.
                            MatchCollection matches = Regex.Matches(line, @"-*[0-9,\.]+");                           
                            foreach (Match match in matches)
                            {
                                result.Append(match); result.Append(";"); // Output file is a csv file that uses ; as separator.
                            }
                            break;
                        }                               
                    }

                    file.Close();
                    results.Add(result.ToString());
                 });
                
                // Output file is a csv file that uses ; as separator.
                File.WriteAllLines("ResultadosCondensados.csv", results.ToArray());
                label2.Text = "Ejecución terminada con éxito. " + directoriesInDirectory.Length + " archivos leidos.";

            }
        }
    }
}
