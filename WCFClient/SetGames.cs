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
    public partial class SetGames : Form
    {
        private static Mwcf Wcf = new Mwcf();
        public SetGames()
        {
            InitializeComponent();
        }
        private void showInfo(string aux)
        {
            MessageBox.Show(aux);

        }
        private void skinButton1_Click(object sender, EventArgs e)
        {
            string str = string.Format("!play {0}",skinTextBox1.Text);
            showInfo( Wcf.SendCommand(str));
        }
    }
}
