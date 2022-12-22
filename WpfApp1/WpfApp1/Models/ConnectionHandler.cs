using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TDDD49Template.ViewModels;
using System.Text.Json;

using TDDD49Template.Models;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace WpfApp1.Models
{
    public class ConnectionHandler : INotifyPropertyChanged
    {
        
   
        TcpClient client;
        TcpListener listener;
        NetworkStream stream;
        int port = 13000;
        string address = "127.0.0.1";
        string name = "namn";
        public string otheruser = "";
        public bool connected = false;
        bool listening = false;
        bool incomingConnection = false;
        bool disconnected = true;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public string Port  
        {
            get { return port.ToString(); }
            set { port = int.Parse(value); }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string OtherUser
        {
            get { return otheruser; }
            set { otheruser = value; }
        }

        public bool IncomingConnection
        {
            get { return incomingConnection; }
            set
            {
                incomingConnection = value;
                OnPropertyChanged("IncomingConnection");

            }
        }

        public bool Disconnected    
        {
            get { return disconnected; }
            set
            {
                OnPropertyChanged("Disconnected");
                CloseConnection();
            }
            
        }

        public bool Connected
        {
            get { return connected; }
            set 
            { 
                connected = value;
                OnPropertyChanged("Connected");
            }
        }

        private bool declined;
        public bool Declined
        {
            get { return declined; }
            set 
            { 
                declined = value;
                OnPropertyChanged("Declined");
            }
        }
        public JSONMessage SendMessage(String message)
        {
            JSONMessage jsonpack = new JSONMessage()
            {
                RequestType = "message",
                Date = DateTime.Now,
                UserName = name,
                Message = message
            };

            sendJSON(jsonpack);
            return jsonpack;
        }

        void sendJSON (JSONMessage jsonMessage)
        {
            var jsonpack = JsonSerializer.Serialize<JSONMessage>(jsonMessage);
            var data = System.Text.Encoding.ASCII.GetBytes(jsonpack);
            if(stream != null)
                stream.Write(data, 0, data.Length);
            
        }

        private JSONMessage recievedMessage;
        public JSONMessage RecievedMessage
        {
            get { return recievedMessage; }
            set
            {
                
                recievedMessage = value;
                if (otheruser == "")
                    OtherUser = recievedMessage.UserName;
                OnPropertyChanged("RecievedMessage");
            }
        }

        public void ConnectToListener()
        {
            try
            {

                client = new TcpClient(address, port);
                stream = client.GetStream();

                JSONMessage estasblishConnection = new JSONMessage()
                {
                    RequestType = "establishConnection",
                    Date = DateTime.Now,
                    UserName = name,
                    Message = ""
                };

                sendJSON(estasblishConnection);
            }
            catch (ArgumentNullException e)
            {
                otheruser = "";
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                connected = true;
                
                ListenForMessage();
            }
        }

        public void Startlistener()
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                
                listener.Start();

                client = listener.AcceptTcpClient();
                connected = true;
            }

            catch (SocketException e)
            {
                MessageBox.Show(e.Message   );

                otheruser = "";
                listener.Stop();
            }
            finally
            {
                listener.Stop();
                
            }
            ListenForMessage();
        }

        public void SendBuzz()
        {
            JSONMessage buzz = new JSONMessage()
            {
                RequestType = "buzz",
                Date = DateTime.Now,
                UserName = name,
                Message = ""
            };

            sendJSON(buzz);
        }

        public void AcceptConnection()
        {
            OtherUser = recievedMessage.UserName;
            connected = true;
            incomingConnection = false;
            JSONMessage msg = new JSONMessage()
            {
                RequestType = "acceptedConnection",
                Date = DateTime.Now,
                UserName = name,
                Message = ""
            };

            sendJSON(msg);
            ListenForMessage();
        }

        public void DeclineConnection()
        {
            
            JSONMessage disconnect = new JSONMessage()
            {
                    RequestType = "declineConnection",
                    Date = DateTime.Now,
                    UserName = name,
                    Message = ""
            };
            
            stream = client.GetStream();

            sendJSON(disconnect);

            CloseConnection();
        }


        private void ListenForMessage()
        {
            try
            {
                while (connected)
                {
                    if (client == null)
                        break;

                    var data = new Byte[256];
                    stream = client.GetStream();

                    Int32 bytes = stream.Read(data);

                    var message = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                    if (bytes > 0)
                    {
                        recievedMessage = JsonSerializer.Deserialize<JSONMessage>(message);

                        string type = recievedMessage.RequestType;
                        if (type == "message")
                        {
                            RecievedMessage = recievedMessage;
                        }
                        else if (type == "establishConnection")
                        {
                            otheruser = recievedMessage.UserName;
                            IncomingConnection = true;
                        }
                        else if (type == "declineConnection")
                        {
                            Declined = true;
                            break;
                        }
                        else if (type == "buzz")
                        {
                            OnPropertyChanged("buzz");
                        }
                        else if (type == "acceptedConnection")
                        {
                            Connected = true;
                        }

                        stream.Flush();
                    }

                }
            }
            catch
            { 
                App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    Disconnected = true;
                    CloseConnection();
                });
            
            }
        }

        public void DisconnectConnection()
        {
            CloseConnection();
        }

        private void CloseConnection()
        {
            if(client != null) 
                client.Close();
            connected = false;
            otheruser = "";
        }
    }
}
