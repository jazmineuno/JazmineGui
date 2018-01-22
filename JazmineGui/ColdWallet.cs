using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace JazmineGui
{
    public partial class ColdWallet : Form
    {

        int daemon_port = 0;

        public ColdWallet()
        {
            InitializeComponent();

        }

        public void setDaemonPort(int port)
        {
            daemon_port = port;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult res = saveFileDialog1.ShowDialog();
            if (res==DialogResult.OK)
            {
                string curdir = Directory.GetCurrentDirectory(); 
                string dir = Path.GetDirectoryName(saveFileDialog1.FileName);
                File.Copy(curdir + "\\JazmineWallet.exe", dir + "\\JazmineWallet.exe", true);
                Directory.SetCurrentDirectory(dir);
                string param = "--daemon-port=" + daemon_port;

                ProcessStartInfo procStartInfo = new ProcessStartInfo(dir + "\\JazmineWallet.exe", param);

                procStartInfo.UseShellExecute = true;
                procStartInfo.CreateNoWindow = false;

                Process p = new Process();
                p.StartInfo = procStartInfo;
                p.Start();
                p.WaitForExit();
                Directory.SetCurrentDirectory(curdir);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                string curdir = Directory.GetCurrentDirectory();
                string dir = Path.GetDirectoryName(openFileDialog1.FileName);
                Directory.SetCurrentDirectory(dir);
                string param = "--daemon-port=" + daemon_port;

                ProcessStartInfo procStartInfo = new ProcessStartInfo(dir + "\\JazmineWallet.exe", param);

                procStartInfo.UseShellExecute = true;
                procStartInfo.CreateNoWindow = false;

                Process p = new Process();
                p.StartInfo = procStartInfo;
                p.Start();
                p.WaitForExit();
                Directory.SetCurrentDirectory(curdir);
            }

        }
    }
}
