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
using ConsoleControl;

namespace JazmineGui
{
    public partial class ColdWallet : Form
    {

        public ColdWallet()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();
            if (res==DialogResult.OK)
            {
                string curdir = Directory.GetCurrentDirectory(); 
                string dir = Path.GetDirectoryName(openFileDialog1.FileName);
                File.Copy(curdir + "\\JazmineWallet.exe", dir + "\\JazmineWallet.exe", true);
            }
        }
    }
}
