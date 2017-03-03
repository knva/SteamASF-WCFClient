using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;
using Newtonsoft.Json;
using System.IO;

namespace WCFClient
{

    public partial class Form1 : Form
    {
        private static Mwcf Wcf = new Mwcf();
        private int m_PresentMode = 1;

        public Form1(string a)
        {
            InitializeComponent();
            showInfo(a);
        }
        private void showInfo(string aux)
        {
            DateTime DT = System.DateTime.Now;
            string dt = System.DateTime.Now.ToString();
            skinTextBox1.Text += string.Format("{0}|{1}", dt, aux);
            skinTextBox1.Text += Environment.NewLine;
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
            if (x.Length != 0)
            {
                string a = Wcf.SendCommand(x);
                showInfo(a);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExtractKeysFromClipBoardAndCopyToClipboard();

        }

        private List<string> ExtractKeysFromString(string source)
        {
            MatchCollection m = Regex.Matches(source, "([0-9A-Z]{5})(?:\\-[0-9A-Z]{5}){2,3}",
                  RegexOptions.IgnoreCase | RegexOptions.Singleline);
            List<string> result = new List<string>();
            if (m.Count > 0)
            {
                foreach (Match v in m)
                {
                    result.Add(v.Value);
                }
            }
            return result;
        }
        private void ExtractKeysFromClipBoardAndCopyToClipboard()
        {
            string plainText;
            plainText = textBox3.Text;

            List<string> listStrKeys = ExtractKeysFromString(plainText);
            if (listStrKeys.Count > 0)
            {
                string strKeys;

                switch (m_PresentMode)
                {
                    case 1:
                        strKeys = string.Join(",", listStrKeys.ToArray());
                        break;
                    case 0:
                    default:
                        strKeys = string.Join(Environment.NewLine, listStrKeys.ToArray());
                        break;
                }

                try
                {
                    // Clipboard.SetText(strKeys);
                    showInfo(strKeys);
                    showInfo(string.Format("{0} Key被获取,正在激活.", listStrKeys.Count));
                    //  skinTextBox1.Text += (string.Format("{0} keys have been copied to clipboard", listStrKeys.Count));
                    string stra = (string.Format("redeem {0}", strKeys));
                    showInfo(stra);
                    string a = Wcf.SendCommand(stra);
                    showInfo(a);
                }
                catch
                {
                    MessageBox.Show(strKeys, "Ctrl+C to copy");
                }
            }
            else
            {
                showInfo(string.Format("没有获取到KEY!"));
            }
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@".\\ASF.exe", "--server");
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("没有发现ASF,请放在ASF目录下使用.");
            }
        }


        private void skinButton2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@".\\ASF-ConfigGenerator.exe");
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("没有发现ASF,请放在ASF目录下使用.");
            }
        }

        private void skinButton3_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void skinButton4_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.ShowDialog();
        }
    }
}
