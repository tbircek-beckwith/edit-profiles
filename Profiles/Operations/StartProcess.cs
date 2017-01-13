using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EditProfiles.Data;
using EditProfiles.Properties;
using OMICRON.OCCenter;
using System.Text.RegularExpressions;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Start Processing of Scanning Omicron Test Files.
    /// </summary>
    public partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private short ProtectionLevel { get; set; }

        /// <summary>
        /// Holds current file name without folder path.
        /// </summary>
        private string CurrentFileName { get; set; }

        [Obsolete ( " Password must be defined by the user in the 'Password' field" )]
        private string Password { get; set; }

        private IList<string> FileNames;

        private IAutoApp OmicronApplication { get; set; }

        /// <summary>
        /// this holds copy of the current ViewModel for the business logic.
        /// </summary>
        private ViewModel ViewModel { get; set; }

        #endregion

        #region Obsolete Methods

        /// <summary>
        /// Start processing the user selected Omicron Test Files.
        /// </summary>
        /// <param name="fileNames">The file names to process.</param>
        /// <param name="password">The password to open files in case there is a protection.</param>
        /// <param name="viewModel">The copy of the current ViewModel for the business logic.</param>
        [Obsolete ( " Password must be defined by the user in the 'Password' field", true )]
        public void StartProcessing ( IList<string> fileNames, string password, ViewModel viewModel )
        {
            this.FileNames = fileNames;
            // this.Password = password;
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
        [Obsolete ( "This module will not support WPF functionalities.", true )]
        public void StartProcessing ( IList<string> fileNames, string password )
        {
            this.FileNames = fileNames;
            // this.Password = password;

            //// Following items are not allowed to be modified after "Find & Replace" button clicked.
            // this.ItemsToFind = MyCommons.MainFormRestrictedAccessTo.ItemsToFind;
            //this.ItemsToReplace = MyCommons.MainFormRestrictedAccessTo.ItemsToReplace;

            this.StartProcessingFiles ( );
        }

        #endregion                       

        #region IStartProcessInterface Members


        /// <summary>
        /// Start processing the user selected Omicron Test Files.
        /// </summary>
        /// <param name="fileNames">The file names to process.</param>
        /// <param name="viewModel">The copy of the current ViewModel for the business logic.</param>
        public void StartProcessing ( IList<string> fileNames, ViewModel viewModel )
        {
            this.FileNames = fileNames;
            this.ViewModel = viewModel;

            // Following items are not allowed to be modified after "Find & Replace" button clicked.
            this.ItemsToFind = this.ViewModel.FindWhatTextBoxText.Split ( '|' );            
            this.ItemsToReplace = this.ViewModel.ReplaceWithTextBoxText.Split ( '|' );

            this.TestModuleNamesToRemove = ListOfTestModulesToDelete(ItemsToFind);
            
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
                        
            try
            {
                Parallel.ForEach ( this.FileNames, MyCommons.ParallelingOptions, ( currentFile ) =>
                    {
                        // Increment current file number;
                        MyCommons.CurrentFileNumber++;
                        MyCommons.CurrentModuleNumber = 0;
                        MyCommons.MyViewModel.UpdateCommand.Execute ( null );

                        this.CurrentFileName = currentFile;
                        MyCommons.FileName = Path.GetFileName(CurrentFileName);
                        
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
                                        MyCommons.MyViewModel.DetailsTextBoxText = 
                                            MyCommons.LogProcess.Append (
                                                String.Format ( 
                                                        CultureInfo.InvariantCulture,
                                                        MyResources.Strings_CurrentFileName,
                                                        Environment.NewLine,
                                                        Repeat.StringDuplicate(Settings.Default.RepeatChar, Settings.Default.RepeatNumber),
                                                        Path.GetFileName ( CurrentFileName ),
                                                        String.Format ( CultureInfo.InvariantCulture,
                                                                MyResources.Strings_TestStart,
                                                                DateTime.Now ) ) )
                                            .ToString ( );


#if DEBUG
                                        Console.WriteLine ( "StartProcess -------------------------------" );
#endif

                                        // Open Omicron Document.
                                        // this.OmicronDocument = OpenDocument ( this.CurrentFileName, "" ); 
                                        this.OmicronDocument = OpenDocument ( CurrentFileName );

                                    }
                                    catch ( ArgumentException ae )
                                    {
                                        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                        ErrorHandler.Log ( ae, CurrentFileName );
                                        return;
                                    }
                                    catch ( AggregateException ae )
                                    {
                                        foreach ( Exception ex in ae.InnerExceptions )
                                        {
                                            // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                            ErrorHandler.Log ( ex, CurrentFileName );
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
                                        ErrorHandler.Log ( ex, CurrentFileName );
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

                                    }
                                    catch ( AggregateException ae )
                                    {
                                        foreach ( Exception ex in ae.InnerExceptions )
                                        {
                                            // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                            ErrorHandler.Log ( ex, CurrentFileName );
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
                                    if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                    {
                                        MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                    }

                                    // Close Omicron Control Center without saving any changes to the original file.
                                    this.OmicronDocument.Close(false);

                                }
                                catch (System.Runtime.InteropServices.COMException ae)
                                {
                                    ErrorHandler.Log(ae, CurrentFileName);
                                    return;
                                }
                                catch (AggregateException ae)
                                {
                                    foreach (Exception ex in ae.InnerExceptions)
                                    {
                                        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                        ErrorHandler.Log(ex, CurrentFileName);
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
                                    if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                    {
                                        MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                    }

                                    // Close Omicron Control Center Application
                                    this.OmicronApplication.Quit();

                                }
                                catch (System.Runtime.InteropServices.COMException ae)
                                {
                                    ErrorHandler.Log(ae, CurrentFileName);
                                }
                                catch (System.ObjectDisposedException ae)
                                {
                                    ErrorHandler.Log(ae, CurrentFileName);
                                }
                                catch (AggregateException ae)
                                {
                                    foreach (Exception ex in ae.InnerExceptions)
                                    {
                                        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                        ErrorHandler.Log(ex, CurrentFileName);
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
                                    catch (System.Runtime.InteropServices.COMException ae)
                                    {
                                        ErrorHandler.Log(ae, CurrentFileName);
                                    }
                                    catch (System.ObjectDisposedException ae)
                                    {
                                        ErrorHandler.Log(ae, CurrentFileName);
                                    }
                                    catch ( AggregateException ae )
                                    {
                                        foreach ( Exception ex in ae.InnerExceptions )
                                        {
                                            // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                            ErrorHandler.Log ( ex, CurrentFileName );
                                        }
                                        return;
                                    }
                                }
                                , MyCommons.CancellationToken )
                                .Wait ( );

                            MyCommons.MyViewModel.FileSideCoverText = MyResources.Strings_FormEndTest;
                            
                            // Refresh Process bars.
                            MyCommons.MyViewModel.UpdateCommand.Execute ( null );

#if DEBUG
                            Console.WriteLine ( " File opening completed " );
#endif
                        }
                        catch (System.Runtime.InteropServices.COMException ae)
                        {
                            ErrorHandler.Log(ae, CurrentFileName);
                        }
                        catch (System.ObjectDisposedException ae)
                        {
                            ErrorHandler.Log(ae, CurrentFileName);
                        }
                        catch ( AggregateException ae )
                        {
                            foreach ( Exception ex in ae.InnerExceptions )
                            {
                                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                ErrorHandler.Log ( ex, CurrentFileName );
                            }
                            return;
                        }
                    } );

            }
            catch ( OperationCanceledException oe )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( oe, CurrentFileName );
                return;
            }
            catch ( COMException ce )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ce, CurrentFileName );
                return;
            }
        }

        #endregion

    }
}
