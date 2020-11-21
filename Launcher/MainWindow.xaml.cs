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
using System.Security.Cryptography;

namespace Launcher
{
    public static class Globals
    {
        public static string exePath = AppDomain.CurrentDomain.BaseDirectory;
        public static string exefPath = exePath + "build\\";
    }
    public static class Constants
    {
        public const string server_url = "http://127.0.0.1/files/server.json";
        public const string build_url = "http://127.0.0.1/files/build.zip";
        public const string checksum = "5d4a2b158c82af4b978f55b259a46590";
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            check_ver();
            Check_Update();
        }
        public void check_ver()
        {
            var j = new StreamReader(Globals.exePath + "version.json");
            var j1 = j.ReadToEnd();
            ver jj = JsonConvert.DeserializeObject<ver>(j1);
            textblock.Text = "Version: " + jj.version;
            j.Close();
        }
        public class ver
        {
            public string version { get; set; }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(Globals.exePath);
            Process.Start(Globals.exefPath + "mmo.exe");
        }
        public void Check_Update()
        {
            label.Text = "Checking for updates...";
            var json = new WebClient().DownloadString(Constants.server_url);
            var j = new StreamReader(Globals.exePath + "version.json");
            var j1 = j.ReadToEnd();
            j.Close();
            ver jj = JsonConvert.DeserializeObject<ver>(j1);
            ver Server = JsonConvert.DeserializeObject<ver>(json);
            if (jj.version == Server.version)
            {
                var chksm = CreateDirectoryMd5(Globals.exefPath);
                Console.WriteLine(chksm);
                if (chksm != Constants.checksum)
                {
                    label.Text = "File verification failed. Repairing...";
                    Dl();
                }
                else { ready(); }
            }
            else
            {
                Button_Play.IsEnabled = false;
                Dl();
            }
        }
        public void Dl()
        {
            //var exePath = AppDomain.CurrentDomain.BaseDirectory;
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri(Constants.build_url), Globals.exePath + "build.zip");
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        private void Completed(object sender, EventArgs e)
        {
            label.Text = "Download completed!";
            extract();
        }
        public void extract()
        {
            label.Text = "Extracting update";
            try
            {
                if (Directory.Exists(Globals.exefPath))
                {
                    Directory.Delete(Globals.exefPath, true);
                }
            }
            finally
            {
                ZipFile.ExtractToDirectory(Globals.exePath + "build.zip", Globals.exePath);
                File.Delete(Globals.exePath + "build.zip");
                var json = new WebClient().DownloadString(Constants.server_url);
                var j = new StreamReader(Globals.exePath + "version.json");
                var j1 = j.ReadToEnd();
                j.Close();
                string text = File.ReadAllText("version.json");
                text = text.Replace((string)j1, (string)json);
                File.WriteAllText("version.json", text);
                var chksm = CreateDirectoryMd5(Globals.exefPath);
                if (chksm != Constants.checksum)
                {
                    label.Text = "File verification failed. Repairing...";
                    Dl();
                }
                else { ready(); }
                
            }
                

        }
        public static string CreateDirectoryMd5(string srcPath)
        {
            var filePaths = Directory.GetFiles(srcPath, "*", SearchOption.AllDirectories).OrderBy(p => p).ToArray();
            using (var md5 = MD5.Create())
            {
                foreach (var filePath in filePaths)
                {
                    byte[] pathBytes = Encoding.UTF8.GetBytes(filePath);
                    md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);
                    byte[] contentBytes = File.ReadAllBytes(filePath);
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }
                md5.TransformFinalBlock(new byte[0], 0, 0);
                return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
            }
        }
        public void ready()
        {
            label.Text = "Ready to play!";
            Button_Play.IsEnabled = true;
            progressBar.Opacity = 0;
        }
    }
}