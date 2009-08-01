using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wash.Components;
using System.Diagnostics;

namespace Wash.Applets
{
    class FileOps
    {
        public static void remove(CommandCard card, FileSystemGlob fsg)
        {
            string[] args = card.Arguments;
            string ftbr = ""; //file to be removed.
            bool COLORS = !Settings1.Default.cmdMode;
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
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                        Executer.output(card, GSR.Listing[29]);
                        if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                        return;

                    }
                    else if (System.IO.File.Exists(ftbr))
                    {
                        System.IO.File.Delete(ftbr);
                        fsg.changeLocation(fsg.Current_Pointer);  //updates current children.
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                        Executer.output(card, GSR.Listing[25]);
                    }
                    else
                    {
                        string[] files = System.IO.Directory.GetFiles(ftbr);
                        string[] folders = System.IO.Directory.GetFiles(ftbr);
                        for (int i = 0; i < files.Length && i < folders.Length; i++)
                        {
                            if (fsg.Protected.Contains(files[i]) || fsg.Protected.Contains(folders[i]))
                            {
                                if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                                Executer.output(card, GSR.Listing[29]);
                                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                                return;
                            }
                        }
                        System.IO.Directory.Delete(ftbr, true);
                        fsg.changeLocation(fsg.Current_Pointer); //updates current children.
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                        Executer.output(card, GSR.Listing[78]);
                    }


                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }
                catch (Exception e)
                {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Executer.output(card, GSR.Listing[24] + e.Message);
                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }

            }

        }

        public static void create(CommandCard card, FileSystemGlob fsg)
        {
            string file = "";
            string[] args = card.Arguments;
            bool COLORS = !Settings1.Default.cmdMode;
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
                            if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                            Executer.output(card, GSR.Listing[26]);
                            if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(file);
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                        Executer.output(card, GSR.Listing[27]);
                        if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                    }
                    fsg.changeLocation(fsg.Current_Pointer); //the quickest way to update fsg.Current_children
                }
                catch (Exception e)
                {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Executer.output(card, GSR.Listing[28] + e.Message);
                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }
            }


        }

        public static void addTo(CommandCard card, FileSystemGlob fsg, int deck_position)
        {
            string[] args = card.Arguments;
            string file = "";
            bool COLORS = !Settings1.Default.cmdMode;
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
                        args[0] = Executer.Deck[deck_position - 1].Output;
                    }

                    sw.WriteLine(args[0]);

                }
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                Executer.output(card, GSR.Listing[31] + " " + file);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            }
            catch (Exception e)
            {
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                Executer.output(card, GSR.Listing[30]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            }

        }

        public static void executeScript(CommandCard card, FileSystemGlob fsg, AliasTable at, Dictionary<String, float> intvar, Dictionary<String, String> strvar)
        {
            string file = "";
            string[] args = card.Arguments;
            bool COLORS = !Settings1.Default.cmdMode;
            if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; } //every result will be the result of the script.
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
                    if (file.Contains(".bat"))  //run bat script....extremely experimental.
                    {
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Magenta; }
                        Console.WriteLine(GSR.Listing[34]); //.BAT warning
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
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
                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }
                else
                {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Console.WriteLine(GSR.Listing[32]); //WELSH File Not Found Error
                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }
            }

        }

        public static void copy(CommandCard card, FileSystemGlob fsg)
        {
            Random rg = new Random();
            string[] args = card.Arguments;
            string p = "";
            bool COLORS = !Settings1.Default.cmdMode;
            foreach (string arg in args)
            {
                if (!arg.Contains(":\\"))
                {
                    p = fsg.Current_Pointer + arg;
                }
                else { p = arg; }

                string title = Path.GetDirectoryName(p) + "\\" + GSR.Listing[37] + System.IO.Path.GetFileNameWithoutExtension(p) + " " + rg.Next(1000) + Path.GetExtension(p);

                try
                {
                    File.Copy(p, title);
                    fsg.changeLocation(fsg.Current_Pointer);
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                    Console.WriteLine(GSR.Listing[38] + "-" + title);
                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }
                catch (Exception e) {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Console.WriteLine(e.Message);
                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }
            }
        }

        public static void explore(CommandCard card, FileSystemGlob fsg)
        {
            string[] args = card.Arguments;
            string directory;
            bool COLORS = !Settings1.Default.cmdMode;

            if (args.Length == 0)
            {
                directory = fsg.Current_Pointer;
                try { Process.Start(directory); }
                catch (Exception e)
                {
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Console.WriteLine(e.Message);
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Gray; }
                }
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
                        Process.Start(directory); //in C#, if you call Process.Start on a path, it opens up explorer on that path.
                    }
                    catch (Exception e)
                    {
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                        Console.WriteLine(e.Message);
                        if (COLORS) { Console.ForegroundColor =Settings1.Default.foregroundColor; }
                    }
                }


            }

        }

        public static void move(CommandCard card, FileSystemGlob fsg)
        {
            bool COLORS = !Settings1.Default.cmdMode;
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
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                    Console.WriteLine(GSR.Listing[29]); //WELSH protection error.
                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                    return;
                }

                if (!to_file.Contains(":\\"))
                {
                    to_file = fsg.Current_Pointer + to_file;
                }

                Directory.Move(file, to_file);
                // if (file.Contains("."))
                //     File.Delete(file);
                //  else
                //       Directory.Delete(file); //get rid of the file.
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                Executer.output(card, GSR.Listing[52]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            }
            catch (Exception e)
            {
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                Console.WriteLine(GSR.Listing[1] + e.Message + "-" + GSR.Listing[79]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            }

            fsg.changeLocation(fsg.Current_Pointer); //re-get the children of pointer.
        }

        public static void protect(CommandCard card, FileSystemGlob fsg)
        {
            bool COLORS = !Settings1.Default.cmdMode;
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
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                Executer.output(card, GSR.Listing[36]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }

            }
        }

        public static void alias(CommandCard card, FileSystemGlob fsg, AliasTable at)
        {
            bool COLORS = !Settings1.Default.cmdMode;
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
            if (COLORS) { Console.ForegroundColor = ConsoleColor.White; }
            Console.Write(GSR.Listing[40]); //first alias prompt
            if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            al1 = Console.ReadLine();
            if (COLORS) { Console.ForegroundColor = ConsoleColor.White; }
            Console.Write(GSR.Listing[41]); //second alias prompt
            if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            al2 = Console.ReadLine();
            if (COLORS) { Console.ForegroundColor = ConsoleColor.White; }
            Console.WriteLine(GSR.Listing[42]); //third alias prompt
            if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            al3 = Console.ReadLine();

            string write_in_table = al1 + " " + al2 + " " + al3 + " 0 " + aliased_file; //built string to insert into the string.

            using (StreamWriter sw = System.IO.File.AppendText("Data/alias"))
            {
                try
                {
                    sw.WriteLine(write_in_table);
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                    Executer.output(card, GSR.Listing[50]);
                    if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }
                catch (Exception e) { 
                    if(COLORS){Console.ForegroundColor = ConsoleColor.Red; }
                    Executer.output(card, GSR.Listing[51] + e.Message);
                    if(COLORS){ Console.ForegroundColor = Settings1.Default.foregroundColor; }
                }
            }

            at.populate(); //repopulate the table.
        }
    }
}
