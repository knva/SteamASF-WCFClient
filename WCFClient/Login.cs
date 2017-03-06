using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WCFClient
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (skinTextBox1.Text == null && skinTextBox2.Text == null)
            {
                MessageBox.Show("请输入账号,密码");
                return;
            }
            JsonSerializer serialiser = new JsonSerializer();
            string newContent = string.Empty;
            using (StreamReader reader = new StreamReader(".\\config\\example.json"))
            {
                string json = reader.ReadToEnd();

                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                jsonObj["SteamLogin"] = skinTextBox1.Text;
                jsonObj["SteamPassword"] = skinTextBox2.Text;
                jsonObj["Enabled"] = true;
                newContent = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

            }
            FileStream fs = new FileStream(".\\config\\SteamLogin.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(newContent);
            sw.Close();
            this.Close();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
