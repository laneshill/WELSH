using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wash.Components;

namespace Wash.Applets
{
    /***********
     * VariableOps contributes several variable operations.
     */
    class VariableOps
    {
        public static void save(CommandCard card, int deck_position, Dictionary<string, string> sd, Dictionary<string, float> id)
        {
            string[] keys;
            string[] vals;
            float[] i_vals;
            string[] args = card.Arguments;
            bool COLORS = !Settings1.Default.cmdMode;
            if (args.Length == 0) //if there are no arguments, then we save all variables...however, the user can specify a certain variable to be saved.
            {
                using (StreamWriter sw = new StreamWriter("Data/strvar"))
                {
                    keys = sd.Keys.ToArray<string>();
                    vals = sd.Values.ToArray<string>(); //get all keys and values.

                    for (int i = 0; i < keys.Length; i++)
                    {
                        sw.WriteLine(keys[i] + " " + vals[i].Trim());
                    }
                }
                using (StreamWriter sw = new StreamWriter("Data/intvar"))
                {
                    keys = id.Keys.ToArray<String>();
                    i_vals = id.Values.ToArray<float>();

                    for (int i = 0; i < keys.Length; i++)
                    {
                        sw.WriteLine(keys[i] + " " + i_vals[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string variable = "";
                    if (args[i].StartsWith("#") || args[i].StartsWith("@")) //if it starts with a variable dignifier, 
                    {
                        variable = args[i].Substring(1); //get the variable name 
                    }
                    else { variable = args[i]; }
                    string outv; //our outvalue for strings.
                    float i_outv; //our outvalue for ints.
                    bool validity = false; ; //wether or not the variable is valid. 
                    if (sd.TryGetValue(variable, out outv))
                    {
                        validity = true;
                        using (StreamWriter sw = File.AppendText("Data/strvar")) { sw.WriteLine(variable + " " + outv.Trim()); }
                    }
                    if (id.TryGetValue(variable, out i_outv))
                    {
                        validity = true;
                        using (StreamWriter sw = File.AppendText("Data/intvar")) { sw.WriteLine(variable + " " + i_outv); }
                    }
                    if (!validity) {
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Red; }
                        Console.WriteLine(GSR.Listing[3] + " " + args[i] + " " + GSR.Listing[10]);
                        if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                    }
                    else
                    {
                        if (COLORS) { Console.ForegroundColor = ConsoleColor.Cyan; }
                        Executer.output(card, GSR.Listing[3] + " " + variable + " " + GSR.Listing[46]);
                        if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
                    }

                    //Listing 3 = Variable translation.
                    //Listing 13 = Could not have been saved.  Perhaps...
                    //Listing 46 = was saved successfully.
                }

            }


        }

        public static void list(CommandCard card, Dictionary<string, string> sd, Dictionary<string, float> id)
        {
            bool COLORS = !Settings1.Default.cmdMode;
            Executer.output(card, GSR.Listing[11]);
            string[] keys = sd.Keys.ToArray<String>();
            string[] vals = sd.Values.ToArray<string>();
            for (int i = 0; i < keys.Length; i++)
            {
                if (COLORS) { Console.ForegroundColor = ConsoleColor.Green; }
                Executer.output(card, keys[i] + "=" + vals[i]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            }

            Executer.output(card, "");
            Executer.output(card, GSR.Listing[12]);
            keys = id.Keys.ToArray<String>();
            float[] ivals = id.Values.ToArray<float>();
            for (int i = 0; i < keys.Length; i++)
            {
                if (COLORS) { Console.ForegroundColor = ConsoleColor.DarkYellow; }
                Executer.output(card, keys[i] + "=" + ivals[i]);
                if (COLORS) { Console.ForegroundColor = Settings1.Default.foregroundColor; }
            }

        }
    }
}
