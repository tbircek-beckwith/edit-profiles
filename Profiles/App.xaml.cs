using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using EditProfiles.Operations;
using EditProfiles.Properties;
using System.Diagnostics;
using System.IO;

namespace EditProfiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup ( object sender, StartupEventArgs e )
        {
            if ( !Directory.Exists (MyCommons.FileOutputFolder) )
            {
                Directory.CreateDirectory ( MyCommons.FileOutputFolder );
            }

            // Verify none of the Omicron modules running,
            // without it unexpected behaviors would occur.
            Task.Factory.StartNew ( ( ) =>
                {
                    IStartProcessInterface spi = new ProcessFiles ( );
                    spi.KillOmicronProcesses ( );
                }
                // Allow the user to start right away without waiting to
                // terminate all running Omicron modules.
                , MyCommons.CancellationToken );

            EditProfiles.ViewFactory factory = new EditProfiles.ViewFactory ( );

            EditProfiles.ViewInfrastructure infrastructure = factory.Create ( );

            infrastructure.View.DataContext = infrastructure.ViewModel;

#if DEBUG
            infrastructure.ViewModel.ReplaceWithTextBoxText = MyResources.Strings_Debug_TextBoxReplace;
            infrastructure.ViewModel.FindWhatTextBoxText = MyResources.Strings_Debug_TextBoxFind;

#endif

            infrastructure.View.Show ( );

        }

        private void Application_Exit ( object sender, ExitEventArgs e )
        {
            try
            {
                // terminate any running Omicron related process.
                IStartProcessInterface spi = new ProcessFiles ( );
                do
                {

                } while ( !( Task.Factory.StartNew ( ( ) => spi.KillOmicronProcesses ( ) ) ).Result );

                // Log the operations completed so far.
                if ( true )
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

    }
}
