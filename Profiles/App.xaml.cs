using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EditProfiles.Behaviors;
using EditProfiles.Data;
using EditProfiles.Operations;
using EditProfiles.Properties;

namespace EditProfiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // private void Application_Startup ( object sender, StartupEventArgs e )
        /// <summary>
        /// Start up 
        /// </summary>
        /// <param name="e">arguments</param>
        protected override void OnStartup ( StartupEventArgs e )
        {

            #region Catch All Unhandled Exceptions

            // Following code found on stackoverflow.com
            // http://stackoverflow.com/questions/10202987/in-c-sharp-how-to-collect-stack-trace-of-program-crash

            AppDomain currentDomain = default ( AppDomain );
            currentDomain = AppDomain.CurrentDomain;

            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;

            // Handler for exceptions in thread behind forms.
            Application.Current.DispatcherUnhandledException += GlobalThreadExceptionHandler;
          
            #endregion

            #region Create LocalApplication Folder.

            if ( !Directory.Exists ( MyCommons.FileOutputFolder ) )
            {
                Directory.CreateDirectory ( MyCommons.FileOutputFolder );
            }

            #endregion

            #region Error Logging.

            // Add textwriter for console.
            TextWriterTraceListener consoleOut = 
                new TextWriterTraceListener ( System.Console.Out );

            Debug.Listeners.Add ( consoleOut );

            TextWriterTraceListener fileOut =
                new TextWriterTraceListener ( File.CreateText ( Path.Combine (
                                                                            MyCommons.FileOutputFolder,
                                                                            MyResources.Strings_Error_FileName ) ) );
            Debug.Listeners.Add ( fileOut );

            #endregion

            #region Application Starts Here.

            base.OnStartup ( e );

            //// Verify none of the Omicron modules running,
            //// without it unexpected behaviors would occur.
            DispatchService.Invoke ( ( ) =>
                {

                    IStartProcessInterface spi = new ProcessFiles ( );
                    spi.KillOmicronProcesses ( );       
                } );

            ViewFactory factory = new ViewFactory ( );

            ViewInfrastructure infrastructure = factory.Create ( );

            infrastructure.View.DataContext = infrastructure.ViewModel;

#if DEBUG
            infrastructure.ViewModel.ReplaceWithTextBoxText = MyResources.Strings_Debug_TextBoxReplace;
            infrastructure.ViewModel.FindWhatTextBoxText = MyResources.Strings_Debug_TextBoxFind;

#endif

            MyCommons.MyViewModel.FileSideCoverText = MyResources.Strings_FormStartTest;
            MyCommons.MyViewModel.ModuleSideCoverText = MyResources. Strings_FormStartModuleTest;

            MyCommons.MyViewModel.Disable = true;

            infrastructure.View.Show ( );

            #endregion

        }

        //  private void Application_Exit ( object sender, ExitEventArgs e )
        /// <summary>
        /// Exit 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit ( ExitEventArgs e )
        {
            base.OnExit ( e );

            try
            {
                // terminate any running Omicron related process.
                IStartProcessInterface spi = new ProcessFiles ( );
                do
                {
                    if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                    {
                        break;
                    }
                } while ( !( Task.Factory.StartNew ( ( ) => spi.KillOmicronProcesses ( ) ) ).Result );


                // Log the operations completed so far.
                if ( MyCommons.LogProcess.Length > 0 )
                {
                    // write log to the file.
                    File.WriteAllText ( Path.Combine ( MyCommons.FileOutputFolder, Settings.Default.LogFileName ), MyCommons.LogProcess.ToString ( ) );

                    // open saved log file for the user.
                    Process.Start ( Path.Combine ( MyCommons.FileOutputFolder, Settings.Default.LogFileName ) );
                }
            }
            catch ( NullReferenceException nre )
            {
                // save to the fileOutputFolder and print to Debug window 
                // if the project build is in DEBUG.
                ErrorHandler.Log ( nre );
                Debug.Flush ( );
                Trace.Flush ( );
            }
            finally
            {
                // save debug and trace info before terminating the program.
                Debug.Flush ( );
                Trace.Flush ( );

                // Verify CancellationToken is not null before cancelling it.
                if ( MyCommons.CancellationToken.CanBeCanceled )
                {
                    MyCommons.TokenSource.Dispose ( );
                }
            }
        }

        #region Catch All Unhandled Exceptions

        // Following code found on stackoverflow.com
        // http://stackoverflow.com/questions/10202987/in-c-sharp-how-to-collect-stack-trace-of-program-crash
        private static void GlobalUnhandledExceptionHandler ( object sender, UnhandledExceptionEventArgs e )
        {
            Exception ex = default ( Exception );
            ex = ( Exception ) e.ExceptionObject;

            // Save to the fileOutputFolder and print to Debug window if the project build is in DEBUG.
            ErrorHandler.Log ( ex );
            Debug.Flush ( );
            Trace.Flush ( );
        }

        // Following code found on stackoverflow.com
        // http://stackoverflow.com/questions/10202987/in-c-sharp-how-to-collect-stack-trace-of-program-crash
        private static void GlobalThreadExceptionHandler ( object sender, DispatcherUnhandledExceptionEventArgs e )
        {
            Exception ex = default ( Exception );
            ex = e.Exception;

            // Save to the fileOutputFolder and print to Debug window if the project build is in DEBUG.
            ErrorHandler.Log ( ex );
            Debug.Flush ( );
            Trace.Flush ( );
        }

        #endregion

    }
}
