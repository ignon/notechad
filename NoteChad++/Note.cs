using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;


namespace NoteChad
{
    /// @author Arttu Mäkinen
    /// @version 0.1
    /// 
    /// <summary>
    /// Interface for the Note class.
    /// </summary>
    interface INote
    {

        /// <summary>
        /// All the text in Note .txt file (including text and metadata like @name, @tags & @date)
        /// </summary>
        /// <returns> All the text in Note .txt file</returns>
        string NoteContents { get; }


        /// <summary>
        /// Name of the Note (text after @name tag)
        /// </summary>
        string Name { get; }


        /// <summary>
        /// The first paragraph after @name tag. 
        /// </summary>
        /// <remarks>
        /// Should include a brief summary about rest of the note contents.
        /// </remarks>
        string Summary { get; }


        /// <summary>
        /// All the text in the Note expect metadata (@name,@tags etc)
        /// Includes summary.
        /// </summary>
        ///
        /// <remarks>
        /// for example:
        /// 
        /// """
        /// @name Hello World
        /// Summary
        /// 
        /// Text
        /// @tags hello, world
        /// """
        /// 
        /// returns: "Summary\n\nText"
        /// </remarks>
        string TextBody { get; }


        /// <summary>
        /// An array of all the tags after @tags
        /// </summary>
        /// 
        /// <remarks>
        /// for example "@tags hi, hello, bye" returns array {"hi", "hello", "bye"}
        /// </remarks>
        string[] Tags { get; }     
        

        /// <summary>
        /// FileInfo object of the Note text file.
        /// </summary>
        /// <returns>FileInfo of the Note</returns>
        FileInfo GetFileInfo();


        /// <summary>
        /// @Date of the note in the format it is found from the text file.
        /// </summary>
        string Date { get; }

        /// <summary>
        /// DateTime of note note (based on @Date line / Date property)
        /// </summary>
        /// <returns>DateTime of the note</returns>
        DateTime GetDateTime();

    }


    /// @author Arttu Mäkinen
    /// @version 0.1
    /// 
    /// <summary>
    /// A class describing the Note text file. 
    /// Has properties for note name, summary, text body, date and datetime.
    /// 
    /// Properties are documented only in the INote interface to avoid repetition.
    /// </summary>
    public class Note : INote
    {
        private IArgumentHandler argumentHandler = Program.argumentHandler;

        private string _noteContents;
        private string _name;
        private string _summary;
        private string _textBody;
        private string _date;
        private DateTime? _dateTime;

        private FileInfo _fileInfo = null;
        private string[] _tags = null;
        

        /// <summary>
        /// Constructs the object based on FileInfo object which is used to read text from Note file.
        /// </summary>
        /// <param name="file"></param>
        public Note(FileInfo file)
        {
            _noteContents = LoadNoteContents(file.FullName);
            _fileInfo = file;
        }


        /// <summary>
        /// Constructs the object based on the Note text / note contents string.
        /// Used mostly for making testing easier (so that there is no need to write/read files from harddrive when testing)
        /// </summary>
        /// <param name="noteContents"></param>
        public Note(string noteContents)
        {
            _noteContents = noteContents;
        }

        public string NoteContents { get; private set; }


        /// <summary>
        /// Note properties are already documented in the INote interface.
        /// The implementation loads properties only when needed to increase performance.
        /// </summary>
        public string Name
        {
            get
            {
                if (_name == null) _name = LoadNoteName(_noteContents);
                return _name;
            }
        }


        public string Summary
        {
            get
            {
                if (_summary == null) _summary = LoadNoteSummary(_noteContents);
                return _summary;
            }
        }


        public string TextBody
        {
            get
            {
                if (_textBody == null) _textBody = LoadNoteTextBody(_noteContents);
                return _textBody;
            }
        }


        public string[] Tags
        {
            get
            {
                if (_tags == null) _tags = LoadNoteTags(_noteContents);
                return _tags;
            }
        }


        public string Date
        {
            get
            {
                if (_date == null) _date = GetNoteDate(_noteContents);
                return _date;
            }
        }


        public FileInfo GetFileInfo()
        {
            if (_fileInfo == null) return null;
            return _fileInfo;
        }


