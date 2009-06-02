using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wash.Components;

namespace Wash //Namespace is Wash because that was the original name: Windows Application Shell
{
    /*******************
     * Program
     * 
     * The main entry point into the application.
     * 
     * Fields:
     * None
     * 
     * Methods:
     * LoadVariables
     * Main
     */
    class Program
    {
        /*LoadVariable
         * 
         * Takes the two empty variable dictionaries, and returns the full array from the intvar and strvar files.
         */
        static void LoadVariables(Dictionary<String, float> ivart, Dictionary<String, String> svart)
        {
            try
            {
                using (StreamReader sr = new StreamReader("Data/intvar"))
                {
                    while (!sr.EndOfStream)
                    {
                        string l = sr.ReadLine();
                        string[] ls = l.Split(); //key is the ls[0], value is ls[1].
                        float val = float.Parse(ls[1]);
                        ivart.Add(ls[0], val);
                    }
                }
                using (StreamReader sr = new StreamReader("Data/strvar"))
                {
                    while (!sr.EndOfStream)
                    {
                        string l = sr.ReadLine();
                        string val = "";
                        string[] ls = l.Split(); //key is ls[0], value is ls[1].
                        if (ls.Length > 2)
                        {
                            for (int i = 1; i < ls.Length; i++) //for values with spaces in the name.
                            {
                                val = val + ls[i] + " ";
                            }
                        }
                        else { val = ls[1]; } //otherwise, value of the variable is in ls[1].
                        svart.Add(ls[0], val.Trim());
                    }
                }
            }
            catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[1] + e.Message); Console.ForegroundColor = ConsoleColor.Gray; }
        }

        /******
         * Main
         * 
         * The entry point of the application.
         */
        static void Main(string[] args)
        {   //Create the global string resource.
            try
            {
                StreamReader sr = new StreamReader("Data/strings");
                sr.Close();
            }
            catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[44]); Console.ForegroundColor = ConsoleColor.Gray; return; }
            GSR.getStrings(); 
            //Create the variable dictionaries. 
            Dictionary<String, float> intvart = new Dictionary<String, float>();
            Dictionary<String, String> strvart = new Dictionary<String, String>();
            


         //load variable values.
            LoadVariables(intvart, strvart);
            AliasTable at = new AliasTable();
            FileSystemGlob fsg = new FileSystemGlob();
            Console.WriteLine(GSR.Listing[0]); //WELSH INFORMATION
           // Console.WriteLine("Created by Lane S. Hill");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(fsg.Current_Pointer);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(">");
                String tok = Console.ReadLine();
                    String[] toks = tok.Split(); //splits the string as spaces the delimitator.
                    if (toks[0].Equals(GSR.Listing[6]) || toks[0].Equals(GSR.Listing[7])) //Listing[29] and Listing[30] are quit and exit respectively.
                    {
                        Console.WriteLine(GSR.Listing[8]); //this is "Goodbye."
                        break;
                    }
                    CommandCard[] deck = CommandCard.createDeck(toks, fsg, at, intvart, strvart);
                    if (deck == null)
                    {
                       Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(GSR.Listing[9]); //this is the Command Not Found error.
                       Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Executer.execute(deck, at, fsg, strvart, intvart); //execute the deck.
                    }
                   
                }
            }
        }
    }
    