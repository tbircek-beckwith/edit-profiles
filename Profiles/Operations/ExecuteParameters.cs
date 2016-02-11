using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EditProfiles.Data;
using EditProfiles.Properties;
using OMICRON.OMExec;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Extracts Omicron Execute TestModule parameters.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        // Omicron Execute Application.
        private IAutoApp ExecuteApp { get; set; }

        // Omicron Execute Document.
        private IAutoDoc ExecuteDoc { get; set; }

        // Omicron TestModule
        private OMICRON.OCCenter.IAutoTM TestModule { get; set; }

        // Extracted Execute TestModule parameter values.
        private StringBuilder ExecuteParameter { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Main Method to extract values form Execute TestModule.
        /// </summary>
        /// <param name="testModule">Execute TestModule to process.</param>
        /// <returns>Returns parameter values of Execute TestModule parameters.</returns>
        public string Retrieve ( OMICRON.OCCenter.IAutoTM testModule )
        {
            try
            {
                if ( testModule == null )
                {
                    throw new ArgumentNullException ( "testModule" );
                }

                // Assing new test module.
                this.TestModule = testModule;

                // Load Omicron Execute Application.
                this.ExecuteApp = testModule.Specific; // this.TestModule.Specific;

                // Load Omicron Execute Document.
                this.ExecuteDoc = this.ExecuteApp.Document;

                // Sets visibility of the Test Module while the program running.
                this.ExecuteApp.Visible = Settings.Default.TestModulesVisibility;

                return this.RetrieveParameters ( );

            }
            catch ( ArgumentNullException ae )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ae, this.CurrentFileName );

                // Terminate Execute Module.
                KillOmicronProcesses ( ProgId.Execute );
                return string.Empty;
            }
        }

        #endregion

        #region Private Methods

        private string RetrieveParameters ( )
        {

            Task.Factory.StartNew ( ( ) =>
            {
                // Polling CancellationToken's status.
                // If cancellation requested throw error and exit loop.
                if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                {
                    MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                }
#if DEBUG
                Console.WriteLine ( " -------------Start of Execute Module --------------" );
                Console.WriteLine ( " Execute Test Module thread: {0}", Thread.CurrentThread.GetHashCode ( ) );
                Console.WriteLine ( " Connecting Test Module." );
#endif
                // Connect to the Test Module.
                this.TestModule.Connect ( );

            }
            , MyCommons.CancellationToken )
            .Wait ( );

            Task.Factory.StartNew ( ( ) =>
                {
                    // Polling CancellationToken's status.
                    // If cancellation requested throw error and exit loop.
                    if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                    {
                        MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                    }

                    // Clear Test Modules
                    // If the Setting is set to true Clear test modules.
                    // Otherwise leave them as is.
                    if ( Settings.Default.ClearTestModules == true )
                    {
#if DEBUG
                        Console.WriteLine ( " Clearing Test Module." );
#endif
                        // If it is not clear cannot change the parameters.
                        this.TestModule.Clear ( );
                    }

                }
                , MyCommons.CancellationToken )
                .Wait ( );

            Task.Factory.StartNew ( ( ) =>
                    {

                        // Polling CancellationToken's status.
                        // If cancellation requested throw error and exit loop.
                        if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                        {
                            MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                        }

                        // Following code to handle System.Runtime.InteropServices.COMException (0x8001010A) error.
                        // Generated by Omicron Engine while closing and starting a new Omicron Application File.
                        // Without this modification the program would fail open second and subsequent Omicron Test Files.
                        // http://codereview.stackexchange.com/questions/582/handling-com-exceptions-busy-codes
                        bool retry = false;

                        // Add a loopcounter to protect application from endless loops.
                        int loopCounter = 0;
                        // Add a max loop counter value to break the [do...while] loop.
                        const int MAX_LOOP_Counter = 5;

                        do
                        {
                            try
                            {
                                // Find the user specified texts in the parameters.
                                // Replace the user specified texts.
                                this.ExecuteParameter = FindAndReplaceParameters ( this.ExecuteDoc.Parameters );

                            }
                            catch ( System.Runtime.InteropServices.COMException e )
                            {
                                if ( retry = e.ShouldRetry ( ) )
                                {
                                    // Execute Module is busy. Wait ...
                                    Thread.Sleep ( this.WaitTime / 10 );

                                    // Find the user specified texts in the parameters.
                                    // Replace the user specified texts.
                                    this.ExecuteParameter = FindAndReplaceParameters ( this.ExecuteDoc.Parameters );

                                    // increment the loop counter.
                                    loopCounter++;

                                    // retry has to be false to re-run this loop.
                                    // otherwise the program would stop here indefinitely.
                                    retry = false;
                                }
                                else
                                {
                                    // To enable running [do ... while] one more time.
                                    // In test this routine always ran one time.
                                    // I am not sure if requires to a max number of tries.
                                    if ( loopCounter < MAX_LOOP_Counter )
                                    {
#if DEBUG
                                        Console.WriteLine ( " LOOP COUNTER = {0} ", loopCounter );
#endif
                                        // Countinue the loop.
                                        retry = true;
                                    }
                                    else
                                    {
                                        // exit the loop.
                                        break;
                                    }

                                    // calling throw without a parameter *rethrows* 
                                    // which is important to preserve the stack trace.
                                    throw;
                                }
                            }
                        } while ( retry );

#if DEBUG
                        Console.WriteLine ( " Parameters  : {0}", this.ExecuteParameter.ToString ( ) );
                        Console.WriteLine ( " Find what   : {0}", ViewModel.FindWhatTextBoxText ); // MyCommons.MainFormRestrictedAccessTo.ItemsToFindString );
                        Console.WriteLine ( " Replace with: {0}", ViewModel.ReplaceWithTextBoxText ); // MyCommons.MainFormRestrictedAccessTo.ItemsToReplaceString );
                        Console.WriteLine ( );

                        if ( !( this.ExecuteDoc.Parameters == this.ExecuteParameter.ToString ( ) ) )
                        {
                            Console.WriteLine ( " CHANGE NEEDED " );
                            Console.WriteLine ( " Found matching parameters: {2} Find: {0} {2} Repl: {1} ",
                                                this.ExecuteDoc.Parameters,
                                                this.ExecuteParameter.ToString ( ),
                                                Environment.NewLine
                                                );

                        }
#endif

                        // Update parameters if a change made.
                        if ( !( this.ExecuteDoc.Parameters == this.ExecuteParameter.ToString ( ) ) )
                        {
                            // Update LogTextBox and StringBuilder.
                            ViewModel.DetailsTextBoxText =
                                string.Format ( CultureInfo.InvariantCulture,
                                // " Found matching parameters: {2} Find a match:  {0}{2} Replaced with: {1} ",
                                                MyResources.Strings_ParamsChanged,
                                                this.ExecuteDoc.Parameters,
                                                this.ExecuteParameter.ToString ( ),
                                                Environment.NewLine
                                                );
                            // Update Omicron Execute Module parameters.
                            this.ExecuteDoc.Parameters = this.ExecuteParameter.ToString ( );
                        }
                        else
                        {
                            // Update LogTextBox and StringBuilder.
                            ViewModel.DetailsTextBoxText =
                                string.Format ( CultureInfo.InvariantCulture,
                                // " NO changes to following parameters: {0} ",
                                MyResources.Strings_NoChanges,
                                this.ExecuteDoc.Parameters,
                                Environment.NewLine
                                );
                        }
                    }
                    , MyCommons.CancellationToken )
                    .Wait ( );

            Task.Factory.StartNew ( ( ) =>
                 {
                     // Polling CancellationToken's status.
                     // If cancellation requested throw error and exit loop.
                     if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                     {
                         MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                     }
#if DEBUG
                     Console.WriteLine ( " Quitting Test Module." );
#endif
                     // Exit the TestModule.
                     this.ExecuteApp.Quit ( );

                 }
                 , MyCommons.CancellationToken )
                 .Wait ( );

            Task.Factory.StartNew ( ( ) =>
            {
                // Polling CancellationToken's status.
                // If cancellation requested throw error and exit loop.
                if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                {
                    MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                }

#if DEBUG
                Console.WriteLine ( " Disconnecting Test Module." );
                Console.WriteLine ( " -------------End of Exceute Module --------------" );
#endif
                // Disconnect from the Execute Module.
                this.TestModule.Disconnect ( );

            }
            , MyCommons.CancellationToken )
            .Wait ( );

            try
            {
                Task.Factory.StartNew ( ( ) =>
                {
                    // Polling CancellationToken's status.
                    // If cancellation requested throw error and exit loop.
                    if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                    {
                        MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                    }

                    // Terminate Execute Module in memory.
                    KillOmicronProcesses ( ProgId.Execute );

                }
                , MyCommons.CancellationToken )
                .Wait ( );
            }

            catch ( AggregateException ae )
            {
                foreach ( Exception ex in ae.InnerExceptions )
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log ( ex, this.CurrentFileName );
                }

                // Terminate Execute Module in memory.
                KillOmicronProcesses ( ProgId.Execute );

                return string.Empty;
            }

            return this.ExecuteParameter.ToString ( );

        }

        #endregion

    }
}
