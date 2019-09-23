using Doumi.PuppyServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Akorade
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        internal PuppyClient pup;

        public void AddTab(ClientTab clientTab)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    tabControl1.TabPages.Add(clientTab);
                });
            }
            else
            {
                tabControl1.TabPages.Add(clientTab);
            }
        }
        public void RemoveTab(ClientTab clientTab)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    clientTab.Dispose();
                });
            }
            else
            {
                clientTab.Dispose();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pup = new Doumi.PuppyServer.PuppyClient();
            _= pup.Connect();
        }
    }
}
