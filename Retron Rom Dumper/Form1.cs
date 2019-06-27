using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Retron_Rom_Dumper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            cmd();
            DirectoryInfo taskDirectory = new DirectoryInfo(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
            FileInfo[] taskFiles = taskDirectory.GetFiles("dump.*");


            for (int x = 0; x < taskFiles.Count(); x++)
            {
                string ext = extenstion(taskFiles[x].Extension);
                if (ext == "nes")
                {
                    scanRenamer("dump.nes", "nes.dat");

                }
                else if (ext == "sfc" || ext == "smc")
                {
                    scanRenamer("dump." + ext, "snes.dat");
                }
                else if (ext == "gb")
                {
                    scanRenamer("dump." + ext, "gb.dat");
                }
                else if (ext == "gbc")
                {
                    scanRenamer("dump." + ext, "gbc.dat");
                }
                else if (ext == "gba")
                {
                    scanRenamer("dump." + ext, "gba.dat");
                }
                else if (ext == "gen" || ext == "md")
                {
                    scanRenamer("dump." + ext, "genesis.dat");
                }
                else if (ext == "gg")
                {
                    scanRenamer("dump." + ext, "gamegear.dat");
                }
                else if (ext == "sms")
                {
                    scanRenamer("dump." + ext, "sms.dat");
                }
                else
                    MessageBox.Show("Unable to find dump file");
               
            }
        }
        public void cmd()
        {

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c adb pull /mnt/ram/",
                Verb = "runas"
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }

        
        }

        public string extenstion(string ext)
        {

            return ext.Substring(1).ToLower();
        }
        public void scanRenamer(string filename, string datfile)
        {
            string md5file = CRC32File(filename);
            XmlDocument doc = new XmlDocument();
            doc.Load(@"dat/" + datfile);
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("game");
            int totalNodes = nodes.Count;
            int counter = 1;
            foreach (XmlNode node in nodes)
            {
                string md5 = node.ChildNodes[1].Attributes["crc"].Value;
                string name = node.ChildNodes[1].Attributes["name"].Value;
                if (md5 == md5file)
                {
                    if(File.Exists(name))
                        File.Delete(name);


                    bool exists = System.IO.Directory.Exists("Output");

                    if (!exists)
                        System.IO.Directory.CreateDirectory("Output");

                    System.IO.File.Move(filename, "Output/"+  name);
                    MessageBox.Show("File ripped as " + name.Substring(0,name.Length - 4));

                    break;
                }

                if (counter == totalNodes)
                    MessageBox.Show("Unable to find game in " + datfile);

                counter++;
            }
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant().ToUpper();
                }
            }
        }


        public static string CRC32File(string FILENAME)
        {
            Crc32 crc32 = new Crc32();
            String hash = String.Empty;
            using (FileStream fs = File.Open(FILENAME, FileMode.Open)) //here you pass the file name 
            {
                foreach (byte b in crc32.ComputeHash(fs))
                {
                    hash += b.ToString("x2").ToLower();
                }
                return hash.ToUpper();
            }

        }
    }
}
