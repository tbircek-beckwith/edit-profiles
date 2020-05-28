using System;
using System.Linq;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Provides VB.Net Method String.StrDup ( Int32, String ) 
    /// like functionality.
    /// </summary>
    public static class Repeat
    {
        /// <summary>
        /// Provides VB.Net Method String.StrDup ( Int32, String ) 
        /// like functionality.
        /// </summary>
        /// <param name="value">String value to be repeated.</param>
        /// <param name="number">Number of repeats.</param>
        /// <returns>Returns a string consisted of string repeated the specified number of times.</returns>
        public static string StringDuplicate ( string value, Int32 number )
        {
            return new String ( Enumerable.Range ( 0, number ).SelectMany ( x => value ).ToArray ( ) );
        }

        /// <summary>
        /// Provides VB.Net Method String.StrDup ( Int32, char ) 
        /// like functionality.
        /// </summary>
        /// <param name="value">char value to be repeated.</param>
        /// <param name="number">Number of repeats.</param>
        /// <returns>Returns a string consisted of char repeated the specified number of times.</returns>
        public static string StringDuplicate ( char value, Int32 number )
        {
            return new String ( value, number );
        }
    }
}
