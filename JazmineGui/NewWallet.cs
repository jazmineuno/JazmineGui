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
    public partial class NewWallet : Form
    {
        public NewWallet()
        {
            InitializeComponent();
        }

        public string GetPassword()
        {
            string pwd = password.Text;
            password.Text = "";
            confirmPassword.Text = "";
            password.PasswordChar = '*';
            confirmPassword.PasswordChar = '*';
            return pwd;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            password.PasswordChar = '\0';
            confirmPassword.PasswordChar = '\0'; 
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "AaBbCcDdEeFfGgHhKIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz!@#$%^&*()_+.,><[]}{';:-0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            password.Text = RandomString(48);
            confirmPassword.Text = password.Text;
            password.PasswordChar = '\0';
            confirmPassword.PasswordChar = '\0';
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bitwarden.com/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (password.Text=="")
            {
                MessageBox.Show("Invalid Password", "Password must not be blank");
            } else
            {
                if (password.Text!=confirmPassword.Text)
                {
                    MessageBox.Show("Does Not Match", "The password does not match the confirmation.");
                } else
                {
                    Close();
                }
            }
        }

        private void NewWallet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (password.Text == "")
            {
                MessageBox.Show("Invalid Password", "Password must not be blank");
                e.Cancel = true;
            }
            else
            {
                if (password.Text != confirmPassword.Text)
                {
                    MessageBox.Show("Does Not Match", "The password does not match the confirmation.");
                    e.Cancel = true;
                }
            }

        }
    }
}
