using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using System.Text.RegularExpressions;
using System.Collections;

namespace JazmineGui
{
    public partial class Form1 : Form
    {

        protected PerformanceCounter cpuCounter;
        protected PerformanceCounter ramCounter;
        Process Jazmined_process;
        Process Wallet_process;
        Process PHP_process;
        int jazmined_port = 0;
        int wallet_port = 0;
        int php_port = 0;
        int cnt = 0;
        int w_cnt = 0;
        string path = "";
        CancellationToken ct;
        CancellationTokenSource ts;
        CancellationToken w_ct;
        CancellationTokenSource w_ts;
        string wallet_pwd = "";
        CefSharp.WinForms.ChromiumWebBrowser browser;
        bool firstrun = true;

        public Form1()
        {
            InitializeComponent();

            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            toolStripStatusLabel2.Text = "CPU: " + getCurrentCpuUsage();
            toolStripStatusLabel1.Text = "MEM: " + getAvailableRAM();
            toolStripStatusLabel3.Text = "";
            toolStripStatusLabel4.Text = "";

            statusStrip1.Refresh();

            path = Directory.GetCurrentDirectory();
            demangle();
            


            Process[] pname = Process.GetProcessesByName("Jazmined");

            if (pname.Length > 0)
            {
                Console.WriteLine("Jazmined process already running");
                AlreadyRunning ar = new AlreadyRunning();
                ar.ShowDialog();
                if (ar.docancel)
                {
                    Close();
                }
            }

            wallet_port = FreeTcpPort();
            Console.WriteLine("Wallet port: " + wallet_port);



            if (!File.Exists(path + "\\jazmine.bin.wallet"))
            {
                NewWallet nw = new NewWallet();
                nw.ShowDialog();
                wallet_pwd = nw.GetPassword();
                CreateWallet();
                wallet_pwd = "";
            }
            launchJazmined();

            LoadWallet lw = new LoadWallet();
            lw.ShowDialog();
            wallet_pwd = lw.GetPassword();

            launchWallet();

            Cef.EnableHighDPISupport();

            var settings = new CefSettings()
            {
            };

            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            browser = new CefSharp.WinForms.ChromiumWebBrowser("about:blank")
            {
                Dock = DockStyle.Fill,
            };
            panel1.Controls.Add(browser);
            LoadPhp();

            if (php_port > 0)
            {
                string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                browser.Load("http://127.0.0.1:" + php_port + "/home.php?version=" + version + "&sp=" + jazmined_port + "&wp=" + wallet_port);
            }

            timer1.Enabled = true;
        }

        private void demangle()
        {
            string rn = DateTime.Now.ToString("yyyMMddHHmmss");
            string[] files = Directory.GetFiles(path, "jazmine.bin.wallet.*");
            if (files.Length > 1)
            {
                int cnt = 0;
                foreach (string file in files)
                {
                    cnt++;
                    if (cnt==files.Length)
                    {
                        System.IO.File.Move(file, path + "\\jazmine.bin.wallet");
                    } else
                    {
                        System.IO.File.Move(file, file + "." + rn);
                    }

                    //Console.WriteLine(file);

                }
            }
        }

        private void launchlogs()
        {

            firstrun = false;
            //var uiContext = TaskScheduler.FromCurrentSynchronizationContext();

            ts = new CancellationTokenSource();
            ct = ts.Token;
            var jazmined_progress = new Progress<string>(s => { jazminedlog.Invoke((MethodInvoker)delegate { jazminedlog.AppendText(s + "\r\n"); }); });
            Task.Factory.StartNew(() => WatchJazminedLog(jazmined_progress), w_ct, TaskCreationOptions.LongRunning, TaskScheduler.Default);


            w_ts = new CancellationTokenSource();
            w_ct = w_ts.Token;

            var wallet_progress = new Progress<string>(s => { jazminewalletlog.Invoke((MethodInvoker)delegate { jazminewalletlog.AppendText(s + "\r\n"); }); });
            Task.Factory.StartNew(() => WatchJazmineWalletLog(wallet_progress), w_ct, TaskCreationOptions.LongRunning, TaskScheduler.Default);


        }

        public static string EscapeParm(string ors)
        {
            if (string.IsNullOrEmpty(ors))
            {
                return ors;
            }
            string val = Regex.Replace(ors, @"(\\*)" + "\"", @"$1\$0");
            val = Regex.Replace(val, @"^(.*\s.*?)(\\*)$", "\"$1$2$2\"");
            return val;
        }

