using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
 

namespace Network_Task
{
     //network
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Start();
        }


        void Start()
        {
            var port = 27001;
            var ip = IPAddress.Parse("10.1.18.4");
            var endPoint = new IPEndPoint(ip, port);
            using var listener = new TcpListener(endPoint);
            listener.Start();
            MessageBox.Show($"Listening on: {listener.LocalEndpoint}");
            
            ////////
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var task = Task.Run(() =>
                {
                    Console.WriteLine($"{client.Client.RemoteEndPoint} connected");
                    var stream = client.GetStream();
                    var sr = new StreamReader(stream);
                    while (true)
                    {
                        var message = sr.ReadLine();
                        Console.WriteLine($"{client.Client.RemoteEndPoint}: {message}");
                    }
                });
            }
            }
    }
}