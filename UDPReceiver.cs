using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace FlashDataConverter
{
    public class UdpReceiver
    {
        private UdpClient _udpClient;
        private IPEndPoint remoteIpEndPoint = null;
        private int _port;

        private Thread _receiveThread;
        private bool _isReceiveThreadRunning = false;

        public UdpReceiver(int port)
        {
            _port = port;
            _udpClient = new UdpClient(port);
            _udpClient.Client.ReceiveTimeout = 500;
        }

        public void StartReceiveing()
        {
            if (!_isReceiveThreadRunning)
            {
                _isReceiveThreadRunning = true;
                _receiveThread = new Thread(ReceiveThreadFunction);
                _receiveThread.Start();
            }
        }
        public void StopReceiveing()
        {
            if (_isReceiveThreadRunning)
            {
                _isReceiveThreadRunning = false;
                _receiveThread.Join();                
            }
        }

        private void ReceiveThreadFunction()
        {
            const string startSymbol = "START";
            const string stopSymbol = "STOP";
            FileStream outFile = new FileStream("data.bin", FileMode.OpenOrCreate);
            bool isData = false;

            while (_isReceiveThreadRunning)
            {
                byte[] udpMessage;
                try
                {                    
                    udpMessage = _udpClient.Receive(ref remoteIpEndPoint);
                    if (isData)
                    {
                        outFile.Write(udpMessage, 0, udpMessage.Length);
                    }

                    string st = System.Text.Encoding.Default.GetString(udpMessage);
                    if (st == startSymbol)
                    {
                        isData = true;
                    }
                    else if(st == stopSymbol)
                    {
                        _isReceiveThreadRunning = false;
                        System.Windows.MessageBox.Show("All Data Received");
                        outFile.Close();
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Exception in UdpAdapterReceiver. Port: {0}, Message: {1}", e.Message, _port);
                }
            }
        }
    }
}
