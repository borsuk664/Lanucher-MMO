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
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            var j = new StreamReader(exePath + "version.json");
            var j1 = j.ReadToEnd();
            ver_actual jj = JsonConvert.DeserializeObject<ver_actual>(j1);
            Console.WriteLine("Aktualna: " + jj.version);
            textblock.Text = "Version: " + jj.version;
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
            Process.Start(exePath + "Launcher.exe");
        }
        public void Check_Update()
        {
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            var json = new WebClient().DownloadString("http://127.0.0.1/files/server.json");
            var j = new StreamReader(exePath + "version.json");
            var j1 = j.ReadToEnd();
            ver_actual jj = JsonConvert.DeserializeObject<ver_actual>(j1);
            ver Server = JsonConvert.DeserializeObject<ver>(json);
            if (jj.version == Server.version)
            {
                Button_Play.IsEnabled = true;
            }
            else
            {
                var Uri = "http://127.0.0.1/files/Aslains_WoT_Modpack_Installer_v.1.10.0.2_02.exe";
                Button_Play.IsEnabled = false;
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileAsync(new Uri(Uri), exePath + "myfile.exe");
            }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, EventArgs e)
        {
            label.Text = "Download completed!";
            Button_Play.IsEnabled = true;
        }
    }    
}
