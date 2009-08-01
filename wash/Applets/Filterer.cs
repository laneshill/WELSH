using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wash.Components;

namespace Wash.Applets
{
    class Filterer
    {
        public static void run(CommandCard card, int deck_position)
        {
            bool COLORS = !Settings1.Default.cmdMode;
            string filter;
            string filtered = "";

            List<String> results = new List<String>();
            if (card.Arguments.Length < 2)
            {
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                Console.WriteLine(GSR.Listing[76]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                return;
            }
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
                filtered = Executer.Deck[deck_position - 1].Output;
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
            if (results.Count == 0)
            {
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Yellow; }
                Console.WriteLine(GSR.Listing[77]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            }
            foreach (string s in results)
            {
                Executer.output(card, s);

            }

        }
    }
}
