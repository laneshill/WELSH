using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wash.Components;

namespace Wash.Applets
{
    /*************
     * FileViewer opens up a text file for the user to view.
     */
    class FileViewer
    {
        public static void run(CommandCard card, FileSystemGlob fsg)
        {
            string[] args = card.Arguments;
            string file;
            bool COLORS = !Settings1.Default.cmdMode;
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
                catch (Exception e) {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Console.WriteLine(GSR.Listing[1] + e.Message);
                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }
            }
        }

        public static void infoView(CommandCard card, int deck_position, FileSystemGlob fsg)
        {
            string[] args = card.Arguments;
            string file;
            string attributes, control, access, creation, write;
            bool COLORS = !Settings1.Default.cmdMode;
            foreach (string fil in args)
            {
                if (!fil.Contains("C:\\")) //path is not relative.
                {
                    file = fsg.Current_Pointer + "\\" + fil;
                }
                else if (fil.Equals("#OUT#"))
                {
                    file = Executer.Deck[deck_position - 1].Output;
                }
                else { file = fil; }

                Executer.output(card, GSR.Listing[13] + file);
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
                try
                {
                    attributes = System.IO.File.GetAttributes(file).ToString();
                    Executer.output(card, GSR.Listing[14] + attributes);
                }
                catch (Exception e) {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Executer.output(card, GSR.Listing[15]);
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
                }

                try
                {
                    control = System.IO.File.GetAccessControl(file).ToString();
                    Executer.output(card, GSR.Listing[16] + control);
                }
                catch (Exception e) { 
                    if(COLORS){Console.ForegroundColor = ConsoleColor.Red;}
                    Executer.output(card, GSR.Listing[17]);
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
                    }

                try
                {
                    access = System.IO.File.GetLastAccessTime(file).ToString();
                    Executer.output(card, GSR.Listing[18] + access);
                }
                catch (Exception e) {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Executer.output(card, GSR.Listing[19]);
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
                    }

                try { write = System.IO.File.GetLastWriteTime(file).ToString(); Executer.output(card, GSR.Listing[20] + write); }
                catch (Exception e) {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Executer.output(card, GSR.Listing[21]);
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
                }

                try
                {
                    creation = System.IO.File.GetCreationTime(file).ToString();
                    Executer.output(card, GSR.Listing[22] + creation);
                }
                catch (Exception e) {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Executer.output(card, GSR.Listing[23]);
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
                }


                Executer.output(card, " ");
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            }


        }
    }
}
