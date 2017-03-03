using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
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
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Mwcf.Init();
            string a = Wcf.SendCommand("version");
            Application.Run(new Form1(a));
        }
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
        internal static void Init()
        {

            Host = "106.184.7.15";
            HostPort = "1242";

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

            return Client.HandleCommand(input);
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
                //ASF.ArchiLogger.LogGenericException(e);
                return null;
            }
        }
    }
}
