using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace Wash.Components
{
    /***************************
     * 
     * AliasTable
     * Created by Lane S. Hill
     * Provides a special table to keep aliases in.
     * 
     * Variables:
     * array - the table itself
     * count - the number of entries in the table.
     * 
     * Functions:
     * constructor
     * get/set
     * search - searches the table for a string, returns the AE.
     * 
     * 
     */
    class AliasTable
    {
        private ArrayList array = new ArrayList(); //What we hold the actual AliasEntries used. ArrayLists are used because we are not worried about type safety here - we control the process in which this table is populated.
        public ArrayList Array
        {
            get { return array; }
            set { array = value; }
        }
        public int count
        {
            get{return array.Count;} //so we don't have to call array.Count everytime we want the count of the table.
        }
        //default constructor.
        public AliasTable()
        {
            this.populate();
        }
        
        /******************
         * add
         * 
         * Adds an entry to the table.
         */
        public void add(AliasEntry ae)
        {
            this.Array.Add(ae);
        }
        
        /************************************
         * search
         * 
         * Searches the table, comes back with an AE that corresponds with the command.
         * 
         */
        public AliasEntry search(string str)
        {
            foreach (AliasEntry ae in this.array)
            {
                if (String.Equals(ae.Alias0, str) || String.Equals(ae.Alias1, str) || String.Equals(ae.Alias2, str))
                {
                    return ae;
                }
            }
            return null; //the entry does not exist
        }
        //populate
        //populates the AliasTable from the Data/alias file
        public void populate()
        {
            using (StreamReader sr = new StreamReader("Data/alias"))
            {
                try
                {
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        String[] toks = line.Split(); //split line based on the ' ' character.
                        if (toks.Length > 5) //we have a space in the file path.
                        {
                            string path = "";
                            for (int i = 4; i < toks.Length; i++) //i = 3.  
                            {
                                path = path + toks[i] + " ";
                            }
                            toks[4] = path.Trim();
                            string[] new_toks = new string[5];

                            for (int i = 0; i < new_toks.Length; i++)
                            {
                                new_toks[i] = toks[i];
                            }

                            toks = new_toks; //reassign the array.
                        }
                        this.add(new AliasEntry(toks));
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(GSR.Listing[1] + e.Message); //Listing[1] is translation for error.
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            }

        }

    }
}
