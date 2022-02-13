using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NoteChad
{
    /// @author Arttu Mäkinen
    /// @version 0.1
    /// 
    /// <summary>
    /// 
    /// </summary>
    public class NoteManager
    {

        IArgumentHandler argumentHandler;

        public NoteManager(IArgumentHandler argHandler)
        {
            argumentHandler = argHandler;
        }


        /// <summary>
        /// Creates a new note with given name and tags to working directory.
        /// Generates a file name based on the Note name given by the user (with special characters removed)
        /// If a note with same file name exists already, asks if the user wants to replace the old one.
        /// </summary>
        /// /// <param name="args">Arguments the application has received from terminal.</param>
        public void CreateNewNote(string[] args)
        {
            string directoryPath = Environment.CurrentDirectory;

            string noteName = argumentHandler.GetNoteName(args);
            string fileName = argumentHandler.GetFileName(args);
            string filePath = $"{directoryPath}\\{fileName}";

            string[] noteTagsArray = argumentHandler.GetNoteTags(args);
            string noteTags = string.Join(", ", noteTagsArray);

            if (File.Exists(filePath))
            {
                bool replaceFile = AskYesNoQuestion($"\nFile \"{fileName}\" already exists. Replace it?");
                if (replaceFile == false)
                {
                    Console.WriteLine($"\n\nCreation of \"{fileName}\" cancelled.");
                    MyAssert.CloseApp();
                }
            }

            string noteContents = $"@Name {noteName}\n\n" +
                                  $"@Tags {noteTags}";

            TextFileCreate(filePath, noteContents);
            TextFileOpen(filePath);
        }


        /// <summary>
        /// Asks a Yes/no (Y/n) question and waits for user input.
        /// </summary>
        /// <param name="question">The question to be asked</param>
        /// <returns>The answer as boolean</returns>
        private bool AskYesNoQuestion(string question)
        {
            while (true)
            {
                Console.WriteLine($"\n{question} Y/n");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        return true;
                    case ConsoleKey.N:
                        return false;
                    default:
                        Console.WriteLine("\nInvalid answer");
                        break;
                }
            }
        }

        /// <summary>
        /// Finds all notes which have matching NoteName, FileName or any of the NoteTags given in arguments.
        /// Makes also Regex search for note names so that "hello" queries for all notes with text "hello" in their name ("hello world,
        /// "hi_hello_bye.txt" etc)
        /// 
        /// Prints all the found Notes to terminal and gives the user ability sort the notes, peek contents in terminal and open them in terminal.
        /// </summary>
        /// <param name="args">Arguments the application has received from terminal.</param>
        public void Find(string[] args)
        {
            Console.Clear();

            string noteName = argumentHandler.GetNoteName(args);
            string fileName = argumentHandler.GetFileName(args);
            string[] noteTags = argumentHandler.GetNoteTags(args);
            string directoryPath = Environment.CurrentDirectory;

            List<Note> notes = GetNotesInDirectory(directoryPath, noteName, fileName, noteTags);

            if (notes.Count == 0)
            {
                Console.WriteLine("No notes found!");
                Console.ReadKey();
                MyAssert.CloseApp();
            }

            PrintQuery(notes, "", true);
            PrintQueryOptions(notes);

        }


        /// <summary>
        /// Queries for all the Notes in given directory (and subdirectories) with matching NoteName, FileName or NoteTags
        /// Seaching for note "hello" returns all notes with text "hello" in their name (including "hello", "hello word", "Hello network",
        /// "hello2.txt" etc)
        /// 
        /// </summary>
        /// <param name="directoryPath">Path of the directory to search from.</param>
        /// <param name="noteName">Name or part of the name to search for.</param>
        /// <param name="fileName">FileName or part of the FileName to seach for.</param>
        /// <param name="noteTags">Tags to search for (matching any of them is enough)</param>
        /// <returns></returns>
        private List<Note> GetNotesInDirectory(string directoryPath, string noteName, string fileName, string[] noteTags)
        {
            string fileType = "*.txt";

            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            FileInfo[] files = dir.GetFiles(fileType, SearchOption.AllDirectories);

            List<Note> notes = new List<Note>();

            foreach (FileInfo file in files)
            {
                Note note = new Note(file);
                if (note.Name == "") continue;

                notes.Add(note);
            }

            // Filters notes by name
            if (noteName != "" || fileName != "")
            {
                string[] noteNameParts = noteName.ToLower().Split(' ');
                notes.RemoveAll(note => !(
                            note.GetFileInfo().Name == fileName ||
                            note.Name.ToLower() == noteName.ToLower() ||
                            note.Name.ToLower().Split(' ').Intersect(noteNameParts).Any()
                    )
                );
            }

            // Filters notes by tag

            if (noteTags != null && noteTags.Length != 0)
            {
                notes.RemoveAll(
                    note => note.Tags.Intersect(noteTags).Any() == false
                );
            }
            return notes;
        }

        /// <summary>
        /// Prints all the option user has in terminal after "find" command such as "Sort", "Open", "Peek" and "Quit"
        /// and waits for input (and then acts accordingly)
        /// </summary>
        /// <param name="notes">List of notes</param>
        public void PrintQueryOptions(List<Note> notes)
        {
            Console.WriteLine("\n");
            Console.WriteLine(" [O]pen all (in text editor)");
            Console.WriteLine(" [P]eek summaries (in terminal)");
            Console.WriteLine(" [S]ort");
            Console.WriteLine(" [Q]uit");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.O:
                    notes.ForEach(note =>
                        TextFileOpen(note.GetFileInfo().FullName)
                    );
                    break;
                case ConsoleKey.P:
                    Console.Clear();
                    PeekSummariesInTerminal(notes.ToArray());
                    break;
                case ConsoleKey.Q:
                    MyAssert.CloseApp();
                    break;
                case ConsoleKey.S:
                    SortNotes(notes);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Asks whether the notes should be sorted in ascending or descending order and then calls PrintQuery with maching parameters)
        /// </summary>
        /// <param name="notes"></param>
        public void SortNotes(List<Note> notes)
        {
            Console.WriteLine("\nSort by:");
            Console.WriteLine("- [D]ate");

            string sortBy = "";
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D:
                    sortBy = "date";
                    break;
                case ConsoleKey.Q:
                    return;
            }

            Console.WriteLine("\n Order: \n - [A]scending \n - [D]escending");

            bool isAscending;
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.A:
                    isAscending = true;
                    break;
                case ConsoleKey.D:
                    isAscending = false;
                    break;
                default:
                    return;
            }
            PrintQuery(notes, sortBy, isAscending);
            PrintQueryOptions(notes);
        }


        /// <summary>
        /// Prints all the queried notes to terminal.
        /// </summary>
        /// <param name="notes">List of notes to print</param>
        /// <param name="sortBy">How to sort array, currently only supports empty "" and "date"</param>
        /// <param name="ascending">Whether to query is ascending (true) or descending (false)</param>
        public void PrintQuery(List<Note> notes, string sortBy, bool ascending)
        {
            Console.Clear();
            Note[] notesFromQuery;
            switch (sortBy)
            {
                case "date":
                    if (ascending) notesFromQuery = notes.OrderBy(note => note.GetDateTime()).ToArray();
                    else notesFromQuery = notes.OrderByDescending(note => note.GetDateTime()).ToArray();
                    break;

                default:
                    notesFromQuery = notes.ToArray();
                    break;
            }

            int nameMaxLength = notesFromQuery.Max(note => note.Name.Length);
            string nameAligment = nameMaxLength.ToString();

            int fileNameMaxLength = notesFromQuery.Max(note => note.GetFileInfo().Name.Length);
            string fileNameAligment = fileNameMaxLength.ToString();

            int folderMaxLength = notesFromQuery.Max(note => note.GetFileInfo().Directory.Name.Length);
            if (folderMaxLength < ("Folder").Length) folderMaxLength = ("Folder").Length;

            string folderAligment = folderMaxLength.ToString();

            string tableProperties = String.Format(" {0,-" + nameAligment + "} | {1,-" + fileNameAligment + "} | {2,-" + folderAligment + "} | {3}", "NoteName", "FileName", "Folder", "Date");
            Console.WriteLine(tableProperties);
            Console.WriteLine("" + new String('-', tableProperties.Length));

            foreach (Note note in notesFromQuery)
            {
                string noteDate = note.Date;
                if (noteDate == "") noteDate = "";
                Console.WriteLine(" {0,-" + nameAligment + "} | {1,-" + fileNameAligment + "} | {2,-" + folderAligment + "} | {3}", note.Name, note.GetFileInfo().Name, note.GetFileInfo().Directory.Name, note.Date);
            }


            string[] tags = GetAllTagsFromNotes(notes);
            string tagsString = String.Join(", ", tags);
            tagsString = SpliceText(tagsString, 60);

            Console.Write($"\n Tags:\n ------\n{tagsString}");
        }

        /// <summary>
        /// Prints first paragraphs of the given notes to terminal.
        /// </summary>
        /// <param name="notes">List of notes</param>
        public void PeekSummariesInTerminal(Note[] notes)
        {
            Console.WriteLine();
            Array.ForEach(notes,
                note =>
                {
                    string noteDate = note.Date;
                    if (noteDate != "") noteDate = " - " + noteDate;
                    string tagsString = String.Join(",", note.Tags);
                    if (tagsString != "") tagsString = "[" + tagsString + "]";
                    string summary = note.Summary;
                    summary = SpliceText(summary, 60);
                    string dashes = new String('-', note.Name.Length);
                    Console.WriteLine($"\n{note.Name}{noteDate}\n{dashes}\n{summary}");
                }
            );
        }


        /// <summary>
        /// Finds all tags from given notes and puts them to array which is sorted from the most used tag to the
        /// least used tag.
        /// </summary>
        /// <param name="notes"></param>
        /// <returns>Array of tags sorted from the most popular to least used.</returns>
        public string[] GetAllTagsFromNotes(List<Note> notes)
        {
            Dictionary<string, int> tags = new Dictionary<string, int>();

            foreach (Note note in notes)
            {
                foreach (string tag in note.Tags)
                {
                    if (!tags.ContainsKey(tag)) tags.Add(tag, 0);
                    tags[tag]++;
                }
            }

            var sortedTagsQuery =
                from tag in tags
                orderby tag.Value
                descending
                select tag;
            Dictionary<string, int> sortedTags = sortedTagsQuery.ToDictionary(pair => pair.Key, pair => pair.Value);

            return sortedTags.Keys.ToArray();
        }


        /// <summary>
        /// Method stolen from internet.
        /// 
        /// Inserts line breaks to string based on maximum line length given in arguments.
        /// </summary>
        /// <param name="text">Text to add linebreaks to</param>
        /// <param name="lineLength">The maximum line length</param>
        /// <returns></returns>
        public string SpliceText(string text, int lineLength)
        {
            return " " + Regex.Replace(text, @"(.{1," + lineLength + @"})(?:\s|$)", "$1\n ");
        }


        /// <summary>
        /// Opens the first Note with matching NoteName or FileName
        /// </summary>
        /// <param name="args"></param>
        public void Open(string[] args) //should be in NoteReader class
        {
            string noteName = argumentHandler.GetNoteName(args);
            string fileName = argumentHandler.GetFileName(args);
            string directoryPath = Environment.CurrentDirectory;

            MyAssert.Assert(noteName == "", "\"Open\" command needs Note Name or File Name as argument");
            MyAssert.Assert(fileName == "", "\"Open\" command needs Note Name or File Name as argument");

            List<Note> notes = GetNotesInDirectory(directoryPath, noteName, fileName, null);

            if (notes.Count == 0)
            {
                Console.WriteLine("No note with corresponding name exists!");
                Console.ReadKey();
                MyAssert.CloseApp();
            }

            Note note = notes[0];
            string filePath = note.GetFileInfo().FullName;

            TextFileOpen(filePath);
        }


        /// <summary>
        /// Creates a text file to path with given contents.
        /// </summary>
        /// <param name="filePath">File path for the text file</param>
        /// <param name="text">Contents of the text file</param>
        public void TextFileCreate(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }


        /// <summary>
        /// Opens text file in default text editor.
        /// </summary>
        /// <param name="filePath"></param>
        public void TextFileOpen(string filePath)
        {
            System.Diagnostics.Process.Start(/*"notepad++",*/ filePath);
        }
    }
}


/*
NoteReader.ParseJSON(root_folder);

NoteReader.GetName(path\name\GUID?);
NoteReader.GetSummary();
NoteReader.GetText();
NoteReader.GetTags();

NoteReader.SetName();
NoteReader.SetText();
NoteReader.SetTags(); (also reads tags from the file is exists)

NoteReader.ParseJSON(path\name\GUID?);
NoteReader.Parse

Should we keep reading the file structure to find new files?
HASH list to keep reading file changes? Or just current notes?

public class Note() 

*/
