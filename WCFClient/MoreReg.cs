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

namespace WCFClient
{
    public partial class MoreReg : Form
    {
        private static Mwcf Wcf = new Mwcf();
        public MoreReg()
        {

            InitializeComponent();
           
    }

        private void showInfo(string aux)
        {
            MessageBox.Show(aux);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            ExtractKeysAndReg();
        }

        private void ExtractKeysAndReg()
        {
            string plainText;
            plainText = textBox3.Text;

            List<string> listStrKeys = ExtractKeysFromString(plainText);
            if (listStrKeys.Count > 0)
            {
                string strKeys;


                strKeys = string.Join(",", listStrKeys.ToArray());
                try
                {
                    // Clipboard.SetText(strKeys);
                   // showInfo(strKeys);
                    showInfo(string.Format("{0} Key被获取,正在激活.", listStrKeys.Count));
                    //  skinTextBox1.Text += (string.Format("{0} keys have been copied to clipboard", listStrKeys.Count));
                    string stra = (string.Format("redeem {0}", strKeys));
                    //showInfo(stra);
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

    }
}
