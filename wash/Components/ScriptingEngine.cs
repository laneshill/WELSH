using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/*********************
 * WISE Scripting Engine
 * Created by Lane S. Hill
 * 
 * Allows the user to supply scripts to run under WELSH.
 * 
 * Fields:
 * Pointer_Position (current position in the script)
 * Label_Dict (Dictionary of Label positions)
 * Script_Path (Location of the script)
 * Script_Array (The entire script in an array deliminated on lines)
 * 
 * Methods:
 * constructor
 * run (static) - runs the scripts
 * 
 * 
 */
namespace Wash.Components
{
    //scripting engine - in charge of executing scripts.
    class ScriptingEngine
    {
        private int pos; //position in the script.
        public int Pointer_Position
        {
            get { return pos; }
            set { pos = value; }
        }

        private Dictionary<String, Int32> label = new Dictionary<string, int>(); //dictionary for gotos and labels.
        public Dictionary<String, Int32> Label_Dict
        {
            get { return label; }
            set { label = value; }
        }        
        private string script; //file path of script
        public string Script_Path
        {
            get { return script; }
            set { script = value; }
        }

        private string[] script_array; //array of statements.
        public string[] Script_Array
        {
            get { return script_array; }
            set { script_array = value; }
        }

        public ScriptingEngine(string file_path)
        {
            script = file_path;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(script))
            {
                script_array = sr.ReadToEnd().Split('\n'); //get the contents into the file into a string, and then split the giant string into an array.
            }
            pos = 0;
        }

        /**************
         * run
         * 
         * Runs a script.  Requires the FileSystemGlob, the AliasTable, and the variable dictionaries.
         */
        public void run(FileSystemGlob fsg, AliasTable at, Dictionary<String, float> intvar, Dictionary<String,String> strvar)
        {
            int i = 0;
            int p_store = 0; //pointer store for jumping to labels.
            while(i < Script_Array.Length) //first pass - grab the methods & labels.
            {
                string cmds = Script_Array[i];
                if (cmds.Contains("method")) //we are defining a label.
                {
                    string[] label_declaration = cmds.Split();
                    try { Label_Dict.Add(label_declaration[1].Trim(), Pointer_Position); }
                    catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[35]); Console.ForegroundColor = ConsoleColor.Gray; }
                    for(int j = i; j < Script_Array.Length; j++)//cycle until we get past end.
                    {
                        i++;
                        if(Script_Array[j].Contains("end"))
                        {
                            break;
                        }
                    }
                }
                else if (cmds.Contains("label")) //we are defining a label.
                {
                    string[] label_declaration = cmds.Split();
                    try { Label_Dict.Add(label_declaration[1].Trim(), Pointer_Position); }
                    catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[35]); Console.ForegroundColor = ConsoleColor.Gray; }
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while(i<Script_Array.Length) //second pass - start execution
            {
                string cmds = Script_Array[i];
                if(cmds.Contains("end"))
                {
                    Pointer_Position = p_store;
                    i = p_store + 1;
                }
                else if(cmds.Contains("goto"))
                {
                    p_store = i;
                    string[] label_goto = cmds.Split();
                    int spos; //new script position
                    if (Label_Dict.TryGetValue(label_goto[1], out spos))
                    {
                        i = spos+1; //spos will be where the label declaration is - we want the declaration afterwards.
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine( GSR.Listing[32]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                else if(String.IsNullOrEmpty(cmds) || cmds.Equals(' ') || cmds.Equals("\r") || cmds.StartsWith("//"))
                {
                    //do nothing but incremate.
                    i++;
                }
                else if (cmds.Contains("method"))
                {
                    while (!cmds.Contains("end"))
                    {
                        i++;
                        cmds = Script_Array[i];
                    }
                    i++; //incremate one after end to avoid odd loop problems.
                }
                else
                {
                    string[] commands = cmds.Trim().Split();
                    CommandCard[] deck = CommandCard.createDeck(commands, fsg, at, intvar, strvar);
                    if (deck == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(GSR.Listing[9]); //this is the Command Not Found error.
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Executer.execute(deck, at, fsg, strvar, intvar);
                    }
                    i++;
                }


                Pointer_Position = i;
                }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("-");
            }
        }
    }

