using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Wash.Components;

namespace Wash.Applets
{
    class NetOps
    {
        public static void google(CommandCard card)
        {
            string[] args = card.Arguments;
            string term;
            foreach (string arg in args)
            {
                term = arg.Replace(' ', '+');
                Process.Start("http://www.google.com/search?q=" + term);
            }
        }

        public static void wiki(CommandCard card)
        {
            string[] args = card.Arguments;
            string term;
            foreach (string arg in args)
            {
                term = arg.Replace(' ', '_');
                Process.Start("http://en.wikipedia.org/wiki/" + term);
            }
        }
    }
}
