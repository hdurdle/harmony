using HarmonyHub;
using HarmonyHub.Entities.Response;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarmonyDemo
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            // ConnectAsync already if we have an existing session cookie
            if (File.Exists("SessionToken"))
            {

                buttonConnect.Enabled = false;
                try
                {
                    await ConnectAsync();
                }
                finally
                {
                    buttonConnect.Enabled = true;
                }
            }

        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            buttonConnect.Enabled = false;
            try
            {
                await ConnectAsync();
            }
            catch (Exception ex)
            {
                buttonConnect.Enabled = true;
            }
        }


        private async Task ConnectAsync()
        {
            toolStripStatusLabelConnection.Text = "Connecting... ";
            //First create our client and login
            if (File.Exists("SessionToken"))
            {
                var sessionToken = File.ReadAllText("SessionToken");
                Console.WriteLine("Reusing token: {0}", sessionToken);
                toolStripStatusLabelConnection.Text += $"Reusing token: {sessionToken}";
                Program.Client = HarmonyClient.Create(textBoxHarmonyHubAddress.Text, sessionToken);
            }
            else
            {
                if (string.IsNullOrEmpty(textBoxPassword.Text))
                {
                    toolStripStatusLabelConnection.Text = "Credentials missing!";
                    return;
                }

                toolStripStatusLabelConnection.Text += "authenticating with Logitech servers...";
                Program.Client = await HarmonyClient.Create(textBoxHarmonyHubAddress.Text, textBoxUserName.Text, textBoxPassword.Text);
                File.WriteAllText("SessionToken", Program.Client.Token);
            }

            toolStripStatusLabelConnection.Text = "Fetching Harmony Hub configuration...";

            //Fetch our config
            var harmonyConfig = await Program.Client.GetConfigAsync();
            PopulateTreeViewConfig(harmonyConfig);

            toolStripStatusLabelConnection.Text = "Ready";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aConfig"></param>
        private void PopulateTreeViewConfig(Config aConfig)
        {
            treeViewConfig.Nodes.Clear();
            //Add our devices
            foreach (Device device in aConfig.Devices)
            {
                TreeNode deviceNode = treeViewConfig.Nodes.Add(device.Id, $"{device.Label} ({device.DeviceTypeDisplayName}/{device.Model})");
                deviceNode.Tag = device;

                foreach (ControlGroup cg in device.ControlGroups)
                {
                    TreeNode cgNode = deviceNode.Nodes.Add(cg.Name);
                    cgNode.Tag = cg;

                    foreach (Function f in cg.Functions)
                    {
                        TreeNode fNode = cgNode.Nodes.Add(f.Name);
                        fNode.Tag = f;
                    }
                }
            }

            //treeViewConfig.ExpandAll();
        }

        private async void treeViewConfig_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //Upon function node double click we execute it
            var tag = e.Node.Tag as Function;
            if (tag != null && e.Node.Parent.Parent.Tag is Device)
            {
                Function f = tag;
                Device d = (Device)e.Node.Parent.Parent.Tag;

                toolStripStatusLabelConnection.Text = $"Sending {f.Name} to {d.Label}...";

                await Program.Client.SendCommandAsync(d.Id,f.Name);
            }
        }
    }
}
