using ConsoleUI;
using System;
using System.Xml;
using System.IO;


List<string> fileList = new();

string fnameAppend = "-points";
string xmlExt = ".xml";


bool mainLoop = true;
while (mainLoop)
{
    UI.Header("Home");
    UI.Write("This program creates a .CSV file containing points extracted from a given XML surface.");
    UI.Write("");
    UI.Write("Drag and drop an XML file and press ENTER to begin.");
    UI.Write("");
    UI.Option("E[X]IT");

    string input = Input.GetString();
    input = input.Replace("\"", "");

    switch (input.ToUpper())
    {
        case "X":
        case "EXIT":
            mainLoop = false;
            break;

        case "":
            break;

        default:
            // Clear loaded files
            fileList.Clear();

            // Load all files in directory
            if (Directory.Exists(input))
            {
                string[] files = Directory.GetFiles(input);

                foreach (string file in files)
                {
                    if (Path.GetExtension(file).ToLower() == xmlExt)
                    {
                        fileList.Add(file);
                    }
                }
            }

            // Load a single file
            else if (File.Exists(input))
            {
                fileList.Add(input);
            }

            // Process
            if (fileList.Count > 0)
            {
                bool menuProcess = true;
                while (menuProcess)
                {
                    UI.Header("Folder Found");
                    UI.Write($"Current files:");
                    foreach (string file in fileList)
                    {
                        UI.Write($"\t{file}");
                    }
                    UI.Write();
                    UI.Write("Continue?");
                    UI.Option("[Y]", "Yes");
                    UI.Option("[N]", "No");
                    UI.Write();

                    switch (Input.GetString("Y").ToUpper())
                    {
                        case "Y":
                            UI.Header("Extracting points...");
                            foreach (string file in fileList)
                            {
                                ProcessXmlFile(file, true);
                            }
                            UI.Pause();
                            menuProcess = false;
                            break;

                        default:
                            menuProcess = false;
                            break;
                    }
                }
            }
            else
            {
                UI.Error("No files loaded.");
            }
            break;
    }
}

UI.Header("Goodbye");


void ProcessXmlFile(string fname, bool autosave = false)
{
    UI.Write($"\tExtracting {Path.GetFileName(fname)}...");

    XmlDocument xmlDoc = new();
    xmlDoc.Load(fname);

    XmlNodeList points = xmlDoc.GetElementsByTagName("P");

    string output = "";

    if (points.Count > 0)
    {
        // Add to output string
        foreach (XmlNode point in points)
        {
            string pointString = point.InnerText.ToString().Replace(" ", ",");

            output += pointString + "\n";
        }

        // Write output string to file
        string outputFname = Path.GetDirectoryName(fname) + "\\"
                + Path.GetFileNameWithoutExtension(fname)
                + fnameAppend
                + ".csv";
        
        if (File.Exists(outputFname))
        {
            UI.Write("\tOverwriting existing file...");
        }

        File.WriteAllText(outputFname, output, System.Text.Encoding.UTF8);

        UI.Write("\tDone!");
        UI.Write();
    }
    else
    {
        UI.Error("No points found in XML document!");
    }
}


public class Point
{
    float x, y, z;
}