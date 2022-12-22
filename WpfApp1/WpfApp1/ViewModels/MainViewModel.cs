using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TDDD49Template.ViewModels.Commands;
using WpfApp1.Models;
using WpfApp1.ViewModels.Commands;
using System.Threading;
using System.ComponentModel;
using TDDD49Template.ViewModels;
using System.Collections.ObjectModel;
using TDDD49Template.Models;
using System.Windows.Interop;
using System.Runtime.CompilerServices;
using System.IO;
using System.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfApp1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        
        private string _messageToSend;
        private string _changePort;
        private string _changeAddress;
        private string _name;
        private string searchText;
        private string status;

        private ICommand _listenerCommand;
        private ICommand _ClientCommand;
        private ICommand _sendMessageCommand;
        private ICommand _sendBuzzCommand;
        private ICommand _showConversation;
        private ICommand _searchCommand;

        private ConnectionHandler _connection;
        private Conversation _activeConvo = null;
        private JSONMessage _latestMessage;
        private Thread thread;
        private DataHandler dataHandler;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public ObservableCollection<JSONMessage>? ObservableMessage { get; set; }
        public List<Conversation>? OldConversations { get; set; }
        public ObservableCollection<Conversation>? ObservableSearchConvos { get; set; }
        public ObservableCollection<JSONMessage>? ObservableOldConversation { get; set; }
        public ConnectionHandler Connection
        {
            get { return _connection; }
            set { _connection = value; }

        }


        public string Status
        {
            get { return status; }
            set
            { 
                status = value;
                OnPropertyChanged();
            }
        }

        public string MessageToSend
        {
            get { return _messageToSend; }
            set { _messageToSend = value; }
        }

        public string Port
        {
            get { return _changePort; }
            set
            {
                _changePort = value;
                Connection.Port = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get { return _changeAddress; }
            set
            {
                _changeAddress = value;
                Connection.Address = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Connection.Name = value;
                OnPropertyChanged();
            }
        }

        public Conversation ActiveConvo
        {
            get { return _activeConvo; }
            set { _activeConvo = value; }
        }

        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; }
        }

        public DataHandler DataHandler
        {
            get { return dataHandler; }
            set { dataHandler = value; }
        }

        public ICommand ListenerCommand
        {
            get { return _listenerCommand; }
            set { _listenerCommand = value; }
        }

        public ICommand ClientCommand
        {
            get { return _ClientCommand; }
            set { _ClientCommand = value; }
        }

        public ICommand SendMessageCommand
        {
            get { return _sendMessageCommand; }
            set { _sendMessageCommand = value; }
        }

        public ICommand SendBuzzCommand
        {
            get { return _sendBuzzCommand; }
            set { _sendBuzzCommand = value;}
        }

        public ICommand ShowConversationCommand
        {
            get { return _showConversation; }
            set { _showConversation = value; }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand; }
            set { _searchCommand = value; }
        }

        public MainViewModel(ConnectionHandler connectionHandler)
        {
            if(!Directory.Exists("Conversations"))
                Directory.CreateDirectory("Conversations");
            
            Status = "Not Connected";
            this.DataHandler = new DataHandler();
            this.Connection = connectionHandler;
            this.ListenerCommand = new ListenerCommand(this);
            this.ClientCommand = new ClientCommand(this);
            this.SendMessageCommand = new SendMessageCommand(this);
            this.SendBuzzCommand = new SendBuzzCommand(this);
            this.ShowConversationCommand = new ShowConversationCommand(this);
            this.SearchCommand = new SearchCommand(this);
            this.ObservableMessage = new ObservableCollection<JSONMessage>();
            this.OldConversations = new List<Conversation>(GetHistory());
            this.ObservableOldConversation = new ObservableCollection<JSONMessage>();
            this.ObservableSearchConvos = new ObservableCollection<Conversation>(OldConversations);
            this.Connection.PropertyChanged += connections_PropertyChanged;
            Port = "13000";
            Address = "127.0.0.1";

            if (OldConversations.Count > 0)
            {
                ShowConversation(DataHandler.GetHistory().First());
            }
        }

        public void SendBuzz()
        {
            Connection.SendBuzz();
        }

        private List<Conversation> GetHistory()
        {
            return DataHandler.GetHistory();
        }
        public void SendMessage()
        {
            if(Connection.connected == false)
                return;
            JSONMessage msg = Connection.SendMessage(MessageToSend);
            WriteMessageOnScreen(msg);
        }

        
        public void ReceivedMessages()
        {
            _latestMessage = Connection.RecievedMessage;
            WriteMessageOnScreen(Connection.RecievedMessage);
        }

        public void Startlistener()
        {
            if (Connection.connected)
            {
                MessageBox.Show("Already Listening!");
                return;
            }

            Status = "Listening...";

            ObservableMessage.Clear();   
            
            thread = new Thread(new ThreadStart(Connection.Startlistener));
            thread.IsBackground = true;
            thread.Start();
        }

        public void ConnectToListener()
        {
           ObservableMessage.Clear();
           Status = "Connecting...";
           thread = new Thread(new ThreadStart(Connection.ConnectToListener));
           thread.IsBackground = true;
           thread.Start();

        }

        private void WriteMessageOnScreen(JSONMessage message)
        {
            
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                ObservableMessage.Add(message);
            });
            if (_activeConvo == null)
            {
                InitConvo(message);
            }
            else
            {
                UpdateConvo(message);
            }
        }

        private void InitConvo(JSONMessage msg)
        {
            _activeConvo = new Conversation(Connection.OtherUser, msg);
            DataHandler.InitConvo(_activeConvo);
        }

        private void UpdateConvo(JSONMessage msg)
        {
            _activeConvo.Messages.Add(msg);
            DataHandler.UpdateConvo(_activeConvo);
        }

        public void AcceptConnection()
        {
            thread = new Thread(new ThreadStart(Connection.AcceptConnection));
            thread.IsBackground = true;
            thread.Start();
            Status = "Connected";
        }

        public void DeclineConnection()
        {
            Connection.DeclineConnection();
            Status = "Not connected";
        }

        public void DisconnectConnection()
        {
            Status = "Not connected";

            Connection.DisconnectConnection();
        }

        public void ShowConversation(Conversation convo)
        {
            if (OldConversations.Count == 0)
                return;

            ObservableOldConversation.Clear();

            Conversation temp = convo;
            if (temp == null)
                return;
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                foreach (var msg in temp.Messages)
                {
                    ObservableOldConversation.Add(msg);
                }
            });

        }

        public void SearchConversation()
        {

            IEnumerable<Conversation> conversations = from convo in OldConversations
                                                      where convo.Name.ToUpper().Contains(SearchText.ToUpper())
                                                      orderby convo.Date descending
                                                      select convo;

            ObservableSearchConvos.Clear();

            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                foreach (Conversation convo in conversations)
                {
                    ObservableSearchConvos.Add(convo);
                }
            });

        }

        private void connections_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RecievedMessage")
            {
                ReceivedMessages();
                Status = "Connected";
            }
            else if (e.PropertyName == "IncomingConnection")
            {
                MessageBoxResult result = MessageBox.Show("Accept incoming connection from " + Connection.otheruser + "?", "Incoming Connection", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    AcceptConnection();
                }
                else
                    DeclineConnection();
            }
            else if (e.PropertyName == "Disconnected")
            {
                MessageBox.Show(Connection.otheruser + " disconnected");
                OldConversations = DataHandler.GetHistory();
                Status = "Not connected";
            }
            else if (e.PropertyName == "buzz")
            {
                SystemSounds.Beep.Play();
            }
            else if (e.PropertyName == "Declined")
            {
                MessageBox.Show("Listener declined your invitation");
                Status = "Not connected";
            }
            else if (e.PropertyName == "Connected")
            {
                Status = "Connected";
            }
        }
    }
}
