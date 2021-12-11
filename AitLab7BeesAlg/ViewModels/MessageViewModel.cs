using System;
using System.ComponentModel;
using System.Windows.Input;
using AitLab7BeesAlg.Commands;

namespace AitLab7BeesAlg.ViewModels
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        public string Message { get; set; } = String.Empty;

        public bool HasMessage => !String.IsNullOrEmpty(Message);

        public ICommand OkCommand => new ActionCommand(p => Message = String.Empty);
        
        
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}