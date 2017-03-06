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
        private int IsRunning = 0;
        private int pNum = 0;

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
        private void showStatus(string aux)
        {
            try
            { 
                  skinLabel2.Text = aux;
            }
            catch(Win32Exception e)
            {

            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            System.Timers.Timer pTimer = new System.Timers.Timer(1000);//每隔1秒执行一次，没用winfrom自带的
            pTimer.Elapsed += pTimer_Elapsed;//委托，要执行的方法
            pTimer.AutoReset = true;//获取该定时器自动执行
            pTimer.Enabled = true;//这个一定要写，要不然定时器不会执行的
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        private void pTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (pNum == 10)
            {
                string str = Wcf.SendCommand("status");
                if (str.Length != 0)
                {
                    showInfo(str);
                }
                pNum = 0;
            }
            else
            {
                showStatus(string.Format("{0}秒后更新状态", 10 - pNum));
                pNum++;
            }
        }




        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (skinButton1.Text == "重新加载ASF")
            {
                Wcf.SendCommand("restart");
                return;
            }
            try
            {

                System.Diagnostics.Process process = new System.Diagnostics.Process();

                process.StartInfo.FileName = "ASF.exe";   //asf
                process.StartInfo.Arguments = "--server";

                process.Start();
                //IsRunning= process.Id;
                启动ASFToolStripMenuItem.Text = "重新加载ASF";
                skinButton1.Text = "重新加载ASF";
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


        private void 启动ASFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            skinButton1_Click(sender, e);
        }

        private void 配置ASFToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void 打开ASFjsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"notepad.exe", "./config/ASF.json");
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("没有发现ASF,请放在ASF目录下使用.");
            }
        }

        private void 批量激活ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoreReg Mreg = new MoreReg();
            Mreg.ShowDialog();
        }

        private void 查看状态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showInfo(Wcf.SendCommand("status"));
        }

        private void 查看版本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showInfo(Wcf.SendCommand("version"));
        }

        private void 暂停挂机ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showInfo(Wcf.SendCommand("pause"));
        }

        private void 添加账号ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.ShowDialog();
        }

        private void 挂机指定游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGames Sgames = new SetGames();
            Sgames.ShowDialog();
        }

        private void 手动指令ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnterCommands enc = new EnterCommands();
            enc.ShowDialog();
        }

        private void 指令手册ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            skinButton3_Click(sender, e);


        }

        private void skinButton5_Click(object sender, EventArgs e)
        {
            skinTextBox1.Text = "";
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
