using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;
namespace Manager
{
    static class Program
    {
        #region Prop
        internal delegate void Operation( Statement Statement );
        static Operation help = Commands.Help;
        static Operation add = Commands.Add;
        static Operation remove = Commands.Remove;
        static Operation print = Commands.Print;
        static Operation clear = Commands.Clear;
        static Operation alias = Commands.Alias;
        static Operation exit = Commands.Exit;

        internal static Dictionary<string, Operation> CmdFunc { get; private set; }
        internal static Dictionary<string, XElement> FuncHelp { get; private set; }
        internal static Dictionary<string, XElement> Accaunts { get; private set; }
        internal static Dictionary<string, string> AliasFunc { get; set; }
        internal static Dictionary<string, string> Tags { get; private set; }
        internal static Dictionary<string, Dictionary<string, char>> Attributes { get; private set; }
        internal static string FilePath { get; private set; }
        internal static string DataPath { get; private set; }
        internal static string AbsolutePath { get; private set; }
        internal static Dictionary<string, string> NewAlias { get; set; }
        #endregion


        static void Main( string[] args )
        {
            Print( color: ConsoleColor.Magenta, txt: "Welcome use 'help' for get list of commands", sym: "" );

            CmdFunc = new Dictionary<string, Operation>
            {
                { "help", help },
                { "add", add },
                { "remove", remove },
                { "print", print },
                { "alias", alias },
                { "clear", clear },
                { "exit", exit }
            };
            Tags = new Dictionary<string, string>
            {
                { "body", "configuration"},
                { "acnt", "account"},
                { "alias", "alias"},
                { "disc", "description"},
                { "id", "id"},
                { "prop", "property"},
                { "param", "parameters"},
                { "date", "date-created"},
                { "name", "name"},
                { "login", "login"},
            };
            FuncHelp = new Dictionary<string, XElement> { };
            Accaunts = new Dictionary<string, XElement> { };
            AliasFunc = new Dictionary<string, string> { };
            NewAlias = new Dictionary<string, string> { };
            Attributes = new Dictionary<string, Dictionary<string, char>> { };
            AbsolutePath = Directory.GetCurrentDirectory();
            DataPath = $@"{AbsolutePath.Substring( 0, AbsolutePath.IndexOf( "bin" ) )}data\data.xml";
            FilePath = $@"{AbsolutePath.Substring( 0, AbsolutePath.IndexOf( "bin" ) )}boot\core.xml";

            try
            {
                Boot();
            }
            catch ( Exception error )
            {
                EmergencyExit( error );
                return;
            }
            Go();
        }

        #region Main Functions
        internal static void Go()
        {
            Run:
            string txt = Prompt( color: ConsoleColor.DarkGreen, txt: "root: <->" ).Trim();

            if ( txt == "" )
            {
                Print( ConsoleColor.Red, "String can't be empty" );
                goto Run;
            }
            try
            {
                Statement statement = new Statement( txt );
                statement.Parse();
                Run( statement );
            }
            catch ( Exception error )
            {

                Print( ConsoleColor.Red, error.Message );

                goto Run;
            }
        }

        internal static void Print( ConsoleColor color, string txt, string sym = "-> ", ConsoleColor console = ConsoleColor.White )
        {
            Print( color: console, value: sym );
            Print( color: color, value: txt );

            Console.WriteLine();
        }

        internal static void Print( ConsoleColor color, object value )
        {
            var prev =
            Console.ForegroundColor;

            Console.ForegroundColor = color;

            Console.Write( value );

            Console.ForegroundColor = prev;
        }
        internal static string Prompt( ConsoleColor color, string txt, string sym = "<- ", ConsoleColor console = ConsoleColor.White )
        {
            string val;
            var prev =
            Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.WriteLine( txt );
            Console.ForegroundColor = console;
            Console.Write( sym );
            Console.ForegroundColor = color;
            val =
            Console.ReadLine();

            Console.ForegroundColor = prev;

            return val;
        }

        internal static void Run( Statement statement )
        {
            

            run:
            if (  statement.Command == "add" || statement.Command == "remove" )
            {
                throw new Exception( $"Such '{statement.Command}' command is under development " );
            }

            if ( CmdFunc.TryGetValue( statement.Command, out Operation operation ) )
            {
                operation.Invoke( statement );
            }
            else
            if ( AliasFunc.ContainsValue( statement.Command ) )
            {
                foreach ( var item in AliasFunc )
                {
                    if ( item.Value == statement.Command )
                    {
                        statement.Command = item.Key;

                        goto run;
                    }
                }
            }
            else
            {
                throw new Exception( $"Such '{statement.Command}' command is not found " );
            }
        }
        internal static void Save()
        {
            if ( NewAlias.Count == 0 )
            {
                return;
            }

            XElement config = XDocument.Load(FilePath).Element( Tags["body"] ) ;

            foreach ( var alias in NewAlias )
            {
                if ( config.Element( alias.Key ) != null )
                {
                    config.Element( alias.Key ).Element( Tags["alias"] ).Value = alias.Value;
                }
            }
            Print( ConsoleColor.Blue, "Saving..." );
            config.Save( FilePath );
        }
        static void Boot()
        {
            bool hasFile = File.Exists( FilePath ), hasData = File.Exists( DataPath );

            if( !hasFile || !hasData )
            {
                string file = ! hasFile ? Path.GetFileName( FilePath ) : Path.GetFileName( DataPath );

                throw new Exception($"File '{file}' missing. Sorry :(");
            }

            XDocument document, data;
            try
            {
                document = XDocument.Load( FilePath );
                data = XDocument.Load( DataPath );
            }
            catch ( XmlException )
            {
                throw new Exception($"Invalid format loaded files. Sorry :(" ); 
            }

            foreach ( var item in document.Element( Tags["body"] ).Elements() )
            {
                FuncHelp.Add( item.Name.LocalName, item );

                if ( item.Element( Tags["param"] ) != null && item.Element( Tags["alias"] ).Value.Trim().Length != 0 )
                {
                    AliasFunc.Add( item.Name.LocalName, item.Element( Tags["alias"] ).Value );
                }

                var attrs = new Dictionary<string, char>{ };

                if ( item.Element( Tags["param"] ) != null )
                {
                    foreach ( var attr in item.Element( Tags["param"] ).Elements() )
                    {
                        attrs.Add( attr.Name.LocalName, attr.Value.Trim()[0] );
                    }
                }
                else
                {
                    continue;
                }

                Attributes.Add( item.Name.LocalName, attrs );
            }

            foreach ( var item in data.Element( Tags["body"] ).Elements() )
            {
                Accaunts.Add( item.Attribute( Tags["id"] ).Value, item );
            }

        }
        static void EmergencyExit( Exception exception )
        {
            string logPath = $@"{Program.AbsolutePath.Substring( 0, Program.AbsolutePath.IndexOf( "bin" ) )}log\{DateTime.Now.ToFileTime()}.txt";
            StreamWriter writer = new StreamWriter( File.Create( logPath ) );
            writer.Write(
                $"Name exception: Boot Error \n" +
                $"From (where happened exception): {exception.ToString()} \n" +
                $"Date: {DateTime.Now.ToString()}"
                );
            writer.Close();
            Print( ConsoleColor.DarkMagenta, "Boot Error:", "" );
            Print( ConsoleColor.Red, exception.Message );
            Print( ConsoleColor.Red, "Application can't work. Look log files to decide problem, please pass any buttom..." );

            Console.ReadKey();
        }
        #endregion

    }
}
