using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wash.Components;

namespace Wash.Applets
{
    class NavigationOps
    {
        public static void goBack(FileSystemGlob fsg)
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

        public static void goForward(FileSystemGlob fsg)
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
        public static void goUpALevel(FileSystemGlob fsg)
        {
            string up_dir = "";
            bool COLORS = !Settings1.Default.cmdMode;
            try
            {
                up_dir = System.IO.Path.GetDirectoryName(fsg.Current_Pointer.Substring(0, fsg.Current_Pointer.Length - 1));
                fsg.changeLocation(up_dir);
            }
            catch (Exception) {
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                Console.WriteLine(GSR.Listing[39]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            }
        }
    }
}
