using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Sentry;

namespace Akorade
{
    static class Program
    {
        public static mainForm Form;
        public static Server[] Servers = new Server[4];
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (SentrySdk.Init("https://b7a5a586a9b24b7ba044a5ae950f326a@sentry.io/1549168"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                mainForm main = new mainForm();
                Form = main;
                Servers[0] = new Server(new IPAddress(0x195565d2L), 0xa32, Form);
                Servers[1] = new Server(new IPAddress(0x195565d2L), 0x271b, Form);
                Servers[2] = new Server(new IPAddress(0x195565d2L), 0x2725, Form);
                Servers[3] = new Server(new IPAddress(0x195565d2L), 0x272F, Form);
                Server[] servers = Servers;
                for (int i = 0; i < servers.Length; i++)
                {
                    servers[i].Start();
                }
                Application.Run(main);
            }
        }
    }
}
