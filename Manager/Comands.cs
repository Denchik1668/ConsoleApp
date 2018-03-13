using System;
using System.Collections.Generic;
using System.Linq;
using static Manager.Program;
namespace Manager
{
    public static class Commands
    {
    #region Commands
        public static void Help( Statement statement )
        {
            if ( statement.Blocks.Length == 1 )
            {
                Program.Print( ConsoleColor.Magenta, $"For get more information about command use '[command][/?]' ", "" );

                foreach ( var item in FuncHelp )
                { 
                    Program.Print( ConsoleColor.Blue,  $"{item.Key}:\n\t{item.Value.Element( Tags["disc"] ).Value}");
                }
            }
            else
            if( statement.Params != null && statement.Property == null )
            {
                var helpAttr = Attributes["help"];

                foreach ( var attr in statement.Params )
                {
                    if ( attr == helpAttr["help"] )
                    {
                        HelpOf( statement );
                    }
                    else if ( attr == helpAttr["list"] )
                    {
                        Program.Print( ConsoleColor.Magenta, $"List of commands: '{statement.Command} /{attr}' ", "" );

                        foreach ( var item in FuncHelp )
                        {
                            Program.Print( ConsoleColor.Blue, $"{item.Key}" );
                        }
                    }
                    else
                    {
                        Program.Print( ConsoleColor.Red, $"This '{attr}' parametr is not correct" );
                        goto fin;
                    }
                }
            }
            else
            {
                Program.Print( ConsoleColor.Red, $"Invalid using  command: '{statement.Command}', -> '{statement.String}'" );
            }
            
            fin:
            Go();
        }
        public static void Add( Statement statement )
        {
        }
        public static void Remove( Statement statement )
        {
        }
        public static void Print( Statement statement )
        {
            switch ( statement.Blocks.Length )
            {
                case 1:
                        Program.Print(ConsoleColor.Blue,
                            ( Accaunts.Count != 0 )? $"Base contain: {Accaunts.Count} accaunts" : $"Accounts is upsent");
                    
                    break;

                case 2 when statement.Params != null:

                    int count = 0;

                    var printAttrs = Attributes["print"];

                    foreach ( var attr in statement.Params )
                    {
                        if ( attr == printAttrs["help"] )
                        {
                            HelpOf( statement );
                        }
                        else
                        if ( attr == printAttrs["list"] )
                        {
                            count = 0;

                            foreach ( var item in Accaunts )
                            {
                                Program.Print( ConsoleColor.Blue,
                                    $"{++count}\t" +
                                    $"{item.Value.Attribute( Tags["id"] ).Name }: {item.Value.Attribute( Tags["id"] ).Value} \t" +
                                    $"{item.Value.Attribute( Tags["date"] ).Name}: {item.Value.Attribute( Tags["date"] ).Value} \t" +
                                    $"{item.Value.Attribute( Tags["name"] ).Name}: {item.Value.Attribute( Tags["name"] ).Value}"
                                    );
                            }
                        }
                        else
                        if ( attr == printAttrs["all"] )
                        {
                            count = 0;

                            foreach ( var item in Accaunts.Values )
                            {
                                PrintOf( item, count++ );

                            }
                        }
                        else
                        {
                            Program.Print( ConsoleColor.Red, $"This '{attr}' parametr is not correct, use {statement.Command} /{Attributes["alias"]["help"]}" );

                            break;

                        }
                    }

                    break;

                case 2 when statement.Property != null:

                    string[] array = 
                         FuncHelp["print"]
                        .Element( Tags["prop"] )
                        .Value.Split(new char[] {' ', ',' }, StringSplitOptions.RemoveEmptyEntries );

                    count = 0;

                    foreach ( var prop in statement.Property )
                    {
                        if ( !array.Contains( prop.Key ) )
                        {
                            Program.Print( ConsoleColor.Red, $"Accaunt has not search-tag: '{prop.Key}'" );

                            break;
                        }
                        else
                        {
                            foreach ( var acnt in Accaunts.Values )
                            {
                                if ( acnt.Attribute( prop.Key ) != null && acnt.Attribute( prop.Key ).Value == prop.Value )
                                {
                                    PrintOf( acnt, count++ );
                                }
                                else
                                if ( acnt.Element( prop.Key ) != null && acnt.Element( prop.Key ).Value == prop.Value )
                                {
                                    PrintOf( acnt, count++ );
                                }
                            }

                            if ( count == 0 )
                            {
                                Program.Print( ConsoleColor.Red, $"Accaunt {prop.Key}: '{prop.Value}' is not found" );
                            }
                        }
                    }

                    break;

                default:

                    Program.Print( ConsoleColor.Red, $"Invalid using  command: '{statement.Command}', -> '{statement.String}'" );

                    break;
            }

            Go();

            void PrintOf( System.Xml.Linq.XElement element, int count = 0 )
            {
                Program.Print( ConsoleColor.Blue, new String( '-', Console.BufferWidth ), "" );
                Program.Print( ConsoleColor.Blue, $"{element.Name.LocalName.ToUpper()}: {count + 1}", "" );
                foreach ( var elem in element.Attributes() )
                {
                    Program.Print( ConsoleColor.Blue, $"{elem.Name}: '{elem.Value}'" );
                }
                foreach ( var elem in element.Elements() )
                {
                    Program.Print( ConsoleColor.Blue, $"{elem.Name.LocalName}: '{elem.Value}'" );
                }

                Program.Print( ConsoleColor.Blue, new String('-', Console.BufferWidth), "" );
            }
            
        }
        public static void Alias( Statement statement )
        {
            switch ( statement.Blocks.Length )
            {
                case 1 :
                    Program.Print(ConsoleColor.Magenta, "Aliases: command ~ alias ", "");

                    if ( AliasFunc.Count == 0 )
                    {
                        Program.Print( ConsoleColor.Blue, "Upset aliases" );
                    }
                    else
                    {
                        foreach ( var item in AliasFunc )
                        {
                            Program.Print( ConsoleColor.Yellow, $"{item.Key} ~ {item.Value}" );
                        }
                    }
                    
                    break;

                case 2 when statement.Params != null && statement.Property == null:

                    var helpAttrs = Attributes["help"];

                    foreach ( var attr in statement.Params )
                    {
                        if ( attr == helpAttrs["help"] )
                        {
                            HelpOf( statement );
                        }
                        else
                        {
                            Program.Print( ConsoleColor.Red, $"This '{attr}' parametr is not correct, use {statement.Command} /{Attributes["alias"]["help"]}" );

                            break;
                        }
                    }

                    break;

                case 3 when statement.Params != null && statement.Property != null :

                    List<char> list = new List<char>{};
                    var aliasAttrs = Attributes["alias"];

                    foreach ( var attr in statement.Params )
                    {
                        if ( attr == aliasAttrs["help"] && !list.Contains( attr ) )
                        {
                            list.Add( attr );

                            HelpOf( statement );
                        }
                        else if ( attr == aliasAttrs["add"] && !list.Contains( attr ) )
                        {
                            list.Add( attr );

                            foreach ( var item in statement.Property )
                            {
                                if ( TryAddAlias(  item.Key, item.Value, statement.Params.ToArray().Contains( aliasAttrs["force"] ) ) )
                                {
                                    Program.Print( ConsoleColor.Blue, $"Alias successful added '{statement.Command} {item.Key}:{item.Value} /{attr}'" );
 
                                }
                                else
                                {
                                    Program.Print( ConsoleColor.Red, $"The request to add was canceled -> '{statement.Command} {item.Key}:{item.Value} /{attr}'" );

                                }
                            }
                        }
                        else if ( attr == aliasAttrs["remove"] && !list.Contains( attr ) )
                        {
                            list.Add( attr );

                            foreach ( var item in statement.Property )
                            {
                                if ( TryRemoveAlias( item.Key , statement.Params.ToArray().Contains( aliasAttrs["force"] ) ) )
                                {
                                    Program.Print( ConsoleColor.Blue, $"Alias successful removed '{statement.Command} {item.Key}:{item.Value} /{attr}'" );

                                }
                                else
                                {
                                    Program.Print( ConsoleColor.Red, $"The removal request was canceled '{statement.Command} {item.Key}:{item.Value} /{attr}'" );

                                }
                            }
                        }
                        else if ( attr == aliasAttrs["force"] && !list.Contains( attr ) )
                        {
                            list.Add( attr );

                            continue;
                        }
                        else
                        if ( list.Contains( attr ) )
                        {
                            Program.Print( ConsoleColor.Red, $"This '{attr}' has done" );

                            goto fin;
                        }
                        else
                        {
                            Program.Print( ConsoleColor.Red, $"This '{attr}' parametr is not correct" );

                            goto fin;
                        }
                    }

                    break;

                default:

                    Program.Print( ConsoleColor.Red, $"Invalid using  command: '{statement.Command}', -> '{statement.String}'" );

                    break;
            }
            fin:
            Go();

            bool TryAddAlias( string key, string value, bool force = false )
            {
                if( !CmdFunc.Keys.Contains( key ) )
                {
                    Program.Print( ConsoleColor.Red, $"Command: '{key}' is not found" );

                    return false;
                }
                else
                if ( AliasFunc.ContainsKey( key ) && !force )
                {
                    Program.Print( ConsoleColor.Red, $"Command: '{key}' already has alias" );

                    return false;
                }
                else
                if ( AliasFunc.ContainsValue( value ) && !force )
                {
                    Program.Print( ConsoleColor.Red, $"Alias: '{value}' is not availble" );

                    return false;
                }
                else
                if ( force )
                {
                    if ( AliasFunc.ContainsKey( key ) )
                    {
                        AliasFunc.Remove( key );
                    }

                    if ( AliasFunc.ContainsValue( value ) )
                    {
                        foreach ( var item in AliasFunc )
                        {
                            if ( item.Value == value )
                            {
                                AliasFunc.Remove( item.Key );
                            }
                        }
                    }
                }
                
                AliasFunc.Add( key, value );

                if ( !NewAlias.ContainsKey( key) )
                {
                    NewAlias.Add( key, value );
                }

                return true;
            }

            bool TryRemoveAlias( string key, bool force = false )
            {
                if ( !CmdFunc.Keys.Contains( key ) )
                {
                    Program.Print( ConsoleColor.Red, $"Command: '{key}' is not found" );

                    return false;
                }
                else
                if ( force )
                {
                    bool isRm = AliasFunc.Remove( key );

                    if ( isRm && !NewAlias.ContainsKey( key ) )
                    {
                        NewAlias.Add( key, "" );
                    }

                    return isRm;
                }
                else
                {
                    string answer = Prompt( color: ConsoleColor.DarkGreen, txt: "Are you sure complite process [ 'y' | 'n' ]" ).Trim();

                    if ( "y" == answer )
                    {
                        bool isRm = AliasFunc.Remove( key );

                        if ( isRm && !NewAlias.ContainsKey( key ) )
                        {
                            NewAlias.Add( key, "" );
                        }

                        return isRm;
                    }
                    else
                    if ( "n" == answer )
                    {
                        return false;
                    }
                    else
                    {
                        Program.Print( color: ConsoleColor.Red, txt: $"Invalid argument: {answer} " );

                        return false;
                    }
                }
            }
        }
        public static void Clear( Statement statement )
        {
            if ( statement.Blocks.Length == 1 )
            {
                Console.Clear();

                Program.Print( color: ConsoleColor.Magenta, txt: "Console was clean" );
            }
            else
            if ( statement.Params != null )
            {
                foreach ( var item in statement.Params )
                {
                    if ( item == Attributes["clear"]["help"] )
                    {
                        HelpOf( statement );
                    }
                    else
                    {
                        Program.Print( ConsoleColor.Red, $"Invalid using parameters: '{statement.Command} / {item}', -> '{statement.String}'" );

                        goto fin;
                    }
                }
            }
            else
            {
                Program.Print( ConsoleColor.Red, $"Invalid using command: '{statement.Command}', -> '{statement.String}'" );
            }
             
            fin:
            Go();
        }
        public static void Exit ( Statement statement )
        {
            switch ( statement.Blocks.Length )
            {
                case 1 :

                    string answer = Prompt( color: ConsoleColor.DarkGreen, txt: "Are you sure complite process [y/n]" ).Trim();

                    if ( "y" == answer )
                    {
                        Save();

                        return;
                    }
                    else
                    if ( "n" == answer )
                    {
                        Program.Print( color: ConsoleColor.Magenta, txt: "Exit was cansel" );
                    }
                    else
                    {
                        Program.Print( color: ConsoleColor.Red, txt: "Invalid argument " );
                    }

                    break; 

                case 2 when statement.Params != null :

                    var exitAttrs = Attributes["exit"];

                    foreach ( var param in statement.Params )
                    { 
                        if ( param == exitAttrs["help"] )
                        {
                            HelpOf( statement );
                        }
                        else
                        if ( param == exitAttrs["force"] )
                        {
                            Save();

                            return;
                        }
                        else
                        {
                            Program.Print( ConsoleColor.Red, $"This '{param}' parameter is not correct" );
                        }
                    }
                     
                    break;

                default:

                    break;
            }

            Go();
        }
        #endregion
        
        static void HelpOf( Statement statement )
        {
            if ( FuncHelp.TryGetValue( statement.Command, out var help ) )
            {
                uint count = 0;

                PrintHelp( help, ref count );
            }
            else
            {
                Program.Print( ConsoleColor.Red, $"Help by '{statement.Command}' is not found" );
            }

            void PrintHelp( System.Xml.Linq.XElement element, ref uint count )
            {
                foreach ( var item in element.Elements() )
                {
                    if( item.Elements().ToArray().Length == 0 )
                    {
                        Program.Print( ConsoleColor.Blue, $"{item.Name.LocalName.ToUpper()}:\n{new String( '\t', ( int )count + 1 )}{item.Value}\n", new String( '\t', ( int )count ) );
                    }
                    else
                    {
                        Program.Print( ConsoleColor.Blue, $"{item.Name.LocalName.ToUpper()}:\n{new String( '\t', ( int )count + 1 )}", new String( '\t', ( int )count ) );
                        count++;
                        PrintHelp( item, ref count );
                    }
                }
                count--;
            }
        }
    }
}
