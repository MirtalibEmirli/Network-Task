using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Network_Task
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<BitmapImage> ImageCollection { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            ImageCollection = new ObservableCollection<BitmapImage>();
            StartServer();
        }

        private void StartServer()
        {
            Task.Run(() =>
            {
                try
                {
                    var port = 27001;
                    var ip = IPAddress.Parse("192.168.1.105");
                    var endPoint = new IPEndPoint(ip, port);
                    using var listener = new TcpListener(endPoint);
                    listener.Start();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Listening on: {listener.LocalEndpoint}");
                    });

                    while (true)
                    {
                       //bura ancaq gelen sorgulari qebul edmek ucun tekrar edir ve isdyr
                        var client = listener.AcceptTcpClient();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"{client.Client.RemoteEndPoint} connected");
                        });

                        var stream = client.GetStream();
                        var bytes = new byte[1024];
                        int bytesRead;
                        //artiq bizde client teref var ve streamde var qalir clientden gelen byte array i qebul edmek ve yazmaq

                        using (var memoryStream = new MemoryStream())
                        {
                            //bu while array i oxuyur streamdan nve bize verir
                            while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) > 0)
                            {
                                memoryStream.Write(bytes, 0, bytesRead);
                            }

                            var imageData = memoryStream.ToArray();
                            var len = imageData.Length; 
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                            var bitmapImage = ByteArrayToImage(imageData);
                                    File.WriteAllBytes($"image{Guid.NewGuid()}.jpg", imageData);
                                    ImageCollection.Add(bitmapImage);
                                    MessageBox.Show("Image saved and showed ");
                                }
                                catch (Exception ex)
                                {

                                    MessageBox.Show(ex.Message);
                                }

                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message);
                    });
                }
            });
        }

        private BitmapImage ByteArrayToImage(byte[] byteArray)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
