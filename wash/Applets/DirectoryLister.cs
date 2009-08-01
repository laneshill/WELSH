using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wash.Components;

namespace Wash.Applets
{
    /*************
     * DirectoryLister lists the directory in a directory.
     */
    class DirectoryLister
    {
        public static void run(CommandCard card, FileSystemGlob fsg, int deck_position)
        {
            string[] args = card.Arguments;
            string attributes = "", control = "", access = "", creation = "", write = "", old_p = "";
            // string[] dir_c;
            string fil = "";
            bool changed_location = false; //we changed the file system's location during this.
            bool COLORS = !Settings1.Default.cmdMode;
            if (args.Length > 0) //do we have arguments?
            {
                if (args.ElementAt(args.Length - 1).Contains("#OUT"))
                {
                    int x = args.Length - 1;
                    args[x] = Executer.Deck[deck_position - 1].Output; //we replace #OUT# with the ouput of the previous card.
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
                    catch (Exception e) { Executer.output(card, " "); } //if a file doesn't like this - simply remove it from the list.  Most files that are like this are System-level files.
                }
                if (Path.HasExtension(fsg.Current_Children[i]) && COLORS)
                {
                    if (fsg.Current_Children[i].Contains(".txt") || fsg.Current_Children[i].Contains(".rtf") || fsg.Current_Children[i].Contains(".doc") || fsg.Current_Children[i].Contains(".odt") || fsg.Current_Children[i].Contains(".xsl") || fsg.Current_Children[i].Contains(".ppt"))
                        Console.ForegroundColor = ConsoleColor.White;
                    else if (fsg.Current_Children[i].Contains(".htm") || fsg.Current_Children[i].Contains(".xml") || fsg.Current_Children[i].Contains(".rb") || fsg.Current_Children[i].Contains(".php"))
                        Console.ForegroundColor = ConsoleColor.Blue;
                    else if (fsg.Current_Children[i].Contains(".mp3") || fsg.Current_Children[i].Contains(".ogg") || fsg.Current_Children[i].Contains(".mov") || fsg.Current_Children[i].Contains(".wm") || fsg.Current_Children[i].Contains(".flac") || fsg.Current_Children[i].Contains(".aac"))
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else if (fsg.Current_Children[i].Contains(".cpp") || fsg.Current_Children[i].Contains(".java") || fsg.Current_Children[i].Contains(".h") || fsg.Current_Children[i].Contains(".c") || fsg.Current_Children[i].Contains(".bat") || fsg.Current_Children[i].Contains(".wis") || fsg.Current_Children[i].Contains(".cs") || fsg.Current_Children[i].Contains(".class"))
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    else if (fsg.Current_Children[i].Contains(".bmp") || fsg.Current_Children[i].Contains(".png") || fsg.Current_Children[i].Contains(".jpg") || fsg.Current_Children[i].Contains(".tiff") || fsg.Current_Children[i].Contains(".psd") || fsg.Current_Children[i].Contains(".gif"))
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    else if (fsg.Current_Children[i].Contains(".ini") || fsg.Current_Children[i].Contains(".sys"))
                        Console.ForegroundColor = ConsoleColor.Red;
                }
                else if(COLORS)
                {
                    if (System.IO.Directory.Exists(fsg.Current_Pointer + fsg.Current_Children[i]))
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Executer.output(card, fsg.Current_Children[i]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
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


        }
    }
}
