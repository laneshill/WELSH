using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wash.Components;

namespace Wash.Applets
{
    class Watch
    {
        public static void run(CommandCard card, int deck_position)
        {
            bool COLORS = !Settings1.Default.cmdMode;
            string[] args = card.Arguments;
            if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
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

            if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }

        }
    }
}
