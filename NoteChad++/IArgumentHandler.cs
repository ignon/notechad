namespace NoteChad
{

    /// @author Arttu Mäkinen
    /// @version 0.1
    /// <summary>
    /// Class which parses information from
    /// command line arguments.
    /// </summary>
    public interface IArgumentHandler
    {
        Command[] Commands { get; }

        /// <summary>
        /// Defines all available terminal commands based on
        /// commands given in parameters.
        /// </summary>
        /// <param name="commands"></param>
        void DefineCommands(params Command[] commands);


        /// <summary>
        /// Parses the Terminal input and returns a Command object matching the command call.
        /// Prints an error message and ends the program if the command call doesn't match with any Notechad Command.
        /// </summary>
        /// <param name="args">Terminal input (the string[] args array the Main() method receives from terminal)</param>
        /// <returns>Command object corresponding to the terminal call.</returns>
        Command GetCommand(string[] args);



        /// <summary>
        /// Generates a file name for the note based on the note name.
        /// </summary>
        /// <param name="noteName">noteName</param>
        /// <returns>File name (as a string)</returns>
        string GetFileName(string noteName);


        /// <summary>
        /// Generates a file name for the note based on the terminal call / user input. 
        /// </summary>
        /// <param name="args">Terminal input (the string[] args array the Main() method receives from terminal)</param>
        /// <returns>File name (as a string)</returns>
        string GetFileName(string[] args);


        /// <summary>
        /// Generates a note name for the note based on the terminal call / user input. 
        /// </summary>
        /// <param name="args"> The string[] args array the Main() method receives</param>
        /// <returns>Note name (as a string)</returns>
        string GetNoteName(string[] args);


        /// <summary>
        /// Returns note tags based on the terminal call / user input.
        /// </summary>
        /// <param name="args">The string[] args array the Main() method receives</param>
        /// <returns>Note tags (as an array)</returns>
        string[] GetNoteTags(string[] args);

        /// <summary>
        /// Prints all available commands (name and description).
        /// Used to print help page.
        /// </summary>
        void PrintCommands();

        /// <summary>
        ///  Removes special characters from strint.
        /// </summary>
        /// <param name="str">String to remove special characters from</param>
        /// <returns>String with special characters removed.</returns>
        string RemoveSpecialCharacters(string str);
    }
}