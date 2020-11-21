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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Reflection.Emit;
using System.IO.Compression;

namespace Launcher
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            check_ver();
        }
        public void check_ver()
        {
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            var j = new StreamReader(exePath + "version.json");
            var j1 = j.ReadToEnd();
            ver_actual jj = JsonConvert.DeserializeObject<ver_actual>(j1);
            textblock.Text = "Build: " + jj.version;
            j.Close();
            Check_Update();
        }
        public class ver
        {
            public string version { get; set; }

        }
        public class ver_actual
        {
            public string version { get; set; }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(exePath);
            var exefPath = exePath + "build\\";
            Process.Start(exefPath + "mmo.exe");
        }
        public void Check_Update()
        {
            label.Text = "Checking for updates...";
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            var json = new WebClient().DownloadString("http://18.192.38.56/server.json");
            var j = new StreamReader(exePath + "version.json");
            var j1 = j.ReadToEnd();
            j.Close();
            ver_actual jj = JsonConvert.DeserializeObject<ver_actual>(j1);
            ver Server = JsonConvert.DeserializeObject<ver>(json);
            if (jj.version == Server.version)
            {
                Button_Play.IsEnabled = true;
                label.Text = "Ready to play!";
                progressBar.Opacity = 0;
            }
            else
            {
                ///var Uri = "http://127.0.0.1/files/Aslains_WoT_Modpack_Installer_v.1.10.0.2_02.exe";
                Button_Play.IsEnabled = false;
                Dl();

            }
        }

        public void Dl()
        {
            var Uri = "http://18.192.38.56/build.zip";
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri(Uri), exePath + "build.zip");
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, EventArgs e)
        {
            label.Text = "Download completed!";
            ///Button_Play.IsEnabled = true;
            extract();
        }
        public void extract()
        {
            label.Text = "Extracting update";
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            string path = exePath + "build";
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            finally
            {
                ZipFile.ExtractToDirectory(exePath + "build.zip", exePath);
                File.Delete(exePath + "build.zip");
                
                var json = new WebClient().DownloadString("http://18.192.38.56/server.json");
                Console.WriteLine(json);

                //File.Delete(exePath + "version.json");
                //using (StreamWriter sw = File.CreateText(exePath))
                //{
                //    sw.WriteLine(json);
                //}
                var j = new StreamReader(exePath + "version.json");
                var j1 = j.ReadToEnd();
                ver_actual jj = JsonConvert.DeserializeObject<ver_actual>(j1);
                Console.WriteLine(j);
                j.Close();
                string text = File.ReadAllText("version.json");
                text = text.Replace((string)j1, (string)json);
                File.WriteAllText("version.json", text);
                label.Text = "Ready to play!";
                Button_Play.IsEnabled = true;
                progressBar.Opacity = 0;
                check_ver();
            }
        }

    }
}