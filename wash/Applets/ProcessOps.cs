using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Wash.Components;

namespace Wash.Applets
{
    /********
     * ProcessOps give operations for working with processors.
     */
    class ProcessOps
    {
        public static void list(CommandCard card)
        {
            bool COLORS = !Settings1.Default.cmdMode;
            if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
            Executer.output(card, "Please note that you may need to log in as an administrator to see all processes.");
            if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
            Process[] plist = Process.GetProcesses();
            // Array.Sort<Process>(plist);
            List<string> naccess = new List<string>();
            string px = "ID" + "\t" + "Process" + "\t" + "Start Time" + "\t" + "Virtual" + "\t" + "Physical";
            string ll = "";

            for (int i = 0; i < px.Length; i++)
                ll = ll + "-";
        
            Executer.output(card, px);
            Executer.output(card, ll);
            foreach (Process p in plist)
            {
                try
                {
                    if (!p.Responding)
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    string str = p.Id + "\t" + p.ProcessName + "\t" + p.StartTime + "\t" + p.VirtualMemorySize64 + "\t" + p.WorkingSet64;
                    string strll = "";
                    for (int i = 0; i < str.Length; i++)
                        strll = strll + "-";
                    Executer.output(card, str);
                    Executer.output(card, strll);
                    if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
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
            if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }

        }

        public static void kill(CommandCard card)
        {
            bool COLORS = !Settings1.Default.cmdMode;
            int pid;
            string[] args = card.Arguments;
            foreach (String arg in args)
            {
                if (Int32.TryParse(arg, out pid)) //user wants to kill process by ID.
                {
                    Process p;
                    try { p = Process.GetProcessById(pid); }
                    catch (Exception){
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                        Executer.output(card, GSR.Listing[47]);
                        if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                        return; 
                    }
                    try {
                        p.Kill();
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                        Executer.output(card, GSR.Listing[48]);
                        if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                    }
                    catch (Exception e) {
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                        Executer.output(card, GSR.Listing[49] + " " + p.ProcessName);
                        if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                    }
                }
                else
                {

                    Process[] p = Process.GetProcessesByName(arg);
                    if (p.Length < 1)
                    {
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                        Executer.output(card, GSR.Listing[47]);
                        if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                        return;
                    }
                    foreach (Process proc in p) //user wants to kill process by string name.
                    {
                        try { 
                           proc.Kill();
                               if (COLORS){ Console.ForegroundColor = ConsoleColor.Cyan; }
                               Executer.output(card, GSR.Listing[48]);
                               if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                        }
                        catch (Exception e) {
                            if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                            Executer.output(card, GSR.Listing[49] + " " + proc.ProcessName);
                            if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                        }
                    }
                }
            }


        }
    }
}
