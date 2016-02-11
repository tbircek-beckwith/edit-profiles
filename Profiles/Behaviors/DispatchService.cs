using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using EditProfiles.Data;
using EditProfiles.Operations;

namespace EditProfiles.Behaviors
{
    /// <summary>
    /// Launches Dispatcher to handle background work.
    /// </summary>
    public static class DispatchService
    {
        /// <summary>
        /// Launches Dispatcher to handle background actions.
        /// </summary>
        /// <param name="action"></param>
        /// This code copied from 
        /// http://blogs.msdn.com/b/davidrickard/archive/2010/04/01/using-the-dispatcher-with-mvvm.aspx
        /// 
        public static void Invoke ( Action action )
        {
            try
            {
                Dispatcher dispatchObject = Application.Current.Dispatcher;

                if ( dispatchObject == null || dispatchObject.CheckAccess ( ) )
                {

#if DEBUG
                    Console.WriteLine ( "DispatchService running: {0}", dispatchObject.Thread.GetHashCode ( ) );
#endif

                    action ( );
                }
                else
                {

#if DEBUG
                    Console.WriteLine ( "INVOKE DispatchService running: {0}", dispatchObject.Thread.GetHashCode ( ) );
#endif

                    dispatchObject.Invoke ( action, DispatcherPriority.Background, MyCommons.CancellationToken );
                }
            }
            catch ( TargetParameterCountException ex )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ex, action.ToString() );
            }
        }
    }
}
