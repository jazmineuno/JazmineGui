using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JazmineGui
{
    public partial class AlreadyRunning : Form
    {
        public bool docancel = false;

        public AlreadyRunning()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            docancel = true;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Process[] pname = Process.GetProcessesByName("Jazmined");

            if (pname.Length > 0)
            {
                Console.WriteLine("Jazmined process already running, terminating...");

                foreach (var process in Process.GetProcessesByName("Jazmined"))
                {
                    process.Kill();
                    Console.WriteLine("Terminated.");
                }
            }

            System.Threading.Thread.Sleep(2000);
            pname = Process.GetProcessesByName("Jazmined");
            if (pname.Length > 0 )
            {
                button1.Enabled = true;
                label1.Text = "Process still running. Please try again.\r\n.You may need to launch the Windows process manager and find the Jazmined.exe\r\nprocess and terminate it manually.";
            } else
            {
                pname = Process.GetProcessesByName("JazmineWalletd");
                if (pname.Length > 0)
                {
                    Console.WriteLine("JazmineWalletd process already running, terminating...");

                    foreach (var process in Process.GetProcessesByName("JazmineWalletd"))
                    {
                        process.Kill();
                        Console.WriteLine("Terminated.");
                    }
                }

                System.Threading.Thread.Sleep(2000);

                pname = Process.GetProcessesByName("JazmineWalletd");

                if (pname.Length > 0)
                {
                    button1.Enabled = true;
                    label1.Text = "Process still running. Please try again.\r\n.You may need to launch the Windows process manager and find the Jazmined.exe\r\nprocess and terminate it manually.";

                }
                else
                {
                    docancel = false;
                    Close();
                }
            }

        }
    }
}
