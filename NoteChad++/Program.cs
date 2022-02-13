using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace NoteChad
{
    /// @author Arttu Mäkinen
    /// @version 0.1
    /// 
    /// <summary>
    /// The class defines all Notechad terminal commands (new, open, find, help etc) 
    /// and implements corresponding methods for them.
    /// </summary>
    public class Program
    {
        public static IArgumentHandler argumentHandler = new ArgumentHandler();
        public static NoteManager noteManager = new NoteManager(argumentHandler);

        /// <summary>
        /// Initializes Notechad commands
        /// Parses the command call from the terminal input and executes it.
        /// </summary>
        /// <param name="args">Arguments the application has received from terminal.</param>
        public static void Main(string[] args)
        {
            InitializeCommands();
            Command command = argumentHandler.GetCommand(args);
            command.Execute(args);
            Console.ReadKey();
        }


        /// <summary>
        /// Initializes Notechad commands (new, open, find help etc) and assigns them the corresponding method
        /// and description (which is used when printing the help document).
        /// 
        /// ArgumentHandler.DefineCommands and its arguments are wrapped inside NoteChad.InitializeComands()
        /// to make testing easier.
        /// </summary>
        public static void InitializeCommands() => argumentHandler.DefineCommands(
            new Command("new", noteManager.CreateNewNote, "creates a new note. for example: \"new Second World War #axis_powers %1936-1945\""),
            new Command("find", noteManager.Find, "finds all files in current folder with corresponding name or tag"),
            new Command("open", noteManager.Open, "opens file matching arguments"),
            new Command("help", PrintHelpPage, "print all available command and their arguments.")
        );


        /// <summary>
        /// Prints all the available Notechad commands (new, open, find help etc)
        /// and their descriptions.
        /// </summary>
        /// /// <param name="args">Arguments the application has received from terminal.</param>
        private static void PrintHelpPage(string[] args)
            => argumentHandler.PrintCommands();
    }
}




/*
notechad "Toinen maailmansota" -d
    - no such note exists. create one?
    - generates toinen_maailmansota.txt to working directory and opens in default editor (or one set by user)
    - generates @name summary on first line @tags $name &name %name £name !@tags @datetime
notechad find "Toinen maailmansota" - searches for toinen_maailmansota
notechad find #hitler #toinen_maailmansota sort date|name|created|modified
notechad find 1941
notechad find 1935-1945
notechad find %sotamies_ryan

[S]ort files
[O]pen all
    Open in: [S]eperate files [Read/Write] | [O]ne file[Read only]
[P]eek all
    [O]pen in editor [E]xit 
[A]nki




Name			File			Folder		Date		Created		Modified		Opened	
[H]itler		hitler.txt		natsisaksa	1889-1945	14/03/2018	20/03/2018		19/08/2020
[N]atsisaksa	natsisaksa.txt	natsisaksa	1935-1945	10/03/2018	21/03/2018		19/08/2020


- read file to string(builder? - readonly?) 
- read with regex: name, tldr, note, [List]tags
[
    {
        name: "Note name",
        tldr: "Summary about note",
        tags: ["note", "notechad", "example"],
        path: [root]+"tunti1/\note_name.txt"
    }
]

On launch:
Note.ParseJSON(root_folder);



Note.GetName(path\name\GUID?);
Note.GetSummary();
Note.GetText();
Note.GetTags();

Note.SetName();
Note.SetText();
Note.SetTags(); (also reads tags from the file is exists)

Note.ParseJSON(path\name\GUID?);
Note.Parse



Should we keep reading the file structure to find new files?
HASH list to keep reading file changes? Or just current notes?

public class Note() 


*/

