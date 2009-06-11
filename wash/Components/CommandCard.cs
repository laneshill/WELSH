using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

/*CommandCard class
 * Written by Lane Hill
 * 
 * CommandCards solve a problem when it comes to linking commands together.
 * 
 * Instead of having the split token array to deal with commands, instead you use a CommandCard.
 * The output of a command will be placed in the command card itself, so that if another card has an argument
 * that is the result of another card, say the command:
 * 
 * filter "Dreams" from ls
 *
 * Then filter needs the output of ls to run filter on.
 * 
 * Using an array, that'd be complex, but with CommandCards, its simple and it'll probably even be quicker.
 * 
 * Methods:
 * constructor (takes in a string (the central command) and an array of strings (the parsed string).
 * createDeck (creates a deck of cards based on a parsed string)
 * replaceVariables (replaces variables based on the tag of the variables.)
 * 
 * Fields:
 * Command (string)
 * Arguments (string[]) 
 * Output (string)
 * PUT_OUTPUT_IN_FILE (bool)
 * OUTPUT_FILE (string)
 * PUT_OUTPUT_IN_VARIABLE (bool, not used)
 * OUTPUT_VARIABLE (string, not used)
 * 
 */

namespace Wash.Components
{
    class CommandCard
    {
        private string command; //this is the central command.
        public string Command{
            get{return command;}
            set { command = value; }
        }

        private string[] arguments; //this is the argument array.
        public string[] Arguments{
            get{return arguments;}
            set{arguments = value;}
        }
        private string output; //output, which isn't updated unless there is a card in the deck.
        public string Output
        {
            get { return output; }
            set { output = value; }
        }
        private bool work_in_background; //works in the background, no output.
        public bool WORK_BACKGROUND
        {
            get { return work_in_background; }
            set { work_in_background = value; }
        }
        private bool dump_output_in_file; //dumps output in file.
        public bool PUT_OUTPUT_IN_FILE
        {
            get { return dump_output_in_file; }
            set { dump_output_in_file = value; }
        }
        private string output_file; //the file to dump output, if any.
        public string OUTPUT_FILE
        { //file to dump output.
            get { return output_file; }
            set { output_file = value; }

        }
        private bool dump_output_in_variable;
        public bool PUT_OUTPUT_IN_VARIABLE
        {
            get { return dump_output_in_variable; }
            set { dump_output_in_variable = value; }
        }

        private string output_variable;
        public string OUTPUT_VARIABLE
        {
            get { return output_variable; }
            set { output_variable = value; }
        }



        public CommandCard(string c_command, string[] args) //constructor
        {
            Command = c_command;
            Arguments = args;
        }

