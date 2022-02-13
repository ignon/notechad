using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NoteChad
{

    /// @author Arttu Mäkinen
    /// @version 0.1
    /// 
    /// <summary>
    ///  A class which has methods for parsing command line parameters.
    ///  Also used to define Notechad commands (with name, description and corresponding method)
    /// </summary>
    public class ArgumentHandler : IArgumentHandler
    {


        /// <summary>
        /// A property which returns an array of all the defined Commands.
        /// </summary>
        public Command[] Commands { get; private set; }

        /// <summary>
        /// Used to define all available Notechad Commands.
        /// </summary>
        /// <param name="commands"></param>
        public void DefineCommands(params Command[] commands)
        {
            Commands = commands;
        }

        /// <summary>
        /// Parses the Terminal input and returns a Command object matching the command call.
        /// Prints an error message and ends the program if the command call doesn't match with any Notechad Command.
        /// </summary>
        /// <param name="args">Terminal input (the string[] args array the Main() method receives from terminal)</param>
        /// <returns>Command object corresponding to the terminal call.</returns>
        public Command GetCommand(string[] args)
        {
            MyAssert.Assert(args.Length == 0, "NoteChad++ needs arguments to run. Write \"notechad help\" for documentation.");

            string commandName = args[0];

            Command command = Array.Find(Commands,
                com => com.Name == commandName
            );

            MyAssert.Assert(command == null, "No command with such a name exists! Write \"notechad help\" for documentation");

            return command;
        }


        /// <summary>
        /// Generates a file name for the note based on the terminal call / user input. 
        /// </summary>
        /// <param name="args">Terminal input (the string[] args array the Main() method receives from terminal)</param>
        /// <returns>File name (as a string)</returns>
        public string GetFileName(string[] args)
        {
            string noteName = GetNoteName(args);
            string fileName = GetFileName(noteName);
            return fileName;
        }


        /// <summary>
        /// Generates a file name for the note based on the note name.
        /// </summary>
        /// <param name="noteName">noteName</param>
        /// <returns>File name (as a string)</returns>
        public string GetFileName(string noteName)
        {
            string fileName = noteName;
            fileName = RemoveIllegalFileNameCharacters(fileName);
            fileName = fileName.ToLower();
            fileName = Regex.Replace(fileName, @"\s", "");
            fileName = Regex.Replace(fileName, @".txt$", "");
            if (fileName == "") return "";
            fileName += ".txt";
            return fileName;
        }


        /// <summary>
        /// Generates a note name for the note based on the terminal call / user input. 
        /// </summary>
        /// <param name="args"> The string[] args array the Main() method receives</param>
        /// <returns>Note name (as a string)</returns>
        public string GetNoteName(string[] args)
        {
            if (args.Length < 2) return "";

            string name = "";

            for (int i = 1; i < args.Length; ++i)
            {
                string keyword = args[i];
                bool isTag = keyword[0] == '#';
                if (isTag) break;

                if (name != "") name += " ";
                name += keyword;
            }

            if (name != "") return name;
            return "";
        }


        /// <summary>
        /// Returns note tags based on the terminal call / user input.
        /// </summary>
        /// <param name="args">The string[] args array the Main() method receives</param>
        /// <returns>Note tags (as an array)</returns>
        public string[] GetNoteTags(string[] args)
        {
            string[] tags = Array.FindAll(args, tag => (tag[0] == '#'));

            for (int i = 0; i < tags.Length; ++i)
            {
                string tag = tags[i];
                tag = tag.Remove(0, 1); // removes '#' aka hashtag
                tag = RemoveSpecialCharacters(tag);
                tags[i] = tag;
            }

            string[] acceptedTags = Array.FindAll<string>(tags, tag => tag.Length > 0);
            return acceptedTags;
        }


        /// <summary>
        /// Removes illegal characters from string (mainly used to parse proper file names)
        /// Also removes scandic letters, white space and dots to avoid possible operating system and text format issues.
        /// </summary>
        /// <param name="name">File name with illegal characters removed</param>
        /// <returns></returns>
        private string RemoveIllegalFileNameCharacters(string name)
        {
            string[] charPairs = ("äa|öo|åo| _|._").Split('|');

            foreach (string pair in charPairs)
            {
                char illergalChar = pair[0];
                char substituteChar = pair[1];
                name = name.Replace(illergalChar, substituteChar);
            }

            name = string.Join("", name.Split(Path.GetInvalidFileNameChars()));

            return name;
        }


        /// <summary>
        /// Removes everything expect letters, numbers, underscores, lines and commas.
        /// (used mainly to format Tags)
        /// </summary>
        /// <param name="str">String to remove special characters from</param>
        /// <returns>String with special characters removed.</returns>
        public string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, @"[^\w\d\._\-,]+", "", RegexOptions.Compiled);
        }


        /// <summary>
        /// Prints all available commands (name and description).
        /// Used to print help page.
        /// </summary>
        public void PrintCommands()
        {
            Console.WriteLine("\nAvailable commands:");
            foreach (Command command in Commands)
            {
                Console.WriteLine("  {0,-10}{1}", command.Name, command.Description);
            }
        }
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

