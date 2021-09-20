using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AvaloniaCalculator.ViewModels {
    public class MainWindowViewModel : INotifyPropertyChanged{
        private string _textFieldText = "";
        public string Greeting => "Welcome to Avalonia!";
        public string FieldText {
            get => _textFieldText;
            set {
                _textFieldText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FieldText)));
            }
        }

        public void ButtonClicked(string input) => FieldText = input;
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}