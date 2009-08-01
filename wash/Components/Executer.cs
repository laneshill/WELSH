using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Wash.Applets;
/***************************
 * Executer
 * Created by Lane S. Hill
 * 
 * 
 * Responsible for executing all built in commands.
 * 
 * Fields: 
 * deck - the deck of CommandCards that were passed to the Executer.
 * 
 * Methods:
 * output
 * execute
 * 
 * All internal commands have been moved into "Applets" to make editing easier.
 * 
 */
namespace Wash.Components
{
    class Executer
    {
        static private CommandCard[] deck;
        public static CommandCard[] Deck
        {
            get { return deck; }
            set { deck = value; }
        }
        /***************
         * output
         * 
         * Takes in a CommandCard and a string, and outputs the string according to the Command Card.
         */
        public static void output(CommandCard card, string output)
        {
            if (card.PUT_OUTPUT_IN_FILE) //write to file
            {
                using (StreamWriter sw = new StreamWriter(card.OUTPUT_FILE))
                {
                    sw.WriteLine(output);
                }
            }

            else if (!card.WORK_BACKGROUND) //if it's not supposed to work in background, then display in console.
            {
                Console.WriteLine(output);
            }
            card.Output = card.Output + output;//write output to output.
        }
        /*****************
         * 
         * execute
         * 
         * Accepts a deck of CommandCards, the AliasTable, the FileSystemGlob, the two variable dictionaries.
         * 
         * Executes a built in method based on the command on the CommandCard.
         */
        public static void execute(CommandCard[] cards, AliasTable at, FileSystemGlob fsg, Dictionary<string, string> sd, Dictionary<string, float> id)
        {
            Executer.deck = cards;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].Command.Contains(":\\")) //this is not a built in method, but a path.
                {
                    try
                    {
                        Process.Start(cards[i].Command);
                    }
                    catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(GSR.Listing[1] + e.Message); Console.ForegroundColor = ConsoleColor.Gray; }
                    break;
                }
                switch (cards[i].Command)
                {
                    case "view": FileViewer.run(cards[i], fsg);
                        break; //view a file.
                    case "cd": ChangeDirectory.run(cards[i], fsg, i);
                        break;  //change directory
                    case "ls": DirectoryLister.run(cards[i], fsg, i);
                        break; //list elements in directory.
                    case "save": VariableOps.save(cards[i], i, sd, id);
                        break; //saves both dictionaries to file.
                    //v.2: case "cs": break; //open a sever connection.
                    //v2 :case "con": break; //connect to a server.
                    case "info": FileViewer.infoView(cards[i], i, fsg);
                        break; //view information about the file
                    case "proc": ProcessOps.list(cards[i]);
                        break; //view processes
                    case "kill": ProcessOps.kill(cards[i]);
                        break; //kill a process
                    case "rm": FileOps.remove(cards[i], fsg);
                        break; //remove a folder or file
                    case "new": FileOps.create(cards[i], fsg);
                        break; //create a new folder or file
                    case "add": FileOps.addTo(cards[i], fsg, i);
                        break; //add a text to a file.
                    case "conf": break; //look at and edit the configuration file.
                   
                    case "exec": FileOps.executeScript(cards[i], fsg, at, id, sd);
                        break; //execute a script.
                    //v.3 or v.4: case "import": break; //import a script/modue.
                    case "copy": FileOps.copy(cards[i], fsg);
                        break; //only copy a file (in the way of xeroxing)
                    case "bck": NavigationOps.goBack(fsg);
                        break; //go back
                    case "fwd": NavigationOps.goForward(fsg);
                        break; //go forward
                    case "up": NavigationOps.goUpALevel(fsg);
                        break; //go up a level.
                    case "clr": Console.Clear();
                        break; //clear screen.
                    
                    //v.2: case "attr": break; //change attributes 
                    case "filter": Filterer.run(cards[i], i);
                        break; //filter strings.
                    case "vars": VariableOps.list(cards[i], sd, id);
                        break; //create a list of variables.
                    case "explore": FileOps.explore(cards[i], fsg);
                        break; //opens up explorer on the argument.
                    case "move": FileOps.move(cards[i], fsg);
                                break; //moves a file (aka copy and paste).
                    case "protect": FileOps.protect(cards[i], fsg);
                        break; //add a file to the protected file list.
                    case "alias": FileOps.alias(cards[i], fsg, at);
                        break; //adds a new alias to the alias file.
                    case "google": NetOps.google(cards[i]);
                        break; //searches google for the word supplied - Google opens in a Web Browser.
                    case "wiki": NetOps.wiki(cards[i]);
                        break; //searches Wikipedia for the word supplied - Wikipedia opens in a Web Browser.
                    case "echo": Echoer.run(cards[i], i);
                                break; //echos the string to output.
                    case "time": Watch.run(cards[i], i);
                                break;
                }
            }

        }
                

            }
        }
