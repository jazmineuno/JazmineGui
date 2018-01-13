using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JazmineGui
{
    public partial class LoadWallet : Form
    {
        public LoadWallet()
        {
            InitializeComponent();
        }

        public string GetPassword()
        {
            string pwd = password.Text;
            password.Text = "";
            password.PasswordChar = '*';
            return pwd;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://jazmine.io/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (password.Text == "")
            {
                MessageBox.Show("Invalid Password", "Password must not be blank");
            }
            else { Close(); }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            password.PasswordChar = '\0';
        }

        private void LoadWallet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (password.Text == "")
            {
                MessageBox.Show("Invalid Password", "Password must not be blank");
                e.Cancel = true;
            }
        }
    }
}
