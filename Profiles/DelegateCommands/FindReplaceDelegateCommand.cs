using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EditProfiles.Operations;
using EditProfiles.Properties;
using Microsoft.Win32;
using System.Globalization;


namespace EditProfiles
{
    /// <summary>
    /// Opens FileDialog UI interface to provide
    /// entry point to the this program.
    /// </summary>
    public class FindReplaceDelegateCommand
    {
        private Stopwatch stopwatch;

        /// <summary>
        /// 
        /// </summary>
        public DelegateCommand Command { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ViewModel ViewModel { get; set; }

        /// <summary>
        /// Provides FindReplace Button command.
        /// </summary>
        public FindReplaceDelegateCommand ( )
        {
            this.Command = new DelegateCommand ( this.ExecuteFindReplace, this.CanExecute );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unused"></param>
        public void ExecuteFindReplace ( object unused )
        {
            OpenFileDialog dlg = new OpenFileDialog ( );

            // Set OpenFileDialog defaults.
            dlg.Title = MyResources.Strings_OpenFileDialog;
            dlg.Filter = MyResources.Strings_FileDialogFilter;
            dlg.DefaultExt = MyResources.Strings_FileDialogDefault;
            dlg.Multiselect = true;

#if DEBUG
            dlg.InitialDirectory = System.IO.Path.Combine ( Environment.GetFolderPath ( Environment.SpecialFolder.DesktopDirectory ), MyResources.Strings_Debug_InitialDirectory );
#else
            dlg.InitialDirectory = Environment.GetFolderPath ( Environment.SpecialFolder.DesktopDirectory );
#endif

            if ( dlg.ShowDialog ( ) == true )
            {
                // initialize stopwatch 
                stopwatch = new Stopwatch ( );
                stopwatch.Start ( );

                // Initialize Common Properties.
                MyCommons.TokenSource = new CancellationTokenSource ( );
                MyCommons.CancellationToken = MyCommons.TokenSource.Token;
                MyCommons.LogProcess = new StringBuilder ( );

                // Start Processing the user specified files.
                IStartProcessInterface spi = new ProcessFiles ( );

                Task.Factory.StartNew ( ( ) =>
                    {

                        try
                        {
                            // Polling Cancellation Token Status.
                            // If cancellation requested throw an error and exit loop.
                            if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                            {
                                MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                            }

                            //// Start process.
                            //spi.StartProcessing ( dlg.FileNames, ViewModel.PasswordTextBoxText );

                            spi.StartProcessing ( dlg.FileNames, ViewModel.PasswordTextBoxText, this.ViewModel );
                        }
                        catch ( AggregateException ae )
                        {
                            //foreach ( Exception ex in ae.InnerException )
                            //{
                            // Save to the fileOutputFolder and print to the Debug window
                            // if build == Debug.
                            ErrorHandler.Log ( ae );
                            //}                            
                        }

                    }
                , MyCommons.CancellationToken )
                .ContinueWith ( value =>
                    {
                        // Reset and Activate controls.
                        ResetControls ( );
                    } );
            }
        }

        /// <summary>
        /// Reset Controls.
        /// </summary>
        private void ResetControls ()
        {
            stopwatch.Stop ( );

            ViewModel.DetailsTextBoxText =
                            ( string.Format (
                            CultureInfo.InvariantCulture,
                            MyResources.Strings_TestEnd,
                            DateTime.Now,
                            Environment.NewLine,
                            stopwatch.Elapsed.Days,
                            stopwatch.Elapsed.Hours,
                            stopwatch.Elapsed.Minutes,
                            stopwatch.Elapsed.Seconds,
                            stopwatch.Elapsed.Milliseconds,
                            Repeat.StringDuplicate ( '-', 50 ) ) );
        }

        /// <summary>
        /// Insert any validation requirement for the 
        /// FindReplaceCommand here.
        /// </summary>
        /// <param name="unused"></param>
        /// <returns></returns>
        public bool CanExecute ( object unused )
        {
            return true;
        }
    }
}