        /**********
         * createDeck
         * 
         * A big and ugly, but important method that returns an array of CommandCards based on the 
         *inputted, splitted string.  
         *
         * Takes in the parsed string, the File System Glob, the Alias Table, and the variable dictionaries.
         * 
         * Will probably be modified in the next revision to make the function much more smaller.
         * 
         * If we hit the special cases, we return a command card with "var" as the c_central and no arguments.
         * 
         */
        public static CommandCard[] createDeck(string[] parsed, FileSystemGlob fsg, AliasTable at, Dictionary<String, float> intvar, Dictionary<String, String> strvar)
        {

            CommandCard[] dft = new CommandCard[1];
            dft[0] = new CommandCard("var", null);
            bool from_test = false; //sets to true when a "from" or a "|", so that the from event is silenced.
            bool background = false; //set to true if the command works in background.
            string output_file = ""; //the output_file of a command.
            bool output_file_bool = false; //whether or not the file needs an output file.
            bool output_variable_bool = false;
            string output_variable = ""; //user wants to output into a variable.
            List<String> arg = new List<String>();
            List<CommandCard> deck = new List<CommandCard>();
            int i = 0;
            /* We may need to split this section into different functions
             */
            if (parsed[0].Contains("http://"))
            {
                Process p = Process.Start(parsed[0]);
                //Debug: dft[0].Command = "l0";
            }
            else if (parsed[0].Contains(".exe") || parsed[0].Contains(".bin"))
            {
                Process p = Process.Start(parsed[0]);
                //Debug: dft[0].Command = "l0";
            }
            else if (fsg.Current_Children.Contains(parsed[0]))
            {
                Process p = Process.Start(fsg.Current_Pointer + parsed[0]);
                //Debug: dft[0].Command = "l0";
            }
            else if (parsed[0].Contains("#") && parsed.Length < 3) //check if the command entered was a variable declaration.
            {
                string variable = parsed[0].Substring(1); //the actual variable name.
                if (parsed.Length == 3) //3 is the number of tokens in the statement #int = 3
                {
                    int value;
                    if (Int32.TryParse(parsed[2], out value))
                    {
                        try { intvar.Add(variable, value); }
                        catch (Exception) { intvar.Remove(variable); intvar.Add(variable, value); }
                    }
                    else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[2]); Console.ForegroundColor = ConsoleColor.Gray; } //Listing[2] refers to a Format mismatch error.
                }
                else if (parsed.Length == 1)//if they typed in the name of the variable, we return the value of the variable.
                {
                    float value;
                    if (intvar.TryGetValue(variable, out value))
                    {
                        Console.WriteLine(value);
                    }
                    else { Console.WriteLine(GSR.Listing[3] + " " + variable + " " + GSR.Listing[4]); } //Listing[3] & [4] represent the declaration error.
                    //  dft[0].Command = "l0";
                }
            }
            else if (parsed[0].Contains("@")) //check if the command entered was a variable declaration in the form of @string = var.
            {
                string variable = parsed[0].Substring(1);
                if (parsed.Length >= 3)
                {
                    string str = "";
                    for (int j = 2; j < parsed.Length; j++) //this will break things in the future, probably.
                    {
                        str = str + " " + parsed[j]; //connect strings that might've had spaces, which would break up the string.
                    }
                    try { strvar.Add(variable, str); }
                    //if there is a variable already declared, we simply remove it, and add it again.
                    catch (Exception) { strvar.Remove(variable); strvar.Add(variable, str); }
                }
                else if (parsed.Length == 1)//if they typed in the name of the variable, we return the value of the variable.
                {
                    string value;
                    if (strvar.TryGetValue(variable, out value))
                    {
                        Console.WriteLine(value.Trim());
                    }
                    else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[3] + " " + variable + " " + GSR.Listing[4]); Console.ForegroundColor = ConsoleColor.Gray; } //Listing[3] and Listing[4] represent the variable declaration error.
                }
                //dft[0].Command = "l0";
            }
                else if (parsed.Contains("="))
                {
                    string left_variable = "";
                    //replace all integer variables
                    for (int ci = 0; ci < parsed.Length; ci++)
                    {
                        if (parsed[ci].Contains("#") && !(ci == 0))
                        {
                            string p2 = replaceVariables(parsed[ci], intvar, strvar);
                            if (!String.IsNullOrEmpty(p2)) { parsed[ci] = p2; }
                            else { return null; }
                        }
                    }
                    if (!parsed[0].Equals("="))
                    {
                        left_variable = parsed[0];
                        string[] new_parsed = new string[parsed.Length - 1];
                        for (int k = 1; k < parsed.Length; k++)
                        {
                            new_parsed[k - 1] = parsed[k];
                        }
                        parsed = new_parsed;
                    }
                        float result = Calculator.calculate(parsed);
                        //if there is a variable on the right side of =, then update a variable with that value.
                        //otherwise, write it to console.
                        //if left_variable is empty, then we do not have to do an assignment.
                        if (!String.IsNullOrEmpty(left_variable))
                        {
                            string vari = left_variable.Substring(1);

                            try { intvar.Add(vari, (int)result); }
                            catch (Exception) { intvar.Remove(vari); intvar.Add(vari, (int)result); }
                            //the catch function removes the existing var
                        }
                        else { if(!float.IsNaN(result)) Console.WriteLine(result); } 
                }
                else
                {
                    while (i < parsed.Length)
                    {
                        AliasEntry ae = at.search(parsed[i]);
                        if (ae == null) { return null; } //the command has not been found.  Program will deal with reporting the error.
                        string c_control = ae.Central_Command; //gets the Central_command from a AliasEntry.
                        i++;
                        if (i < parsed.Length)
                        {
                            while (i < parsed.Length && (!parsed[i].Equals(GSR.Listing[5]) || !parsed[i].Equals("|")))
                            { //loop ends when it reaches a "from" or a "|", or there is no 
                                if (parsed[i].Equals("&"))
                                { //command works in background.
                                    background = true; i++;
                                }
                                else if (parsed[i].Equals("<"))
                                { //command has an output file.
                                    if (parsed[i + 1].StartsWith("#") || parsed[i + 1].StartsWith("@"))
                                    {
                                        output_variable_bool = true;
                                        output_variable = parsed[i + 1];
                                    }
                                    else
                                    {
                                        output_file_bool = true;
                                        output_file = parsed[i + 1];
                                        i = i + 2; //skip past this block.,
                                    }
                                }
                                else if ((parsed[i].Contains("#") || parsed[i].Contains("@")) && !(parsed[i].Equals("#OUT#"))) 
                                {//if it enters here, then we have a variable.
                                    
                                    string p = replaceVariables(parsed[i], intvar, strvar);
                                    if (!String.IsNullOrEmpty(p)) { parsed[i] = p; }
                                    else { return null; }

                                }
                                else
                                { //if it enters here, we do not have any special cases to check for, and we can get the arguments.
                                    string new_arg = "";
                                    if (parsed[i].StartsWith("\"")) //parsed[i] is possibly a file, that has a space in the middle.
                                    {  
                                        new_arg = parsed[i].Substring(1); //removes the " character.
                                        for (int j = i+1; j < parsed.Length; j++)
                                        {
                                            if (!parsed[j].Contains("\""))
                                            {
                                                new_arg = new_arg + " " + parsed[j];
                                                i = j; //we still need to keep track of the quoatations.
                                            }
                                            else { break; }
                                            if (j > 100) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[24]); Console.ForegroundColor = ConsoleColor.Gray; return dft; } //Format mismatch.
                                        }
                                        new_arg = new_arg + " " + parsed[i+1].Substring(0, parsed[i+1].Length - 1);
                                        arg.Add(new_arg);
                                        i=i+2;
                                    }
                                    else
                                    {
                                        arg.Add(parsed[i]);
                                        i++;
                                    }
                                    
                                }
                                //GSR.Listing[5] is the translation for "From".
                                if (i < parsed.Length && ((parsed[i].Equals(GSR.Listing[5]) || parsed[i].Equals("|"))))
                                {
                                    from_test = true;
                                    arg.Add("#OUT#");  //#OUT# is a special hash code that means that the function refers to the output of another command in a chain.
                                    i++;
                                }

                            }


                        }
                        string[] args = arg.ToArray();

                        CommandCard temp = new CommandCard(c_control, args);
                        temp.WORK_BACKGROUND = background; //set WORK_BACKGROUND based on arguments.
                        temp.OUTPUT_FILE = output_file;  //where output should go if any.
                        temp.PUT_OUTPUT_IN_FILE = output_file_bool; //set PUT_OUTPUT_IN_FILE based on arguments.
                        temp.PUT_OUTPUT_IN_VARIABLE = output_variable_bool;
                        temp.OUTPUT_VARIABLE = output_variable;
                        
                        deck.Add(temp);
                        background = from_test; //the next command should be silenced.

                    }
                    deck.Reverse(); //reverse the deck as the execution order will be backwards from the order that is typed in.
                    CommandCard[] new_deck = deck.ToArray();
                    return new_deck;

                }
             return dft; //if we hit the special cases, we return a command card with "var" as the c_central and no arguments.

            }
           
        /******************
         * replaceVariables
         * 
         * Replace Variables in a string based on their key.
         */
        private static string replaceVariables(string var, Dictionary<String, float> intvar, Dictionary<String,String> strvar)
        {
            string var_name = var.Substring(1);
            float val;
            string s_val;

            if (var.StartsWith("#"))
            {
                if (intvar.TryGetValue(var_name, out val))
                {
                    var = val.ToString();
                    return var;
                }
                else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[3] + " " + var_name + " " + GSR.Listing[4]); Console.ForegroundColor = ConsoleColor.Gray; } //Listing[3] and Listing[4] represent the variable declaration error.
            }
            else if (var.StartsWith("@"))
            {
                if (strvar.TryGetValue(var_name, out s_val))
                {
                    var = s_val;
                    return s_val;
                }
                else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[3] + " " + var_name + " " + GSR.Listing[4]); Console.ForegroundColor = ConsoleColor.Gray; }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red; 
                Console.WriteLine(GSR.Listing[3] + " " + var_name + " " + GSR.Listing[4]);
                Console.ForegroundColor = ConsoleColor.Gray; //Listing[3] and Listing[4] represent the variable declaration error.
             
            }
            return null;
        }
    }
}
