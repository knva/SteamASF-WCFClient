using System;
using System.ServiceModel;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
//wcf核心部分

namespace WCFClient
{
    [ServiceContract]
    internal interface IWCF
    {
        [OperationContract]
        string GetStatus();

        [OperationContract]
        string HandleCommand(string input);
    }
    static class Program
    {
        private static Mwcf Wcf = new Mwcf();
        private static HostData hd = new HostData();

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // bool running = ReadJson.ReadConfig(Mwcf w);
            if (false) {

                if (ReadJson.SetJson() && ReadJson.ReadConfig(hd))
                {
                    Mwcf.Init(hd);
                    string a = "本程序必须使用管理员权限运行.\n作者不承担一切后果.风险自负!";
                    Application.Run(new Form1(a));
                }
                else
                {
                    MessageBox.Show("没有发现ASF,请放在ASF目录下使用.");

                    Application.Exit();
                }
            }
           else
            {
                if ( ReadJson.ReadConfig(hd))
                {
                    Mwcf.Init(hd);
                    string a = "本程序必须使用管理员权限运行.\n作者不承担一切后果.风险自负!";
                    Application.Run(new Form1(a));
                }
                else
                {
                    MessageBox.Show("没有发现ASF,请放在ASF目录下使用.");

                    Application.Exit();
                }

            }
         
        }


    }
    public class HostData
    {
        public string chost { get; set; }
        public int chostport { get; set; }
    }
    internal sealed class Mwcf : IWCF, IDisposable
    {
       
        private Client Client;
        private static string Host;
        private static string HostPort;
        //    private ServiceHost ServiceHost;
        private static System.ServiceModel.Channels.Binding GetTargetBinding()
        {
            System.ServiceModel.Channels.Binding result;

            result = new WSHttpBinding
            {
                // This is a balance between default of 8192 which is not enough, and int.MaxValue which is prone to DoS attacks
                // We assume maximum of 255 bots and maximum of 1024 characters per each bot included in the response
                ReaderQuotas = { MaxStringContentLength = byte.MaxValue * 1024 },

                // We use SecurityMode.None for Mono compatibility
                // Yes, also on Windows, for Mono<->Windows communication
                Security = { Mode = SecurityMode.None }
            };


            result.SendTimeout = new TimeSpan(0, 0, 60);
            return result;
        }
        //初始化
        internal static void Init(HostData h)
        {

            Host = h.chost;
            HostPort = string.Format("{0}",h.chostport);

        }
    
        internal string SendCommand(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                //   ASF.ArchiLogger.LogNullError(nameof(input));
                return null;
            }

            System.ServiceModel.Channels.Binding binding = GetTargetBinding();
            if (binding == null)
            {
                //ASF.ArchiLogger.LogNullError(nameof(binding));
                return null;
            }

            string url = GetUrlFromBinding(binding);
            if (string.IsNullOrEmpty(url))
            {
                //  ASF.ArchiLogger.LogNullError(nameof(url));
                return null;
            }

            //   ASF.ArchiLogger.LogGenericInfo(string.Format(Strings.WCFSendingCommand, input, url));

            if (Client == null)
            {
                Client = new Client(
                    binding,
                    new EndpointAddress(url)
                );
            }
            string result = Client.HandleCommand(input);
            if (result==null)
            {
                result = "没有返回值,ASF.exe没有运行,或者配置错误.";
            }
            return result;
        }
        private static string GetUrlFromBinding(System.ServiceModel.Channels.Binding binding)
        {
            if (binding != null)
            {
                return binding.Scheme + "://" + Mwcf.Host + ":" + Mwcf.HostPort + "/ASF";
            }

            // ASF.ArchiLogger.LogNullError(nameof(binding));
            return null;
        }

        public string GetStatus()
        {
            throw new NotImplementedException();
        }

        public string HandleCommand(string input)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }

    internal sealed class Client : ClientBase<IWCF>
    {

        internal Client(System.ServiceModel.Channels.Binding binding, EndpointAddress address) : base(binding, address) { }

        internal string HandleCommand(string input)
        {
            if (string.IsNullOrEmpty(input))
            {

                return null;
            }

            try
            {
                return Channel.HandleCommand(input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //ASF.ArchiLogger.LogGenericException(e);
                return null;
            }
        }
    }
    public class Data
    {
        public bool AutoRestart { get; set; }
        public bool AutoUpdates { get; set; }
        public int[] Blacklist { get; set; }
        public int ConnectionTimeout { get; set; }
        public bool Debug { get; set; }
        public int FarmingDelay { get; set; }
        public int GiftsLimiterDelay { get; set; }
        public bool Headless { get; set; }
        public int IdleFarmingPeriod { get; set; }
        public int InventoryLimiterDelay { get; set; }
        public int LoginLimiterDelay { get; set; }
        public int MaxFarmingTime { get; set; }
        public int MaxTradeHoldDuration { get; set; }
        public int OptimizationMode { get; set; }
        public bool Statistics { get; set; }
        public UInt64 SteamOwnerID { get; set; }
        public int SteamProtocol { get; set; }
        public int UpdateChannel { get; set; }
        public int WCFBinding { get; set; }
        public string WCFHost { get; set; }
        public int WCFPort { get; set; }
    }

    public class JsonPaserWeb
    {
        // Object->Json
        public string Serialize(Data obj)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(obj);
            return json;
        }

        // Json->Object
        public Data Deserialize(string json)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //执行反序列化
            Data obj = jsonSerializer.Deserialize<Data>(json);
            return obj;
        }
    }
    internal sealed class ReadJson
    {
        public static bool SetJson()
        {
            try
            {
                JsonSerializer serialiser = new JsonSerializer();
                string newContent = string.Empty;
                using (StreamReader reader = new StreamReader(".\\config\\ASF.json"))
                {
                    string json = reader.ReadToEnd();

                    dynamic jsonObj = JsonConvert.DeserializeObject(json);
                    jsonObj["WCFBinding"] = 2;
                    jsonObj["WCFHost"] = "127.0.0.1";
                    jsonObj["WCFPort"] = 1242;
                    jsonObj["SteamOwnerID"] = 77777777777777777;
                    newContent = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    reader.Close();
                    FileStream fs = new FileStream(".\\config\\ASF.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(newContent);
                    sw.Close();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return false;
            }
    
            return true;

           
        }
        public static bool ReadConfig(HostData w)
        {

            string jsondata;
            try
            {
               // FileStream file = new FileStream(".\\config\\ASF.json", FileMode.Open);
                StreamReader sr = new StreamReader(".\\config\\ASF.json", Encoding.Default);
                jsondata = sr.ReadToEnd();
                sr.Close();

            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return false;
            }

            JsonPaserWeb json = new JsonPaserWeb();
            Data s = json.Deserialize(jsondata);
            w.chost = s.WCFHost;
            w.chostport = s.WCFPort;
            return true;
        }
    }
}
