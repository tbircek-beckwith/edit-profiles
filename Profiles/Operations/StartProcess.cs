using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EditProfiles.Data;
using EditProfiles.Properties;
using OMICRON.OCCenter;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Start Processing of Scanning Omicron Test Files.
    /// </summary>
    public partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private string CurrentFileName { get; set; }

        private string Password { get; set; }

        private IList<string> FileNames;

        private IAutoApp OmicronApplication { get; set; }

        /// <summary>
        /// this holds copy of the current ViewModel for the business logic.
        /// </summary>
        private ViewModel ViewModel { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Start processing the user selected Omicron Test Files.
        /// </summary>
        /// <param name="fileNames">The file names to process.</param>
        /// <param name="password">The password to open files in case there is a protection.</param>
        /// <param name="viewModel">The copy of the current ViewModel for the business logic.</param>
        public void StartProcessing ( IList<string> fileNames, string password, ViewModel viewModel )
        {
            this.FileNames = fileNames;
            this.Password = password;
            this.ViewModel = viewModel;

            // Following items are not allowed to be modified after "Find & Replace" button clicked.
            this.ItemsToFind = this.ViewModel.FindWhatTextBoxText.Split ( '|' );
            //  this.ItemsToFindList ( this.ViewModel.FindWhatTextBoxText );

            this.ItemsToReplace = this.ViewModel.ReplaceWithTextBoxText.Split ( '|' );
            //this.ItemsToFindList ( this.ViewModel.ReplaceWithTextBoxText );

            this.StartProcessingFiles ( );
        }

        /// <summary>
        /// Start processing the user selected Omicron Test Files.
        /// </summary>
        /// <param name="fileNames">The file names to process.</param>
        /// <param name="password">The password to open files in case there is a protection.</param>
        [Obsolete ( "This module will not support WPF functionalities." )]
        public void StartProcessing ( IList<string> fileNames, string password )
        {
            this.FileNames = fileNames;
            this.Password = password;

            //// Following items are not allowed to be modified after "Find & Replace" button clicked.
            // this.ItemsToFind = MyCommons.MainFormRestrictedAccessTo.ItemsToFind;
            //this.ItemsToReplace = MyCommons.MainFormRestrictedAccessTo.ItemsToReplace;

            this.StartProcessingFiles ( );
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Start processing the user selected Omicron Test Files.
        /// </summary>
        private void StartProcessingFiles ( )
        {
            // Reset File counter.
            MyCommons.CurrentFileNumber = 0;

            // Update FileProcessBar;
            MyCommons.TotalFileNumber = this.FileNames.Count;
            MyCommons.MyViewModel.FileProgressBarMax = this.FileNames.Count;

            // Refresh Process bars.
            MyCommons.MyViewModel.UpdateCommand.Execute ( null );

            ParallelOptions parallelingOptions = new ParallelOptions ( );
            parallelingOptions.MaxDegreeOfParallelism = 1;
            parallelingOptions.CancellationToken = MyCommons.CancellationToken;

            try
            {
                Parallel.ForEach ( this.FileNames, parallelingOptions, ( currentFile ) =>
                    {
                        // Increment current file number;
                        MyCommons.CurrentFileNumber++;
                        MyCommons.CurrentModuleNumber = 0;
                        MyCommons.MyViewModel.UpdateCommand.Execute ( null );

                        this.CurrentFileName = currentFile;

                        try
                        {
                            Task.Factory.StartNew ( ( ) =>
                                {
                                    try
                                    {
                                        // Polling CancellationToken's status.
                                        // If cancellation requested throw error and exit loop.
                                        if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                                        {
                                            MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                                        }

                                        // Update DetailsTextBoxText.
                                        this.ViewModel.DetailsTextBoxText =
                                            string.Format ( CultureInfo.InvariantCulture,
                                                            MyResources.Strings_CurrentFileName,
                                                            Environment.NewLine,
                                                            Repeat.StringDuplicate ( '-', 50 ),
                                                            Path.GetFileName ( currentFile ),
                                                            string.Format ( CultureInfo.InvariantCulture,
                                            //"Start time: {0}",
                                                            MyResources.Strings_TestStart,
                                                            DateTime.Now ) );
#if DEBUG
                                        Console.WriteLine ( "StartProcess -------------------------------" );
#endif

                                        // Open Omicron Document.
                                         this.OmicronDocument = OpenDocument ( currentFile, "" );

                                    }
                                    catch ( ArgumentException ae )
                                    {
                                        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                        ErrorHandler.Log ( ae, this.CurrentFileName );
                                        return;
                                    }
                                    catch ( AggregateException ae )
                                    {
                                        foreach ( Exception ex in ae.InnerExceptions )
                                        {
                                            // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                            ErrorHandler.Log ( ex, this.CurrentFileName );
                                        }
                                        return;
                                    }
                                }
                                , MyCommons.CancellationToken )
                                .ContinueWith ( scanThread =>
                            {
                                try
                                {

                                    // Polling CancellationToken's status.
                                    // If cancellation requested throw error and exit loop.
                                    if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                                    {
                                        MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                                    }

                                    // Scan the Test Document for the Test Modules.
                                    Scan ( );

                                }
                                catch ( AggregateException ae )
                                {
                                    foreach ( Exception ex in ae.InnerExceptions )
                                    {
                                        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                        ErrorHandler.Log ( ex, this.CurrentFileName );
                                    }
                                    return;
                                }
                            }
                            , MyCommons.CancellationToken )
                            .Wait ( );

                            // Save the new file with a different name.
                            Task.Factory.StartNew ( ( ) =>
                                {
                                    try
                                    {

                                        // Polling CancellationToken's status.
                                        // If cancellation requested throw error and exit loop.
                                        if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                                        {
                                            MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                                        }

                                        // Save the new file.
                                        SaveOmicronFiles ( this.OmicronDocument.FullName, true );

                                        // TODO: Save files with their original protection level.

                                    }
                                    catch ( AggregateException ae )
                                    {
                                        foreach ( Exception ex in ae.InnerExceptions )
                                        {
                                            // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                            ErrorHandler.Log ( ex, this.CurrentFileName );
                                        }
                                        return;
                                    }
                                }
                                , MyCommons.CancellationToken )
                                .ContinueWith ( closeThread =>
                            {
                                try
                                {

                                    // Polling CancellationToken's status.
                                    // If cancellation requested throw error and exit loop.
                                    if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                                    {
                                        MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                                    }

                                    // Close Omicron Control Center without saving any changes to the original file.
                                    this.OmicronDocument.Close ( false );

                                }
                                catch ( AggregateException ae )
                                {
                                    foreach ( Exception ex in ae.InnerExceptions )
                                    {
                                        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                        ErrorHandler.Log ( ex, this.CurrentFileName );
                                    }
                                    return;
                                }
                            }
                            , MyCommons.CancellationToken )
                            .ContinueWith ( threadCloseApp =>
                            {
                                try
                                {
                                    // Polling CancellationToken's status.
                                    // If cancellation requested throw error and exit loop.
                                    if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                                    {
                                        MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                                    }

                                    // Close Omicron Control Center Application
                                    this.OmicronApplication.Quit ( );

                                }
                                catch ( AggregateException ae )
                                {
                                    foreach ( Exception ex in ae.InnerExceptions )
                                    {
                                        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                        ErrorHandler.Log ( ex, this.CurrentFileName );
                                    }
                                    return;
                                }
                            }
                            , MyCommons.CancellationToken )
                            .Wait ( );

                            Task.Factory.StartNew ( ( ) =>
                                {
                                    try
                                    {
                                        // TODO: Verify this change.
                                        // Garbage Collection.
                                        this.OmicronApplication = null;
                                        this.OmicronDocument = null;

                                        // Polling CancellationToken's status.
                                        // If cancellation requested throw error and exit loop.
                                        if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                                        {
                                            MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                                        }

                                        // Terminate Omicron Processes.
                                        KillOmicronProcesses ( );
                                    }
                                    catch ( AggregateException ae )
                                    {
                                        foreach ( Exception ex in ae.InnerExceptions )
                                        {
                                            // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                            ErrorHandler.Log ( ex, this.CurrentFileName );
                                        }
                                        return;
                                    }
                                }
                                , MyCommons.CancellationToken )
                                .Wait ( );
#if DEBUG
                            Console.WriteLine ( " File opening completed " );
#endif
                        }                        
                        catch ( AggregateException ae )
                        {
                            foreach ( Exception ex in ae.InnerExceptions )
                            {
                                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                ErrorHandler.Log ( ex, this.CurrentFileName );
                            }
                            return;
                        }
                    } );

            }
            catch ( OperationCanceledException oe )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( oe, this.CurrentFileName );
                return;
            }
            catch ( COMException ce )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ce, this.CurrentFileName );
                return;
            }
        }

        #endregion

    }
}
