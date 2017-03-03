using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WCFClient
{
   
    public partial class Form1 : Form
    {
        private static Mwcf Wcf = new Mwcf();
        public Form1(string a )
        {
            InitializeComponent();
            label1.Text = a;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string x = textBox1.Text;
            string a = Wcf.SendCommand(x);
            label1.Text = a;
        }
    }
}
