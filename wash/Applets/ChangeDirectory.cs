using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wash.Components;

namespace Wash.Applets
{
    /*************
     *ChangeDirectory changes the current working directory.
     */
    class ChangeDirectory
    {
        public static void run(CommandCard card, FileSystemGlob fsg, int deck_position)
        {
            string[] args = card.Arguments;
            bool emptyArgs = false;
            string suffix = "\\";
            bool COLORS = !Settings1.Default.cmdMode;
            if (args.Length == 0) { emptyArgs = true; } 
            foreach (string arg in args)
            {
                if (arg.Equals(""))
                {
                    emptyArgs = true;
                    break;
                }
            }
            if (emptyArgs && Settings1.Default.cdNoArgs.Equals("dos"))
            {
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                Executer.output(card, GSR.Listing[45] + " " + fsg.Current_Pointer);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                return;          
            }
            else if (emptyArgs && Settings1.Default.cdNoArgs.Equals("root"))
            {
                fsg.changeLocation(fsg.Current_Pointer[0] + ":\\");
                return;
            }
            else if (emptyArgs && Settings1.Default.cdNoArgs.Equals("home"))
            {
                fsg.changeLocation(Settings1.Default.homeDirectory);
                return;
            }

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
                dir = fsg.Current_Pointer + dir;
            }
            if (args[0].Equals("..")) //we are supposed to go up one level.
            {
                if (fsg.Current_Pointer.EndsWith("\\"))
                {
                    try
                    {
                        fsg.Current_Pointer = fsg.Current_Pointer.Remove(fsg.Current_Pointer.Length - 1);
                    }
                    catch (Exception)
                    {
                        //Do nothing yet.
                    }
                }
                dir = System.IO.Path.GetDirectoryName(fsg.Current_Pointer);
                if (String.IsNullOrEmpty(dir)) //GetDirectoryName returns an empty value string when you are a level away from the root.
                {
                    dir = System.IO.Path.GetPathRoot(fsg.Current_Pointer);
                }

            }
            if (args[0].Equals("#OUT#"))
            {//if this command relies on the result of a chained command, then get the position
                //of the previous card.  Please note that this might be fatal if the directory has
                //has spaces.
                dir = Executer.Deck[deck_position - 1].Output;
            }
            if (dir.EndsWith(":\\") || dir.EndsWith("\\"))
                suffix = "";


             fsg.changeLocation(dir + suffix);
        }
    }
}
