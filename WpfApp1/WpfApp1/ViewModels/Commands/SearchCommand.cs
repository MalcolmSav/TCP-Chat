﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.ViewModels;

namespace TDDD49Template.ViewModels.Commands
{
    class SearchCommand : ICommand
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
            Parent.SearchConversation();
        }

        public SearchCommand(MainViewModel parent)
        {
            this.Parent = parent;
        }
    }
}