        /// <summary>
        /// Tries to parse DateTime from the Date string.
        /// 
        /// Tries to load full DateTime first (01.12.2018), if that fails it tries to load year only.
        /// If Date is date range (2018-2020), it parses only the first date (2018, the part before '-' character)
        /// 
        /// Currently only supports the dd.mm.yyyy format or year (yyyy)
        /// </summary>
        /// <returns>DateTime of the @Date tag</returns>
        public DateTime GetDateTime()
        {
            if (!_dateTime.HasValue) _dateTime = ParseDateTime();
            return _dateTime.Value;
        }


        /// <summary>
        /// Tries to parse DateTime from Date string.
        /// If the parsing fails, returns DateTime.MinValue
        /// </summary>
        /// <returns>Nullable datetime</returns>
        private DateTime? ParseDateTime()
        {
            DateTime dateTime = DateTime.MinValue;
            string date = _date;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            Match match = Regex.Match(Date, @"(\d+)\s*-", RegexOptions.IgnoreCase);
            bool dateIsDateRange = match.Success; //for example "2018-2019"
            if (match.Success) date = match.Groups[1].Value; // We parse the first value;

            bool success = DateTime.TryParse(date, culture, DateTimeStyles.AssumeLocal, out dateTime);
            if (!success)
            {
                try
                {
                    int year = Convert.ToInt32(date);
                    dateTime = new DateTime(year, 1, 1);
                }
                catch { }
            }
            return dateTime;
        }


        /// <summary>
        /// Loads note text from file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>NoteContents / all text inside note text file.</returns>
        private string LoadNoteContents(string filePath)
        {
            string noteContents = File.ReadAllText(filePath);
            if (noteContents == null) return "";
            return noteContents;
        }


        /// <summary>
        /// Loads text after @Name tag with Regex and trims excess whitespace.
        /// </summary>
        /// <param name="noteContents"></param>
        /// <returns></returns>
        private string LoadNoteName(string noteContents)
        {
            Match match = Regex.Match(noteContents, @"@name[ :](.+)", RegexOptions.IgnoreCase);
            if (!match.Success) return "";

            string noteName = match.Groups[1].Value;
            noteName = noteName.Trim();
            return noteName;
        }


        /// <summary>
        /// Loads the tags after "@Tags" tag which should be seperated with space or comma ','.  
        /// Then splits them to array (after removing special characters)
        /// </summary>
        /// <param name="noteContents">Text inside note text file</param>
        /// <returns>Array of tags.</returns>
        private string[] LoadNoteTags(string noteContents)
        {
            Match match = Regex.Match(noteContents, @"@tags (.+)", RegexOptions.IgnoreCase);
            if (!match.Success) return new string[] { };

            string tags = match.Groups[1].Value; //for example "hi,hello,bye"
            tags = tags.Replace(' ', ',');
            tags = Regex.Replace(tags, @"\.", "", RegexOptions.IgnoreCase); // Removes periods
            tags = argumentHandler.RemoveSpecialCharacters(tags);
            string[] tagArray = Array.FindAll(tags.Split(','),
                    tag => tag.Length > 0
            );
            return tagArray;
        }


        /// <summary>
        /// Used Regex to load the date string after @Date tag
        /// </summary>
        /// <param name="noteContents"></param>
        /// <returns></returns>
        private string GetNoteDate(string noteContents)
        {
            Match match = Regex.Match(noteContents, @"@date (.+)", RegexOptions.IgnoreCase);
            if (!match.Success) return "";

            string date = match.Groups[1].Value;
            date = Regex.Replace(date, "[^\\d-./]+", "");
            //date = date.Trim();
            return date;
        }


