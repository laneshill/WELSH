using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Wash.Components
{
    /********************************************
     * FileSystemGlob
     * 
     * Contains a pointer to the current position, the children of the position,
     * a list of 10 historic entries, etc.
     * 
     * 
     * Fields:
     * Current_Pointer - current pointer in directory tree.
     * Current_Children - current children of the current pointer in directory tree.
     * History - history of current pointers
     * Protected - The list of protected files.
     * HistoryPointer - current position in History.
     * 
     * Methods:
     * -constructer, get/sets
     * -update: updates everything based on the current position.
     * -changeDirectory: updates pointer based on a filepath, and runs update.
     * 
     */
    class FileSystemGlob
    {
        private string pointer = ""; //pointer to the current location.
        public string Current_Pointer
        {
            get { return pointer; } 
            set { pointer = value; }
        }

        private List<String> children = new List<String>(); //children of the current list.
        public List<String> Current_Children
        {
            get { return children; }
            set { children = value; }
        }



        private List<String> history = new List<String>(); //history of the pointers.
        public List<String> History
        {
            get { return history; }
            set { history = value; }
        }

        private List<String> protectd = new List<String>();
        public List<String> Protected
        {
            get { return protectd; }
            set { protectd = value; }
        }

        private int hp = 0; //position of where you are in history.
        public int HistoryPosition
        {
            get { return hp; }
            set { hp = value; }
        }

        //constructor is also in charge of adding the Protected list.
        public FileSystemGlob()
        {
            this.changeLocation(Settings1.Default.startDirectory);
            //now we populate the protected list.
            using(System.IO.StreamReader sr = new System.IO.StreamReader("Data/protected"))
            {
                while (!sr.EndOfStream)
                {
                    string pv = sr.ReadLine();
                    Protected.Add(pv);
                }
            }
        }
    
        
        /****************
         * 
         * changeLocation
         * 
         * Changes the current pointer, adds the previous current pointer to history, and then sdds the current children.
         * 
         * Subdirectories are first on the list of current Children, and it returns whetrher or not the change succeeded.
         */
        public bool changeLocation(string file_path)
        {
           // if(file_path.StartsWith("...")){ //... is the symbol of a relative path.
           //     file_path = file_path.Remove(0, 3); //deletes the first three characters.
            //file_path = this.Current_Pointer + file_path;
           // }
            string old_pointer = this.Current_Pointer;
            this.Current_Pointer = file_path;
            
            this.History.Add(file_path); //add to the history.
            this.HistoryPosition = this.History.Count;
            try
            {
                //file_path = System.IO.Path.GetDirectoryName(file_path); //get all but the file, just in case.
                string[] dirs = System.IO.Directory.GetDirectories(file_path); //get all the directories.
                string[] files = System.IO.Directory.GetFiles(file_path); //get all the files in the subdirectory
                this.Current_Children.Clear();
                children.Clear();
                foreach (string s in dirs)
                {
                    
                    this.Current_Children.Add(System.IO.Path.GetFileName(s));
                }
                foreach (string s in files)
                {
                    this.Current_Children.Add(System.IO.Path.GetFileName(s));
                }
                return true;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (e.Message.Contains("denied"))
                    Console.WriteLine(GSR.Listing[74]);
                else if (e.Message.Contains("not find"))
                    Console.WriteLine(GSR.Listing[81]);
                else
                    Console.WriteLine(GSR.Listing[39]);

                Console.ForegroundColor = ConsoleColor.Gray;
                this.History.Remove(this.Current_Pointer);
                this.HistoryPosition--;
                this.Current_Pointer = old_pointer;
                
                return false;
            }
        }


        }


        

    }