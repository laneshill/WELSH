using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
/***************************
 * Executer
 * Created by Lane S. Hill
 * 
 * 
 * Responsible for executing all built in commands.
 * 
 * Fields: 
 * deck - the deck of CommandCards that were passed to the Executer.
 * 
 * Methods:
 * output
 * execute
 * change_directory
 * listFilesDirectories
 * filter
 * save_cariable_dictionaries
 * view_file_info
 * go_back
 * go_forward
 * go_up
 * variable_list
 * process_list
 * kill_process
 * removeFileOrFolder
 * newFileOrFolder
 * addToFile
 * executeScript
 * exploreDirectory
 * protectFileDirectory
 * searchWiki
 * googleIt
 * copyFile
 * aliasFile
 * moveFile
 * echoString
 * time
 * 
 */
namespace Wash.Components
{
    class Executer
    {
        static private CommandCard[] deck;
        /***************
         * output
         * 
         * Takes in a CommandCard and a string, and outputs the string according to the Command Card.
         */
        public static void output(CommandCard card, string output)
        {
            if (card.PUT_OUTPUT_IN_FILE) //write to file
            {
                using (StreamWriter sw = new StreamWriter(card.OUTPUT_FILE))
                {
                    sw.WriteLine(output);
                }
            }

            else if (!card.WORK_BACKGROUND) //if it's not supposed to work in background, then display in console.
            {
                Console.WriteLine(output);
            }
            card.Output = card.Output + output;//write output to output.
        }
        /*****************
         * 
         * execute
         * 
         * Accepts a deck of CommandCards, the AliasTable, the FileSystemGlob, the two variable dictionaries.
         * 
         * Executes a built in method based on the command on the CommandCard.
         */
        public static void execute(CommandCard[] cards, AliasTable at, FileSystemGlob fsg, Dictionary<string, string> sd, Dictionary<string, float> id)
        {
            Executer.deck = cards;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].Command.Contains(":\\")) //this is not a built in method, but a path.
                {
                    try
                    {
                        Process.Start(cards[i].Command);
                    }
                    catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[1] + e.Message); Console.ForegroundColor = ConsoleColor.Gray; }
                    break;
                }
                switch (cards[i].Command)
                {
                    case "view": viewFile(cards[i], fsg);
                        break; //view a file.
                    case "cd": change_directory(cards[i], fsg, i);
                        break;  //change directory
                    case "ls": listFilesDirectories(cards[i], fsg, i);
                        break; //list elements in directory.
                    case "save": save_variable_dictionaries(cards[i], i, sd, id);
                        break; //saves both dictionaries to file.
                    //v.2: case "cs": break; //open a sever connection.
                    //v2 :case "con": break; //connect to a server.
                    case "info": view_file_info(cards[i], i, fsg);
                        break; //view information about the file
                    case "proc": process_list(cards[i]);
                        break; //view processes
                    case "kill": kill_process(cards[i]);
                        break; //kill a process
                    case "rm": removeFileOrFolder(cards[i], fsg);
                        break; //remove a folder or file
                    case "new": newFileOrFolder(cards[i], fsg);
                        break; //create a new folder or file
                    case "add": addToFile(cards[i], fsg, i);
                        break; //add a text to a file.
                    //          v.2        case "conf": break; //look at and edit the configuration file.
                    case "sudo": break; //elevate the shell.
                    case "exec": executeScripts(cards[i], fsg, at, id, sd);
                        break; //execute a script.
                    //v.3 or v.4: case "import": break; //import a script/modue.
                    case "copy": copyFile(cards[i], fsg);
                        break; //only copy a file (in the way of xeroxing)
                    case "bck": go_back(fsg);
                        break; //go back
                    case "fwd": go_forward(fsg);
                        break; //go forward
                    case "up": go_up(fsg);
                        break; //go up a level.
                    case "clr": Console.Clear();
                        break; //clear screen.
                    
                    //v.2: case "attr": break; //change attributes 
                    case "filter": filter_list(cards[i], i);
                        break; //filter strings.
                    case "vars": variable_list(cards[i], sd, id);
                        break; //create a list of variables.
                    case "explore": exploreDirectory(cards[i], fsg);
                        break; //opens up explorer on the argument.
                    case "move": moveFile(cards[i], fsg);
                                break; //moves a file (aka copy and paste).
                    case "protect": protectFileDirectory(cards[i], fsg);
                        break; //add a file to the protected file list.
                    case "alias": aliasFile(cards[i], fsg, at);
                        break; //adds a new alias to the alias file.
                    case "google": googleIt(cards[i]);
                        break; //searches google for the word supplied - Google opens in a Web Browser.
                    case "wiki": searchWiki(cards[i]);
                        break; //searches Wikipedia for the word supplied - Wikipedia opens in a Web Browser.
                    case "echo": echoString(cards[i], i);
                                break; //echos the string to output.
                    case "time": displayTime(cards[i], i);
                                break;
                }
            }

        }

        /*******************
         * change_directory
         * 
         * Changes the directory that is currently used.  
         * 
         * If the function does not work, it returns false.  This feature is not used for anything.
         * 
         */ 

        private static bool change_directory(CommandCard card, FileSystemGlob fsg, int deck_position)
        {
            string[] args = card.Arguments;
            if (args.Length == 0) { Console.ForegroundColor = ConsoleColor.Cyan; Executer.output(card, GSR.Listing[45] + " " + fsg.Current_Pointer); Console.ForegroundColor = ConsoleColor.Cyan; return true; }
            string dir = "";
            //Checking if there is a space in the path.  Because we tokenized it, spaces are in a new part of args.
            if (args.Length > 1)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    if (i >= 2) //we don't want spaces every other place.
                    {
                        dir = dir + " " + args[i]; //everything after the first cell needs one.
                    }
                    else { dir = dir + args[i]; } //first cell of args will not need a space before the cell.
                }
            }
            else
            {
                dir = args[0];
            }
            if (!dir.Contains(":\\")) //Folder is not root.  Important, because it allows us not to worry about having slashes at the beginning of relative roots.
            {
                dir = fsg.Current_Pointer + dir + "\\";
            }
            if (args[0].Equals("..")) //we are supposed to go up one level.
            {
                dir = System.IO.Path.GetDirectoryName(fsg.Current_Pointer);
                if (dir.Equals("")) //GetDirectoryName returns an empty value string when you are a level away from the root.
                {
                    dir = System.IO.Path.GetPathRoot(fsg.Current_Pointer);
                }

            }
            if (args[0].Equals("#OUT#"))
            {//if this command relies on the result of a chained command, then get the position
                //of the previous card.  Please note that this might be fatal if the directory has
                //has spaces.
                dir = deck[deck_position - 1].Output;
            }

            bool check = fsg.changeLocation(dir);
            return check;
        }

        /**************************
         * 
         * listFilesDirectories
         * 
         * Lists files and directories in a given directory or the current directory.
         * 
         * Arguments:
         * -i List all information.
         * -a list all attributes
         * -c list control information
         * -e list creation time
         * -r list write time
         * -s list access time
         */
        private static Boolean listFilesDirectories(CommandCard card, FileSystemGlob fsg, int deck_position)
        {
            string[] args = card.Arguments;
            string attributes = "", control = "", access = "", creation = "", write = "", old_p = "";
            // string[] dir_c;
            string fil = "";
            bool changed_location = false; //we changed the file system's location during this.
                if (args.Length > 0) //do we have arguments?
                {
                    if (args.ElementAt(args.Length - 1).Contains("#OUT"))
                    {
                        int x = args.Length - 1;
                        args[x] = deck[deck_position - 1].Output; //we replace #OUT# with the ouput of the previous card.
                    }
                    if (args.ElementAt(args.Length - 1).Length > 2)
                    {
                        if (args.ElementAt(args.Length - 1).Contains(":\\")) //path is supplied.
                        {
                            old_p = fsg.Current_Pointer;
                            fsg.changeLocation(args.ElementAt(args.Length - 1));
                            changed_location = true;
                        }
                        else
                        { //we work on the current string.
                            string dir = fsg.Current_Pointer + args.ElementAt(args.Length - 1);
                            old_p = fsg.Current_Pointer;
                            fsg.changeLocation(dir);
                            changed_location = true;
                        }
                    }
                }
                //int det = 1; //used to get one file on each line.
                for (int i = 0; i < fsg.Current_Children.Count; i++)
                {
                    if (args.Length > 0) //we have arguments.
                    {
                        try
                        {
                            fil = fsg.Current_Pointer + "\\" + fsg.Current_Children[i]; //create an absolute path.
                            attributes = System.IO.File.GetAttributes(fil).ToString();
                            control = System.IO.File.GetAccessControl(fil).ToString();
                            access = System.IO.File.GetLastAccessTime(fil).ToString();
                            creation = System.IO.File.GetCreationTime(fil).ToString();
                            write = System.IO.File.GetLastWriteTime(fil).ToString();

                        }
                        catch (Exception e) { Executer.output(card, " "); return false; } //if a file doesn't like this - simply remove it from the list.  Most files that are like this are System-level files.
                    }
                    if (Path.HasExtension(fsg.Current_Children[i]))
                    {
                        if (fsg.Current_Children[i].Contains(".txt") || fsg.Current_Children[i].Contains(".rtf") || fsg.Current_Children[i].Contains(".doc") || fsg.Current_Children[i].Contains(".odt") || fsg.Current_Children[i].Contains(".xsl") || fsg.Current_Children[i].Contains(".ppt"))
                            Console.ForegroundColor = ConsoleColor.White;
                        else if (fsg.Current_Children[i].Contains(".htm") || fsg.Current_Children[i].Contains(".xml") || fsg.Current_Children[i].Contains(".rb") || fsg.Current_Children[i].Contains(".php"))
                            Console.ForegroundColor = ConsoleColor.Blue;
                        else if (fsg.Current_Children[i].Contains(".mp") || fsg.Current_Children[i].Contains(".ogg") || fsg.Current_Children[i].Contains(".mov") || fsg.Current_Children[i].Contains(".wm") || fsg.Current_Children[i].Contains(".flac") || fsg.Current_Children[i].Contains(".aac"))
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        else if (fsg.Current_Children[i].Contains(".cpp") || fsg.Current_Children[i].Contains(".java") || fsg.Current_Children[i].Contains(".h") || fsg.Current_Children[i].Contains(".c") || fsg.Current_Children[i].Contains(".bat") || fsg.Current_Children[i].Contains(".wis") || fsg.Current_Children[i].Contains(".cs") || fsg.Current_Children[i].Contains(".class"))
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        else if (fsg.Current_Children[i].Contains(".bmp") || fsg.Current_Children[i].Contains(".png") || fsg.Current_Children[i].Contains(".jpg") || fsg.Current_Children[i].Contains(".tiff") || fsg.Current_Children[i].Contains(".psd") || fsg.Current_Children[i].Contains(".gif"))
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        else if (fsg.Current_Children[i].Contains(".ini") || fsg.Current_Children[i].Contains(".sys"))
                            Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    
                    Executer.output(card, fsg.Current_Children[i]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if (args.Length > 0)//now, based on arguments, we display the pieces of information.
                    {
                        for (int j = 0; j < args.Length; j++)
                        {
                            switch (args[j])
                            {//based on the arguments, display necessary information.
                                case ("-a"): //User wants to see attributes of each file.
                                case ("-attributes"):
                                case ("-show_attributes"): Executer.output(card, " Attributes:" + attributes + "\n"); break;
                                case ("-c"): //User wants to see control information of each file.
                                case ("-control"):
                                case ("-show_control_information"): Executer.output(card, " Control:" + control + "\n"); break;
                                case ("-r"): //User wants to see write time of each file.
                                case ("-write_time"):
                                case ("-show_write_time"): Executer.output(card, " Write Time:" + write + "\n"); break;
                                case ("-e"): //User wants to see creation time of each file.
                                case ("-creation"):
                                case ("-show_creation_time"): Executer.output(card, " Creation Time:" + creation + "\n"); break;
                                case ("-s"): //User wants to see access time of each file.
                                case ("-access"):
                                case ("-show_access_time"): Executer.output(card, " Access Time:" + access + "\n"); break;
                                case ("-i"): //user wants to see everything.
                                case ("-information"):
                                case ("-show_all_info"):
                                    Console.WriteLine(" A:" + attributes + " C:" + control + " WT:" + write + " CT:" + creation + " AT:" + access); break;
                            }
                        }

                    }
                    // Executer.output(card, "\n");

                }
                if (changed_location) //change back the location.
                {
                    fsg.changeLocation(old_p);
                }
                return true;
            
            

        }
        /*****************
         * 
         * filter
         * 
         * Filters a list.
         * 
         * Syntax: filter <filtering word> <list of items, seperated by spaces>
         */
        public static void filter_list(CommandCard card, int deck_position)
        {
            string filter;
            string filtered = "";
            List<String> results = new List<String>();
            if (card.Arguments[0].Contains("\\")) //what to be filtered is in a file.
            {
                using (System.IO.StreamReader sr = new StreamReader(card.Arguments[0]))
                {
                    filter = sr.ReadToEnd();
                }
            }
            else { filter = card.Arguments[0]; }
            if (card.Arguments[1].Equals("#OUT#")) //output is in the previous card.
            {
                filtered = deck[deck_position - 1].Output;
            }
            else if (card.Arguments[1].Contains("\\")) //filtered is in a file.
            {
                using (System.IO.StreamReader sr = new StreamReader(card.Arguments[1]))
                {
                    filtered = sr.ReadToEnd();
                }
            }
            else
            {
                for (int i = 1; i < card.Arguments.Length; i++)
                {
                    filtered = filtered + card.Arguments[i] + " "; //construct a string that contains all thestrings of arguments.
                }

            }

            string[] list = filtered.Split();
            foreach (string s in list)
            {
                if (s.Contains(filter))
                    results.Add(s);
            }
            foreach (string s in results)
            {
                Executer.output(card, s);
            }

        }

        /*******************
         * save_variable_dictionaries
         * 
         * Saves the selected variable to the dictionary, or if no argument, save all variables to dictionaries.
         *
         * Syntax: save <optional: variable>
         */
        public static void save_variable_dictionaries(CommandCard card, int deck_position, Dictionary<string, string> sd, Dictionary<string, float> id)
        {
            string[] keys;
            string[] vals;
            float[] i_vals;
            string[] args = card.Arguments;
            if (args.Length == 0)
            {
                using (StreamWriter sw = new StreamWriter("Data/strvar"))
                {
                    keys = sd.Keys.ToArray<string>();
                    vals = sd.Values.ToArray<string>(); //get all keys and values.

                    for (int i = 0; i < keys.Length; i++)
                    {
                        sw.WriteLine(keys[i] + " " + vals[i].Trim());
                    }
                }
                using (StreamWriter sw = new StreamWriter("Data/intvar"))
                {
                    keys = id.Keys.ToArray<String>();
                    i_vals = id.Values.ToArray<float>();

                    for (int i = 0; i < keys.Length; i++)
                    {
                        sw.WriteLine(keys[i] + " " + i_vals[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string variable = "";
                    if (args[i].StartsWith("#") || args[i].StartsWith("@")) //if it starts with a variable dignifier, 
                    {
                        variable = args[i].Substring(1); //get the variable name 
                    }
                    else { variable = args[i]; }
                    string outv; //our outvalue for strings.
                    float i_outv; //our outvalue for ints.
                    bool validity = false; ; //wether or not the variable is valid. 
                    if (sd.TryGetValue(variable, out outv))
                    {
                        validity = true;
                        using (StreamWriter sw = new StreamWriter("Data/strvar")) { sw.WriteLine(variable + " " + outv.Trim()); }
                    }
                    if (id.TryGetValue(variable, out i_outv))
                    {
                        validity = true;
                        using (StreamWriter sw = new StreamWriter("Data/intvar")) { sw.WriteLine(variable + " " + i_outv); }
                    }
                    if (!validity) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[3] + " " + args[i] + " " + GSR.Listing[10]); Console.ForegroundColor = ConsoleColor.Gray; }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Executer.output(card, GSR.Listing[3] + " "+ variable + " " + GSR.Listing[46]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                     
                    //Listing 3 = Variable translation.
                    //Listing 13 = Could not have been saved.  Perhaps...
                    //Listing 46 = was saved successfully.
                }
                
            }
        }

        /**********************
         * 
         * view_file_info
         * 
         * Views the information of the file(s) in the arguments.  If the file won't allow a certain attribute, it'll return an error
         * saying that the access to that file is denied.
         * 
         * Syntax: info <file(s)>
         */
        public static void view_file_info(CommandCard card, int deck_position, FileSystemGlob fsg)
        {
            string[] args = card.Arguments;
            string file;
            string attributes, control, access, creation, write;
            foreach (string fil in args)
            {
                if (!fil.Contains("C:\\")) //path is not relative.
                {
                    file = fsg.Current_Pointer + "\\" + fil;
                }
                else if (fil.Equals("#OUT#"))
                {
                    file = deck[deck_position - 1].Output;
                }
                else { file = fil; }

                Executer.output(card, GSR.Listing[13] + file);

                try
                {
                    attributes = System.IO.File.GetAttributes(file).ToString();
                    Executer.output(card, GSR.Listing[14] + attributes);
                }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Executer.output(card, GSR.Listing[15]); Console.ForegroundColor = ConsoleColor.Gray; }

                try
                {
                    control = System.IO.File.GetAccessControl(file).ToString();
                    Executer.output(card, GSR.Listing[16] + control);
                }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Executer.output(card, GSR.Listing[17]); Console.ForegroundColor = ConsoleColor.Gray; }

                try
                {
                    access = System.IO.File.GetLastAccessTime(file).ToString();
                    Executer.output(card, GSR.Listing[18] + access);
                }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Executer.output(card, GSR.Listing[19]); Console.ForegroundColor = ConsoleColor.Gray; }

                try { write = System.IO.File.GetLastWriteTime(file).ToString(); Executer.output(card, GSR.Listing[20] + write); }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Executer.output(card, GSR.Listing[21]); Console.ForegroundColor = ConsoleColor.Gray; }

                try
                {
                    creation = System.IO.File.GetCreationTime(file).ToString();
                    Executer.output(card, GSR.Listing[22] + creation);
                }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Executer.output(card, GSR.Listing[23]); Console.ForegroundColor = ConsoleColor.Gray; }


                Executer.output(card, " ");
            }


        }
        /******************
         * go_back
         * 
         * Return to a previous space in history.
         * 
         * No arguments needed.
         */
        public static void go_back(FileSystemGlob fsg) //goes back in history...doesn't exactly do what we want to do.
        {
            try
            {
                fsg.changeLocation(fsg.History[fsg.HistoryPosition - 2]); //fsg.History.Count - 2 represents the previous place in history.
                fsg.HistoryPosition = fsg.HistoryPosition - 3;
                //   Console.WriteLine(fsg.HistoryPosition);
                if (fsg.HistoryPosition > fsg.History.Count)
                    fsg.HistoryPosition = fsg.History.Count;
            }
            catch (Exception) { }
        }

        /****************
         * go_forward
         * 
         * Goes forward in history, or does nothing if tdhere is no place to go forward too.
         */
        public static void go_forward(FileSystemGlob fsg)
        {
            try
            {
                fsg.changeLocation(fsg.History[fsg.HistoryPosition + 1]);
                //        fsg.HistoryPosition = fsg.HistoryPosition + 2;
                if (fsg.HistoryPosition > fsg.History.Count)
                    fsg.HistoryPosition = fsg.History.Count;

            }
            catch (Exception) { }
        }

        /**************
         * go_up
         * 
         * Goes up one folder level.
         * 
         * 
         */
        public static void go_up(FileSystemGlob fsg) // goes up one level.
        {
            string up_dir = "";
               
            try
            {
                    up_dir = System.IO.Path.GetDirectoryName(fsg.Current_Pointer.Substring(0, fsg.Current_Pointer.Length - 1));
                    fsg.changeLocation(up_dir);
            }
            catch (Exception) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[39]); Console.ForegroundColor = ConsoleColor.Gray; } 
        }
        /*****************
         * variable_list
         * 
         * Lists all the variables in memory, and shows the contents of each.
         */
        public static void variable_list(CommandCard card, Dictionary<string, string> sd, Dictionary<string, float> id)
        {
            Executer.output(card, GSR.Listing[11]);
            string[] keys = sd.Keys.ToArray<String>();
            string[] vals = sd.Values.ToArray<string>();
            for (int i = 0; i < keys.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Executer.output(card, keys[i] + "=" + vals[i]);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Executer.output(card, "");
            Executer.output(card, GSR.Listing[12]);
            keys = id.Keys.ToArray<String>();
            float[] ivals = id.Values.ToArray<float>();
            for (int i = 0; i < keys.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Executer.output(card, keys[i] + "=" + ivals[i]);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        }

        /*************************
         * process_list
         * 
         * Displays list of current processes running.  
         * 
         * BUG: Process list display is ugly.
         * 
         */
        public static void process_list(CommandCard card)
        {
            output(card, "Please note that you may need to log in as an administrator to see all processes.");
            Process[] plist = Process.GetProcesses();
           // Array.Sort<Process>(plist);
            List<string> naccess = new List<string>();
            string px = "ID"+"\t"+"Process"+"     "+"Start Time"+"         "+"Virtual"+"    "+"Physical";
            string ll = "----------------------------------------------------------";
            output(card, px);
            output(card, ll);
            foreach (Process p in plist)
            {
                try
                {
                    if (!p.Responding)
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    string str = p.Id + "  " + p.ProcessName + "   " + p.StartTime +"   " + p.VirtualMemorySize64 + "   " + p.WorkingSet64;
                    output(card, str);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                catch (Exception e) { naccess.Add(p.ProcessName); }
            }
            //DEBUG:
            //  if (naccess.Count > 0)
            //  {
            //     output(card, "You do not have access to these processes:");
            //    foreach (string name in naccess)
            //    {
            //       output(card, name);
            //   }
            // }
        }

        /**********************
         * kill_process
         * 
         * Kills a process based on the process's PID or name.  If by name, it kills all processes
         * with that name.
         * 
         * Syntax: kill <PID or Name>
         */
        public static void kill_process(CommandCard card)
        {
            int pid;
            string[] args = card.Arguments;
            foreach (String arg in args)
            {
                if (Int32.TryParse(arg, out pid)) //user wants to kill process by ID.
                {
                    Process p;
                    try { p = Process.GetProcessById(pid); }
                    catch(Exception) { Console.ForegroundColor = ConsoleColor.Red; output(card, GSR.Listing[47]); Console.ForegroundColor = ConsoleColor.Gray; return; }
                    try { p.Kill(); Console.ForegroundColor = ConsoleColor.Cyan; output(card, GSR.Listing[48]); Console.ForegroundColor = ConsoleColor.Gray; }
                    catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; output(card, GSR.Listing[49] + " " + p.ProcessName); Console.ForegroundColor = ConsoleColor.Gray; }
                }
                else
                {

                    Process[] p = Process.GetProcessesByName(arg);
                    if (p.Length < 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        output(card, GSR.Listing[47]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        return;
                    }
                    foreach (Process proc in p) //user wants to kill process by string name.
                    {
                        try { proc.Kill(); Console.ForegroundColor = ConsoleColor.Cyan; output(card, GSR.Listing[48]); Console.ForegroundColor = ConsoleColor.Gray; }
                        catch (Exception e) { output(card, GSR.Listing[49] + " " + proc.ProcessName); }
                    }
                }
            }
        }

        /****************
         * removeFileOrFolder
         * 
         * Removes the file or folder in the argument.
         * 
         * Syntax: rm <File or Folder>
         */
        public static void removeFileOrFolder(CommandCard card, FileSystemGlob fsg)
        {
            string[] args = card.Arguments;
            string ftbr = ""; //file to be removed.
            foreach (string file in args)
            {
                if (!file.Contains(":\\"))
                {
                    ftbr = fsg.Current_Pointer + file;
                }
                else { ftbr = file; }
                try
                {
                    if (fsg.Protected.Contains(ftbr))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Executer.output(card, GSR.Listing[29]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        return;

                    }
                    else if (System.IO.File.Exists(ftbr))
                    {

                        System.IO.File.Delete(ftbr);
                        fsg.changeLocation(fsg.Current_Pointer);  //updates current children.
                    }
                    else
                    {
                        System.IO.Directory.Delete(ftbr);
                        fsg.changeLocation(fsg.Current_Pointer); //updates current children.
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Executer.output(card, GSR.Listing[25]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Executer.output(card, GSR.Listing[24] + e.Message); Console.ForegroundColor = ConsoleColor.Gray; }

            }
        }

       /***************
        * newFileOrFolder
        * 
        * Creates new File(s) or Folder(s) depending on whether or not the name has an extension.
        *
        * Syntax: new <File(s) or Folder(s)>
        */
        public static void newFileOrFolder(CommandCard card, FileSystemGlob fsg)
        {
            string file = "";
            string[] args = card.Arguments;
            foreach (string arg in args)
            {
                try
                {
                    if (!arg.Contains("\\"))
                    {
                        file = fsg.Current_Pointer + arg;
                    }
                    else { file = arg; }

                    if (arg.Contains(".")) //might be changed later on.
                    {
                        using (FileStream fs = System.IO.File.Create(file))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Executer.output(card, GSR.Listing[26]);
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(file);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Executer.output(card, GSR.Listing[27]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    fsg.changeLocation(fsg.Current_Pointer); //the quickest way to update fsg.Current_children
                }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Executer.output(card, GSR.Listing[28] + e.Message); Console.ForegroundColor = ConsoleColor.Gray; }


            }
        }
        /*************
         * addToFile
         * 
         * Appends a string to a file.
         * 
         * Syntax: add "String" <File>
         */
        public static void addToFile(CommandCard card, FileSystemGlob fsg, int deck_position)
        {
            string[] args = card.Arguments;
            string file = "";
            try
            {
                if (!args[1].Contains(":\\"))
                {
                    file = fsg.Current_Pointer + args[1];
                }
                else { file = args[1]; }
            }
            catch (Exception e) { }
            try
            {

                using (StreamWriter sw = System.IO.File.AppendText(file))
                {
                    if (args[0].Equals("#OUT#"))
                    {
                        args[0] = deck[deck_position - 1].Output;
                    }

                    sw.WriteLine(args[0]);

                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                Executer.output(card, GSR.Listing[31] + " " + file);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Executer.output(card, GSR.Listing[30]); Console.ForegroundColor = ConsoleColor.Gray; }
        }

        /******************
         * executeScripts
         * 
         * Calls WISE to execute a script.
         * 
         * Syntax: execute <script file>
         */
        public static void executeScripts(CommandCard card, FileSystemGlob fsg, AliasTable at, Dictionary<String, float> intvar, Dictionary<String, String> strvar)
        {
            string file = "";
            string[] args = card.Arguments;
            Console.ForegroundColor = ConsoleColor.Yellow; //every result will be the result of the script.
            foreach (string arg in args)
            {
                if (arg.Contains(":\\"))
                {
                    file = arg;
                }
                else
                {
                    file = fsg.Current_Pointer + "\\" + arg;
                }

                if (System.IO.File.Exists(file))
                {
                    if (file.Contains(".bat"))  //run bat script.
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine(GSR.Listing[34]); //.BAT warning
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        using (StreamReader sr = new StreamReader(file))
                        {
                            string[] cmds = sr.ReadToEnd().Split('\n');
                            foreach (string cmd in cmds)
                            {
                                Process.Start(cmd);
                            }
                        }
                    }
                    ScriptingEngine script = new ScriptingEngine(file);
                    script.run(fsg, at, intvar, strvar);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(GSR.Listing[32]); //WELSH File Not Found Error
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }

       /*****************
        * exploreDirectory
        * 
        * Opens Explorer on the supplied directory.
        * 
        * Syntax: explore <Directory>
        */
        public static void exploreDirectory(CommandCard card, FileSystemGlob fsg)
        {
            string[] args = card.Arguments;
            string directory;

            if (args.Length == 0)
            {
                directory = fsg.Current_Pointer;
                try { Process.Start(directory); }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(e.Message); Console.ForegroundColor = ConsoleColor.Gray; }
            }
            else
            {
                foreach (string arg in args)
                {
                    if (!arg.Contains(":\\"))
                    {
                        directory = fsg.Current_Pointer + arg;
                    }
                    else { directory = arg; }
                    try
                    {
                        Process.Start(directory);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Message);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }


            }
        }
        /*****************
         * protectFileDirectory
         * 
         * Protects a file or Directory by adding it to the protected file or folder list.
         * 
         */
        public static void protectFileDirectory(CommandCard card, FileSystemGlob fsg)
        {
            string p = "";
            string[] args = card.Arguments;
            foreach (string arg in args)
            {
                using (StreamWriter sw = System.IO.File.AppendText("Data\\protected"))
                {
                    if (!arg.Contains(":\\"))
                    {
                        p = fsg.Current_Pointer + arg;
                    }
                    else { p = arg; }

                    sw.WriteLine(p);

                }
                fsg.Protected.Add(p);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Executer.output(card, GSR.Listing[36]);
                Console.ForegroundColor = ConsoleColor.Gray;

            }

        }
        /****************
         * searchWiki
         * 
         * Opens a Wikipedia page on the term supplied in the arguments.
         */
        public static void searchWiki(CommandCard card)
        {
            string[] args = card.Arguments;
            string term;
            foreach (string arg in args)
            {
                term = arg.Replace(' ', '_');
                Process.Start("http://en.wikipedia.org/wiki/" + term);
            }
        }

        /******************
         * googleIt
         * 
         * Googles the word(s) in the arguments.
         */
        public static void googleIt(CommandCard card)
        {
            string[] args = card.Arguments;
            string term;
            foreach (string arg in args)
            {
                term = arg.Replace(' ', '+');
                Process.Start("http://www.google.com/search?q=" + term);
            }
        }

        /********************
         * copyFile
         * 
         * Creates a copy of the file.  The new file's name is Copy of (file) with a random number, and then the
         * extension.  The random number is so that it doesn't overwrite any other copy of the file.
         */
        public static void copyFile(CommandCard card, FileSystemGlob fsg)
        {
            Random rg = new Random();
            string[] args = card.Arguments;
            string p = "";
            foreach (string arg in args)
            {
                if (!arg.Contains(":\\"))
                {
                    p = fsg.Current_Pointer + arg;
                }
                else { p = arg; }

                string title = Path.GetDirectoryName(p) + GSR.Listing[37] + System.IO.Path.GetFileNameWithoutExtension(p) + " " + rg.Next(1000) + Path.GetExtension(p);
                Console.WriteLine(title);
                try
                {
                    File.Copy(p, title);
                    fsg.changeLocation(fsg.Current_Pointer);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(GSR.Listing[38]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(e.Message); Console.ForegroundColor = ConsoleColor.Gray; }

                


            }


        }
    /**************
    * aliasFile
     * 
     * Adds a new alias to the alias File by helpful prompts.
     */
        public static void aliasFile(CommandCard card, FileSystemGlob fsg, AliasTable at)
        {
            string aliased_file = "";
            string[] args = card.Arguments;

            if (!args[0].Contains(":\\"))
            {
                aliased_file = fsg.Current_Pointer + args[0];
            }
            else { aliased_file = args[0]; }

            string al1 = ""; //alias name 1
            string al2 = ""; //alias name 2
            string al3 = ""; //alias name 3
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(GSR.Listing[40]); //first alias prompt
            Console.ForegroundColor = ConsoleColor.Gray;
            al1 = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(GSR.Listing[41]); //second alias prompt
            Console.ForegroundColor = ConsoleColor.Gray;
            al2 = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GSR.Listing[42]); //third alias prompt
            Console.ForegroundColor = ConsoleColor.Gray;
            al3 = Console.ReadLine();

            string write_in_table = al1 + " " + al2 + " " + al3 + " 0 " + aliased_file; //built string to insert into the string.

            using (StreamWriter sw = System.IO.File.AppendText("Data/alias"))
            {
                try{
                sw.WriteLine(write_in_table);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Executer.output(card, GSR.Listing[50]);
                Console.ForegroundColor = ConsoleColor.Gray;
                }
                catch(Exception e){Console.ForegroundColor = ConsoleColor.Red; Executer.output(card, GSR.Listing[51] + e.Message); Console.ForegroundColor = ConsoleColor.Gray;}
            }
            
            at.populate(); //repopulate the table.
        }

        /****************************
         * moveFile
         * 
         * Moves a file from one directory to another.  Also used to rename files.
         */
        public static void moveFile(CommandCard card, FileSystemGlob fsg)
        {
            try
            {
                string[] args = card.Arguments;
                string file = args[0]; //The first argument is the file.
                string to_file = args[1]; //the sec ond argument is the "to file".

                if (!file.Contains(":\\"))
                {
                    file = fsg.Current_Pointer + file;
                }

                if (fsg.Protected.Contains(file))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(GSR.Listing[29]); //WELSH protection error.
                    Console.ForegroundColor = ConsoleColor.Gray;
                    return;
                }

                if (!to_file.Contains(":\\"))
                {
                    to_file = fsg.Current_Pointer + to_file;
                }

                File.Move(file, to_file);
                File.Delete(file); //get rid of the file.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Executer.output(card, GSR.Listing[52]);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(GSR.Listing[1] + e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /*****************
         * viewFile
         * 
         * Opens a file for viewing, but cannot edit the file.
         */
        public static void viewFile(CommandCard card, FileSystemGlob fsg)
        {
            string[] args = card.Arguments;
            string file;
            foreach (string arg in args)
            {
                if (!arg.Contains(":\\"))
                {
                    file = fsg.Current_Pointer + arg;
                }
                else { file = arg; }
                try
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        string fil = sr.ReadToEnd();
                        Executer.output(card, fil);



                    }
                }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[1] + e.Message); Console.ForegroundColor = ConsoleColor.Gray; }
            }
        }
        /*************
         * echoString
         * 
         * Echos a string into the output.
         */
        public static void echoString(CommandCard card, int card_position)
        {
            //build string
            string[] args = card.Arguments;
            string str = "";
            foreach (string arg in args)
            {
                if (arg.Contains("$$$"))
                {  //WELSH 1.2 Change: Echo can now display colors.
                    string color = arg.Trim('$').ToLower();
                    if (color.Equals(GSR.Listing[53]))//red
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (color.Equals(GSR.Listing[54])) //blue
                        Console.ForegroundColor = ConsoleColor.Blue;
                    else if (color.Equals(GSR.Listing[55])) //yellow
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else if (color.Equals(GSR.Listing[56])) //gray
                        Console.ForegroundColor = ConsoleColor.Gray;
                    else if (color.Equals(GSR.Listing[57])) //cyan
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    else if (color.Equals(GSR.Listing[58])) //green
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (color.Equals(GSR.Listing[59])) //magenta
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else if (color.Equals(GSR.Listing[60])) //white
                        Console.ForegroundColor = ConsoleColor.White;
                    else if (color.Equals(GSR.Listing[61])) //black
                        Console.ForegroundColor = ConsoleColor.Black;
                    else Console.ForegroundColor = ConsoleColor.Gray; //if it's 
                }
                else
                {
                    str = str + arg + " ";
                }
            }

            Executer.output(card, str);

            Console.ForegroundColor = ConsoleColor.Gray; //return to default color.
        }
        /**************************
         * displayTime
         * 
         * Displays the time and date.
         */
        public static void displayTime(CommandCard card, int deck_position)
        {
            string[] args = card.Arguments;
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (args.Length == 0)
            {
                string str = DateTime.Now.ToString();
                Executer.output(card, str);
            }
            else
            {
                string arg = args[0];
                arg = arg.Trim('-');
                if (arg.Equals(GSR.Listing[62]) || arg.Equals(GSR.Listing[63])) //day or d, displays the day.
                {
                    string str = DateTime.Now.DayOfWeek.ToString();
                    Executer.output(card, str);
                }
                else if (arg.Equals(GSR.Listing[64]) || arg.Equals(GSR.Listing[65])) //hour or h, displays hour.
                {
                    string str = DateTime.Now.Hour.ToString();
                    Executer.output(card, str);
                }
                else if (arg.Equals(GSR.Listing[66]) || arg.Equals(GSR.Listing[67])) //minute or m, displays minute.
                {
                    string str = DateTime.Now.Minute.ToString();
                    Executer.output(card, str);
                }
                else if (arg.Equals(GSR.Listing[68]) || arg.Equals(GSR.Listing[69])) //second or s, displays second.
                {
                    string str = DateTime.Now.Second.ToString();
                    Executer.output(card, str);
                }
                else if (arg.Equals(GSR.Listing[70]) || arg.Equals(GSR.Listing[71])) //date or a, displays the date.
                {
                    string str = DateTime.Now.Date.ToString();
                    Executer.output(card, str);
                }
                else if (arg.Equals(GSR.Listing[72]) || arg.Equals(GSR.Listing[73]))
                {
                    string str = DateTime.Now.TimeOfDay.ToString();
                    Executer.output(card, str);
                }
            }
        }
                

            }
        }
