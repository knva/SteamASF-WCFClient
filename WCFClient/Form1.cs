using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace WCFClient
{

    public partial class Form1 : Form
    {
     
        //const int SW_HIDE = 0;
        //const int SW_SHOWNORMAL = 1;
        //const int SW_SHOWMINIMIZED = 2;
        //const int SW_SHOWMAXIMIZED = 3;

        //[DllImport("User32.dll")]
        //public static extern bool ShowWindow(IntPtr HWND, int MSG);
        //[DllImport("User32.dll")]
        //public static extern IntPtr FindWindow(string ClassN, string WindN);
        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        const int WM_CLIPBOARDUPDATE = 0x031D;
        const int WM_COPYDATA = 0x004A;
        private static Mwcf Wcf = new Mwcf();
       
        private int pNum = 0;

        private bool RunFlag = false;

      
        public Form1(string a)
        {
            InitializeComponent();
            showInfo(a);
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }
        private void showInfo(string aux)
        {
            DateTime DT = System.DateTime.Now;
            string dt = System.DateTime.Now.ToString();
            skinTextBox1.Text += string.Format(@"{0}|{1}", dt, aux);
            skinTextBox1.Text += @Environment.NewLine;
        }
        private void showStatus(string aux)
        {
            try
            { 
                  skinLabel2.Text = aux;
            }
            catch(Win32Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            System.Timers.Timer pTimer = new System.Timers.Timer(1000);//每隔1秒执行一次，没用winfrom自带的
            pTimer.Elapsed += pTimer_Elapsed;//委托，要执行的方法
            pTimer.AutoReset = true;//获取该定时器自动执行
            pTimer.Enabled = true;//这个一定要写，要不然定时器不会执行的
            Control.CheckForIllegalCrossThreadCalls = false;
            this.notifyIcon1.Text = "WCF客户端";

        }
        #region 隐藏任务栏图标、显示托盘图标
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮 
            if (WindowState == FormWindowState.Minimized)
            {
                //托盘显示图标等于托盘图标对象 
                //注意notifyIcon1是控件的名字而不是对象的名字 
                //notifyIcon1.Icon = ico;
                //隐藏任务栏区图标 
                this.ShowInTaskbar = false;
                //图标显示在托盘区 
                notifyIcon1.Visible = true;
            }
        }
        #endregion

        #region 还原窗体
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            //判断是否已经最小化于托盘 
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示 
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点 
                this.Activate();
                //任务栏区显示图标 
                this.ShowInTaskbar = true;
                //托盘区图标隐藏 
                notifyIcon1.Visible = false;
            }
        }
        #endregion
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
                if(pNum==10)
                {
                    pNum = 0;
                }
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
            批量激活ToolStripMenuItem_Click(sender, e);
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
            if (RunFlag) { 
                showInfo(Wcf.SendCommand("pause"));
                RunFlag = false;
            }
            else
            { 
                showInfo(Wcf.SendCommand("start"));
                RunFlag = true;
            }
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

        private void skinTextBox1_Paint(object sender, PaintEventArgs e)
        {
            
        }
        private static  Socket s;
        private static byte[] result = new byte[1024];
        Thread myThread;
        Thread receiveThread;
        bool threadrun = false;
        private void skinCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (skinCheckBox1.Checked)
            {
                AddClipboardFormatListener(this.Handle);

                int port = 12000;
                string host = "127.0.0.1";

                //创建终结点
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);

                //创建Socket并开始监听

                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建一个Socket对象，如果用UDP协议，则要用SocketTyype.Dgram类型的套接字
                s.Bind(ipe);    //绑定EndPoint对象(2000端口和ip地址)
                s.Listen(0);    //开始监听
                myThread = new Thread(ListenClientConnect);
                threadrun = true;
                myThread.Start();
            }
            else
            {
                RemoveClipboardFormatListener(this.Handle);
                threadrun = false;
                s.Close();
                
            }
        }

        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private  void ListenClientConnect()
        {
            while (threadrun)
            {
                try { 
                Socket clientSocket = s.Accept();
                //clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
                 receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
                }
                catch(SocketException e)
                {
                    break;
                }
            }
        }
        
        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private  void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (threadrun)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    string msg = string.Format("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
                    if (msg.Contains("-"))
                        ExtractKeysAndReg(msg, 0);
                    else
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }
        private void Application_ApplicationExit(object sender, EventArgs e)

        {

            MessageBox.Show("close");
            // RemoveClipboardFormatListener(this.Handle);
            RemoveClipboardFormatListener(this.Handle);
            threadrun = false;
            myThread.Abort();
            s.Close();

        }


        protected override void DefWndProc(ref Message m)
        {
            string message;
             if(m.Msg==0x444)
            {
                MessageBox.Show("!");
            }
            switch (m.Msg)
            {
                 case 0x0444://处理消息 :
                    message = string.Format("{0}", m.LParam);
                    ExtractKeysAndReg(message,1);
                    break;
                case WM_CLIPBOARDUPDATE:
                    CopyFromClipboard();
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }

        }
        private void CopyFromClipboard()
        {
            string clipstr ="";

            try
            {
                clipstr = Clipboard.GetText();
            }
            catch(System.Runtime.InteropServices.ExternalException e)
            {
                return;
            }
          
            if (clipstr.Length > 10)
            {
                ExtractKeysAndReg(clipstr, 1);
            }
        }

        private void ExtractKeysAndReg(string plainText, int showMode)
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
                    if (showMode == 0)
                    {
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



    }
}
