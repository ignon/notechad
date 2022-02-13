using System;

namespace NoteChad
{
    /// @author Arttu Mäkinen
    /// @version 0.1
    /// 
    /// <summary>
    /// Live assert methods used to validate user input from terminal.
    /// </summary>
    public class MyAssert
    {


        /// <summary>
        /// Validates the user terminal input and closes the application after printing errorMessage
        /// </summary>
        /// <param name="test">A boolean test to validate</param>
        /// <param name="errorMessage">The error message to print before the app is closed</param>
        public static void Assert(bool test, string errorMessage)
        {
            if (test)
            {
                Console.WriteLine(errorMessage);
                Console.ReadKey();
                CloseApp();
            }
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        public static void CloseApp()
            => Environment.Exit(0);
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

