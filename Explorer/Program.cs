using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    class Program
    {
        private static Dictionary<int, string> FindAssetNames(byte[] bytes)
        {
            Dictionary<int, string> names = new Dictionary<int, string>();
            Array.Reverse(bytes);
            int i = 5;

            string itemName = "";
            while (bytes[i] != 0)
            {
                itemName = (char)bytes[i++] + itemName;
            }
            i += 3;
            //names[0] = itemName;

            string tempName = "";
            while (true)
            {
                tempName = "";
                while (bytes[i] != 0)
                {
                    tempName = (char)bytes[i++] + tempName;
                }

                i++;
                if (bytes[i] == 0)
                    break;
                names[bytes[i]] = tempName.Substring(itemName.Length + 1);
                i += 2;
            }
            return names;
        }

        static void ExtractFile(string inputFileName, string outputFileName, int id)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "swfextract.exe";
            startInfo.Arguments = inputFileName + " -a " + id + " -o " + outputFileName;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        static void DecompressSWF(string fileName)
        {
            //Decompress
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "swfdecompress.exe";
            startInfo.Arguments = fileName;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        static void Main(string[] args)
        {
            string fileName = "file.swf";
            if (args.Length > 0)
            {
                fileName = args[0];
            }
            else
            {
                Console.WriteLine("No file specified");
                return;
            }

            if (!File.Exists(fileName))
            {
                Console.WriteLine("File doesn't exists!");
                return;
            }

            DecompressSWF(fileName);
            Console.WriteLine("Decompressed");

            string dirName = fileName.Split('.')[0];

            if (!Directory.Exists(dirName))
            {
                Console.WriteLine("Directory " + dirName + " does not exist, creating the directory!", ConsoleColor.Blue);
                Directory.CreateDirectory(dirName);
            }

            byte[] bytes = File.ReadAllBytes(fileName);

            Dictionary<int, string> names = FindAssetNames(bytes);

            List<string> xmls = new List<string>() { "index", "manifest", "assets", "logic", "visualization" };

            foreach (KeyValuePair<int, string> pair in names)
            {
                string ext = xmls.Any(e => pair.Value.EndsWith(e)) ? ".xml" : ".png";
                Console.WriteLine(pair.Key + " -- >" + pair.Value + ext);
                ExtractFile(fileName, dirName + "\\" + pair.Value + ext, pair.Key);
            }
        }
    }
}
