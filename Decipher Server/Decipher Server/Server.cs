using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decipher_Server
{
    public sealed class Server
    {
        private bool IsRunning;

        private readonly object Sync = new object();

        private readonly IPEndPoint ClientEndPoint;

        private readonly Socket ClientListener;

        private readonly List<Client> Clients;

        private Task ListenerTask;

        private Thread thread_Backup = null;

        public static Dictionary<string, string> translationDic = new Dictionary<string, string>();
        internal void RemoveClient(Client client)
        {
            lock (Sync)
            {
                Clients.Remove(client);
            }
        }

        internal Client GetClient(Func<Client, bool> filter)
        {
            lock (Sync)
            {
                return Clients.FirstOrDefault((Client client) => client != null && filter(client));
            }
        }

        internal IEnumerable<Client> GetClients(Func<Client, bool> filter)
        {
            lock (Sync)
            {
                using (IEnumerator<Client> safeEnum = Clients.GetEnumerator())
                {
                    while (safeEnum.MoveNext())
                    {
                        if (safeEnum.Current != null && filter(safeEnum.Current))
                        {
                            yield return safeEnum.Current;
                        }
                    }
                }
            }
        }

        internal Server()
        {
            IsRunning = true;
            ClientEndPoint = new IPEndPoint(IPAddress.Any, 6969);
            ClientListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Clients = new List<Client>();
        }

        internal void Start()
        {
            ListenerTask = Task.Run((Func<Task>)Listen);
            StartTask();
        }

        public void StartTask()
        {
            thread_Backup = new Thread(new ThreadStart(Run_BackupTask));
            thread_Backup.IsBackground = true;
            thread_Backup.Start();
        }

        private void Run_BackupTask()
        {
            var backupFileTempVersion = "Temp\\" + "TextDicionary.dat";
            var backupFileDestination = "Backups\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-tt") + "\\TextDictionary.dat";
            var backupFileSource = "TextDictionary.dat";
            while (true)
            {
                //start your task

                if (File.Exists(backupFileTempVersion))
                {
                    //delete the temp version if it exists 
                    File.Delete(backupFileTempVersion);
                }

                //backup to a temp version
                File.Copy(backupFileSource, backupFileTempVersion);

                if (File.Exists(backupFileDestination))
                {
                    //always replace the old one with the new one
                    File.Delete(backupFileDestination);
                    File.Move(backupFileTempVersion, backupFileDestination);
                }

                if (!File.Exists(backupFileDestination))
                {
                    Directory.CreateDirectory("Backups\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-tt"));
                    File.Move(backupFileTempVersion, backupFileDestination);
                }

                Console.WriteLine("Backup Complete at " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-tt") + ".");
                //complete the backup task and wait for continue the next one
                Thread.Sleep(3600 * 1000);
                // set interval to 6 hr
            }
        }

        ~Server()
        {
            IsRunning = false;
        }

        private async Task Listen()
        {
            ClientListener.Bind(ClientEndPoint);
            ClientListener.Listen(15);
            while (IsRunning)
            {
                try
                {
                    Socket socket = await ClientListener.AcceptAsync();
                    lock (Sync)
                    {
                        Client item = new Client(this, socket);
                        Clients.Add(item);
                        Console.WriteLine("Client has connected.");
                    }
                }
                catch
                {
                    Console.WriteLine("Fail.");
                }
            }
        }
    }
}
