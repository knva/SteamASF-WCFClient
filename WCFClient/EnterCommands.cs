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
    public partial class EnterCommands : Form
    {
        private static Mwcf Wcf = new Mwcf();
        public EnterCommands()
        {
            InitializeComponent();
        }
        private void showInfo(string aux)
        {
            MessageBox.Show(aux);

        }
        private void EnterCommands_Load(object sender, EventArgs e)
        {
            string commands = "2fa,2fano,2faok,addlicense,api,exit,farm,help,input,leave,loot,owns,password,pause,play,redeem,rejoinchat,restart,resume,start,status,stop,update,version";
            for(int i = 0; i < commands.Split(',').Length;i++)
            skinComboBox1.Items.Add(commands.Split(',')[i]);

        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (skinComboBox1.Text.Length > 0 && skinTextBox1.Text.Length==1)
            {
                showInfo(Wcf.SendCommand(string.Format("{0}", skinComboBox1.Text)));

            }
            else if (skinComboBox1.Text.Length > 0 && skinTextBox1.Text.Length>1)
            {
                showInfo(Wcf.SendCommand(string.Format("{0} {1}", skinComboBox1.Text, skinTextBox1.Text)));

            }
        }
    }
}
