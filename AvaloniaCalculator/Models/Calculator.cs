using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace AvaloniaCalculator.Models {
    public class Calculator {
        public string ParseString(string exp, string input) {
            var outputQueue = new Queue<string>();
            var operatorStack = new Stack<string>();
            var expression = exp + input;

            for (var i = 0; i < expression.Length; i++) {
                var c = expression[i];

                // Check for digit input
                if (char.IsDigit(c)) {
                    var d = c.ToString();

                    for (var j = i + 1; j < expression.Length - 1 && char.IsDigit(expression[j]); j++) {
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
                else if (c == ')') {
                    //TODO Something is not working with the handling of the parenthesis 
                    while (operatorStack.Count != 0 && operatorStack.Peek() != "(") {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    // Handle mismatched parenthesis 
                    if (operatorStack.Count == 0) {
                        expression = expression[..^1];
                    }

                    if (operatorStack.Count != 0 && operatorStack.Peek() == "(") {
                        operatorStack.Pop();
                    }

                }
            }

            // Empty stack
            while (operatorStack.Count != 0) {
                outputQueue.Enqueue(operatorStack.Pop());
            }

            try {
                if (input == "=") {
                    expression = Evaluate(outputQueue);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }

            var test = "";

            foreach (var t in outputQueue) {
                test += t + "|";
            }

            Debug.WriteLine(test);

            return expression;
        }

        private string Evaluate(Queue<string> outputQueue) {
            var resultStack = new Stack<double>();

            foreach (var token in outputQueue) {
                if (Precedence(token) != 0) {
                    //Not a god solution but works for now
                    var operand2 = resultStack.Pop();
                    var operand1 = resultStack.Pop();
                    resultStack.Push(calc(token, operand1, operand2));
                }
                else {
                    resultStack.Push(double.Parse(token));
                }
            }

            return resultStack.Pop().ToString(CultureInfo.InvariantCulture);
        }

        private double calc(string token, double operand1, double operand2) {
            return token switch {
                "+" => operand1 + operand2,
                "-" => operand1 - operand2,
                "*" => operand1 * operand2,
                "/" => operand1 / operand2,
                "^" => Math.Pow(operand1, operand2),
                _ => 0
            };
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
                    return 0;
            }
        }
    }
}