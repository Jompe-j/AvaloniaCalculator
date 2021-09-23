using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace AvaloniaCalculator.ViewModels {
    public class MainWindowViewModel : INotifyPropertyChanged {
        private string _textFieldText = "";
        public string Greeting => "Welcome to Avalonia!";
        public string FieldText {
            get => _textFieldText;
            set {
                _textFieldText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FieldText)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void ButtonClicked(string input) => FieldText = EvaluatedExpressionString(input);

        private string EvaluatedExpressionString(string input) {
            var outputQueue = new Queue<string>();
            var operatorStack = new Stack<string>();
            var expression = FieldText + input;
            
            for (var i = 0; i < expression.Length; i++) {
                var c = expression[i];

                if (char.IsDigit(c)) {
                    outputQueue.Enqueue(c.ToString());
                }

                if (c is '+' or '-' or '*' or '/') {
                    while (operatorStack.Count > 0
                           && operatorStack.Peek() != "("
                           && Precedence(operatorStack.Peek()) > Precedence(c.ToString())) {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    operatorStack.Push(c.ToString());
                }

                if (c == '(') {
                    operatorStack.Push(c.ToString());
                }
                else if (c == ')') { //TODO Something is not working with the handling of the parenthesis 
                    while (operatorStack.Count != 0 && operatorStack.Peek() != "(") {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    if (operatorStack.Count != 0 && operatorStack.Peek() == ")") {
                        operatorStack.Pop();
                    }
                }
            }

            while (operatorStack.Count != 0) {
               outputQueue.Enqueue(operatorStack.Pop()); 
            }

            var test = "";
            foreach (var t in outputQueue) {
                test += t;
            }
            
            Debug.WriteLine(test);

            return FieldText + input;
        }

        private static int Precedence(string c) {
            return c is "-"or "+" ? 1 : 2;
        }
    }
}