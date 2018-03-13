using System;
using System.Collections.Generic;

namespace Manager
{
    public class Statement
    {
        public string Command { get; set; }
        public string PreviousCommand { get; set; }
        public List<char> Params { get; set; }
        public Dictionary<string, string> Property { get; set; }
        public string[] Blocks { get; set; }
        public string String { get; set; }
        public const ushort Count = 3;

        public Statement( string statement = "" )
        {
            Blocks = statement.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
            String = statement;
        }

        public void Parse()
        {
            if ( Blocks.Length == 0 )
            {
                throw new Exception( $"String is empty '{nameof( String )}'" );
            }

            Command = Blocks[0];

            if ( Blocks.Length > 1 )
            {
                if ( Blocks[1].IndexOf( "/" ) != -1 )
                {
                    Params = ParseParam( Blocks[1] );
                }
                else
                if ( Blocks.Length > 2 && Blocks[2].IndexOf( "/" ) != -1 )
                {
                    Params = ParseParam( Blocks[2] );
                }
            }

            if ( Blocks.Length > 1 )
            {
                if ( Blocks[1].IndexOf( "$" ) == 0 )
                {
                    Property = ParseProp( Blocks[1] );
                }
                else
                if ( Blocks.Length > 2 && Blocks[2].IndexOf( "$" ) == 0 )
                {
                    Property = ParseProp( Blocks[2] );
                }
            }

        }
        List<char> ParseParam( string param )
        {
            param = param.Substring( 1 );

            if ( param.Length == 0 )
            {
                throw new Exception( $"Have special symbols but have not parametr(s) '{String}', from '{nameof( String )}'" );
            }

            List<char> list = new List<char>( param.Length );

            foreach ( var item in param )
            {
                list.Add( item );
            }

            return list;
        }
        Dictionary<string, string> ParseProp( string prop )
        {

            string[] property = prop.Split( new char[] { '$', '=' }, StringSplitOptions.RemoveEmptyEntries );

            if ( property.Length == 0 )
            {
                throw new Exception( $"Have special symbols but have not property(ies) '{String}', from '{nameof( String )}'" );
            }
            else
            if ( property.Length % 2 != 0 )
            {
                throw new Exception( $"Upset key or value in property(ies) '{String}', from '{nameof( String )}'" );
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>(property.Length);

            for ( int i = 0, j = 0; j < property.Length / 2; i += 2, j++ )
            {
                if ( dictionary.ContainsKey( property[i] ) )
                {
                    continue;
                }

                dictionary.Add( property[i], property[i + 1] );
            }

            return dictionary;
        }
    }
}