        /// <summary>
        /// Loads all the text after line containing @Name tag until the paragraph ends (it comes across an empty line)
        /// Should be probably rewritten to simpler Regex rule.
        /// 
        /// Splits all text lines to array cells.
        /// </summary>
        /// <param name="noteContents">Text inside note text file</param>
        /// 
        /// <returns>A string containing the first paragraph of the note</returns>
        private string LoadNoteSummary(string noteContents)
        {
            string[] noteTextLines = noteContents.Split('\n');
            int indexOfNoteName = Array.FindIndex(noteTextLines,
                line => Contains(line, "@name ")
            );
            if (indexOfNoteName == -1) return "";

            int i = indexOfNoteName+1;

            int summaryStartIndex = -1;
            for(; i < noteTextLines.Length; i++)
            {
                string lineText = noteTextLines[i];

                if (ContainsTags(lineText)) return "";
                if (lineText.Trim() != "" && !ContainsTags(lineText))
                {
                    summaryStartIndex = i;
                    break;
                }
            }

            int summaryEndIndex = -1;
            for (; i < noteTextLines.Length; i++)
            {
                string lineText = noteTextLines[i];
                if (String.IsNullOrEmpty(lineText.Trim()) || ContainsTags(lineText))
                {
                    summaryEndIndex = i;
                    break;
                }
            }

            if (summaryStartIndex == -1 || summaryEndIndex == -1) return "";

            int textLineCount = summaryEndIndex - summaryStartIndex;
            string[] summaryTextLines = new string[textLineCount];
            Array.Copy(noteTextLines, summaryStartIndex, summaryTextLines, 0, textLineCount);

            string summary = String.Join("\n", summaryTextLines).Trim();
            return summary;
        }


        /// <summary>
        /// Loads all the text after line containing "@Name" tag until it comes across a new tag (@Tags, @Date etc)
        /// Trims excess whitespace.
        /// </summary>
        /// <param name="noteContents"></param>
        /// <returns>Note text body</returns>
        private string LoadNoteTextBody(string noteContents)
        {
            string[] noteTextLines = noteContents.Split('\n');

            int indexOfTitle = Array.FindIndex(noteTextLines,
                line => Contains(line, "@name ")
            );

            int indexOfTags = Array.FindIndex(noteTextLines,
                line => Contains(line, "@tags ", "@date ")
            );

            int firstIndex = (indexOfTitle != -1) ? indexOfTitle + 1 : 0;
            int lastIndex = (indexOfTags != -1) ? indexOfTags : noteTextLines.Length;
            int textLineCount = lastIndex - firstIndex;

            string[] textLines = new string[textLineCount];
            Array.Copy(noteTextLines, firstIndex, textLines, 0, textLineCount);

            string text = String.Join("\n", textLines)
                .TrimEnd('\n')
                .TrimStart('\n');
            return text;
        }


        /// <summary>
        /// Checks if string contains any of the substring defined in arguments.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="valuesToCheckFor"></param>
        /// <returns></returns>
        private static bool Contains(string str, params string[] valuesToCheckFor)
        {
            string options = "(" + String.Join("|", valuesToCheckFor) + ")";
            return (Regex.IsMatch(str, options, RegexOptions.IgnoreCase));
        }


        /// <summary>
        /// Checks if the text is in note format (contains at least one note tags).
        /// </summary>
        /// <param name="noteContents"></param>
        /// <returns></returns>
        public static bool IsNoteChadNote(string noteContents)
        {
            bool isNoteChadNote = ContainsTags(noteContents);
            return isNoteChadNote;
        }

        
        /// <summary>
        /// Checks if given string contains note tags.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainsTags(string str)
        {
            return Contains(str, "@name[ :]", "@tags[ :]", "@date[ :]");
        }
    }
}




/*
notechad "Toinen maailmansota" -d
    - no such note exists. create one?
    - generates toinen_maailmansota.txt to working directory and opens in default editor (or one set by user)
    - generates @name _summary on first line @tags $_name &_name %_name £_name !@tags @datetime
notechad find "Toinen maailmansota" - searches for toinen_maailmansota
notechad find #hitler #toinen_maailmansota sort date|_name|created|modified
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
- read with regex: _name, tldr, note, [List]_tags
[
    {
        _name: "Note _name",
        tldr: "Summary about note",
        _tags: ["note", "notechad", "example"],
        path: [root]+"tunti1/\note__name.txt"
    }
]

On launch:
Note.ParseJSON(root_folder);



Note.oteame(path\_name\GUID?);
Note.GetSummary();
Note.GetText();
Note.GetTags();

Note.SetName();
Note.SetText();
Note.SetTags(); (also reads _tags from the file is exists)

Note.ParseJSON(path\_name\GUID?);
Note.Parse



Should we keep reading the file structure to find new files?
HASH list to keep reading file changes? Or just current notes?

public class Note() 

parse, extract, load, get

*/
