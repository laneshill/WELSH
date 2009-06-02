using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/********************
 * GSR
 * 
 * GSR is a wrapper around a list of strings.  GSR is global, and can be accessed in any class.  
 * This is because GSR is in charge of the strings that WELSH uses.
 * 
 * Fields:
 * Listing
 * 
 * Methods:
 * getString 
 * 
 * Listing of Strings:
 * 0:WELSH:Windows Easy-Language Shell 1.0 BETA
1:Error:
2:Format mismatch!  Make sure you're not putting alphabetical characters in integer variables!
3:Variable
4:has not been declared yet.
5:from
6:quit
7:exit
8:Goodbye.
9:I'm sorry, but the command was not found.
10:could not be saved; perhaps, it has not been declared?
11:Strings:
12:Integers:
13:File:
14:Attributes:
15:Couldn't obtain file attribute information.
16:Access Control:
17:Couldn't obtain file access information.
18:Last Access:
19:Couldn't obtain last access time.
20:Last Write:
21:Couldn't obtain last write time.
22:Creation Time:
23:Operation failed.  Double check your quotes - you might've missed one.
24:Problem deleting file:
25:File deleted successfully.
26:File created successfully.
27:Folder created successfully.
28:Problem creating file:
29:File is protected and cannot be deleted.
30:Adding to document failed either by the file not bieng found, or argument mismatch.
31:Text written to
32:Script does not exist.
33:(WISE ERROR): Label or Method Not Found.
34:(WISE WARNING): Batch File support is beta!  Do not use if you absolutly need your files to run!
35:(WISE ERROR): Label or Method already created.
36:File protected.
37:Copy of
38:File copied.
39:Can't go up one level here.
40:Insert an easy to remember name for this file:
41:Insert an alternate name for this file:
42:Insert a second alternate name for this file:
43: Do not use string variables in a calculator expression.
44: You do not have the proper dependencies!  Make sure your system is up to date, and you have the .NET Framework 3.5 and SP 1 installed.
45: Current Location:
46: was saved successfully.
47: Cannot find process.
48: Process killed.
49: Cannot kill process at the moment.
50: Alias successfully created.
51: Could not successfully create alias:
52: File moved successfully.
53: red
54: blue
55: yellow
56: gray
57: cyan
58: green
59: magenta
60: white
61: black
62: day
63: d
64: hour
65: h
66: minute
67: m
68: second
69: s
70: date
71: a
72: clock
73: c
 */
namespace Wash.Components
{
    class GSR
    {
        private static List<string>  dict = new List<string>();
        public static List<string> Listing
        {
            get { return dict; }
            set { dict = value; }
        }

        /*************
         * getStrings
         * 
         * Get the strings from the Data/strings file.
         */
        public static void getStrings()
        {
            using (StreamReader sr = new StreamReader("Data/strings"))
            {
                while (!sr.EndOfStream)
                {
                    string l = sr.ReadLine();
                    if (!l.Contains("#")){
                       // Console.WriteLine(l);
                        Listing.Add(l);}
                }
            }

        }
    }
}
