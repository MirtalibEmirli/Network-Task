using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    public partial class MainWindow : Window
    {
        string sourcePath = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void img_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                sourcePath = openFileDialog.FileName;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {

                MessageBox.Show("Image Selected");
            });
        }
        void send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {

                    MessageBox.Show("Sending started");
                });
                using (FileStream source = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
                {

                    using var client = new TcpClient();
                    var ip1 = IPAddress.Parse(ip.Text);
                    var port1 = int.Parse(port.Text);
                    var endpoint = new IPEndPoint(ip1, port1);
                    client.Connect(endpoint);
                    NetworkStream networkStream = client.GetStream();
                    if (client.Connected)
                    {

                        try
                        {
                            int len = 23;
                            var bytes = new byte[len];
                            do
                            {
                                 source.Read(bytes, 0, len);
                                 networkStream.Write(bytes);

                            } while (len > 0);



                        }


                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                MessageBox.Show(ex.Message);

                            });
                        }
                        finally
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {

                                MessageBox.Show("Sending finished" );
                            });
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {

                            MessageBox.Show("Client cant connectt");
                        });
                    }




                }


            }


            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


    }
}