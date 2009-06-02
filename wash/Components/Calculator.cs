using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/********************8888
 * Calculator class
 * 
 * Provides an interface to a classic calculator that can evaluate a string by 
 * converting the expression to postfix and evalauting that string.
 * 
 * Methods:
 * verifyModifyCalcString - used to berify that the string can be calculated properly.  
 * 
 * 
 */
namespace Wash.Components
{
    class Calculator
    {  /****************
        * verifModifyCalcString
        * 
        * Takes the string to be calculated from the user.
        * 
        * Used to modify the string so that it can be caluclated properly.
        * 
        * 
        * 
        */ 
        private static string[] verifyModifyCalcString(string[] calcs)
        {
            List<string> temp_list = new List<string>();
            for (int i = 1; i < calcs.Length; i++)//skip #var or =.
            {
                float val;
                if (!float.TryParse(calcs[i], out val)) //calcs[i] is a string, and not a parseable value.
                {
                    if (calcs[i].Length > 1)
                    {
                        string broken = calcs[i];
                        for (int j = 0; j < broken.Length; j++)
                        {
                            temp_list.Add(""+broken[j]); //add the pieces of operators pushed together into a new entries in the list.
                        }
                    }
                }
                temp_list.Add(calcs[i]);
            }
            return temp_list.ToArray() ;
            
        }
        /*******************
         * calculate
         * 
         * Calculates a string and returns a float value.  Takes in an expression from the user.
         * 
         * Calls verifyModifyCalcString and infix2Postfix.
         * 
         */
        public static float calculate(string[] expression)
        {
            expression = verifyModifyCalcString(expression);
            Stack<string> operands = new Stack<string>();
            string pf_exp = infix2Postfix(expression);
            if (pf_exp.Equals("---")) { return 0; } //May not actually be used in production.
            pf_exp = pf_exp.Trim(); //infix2Postfix function adds an empty space at the end.
            string[] pf_exp_arr = pf_exp.Split();
            float opd1, opd2, res;
            for (int i = 0; i < pf_exp_arr.Length; i++)
            {
                string c = pf_exp_arr[i];
                switch (c)
                {
                    case ("+"): if (operands.Count != 0)
                               {
                                   opd1 = float.Parse(operands.Pop());
                                   opd2 = float.Parse(operands.Pop());
                                   res = opd1 + opd2;
                                   operands.Push(res.ToString());
                               }
                        break;
                    case ("-"): if (operands.Count != 0)
                        {
                             opd1 = float.Parse(operands.Pop());
                             opd2 = float.Parse(operands.Pop());
                              res = opd2 - opd1;
                            operands.Push(res.ToString());
                        }
                        break;
                    case ("*"): if (operands.Count != 0)
                        {
                            opd1 = float.Parse(operands.Pop());
                            opd2 = float.Parse(operands.Pop());
                            res = opd1 * opd2;
                            operands.Push(res.ToString());
                        }
                        break;
                    case ("/"): if (operands.Count != 0)
                        {
                            opd1 = float.Parse(operands.Pop());
                            opd2 = float.Parse(operands.Pop());
                            res = opd2/opd1;
                            operands.Push(res.ToString());
                        }
                        break;
                    case ("%"): if (operands.Count != 0)
                        {
                            opd1 = float.Parse(operands.Pop());
                            opd2 = float.Parse(operands.Pop());
                            res = opd2 % opd1;
                            operands.Push(res.ToString());
                        }
                        break;
                    default: float val;
                        if (float.TryParse(c, out val))
                        {
                            operands.Push(c);
                        }
                        break;

                }
            }
            float result = float.Parse(operands.Pop());


            return result;
        }

        /*************
         * infix2Postfix
         * 
         * Accepts a splitted expression that has been modified to work properly.
         * 
         * Turns an infix expression into a postfix expression that the calculator can work with.
         * 
         */
        private static string infix2Postfix(string[] expression)
        {
            Stack<string> operators = new Stack<string>(); //stack to use
            string result = ""; //the future postfix string.  
            for (int i = 0; i < expression.Length; i++)
            {
                string c = expression[i];

                if (c.StartsWith("@")) { Console.WriteLine(c + " " + GSR.Listing[43]); return "---"; }
                switch (c)
                {
                    case "=": break;
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                    case "%":
                    case "(":
                     while (operators.Count > 0 && !(operators.Peek().Equals('(') && !operators.Peek().Equals('^')) && higherPrecedence(c, operators.Peek()))
                        {
                            string d = operators.Pop();
                            result = result + " " + d;
                        }
                        if (operators.Count > 0 && operators.Peek().Equals('('))
                        {   operators.Pop();
                        }
                        else if (operators.Count > 0 && operators.Peek().Equals('^'))
                        {
                            result = result + " " + operators.Pop();
                        }
                        operators.Push(c);
                                break;
                    case ")": while (!(operators.Count == 0) && !operators.Peek().Equals("("))
                                {
                                    string d = operators.Pop();
                                    result = result + " " + d;
                                }
                                operators.Pop(); //throw away the (.
                                break;
                    default: if (!string.IsNullOrEmpty(c) && !c.Equals(" "))
                                {
                                    result = result + " " + c;
                                }
                                break;
                }
            }
            if (operators.Count > 0) //there are still some elements left on the stakc that need to be displayed.
            {
                while (operators.Count > 0) //pop them out and append them.
                {
                    string e = operators.Pop();
                    result = result + " " + e;
                }
            }
            return result;
        }
        /*******************************
         * 
         * higherPrecedence
         * 
         * Accepts two operators and returns whether or not the first operator has higher precedence then the second operator.
         * 
         * 
         * 
         */
        private static Boolean higherPrecedence(string op1, string op2)
        {
            if(op1.Equals(op2)) //they have the same precedence
            {
                return false;
            }
            else if (op1.Equals("^")) //^ has highest precedence.
            {
                return false;
            }
            else if (op1.Equals("*") || op1.Equals("/") || op1.Equals("%"))
            {
                if (op2.Equals("+") || op2.Equals("-")) //*, / or % have higher precedence then + or -.
                {
                    return true;
                }
                else { return false; }
            }
            else if (op1.Equals("+") || op1.Equals("-"))
            {
                return false; //+ or - have lowest precedence.
            }

            return false; //probably false for everything.
        }
    }
}
