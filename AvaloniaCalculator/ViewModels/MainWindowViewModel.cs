using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace AvaloniaCalculator.ViewModels {
    public class MainWindowViewModel : INotifyPropertyChanged {
        private string _textFieldText = "";
        private bool _unchanged;
        public string FieldText {
            get => _textFieldText;
            set {
                if (!_unchanged) {
                    _textFieldText = value;
                }
                else {
                    _textFieldText = value[1].ToString();
                    _unchanged = false;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FieldText)));
            }
        }

        public MainWindowViewModel() {
            ClearCalculator();
        }

        private void Backspace() {
            FieldText = FieldText[..^1];
        }
        
        

        private void ClearCalculator() {
            FieldText = "0";
            _unchanged = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void ButtonClicked(string input) => FieldText = EvaluatedExpressionString(input);

        private string EvaluatedExpressionString(string input) {
            var outputQueue = new Queue<string>();
            var operatorStack = new Stack<string>();
            var expression = FieldText + input;
            
            for (var i = 0; i < expression.Length; i++) {
                var c = expression[i];
                
                // Check for digit input
                if (char.IsDigit(c)) {
                    var d = c.ToString(); 
                    for (var j = i + 1; j < expression.Length -1 && char.IsDigit(expression[j]); j++) {
                        d += expression[j].ToString();
                        i = j;
                    }
                    outputQueue.Enqueue(d);
                }
                
                // Check for operator input
                if (c is '+' or '-' or '*' or '/' or '^') {
                    while (operatorStack.Count > 0
                           && operatorStack.Peek() != "("
                           && (Precedence(operatorStack.Peek()) > Precedence(c.ToString()) ||
                           Precedence(operatorStack.Peek()) == Precedence(c.ToString()) && c != '^')) {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    operatorStack.Push(c.ToString());
                }
                
                // Handle Parenthesis 
                if (c == '(') {
                    operatorStack.Push(c.ToString());
                }
                else if (c == ')') { //TODO Something is not working with the handling of the parenthesis 
                    while (operatorStack.Count != 0 && operatorStack.Peek() != "(") {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    if (operatorStack.Count == 0) {
                       expression = expression[..^1];
                    } 

                    if (operatorStack.Count != 0 && operatorStack.Peek() == "(") {
                        operatorStack.Pop();
                    }
                    break;
                    
                }
            }
            // Empty stack
            while (operatorStack.Count != 0) {
               outputQueue.Enqueue(operatorStack.Pop()); 
            }

            var test = "";
            foreach (var t in outputQueue) {
                test += t + "|";
            }
           
            Debug.WriteLine(test);
            return expression;
        }

        private static int Precedence(string c) {
            switch (c) {
                case "+":
                case "-":
                    return 2;
                case "*":
                case "/":
                    return 3;
                case "^":
                    return 4;
                default:
                    return 4;
            }
        }
    }
}