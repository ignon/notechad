using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoteChad;

namespace NoteChadTests
{
    [TestClass]
    public class TestInitializer
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
            => Program.InitializeCommands();

    }

    [TestClass]
    public class ArgumentHandlerTests
    {

        static IArgumentHandler argumentHandler;

        [TestInitialize]
        public void Initialize()
        {
            argumentHandler = Program.argumentHandler;
        }

        [TestMethod]

        public void GetCommands_Test1()
        {
            string[] args = new string[] { "new", "hello", "world" };
            Command command = argumentHandler.GetCommand(args);
            Assert.AreEqual(command.Name, "new", "The correst command was returned");
        }

        [TestMethod]
        public void GetNoteName_TestWithName() {
            string[] args = new string[] { "new", "hello" };
            string fileName = argumentHandler.GetNoteName(args);
            Assert.AreEqual("hello", fileName, "GetNoteName returns incorrect name");
        }

        [TestMethod]
        public void GetNoteName_TestWithMultiPartName()
        {
            string[] args = new string[] { "new", "hello", "world" };
            string fileName = argumentHandler.GetNoteName(args);
            Assert.AreEqual("hello world", fileName, "GetNoteName returns incorrect name");
        }

        [TestMethod]
        public void GetNoteName_TestWithNameAndTags()
        {
            string[] args = new string[] { "new", "hello", "world", "#moi" };
            string fileName = argumentHandler.GetNoteName(args);
            Assert.AreEqual("hello world", fileName, "GetNoteName did't return an empty string");
        }

        [TestMethod]
        public void GetNoteName_TestWithNoName()
        {
            string[] args = new string[] { "new", "#moi" };
            string fileName = argumentHandler.GetNoteName(args);
            Assert.AreEqual("", fileName, "GetNoteName should return an empty string if no name exists in arguments.");
        }

        [TestMethod]
        public void GetFileName_Test1() {
            string[] args = new string[] { "new", "märehtijä" };
            string name = argumentHandler.GetFileName(args);
            Assert.AreEqual("marehtija.txt", name);
        }

        [TestMethod]
        public void GetFileName_Test2()
        {
            string[] args = new string[] { "new", "märehtijä.txt" };
            string name = argumentHandler.GetFileName(args);
            Assert.AreEqual("marehtija.txt", name);
        }

        [TestMethod]
        public void GetFileName_Test3()
        {
            string[] args = new string[] { "new", "märehtijä.txt_moi" };
            string name = argumentHandler.GetFileName(args);
            Assert.AreEqual("marehtija_txt_moi.txt", name);
        }

        [TestMethod]

        public void GetNoteTags_Test_Default()
        {
            string[] args = new string[] { "new", "märehtijä.txt", "#moi" };
            string[] tags = argumentHandler.GetNoteTags(args);
            Assert.AreEqual("moi", tags[0], "List of tags should be returned without # char");
        }

        [TestMethod]

        public void GetNoteTags_Test_RemoveEmptyTags()
        {
            string[] args = new string[] { "new", "märehtijä.txt", "#" };
            string[] tags = argumentHandler.GetNoteTags(args);
            Assert.IsTrue(tags.Length == 0, "Empty tags \"#\" should be disqualified");
        }

        [TestMethod]

        public void GetNoteTags_Test_RemoveMultipleHashtags()
        {
            string[] args = new string[] { "new", "märehtijä.txt", "####" };
            string[] tags = argumentHandler.GetNoteTags(args);
            Assert.IsTrue(tags.Length == 0, "Invalid with only special characters should be disqualified.");
        }

        [TestMethod]

        public void GetNoteTags_Test_RemoveEscapedCharacters()
        {
            string[] args = new string[] { "new", "märehtijä.txt", "#moi\n\t\"h" };
            string[] tags = argumentHandler.GetNoteTags(args);
            Assert.AreEqual("moih", tags[0], "Invalid with only special characters should be disqualified.");
        }

        [TestMethod]

        public void GetNoteTags_Test_RemoveSpecialCharacters()
        {
            string[] args = new string[] { "new", "märehtijä.txt", "#moi%&%¤|´´``+#" };
            string[] tags = argumentHandler.GetNoteTags(args);
            Assert.AreEqual("moi", tags[0], "Special characters should be removed.");
        }

        [TestMethod]

        public void GetNoteTags_Test_Russian()
        {
            string[] args = new string[] { "#сукаблять" };
            string[] tags = argumentHandler.GetNoteTags(args);
            Assert.AreEqual("сукаблять", tags[0], "Russian should work");
        }

        [TestMethod]
        public void GetNoteTags_Test_Japanese()
        {
            string[] args = new string[] { "#かわいい" };
            string[] tags = argumentHandler.GetNoteTags(args);
            Assert.AreEqual("かわいい", tags[0], "Japanese should work (or non-latin alphabets in general )");
        }

        [TestMethod]
        public void GetNoteTags_Test_Numbers()
        {
            string[] args = new string[] { "#1984" };
            string[] tags = argumentHandler.GetNoteTags(args);
            Assert.AreEqual("1984", tags[0]);
        }

        [TestMethod]
        public void GetNoteTags_Test_Decimals()
        {
            string[] args = new string[] { "#1.3,1" };
            string[] tags = argumentHandler.GetNoteTags(args);
            Assert.AreEqual("1.3,1", tags[0]);
        }

        [TestMethod]
        public void GetNoteDate_Test_Default()
        {
            Note note = new Note("@date 201 8");
            Assert.AreEqual("2018", note.Date);
        }

        [TestMethod]
        public void GetNoteDate_Test_2()
        {
            Note note = new Note("@date 2 0 moih 18");
            Assert.AreEqual("2018", note.Date);
        }
    }


    [TestClass]
    public class Note_Tests
    {

        [TestMethod]
        public void NoteName_Test_Default()
        {
            Note note = new Note("moiks @name Hello World \n taas");
            Assert.AreEqual("Hello World", note.Name);
        }

        [TestMethod]
        public void GetNoteName_Test_SpecialCharacters()
        {
            Note note = new Note("moiks @name Hello\t%.,World \n taas");
            Assert.AreEqual("Hello\t%.,World", note.Name);
        }

        [TestMethod]
        public void GetNoteTags_Test_Default()
        {
            Note note = new Note("@tags moi, hei, jaa");
            Assert.AreEqual("moi,hei,jaa", String.Join(",", note.Tags));
        }

        [TestMethod]
        public void GetNoteTags_Test_EmptyTags()
        {
            Note note = new Note("@tags ,,moi, ,.,hei,, jaa,");
            Assert.AreEqual("moi,hei,jaa", String.Join(",", note.Tags));
        }

        [TestMethod]
        public void GetNoteTags_Test_SpecialCharacters()
        {
            Note note = new Note("@tags %moi, \thei, \\jaa, ?joo!");
            Assert.AreEqual("moi,hei,jaa,joo", String.Join(",", note.Tags));
        }

        [TestMethod]
        public void GetNoteTags_Test_NonLatinCharacters()
        {
            Note note = new Note("@tags かわいい,сукаблять");
            Assert.AreEqual("かわいい,сукаблять", String.Join(",", note.Tags));
        }

        [TestMethod]
        public void GetNoteSummary_Test_Default()
        {
            Note note = new Note("@name ExampleNote \nSummary \nMore summary text \n @tags moi, hei, jaa");
            Assert.AreEqual("Summary \nMore summary text", note.Summary);
        }

        [TestMethod]
        public void GetNoteSummary_Without_TextBody()
        {
            Note note = new Note("@name ExampleNote \nSummary \nMore summary text \n\n Text Body\n @tags moi, hei, jaa");
            Assert.AreEqual("Summary \nMore summary text", note.Summary);
        }

        [TestMethod]
        public void GetNoteSummary_ToDo()
        {
            Note note = new Note("@name ToDo-List \nToDo:\n- Thing1\n- Thing2\n @tags moi, hei, jaa");
            Assert.AreEqual("ToDo:\n- Thing1\n- Thing2", note.Summary);
        }

        [TestMethod]
        public void GetNoteSummary_Test_DoNotTrimWhitespace()
        {
            Note note = new Note("@name ExampleNote \nSummary \n Text \n @tags moi, hei, jaa");
            Assert.AreEqual("Summary \n Text", note.Summary);
        }

        [TestMethod]
        public void GetNoteText_Test_Default()
        {
            Note note = new Note("@name ExampleNote \nSummary\nText\n@tags moi, hei, jaa");
            Assert.AreEqual("Summary\nText", note.TextBody);
        }

        [TestMethod]
        public void GetNoteText_Test_WhenEmpty()
        {
            Note note = new Note("@name ExampleNote\n\n@tags moi, hei, jaa");
            Assert.AreEqual("", note.TextBody);
        }

        [TestMethod]
        public void GetNoteText_Test_WhenNoTags()
        {
            Note note = new Note("@name ExampleNote\nSummary\nText\n\n");
            Assert.AreEqual("Summary\nText", note.TextBody);
        }

        [TestMethod]
        public void GetNoteText_Test_WhenNoTitle()
        {
            Note note = new Note("Summary\nText");
            Assert.AreEqual("Summary\nText", note.TextBody);
        }

        [TestMethod]
        public void GetNoteText_Test_WhenTitleOnly()
        {
            Note note = new Note("@name hello world");
            Assert.AreEqual("", note.TextBody);
        }

        [TestMethod]
        public void CreateNote_Test_WhenTitleOnly()
        {
            Note note = new Note("@name Hello World\nsummary\ntext\n@Tags hello, world");
            Assert.AreEqual("Hello World", note.Name);
        }

        [TestMethod]
        public void CreateNote_Test()
        {
            string filePath = $"{Environment.CurrentDirectory}\\testifilu.txt";
            File.WriteAllText(filePath,"@name Testi Filu\nSummary\nText\n@tags moi, hei, voi");

            FileInfo file = new FileInfo(filePath);
            Note note = new Note(file);

            Assert.AreEqual("Testi Filu", note.Name);
            Assert.AreEqual("Summary\nText", note.Summary);
            Assert.AreEqual("moi,hei,voi", String.Join(",", note.Tags));
            Assert.AreEqual("Summary\nText", note.TextBody);
        }
    }
}
