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
using System.Runtime.InteropServices;

namespace WCFClient
{
    public partial class MoreReg : Form
    {
        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        private static int WM_CLIPBOARDUPDATE = 0x031D;

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
            string plainText;
            plainText = textBox3.Text;
            ExtractKeysAndReg(plainText,0);
        }

        private void ExtractKeysAndReg(string plainText ,int showMode)
        {


            List<string> listStrKeys = ExtractKeysFromString(plainText);
            if (listStrKeys.Count > 0)
            {
                string strKeys;


                strKeys = string.Join(",", listStrKeys.ToArray());
                try
                {
                    // Clipboard.SetText(strKeys);
                    // showInfo(strKeys);
                    if (showMode==0) { 
                    showInfo(string.Format("{0} Key被获取,正在激活.", listStrKeys.Count));
                    }
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
                if (showMode == 0)
                {
                    showInfo(string.Format("没有获取到KEY!"));
                }
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
        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                CopyFromClipboard();
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }
        private void CopyFromClipboard()
        {
            string clipstr = Clipboard.GetText();
            if(clipstr.Length>10)
            {
                ExtractKeysAndReg(clipstr,1);
            }
        }
        private void skinCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(skinCheckBox1.Checked)
            {
                AddClipboardFormatListener(this.Handle);
            }
            else
            {
                RemoveClipboardFormatListener(this.Handle);
            }
        }
    }
}
