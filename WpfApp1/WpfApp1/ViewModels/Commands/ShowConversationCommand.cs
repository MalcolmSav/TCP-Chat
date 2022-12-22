using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TDDD49Template.Models;
using WpfApp1.ViewModels;

namespace TDDD49Template.ViewModels.Commands
{
    class ShowConversationCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private MainViewModel _parent;

        public MainViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            //MessageBox.Show(parameter.ToString());
            if(parameter != null)
                Parent.ShowConversation((Conversation)parameter);
        }

        public ShowConversationCommand(MainViewModel parent)
        {

            this.Parent = parent;
        }
    }
}
