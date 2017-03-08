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
        private string MasterId = "0";
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
           // JsonSerializer serialiser = new JsonSerializer();
            string newContent = string.Empty;
            string BotName = "";
            using (StreamReader reader = new StreamReader(".\\config\\example.json"))
            {
                string json = reader.ReadToEnd();

                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                jsonObj["SteamLogin"] = skinTextBox1.Text;
                jsonObj["SteamPassword"] = skinTextBox2.Text;
                jsonObj["Enabled"] = true;
                if (skinCheckBox1.Checked)
                {
                    jsonObj["RedeemingPreferences"] = 1;
                    BotName = string.Format(".\\config\\[Master]{0}.json", skinTextBox1.Text);
                }
                else
                {
                    jsonObj["SteamMasterID"] = MasterId;
                    BotName = string.Format(".\\config\\[Servant]{0}.json", skinTextBox1.Text);

                }
                newContent = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

            }
            FileStream fs = new FileStream(BotName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(newContent);
            sw.Close();
            Login_Load(sender, e);
          //  this.Close();
        }
        //Future
        //one : load to Master 
        //two : get Master ID
        //trhee : set Servant's Master ID
        private void Login_Load(object sender, EventArgs e)
        {

            skinListBox1.Items.Clear();
            foreach (string configFile in Directory.EnumerateFiles(".\\config", "*.json"))
            {
                string botName = Path.GetFileNameWithoutExtension(configFile);
                switch (botName)
                {
                    case "ASF":
                    case "example":
                    case "minimal":
                        continue;
                }
                CCWin.SkinControl.SkinListBoxItem item = new CCWin.SkinControl.SkinListBoxItem(configFile);
                skinListBox1.Items.Add(item);
                if (configFile.Contains("[Master]"))
                {
                    MasterId =  GetMasterId(configFile);
                }   
            }
            if (MasterId == "0")
            {
                MessageBox.Show("请先添加一个主号!");
            }
        }

        private string GetMasterId(string filename)
        {
            string newContent = string.Empty;
            string msid = "";
            using (StreamReader reader = new StreamReader(filename))
            {
                string json = reader.ReadToEnd();
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                msid = jsonObj["SteamMasterID"];
                newContent = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            }
            return msid;
        }

        private void skinLabel3_Click(object sender, EventArgs e)
        {

        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            if (skinListBox1.SelectedIndex != -1)
                skinListBox1.Items.RemoveAt(skinListBox1.SelectedIndex);
        }
    }
}
