using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wash.Components;

namespace Wash.Components
{
    /************************
     * Alias Entry
     * Created by Lane S. Hill
     * Provides an entry for each alias.
     * 
     * An alias is an another name for a central command or a path.
     * 
     * A central command is a command that only Wash understands.
     * 
     * The AliasEntry class defines an Alias Entry for the AliasTable, which is
     * a list of Aliases to run commands off of.
     * 
     * Variables:
     * a0, a1, a2 - actual aliases.
     * mode - provides the Alias Mode.
     * c_central - the central command or the path.
     * 
     * Functions:
     * constructor
     * get/set.
     * 
     */
    class AliasEntry
    {
        private string a0 = "";
        public string Alias0 //alias 1
        {
            get { return a0; }
            set { a0 = value; }
        }
        private string a1 = "";
        public string Alias1 //alias 2
        {
            get { return a1; }
            set { a1 = value; }
        }
        private string a2 = "";
        public string Alias2 //alias 3
        { //alias 3
            get {return a2;}
            set { a2 = value; }
         }
        private AliasMode mode = AliasMode.c_central;
        public AliasMode Mode //the mode of the c_central value.
        {
            get { return mode; }
            set { mode = value; }
        }
        private string c_central = "";
        public string Central_Command //what actually runs when the alias is ran.
        {
            get { return c_central; }
            set { c_central = value; }
        }
        //constructor #1: if you have three spare string values.
        public AliasEntry(string a, string b, string c, AliasMode m, string c_c)
        {
            a0 = a;
            a1 = b;
            a2 = b;
            mode = m;
            c_central = c_c;
        }
        //constructor #2: if you have an array of values that have # entries > 3
        public AliasEntry(string[] arr, AliasMode m, string c_c)
        {
            a0 = arr[0];
            a1 = arr[1];
            a2 = arr[2];
            mode = m;
            c_central = c_c;
        }
        //constructor #3: with an array.
        public AliasEntry(string[] arr)
        {
            a0 = arr[0];
            a1 = arr[1];
            a2 = arr[2];
            mode = (AliasMode)Int16.Parse(arr[3]); //Parse the mode. Int16 is used as there were only 2 modes at the moment, which map to 0 and 1.
            c_central = arr[4];
        }

    }
}
