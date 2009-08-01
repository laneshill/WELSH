using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wash.Components;

namespace Wash.Applets
{
    class Echoer
    {
        public static void run(CommandCard card, int deck_position)
        {
             //build string
            bool COLORS = !Settings1.Default.cmdMode;
            string[] args = card.Arguments;
            string str = "";
            foreach (string arg in args)
            {
                if (arg.Contains("$$$") && COLORS)
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

            if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; } //return to default color.

        }
    }
}