        private void CreateWallet()
        {
            if (wallet_pwd=="")
            {
                MessageBox.Show("Blank Password", "The password must not be blank.");
            } else
            {

                ProcessStartInfo procStartInfo = new ProcessStartInfo(path + "\\JazmineWalletd.exe", " -w jazmine.bin.wallet -p " + EscapeParm(wallet_pwd) + " -g");
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                using (Process process = new Process())
                {
                    process.StartInfo = procStartInfo;
                    process.Start();
                    process.WaitForExit();
                    string res = process.StandardOutput.ReadToEnd();
                    jazminewalletlog.AppendText(res);
                }

            }
        }

        private void LoadPhp()
        {

            php_port = FreeTcpPort();
            Console.WriteLine("PHP port: " + php_port);
            string param = "-S 127.0.0.1:" + php_port + " -t " + '"' + path + "\\jazmine-php" + '"';
            ProcessStartInfo procStartInfo = new ProcessStartInfo(path + "\\php\\php.exe", param);
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            using (PHP_process = new Process())
            {
                PHP_process.StartInfo = procStartInfo;
                PHP_process.Start();
            }
        }


        private void launchJazmined()
        {
            jazmined_port = FreeTcpPort();
            Console.WriteLine("Jazmined port: " + jazmined_port);
            string param = "--rpc-bind-port=" + jazmined_port + " --no-console --log-level=4";
            Console.WriteLine(param);
            ProcessStartInfo procStartInfo = new ProcessStartInfo(path + "\\Jazmined.exe", param);
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            using (Jazmined_process = new Process())
            {
                Jazmined_process.StartInfo = procStartInfo;
                Jazmined_process.Start();
            }
        }

        private void launchWallet()
        {
            string param = "-w jazmine.bin.wallet -p " + EscapeParm(wallet_pwd) + " --log-level=4 --daemon-port=" + jazmined_port + " --bind-address=127.0.0.1 --bind-port=" + wallet_port;
            Console.WriteLine(param);
            
            ProcessStartInfo procStartInfo = new ProcessStartInfo(path + "\\JazmineWalletd.exe", param);
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            using (Wallet_process = new Process())
            {
                Wallet_process.StartInfo = procStartInfo;
                Wallet_process.Start();
            }
        }

        private void WatchJazminedLog(IProgress<string> s)
        {
            cnt = 0;
            bool againagain = true;
            while (!File.Exists(path + "\\Jazmined.log")) { }
            using (var fs = new FileStream(path + "\\Jazmined.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream) { string already = reader.ReadLine();  }
                while (againagain)
                {
                    if (ct.IsCancellationRequested)
                    {
                        againagain = false;
                        break;
                    }
                    else
                    {
                        var line = reader.ReadLine();
                        if (!String.IsNullOrWhiteSpace(line))
                        {
                                ++cnt;
                                if (s != null)
                                {
                                    s.Report(line);
                                }
                        }
                    }
                }
            }

        }


        private void WatchJazmineWalletLog(IProgress<string> s)
        {
            w_cnt = 0;
            bool againagain = true;
            while (!File.Exists(path + "\\payment_gate.log")) { }
            using (var fs = new FileStream(path + "\\payment_gate.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream) { string already = reader.ReadLine(); }
                while (againagain)
                {
                    if (w_ct.IsCancellationRequested)
                    {
                        againagain = false;
                        break;
                    }
                    else
                    {
                        var line = reader.ReadLine();
                        if (!String.IsNullOrWhiteSpace(line))
                        {
                                ++w_cnt;
                                if (s != null)
                                {
                                    s.Report(line);
                                }
                        }
                    }
                }
            }

        }

        static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;

        } 


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public String getCurrentCpuUsage()
        {
            return string.Format("{0:F1}", cpuCounter.NextValue()) + "%";
        }

        public String getAvailableRAM()
        {
            return ramCounter.NextValue() + "Mb";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (firstrun) launchlogs();
            toolStripStatusLabel2.Text = "CPU: " + getCurrentCpuUsage();
            toolStripStatusLabel1.Text = "MEM: " + getAvailableRAM();
            statusStrip1.Refresh();
            if (cnt>6250)
            {
                jazminedlog.Text = "";
                cnt = 0;
            }
            if (w_cnt>6250)
            {
                jazminewalletlog.Text = "";
                w_cnt = 0;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void coldWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColdWallet cs = new ColdWallet();
            cs.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Wallet_process.Kill();
            }
            catch (Exception we)
            {
                Console.WriteLine("Could not terminate wallet daemon. " + we.Message);
            }

            try
            {
                Jazmined_process.Kill();
            }
            catch (Exception je)
            {
                Console.WriteLine("Could not terminate Jazmine daemon. " + je.Message);
            }

            try
            {
                PHP_process.Kill();
            }
            catch (Exception pe)
            {
                Console.WriteLine("Could not terminate PHP daemon. " + pe.Message);
            }
            Thread.Sleep(3000);
            ts.Cancel();
            w_ts.Cancel();

        }
    }
}
