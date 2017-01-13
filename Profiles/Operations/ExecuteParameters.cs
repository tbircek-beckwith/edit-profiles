﻿using System;
using System.Globalization;
using System.Text;
using System.Threading;
using EditProfiles.Data;
using EditProfiles.Properties;
using OMICRON.OMExec;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Extracts Omicron Execute MyTestModule parameters.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        // Omicron Execute Application.
        private IAutoApp ExecuteApp { get; set; }

        // Omicron Execute Document.
        private IAutoDoc ExecuteDoc { get; set; }

        // Omicron MyTestModule
        private OMICRON.OCCenter.IAutoTM MyTestModule { get; set; }

        // Extracted Execute MyTestModule parameter values.
        private StringBuilder ExecuteParameter { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Main Method to extract values form Execute MyTestModule.
        /// </summary>
        /// <param name="testModule">Execute MyTestModule to process.</param>
        /// <returns>Returns parameter values of Execute MyTestModule parameters.</returns>
        public string Retrieve ( OMICRON.OCCenter.IAutoTM testModule )
        {
            try
            {
                if ( testModule == null )
                {
                    throw new ArgumentNullException ( "testModule" );
                }

                // Assing new test module.
                this.MyTestModule = testModule;

                // Load Omicron Execute Application.
                this.ExecuteApp = testModule.Specific; // this.MyTestModule.Specific;

                // Load Omicron Execute Document.
                this.ExecuteDoc = ExecuteApp.Document;

                // Sets visibility of the Test Module while the program running.
                ExecuteApp.Visible = Settings.Default.TestModulesVisibility;

                return this.RetrieveParameters ( );

            }
            catch ( ArgumentNullException ae )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ae, this.CurrentFileName );

                // Terminate Execute Module.
                KillOmicronProcesses ( MyTestModule.ProgID ); // ProgId.Execute );
                return string.Empty;
            }
        }

        #endregion

        #region Private Methods

        private string RetrieveParameters ( )
        {

#if DEBUG
            Console.WriteLine ( " -------------Start of Execute Module --------------" );
            Console.WriteLine ( " Execute Test Module thread: {0}", Thread.CurrentThread.GetHashCode ( ) );
            Console.WriteLine ( " Connecting Test Module." );
#endif
            // Connect to the Test Module.
            MyTestModule.Connect ( );

            // Clear Test Modules
            // If the Setting is set to true Clear test modules.
            // Otherwise leave them as is.
            if ( Settings.Default.ClearTestModules == true )
            {
#if DEBUG
                Console.WriteLine ( " Clearing Test Module." );
#endif
                // If it is not clear cannot change the parameters.
                MyTestModule.Clear ( );
            }

            // Following code to handle System.Runtime.InteropServices.COMException (0x8001010A) error.
            // Generated by Omicron Engine while closing and starting a new Omicron Application File.
            // Without this modification the program would fail open second and subsequent Omicron Test Files.
            // http://codereview.stackexchange.com/questions/582/handling-com-exceptions-busy-codes
            //
            bool retry = false;

            // Add a loopcounter to protect application from endless loops.
            int loopCounter = 0;
            // Add a max loop counter value to break the [do...while] loop.
            const int MAX_LOOP_COUNTER = 5;

            do
            {
                try
                {
                    // Find the user specified texts in the parameters.
                    // Replace the user specified texts.
                    this.ExecuteParameter = FindAndReplaceParameters ( ExecuteDoc.Parameters );

                }
                catch ( System.Runtime.InteropServices.COMException e )
                {
                    if ( retry = e.ShouldRetry ( ) )
                    {
                        // Execute Module is busy. Wait ...
                        Thread.Sleep ( WaitTime / 10 );

                        // Find the user specified texts in the parameters.
                        // Replace the user specified texts.
                        ExecuteParameter = FindAndReplaceParameters ( ExecuteDoc.Parameters );

#if DEBUG
                        Console.WriteLine ( " LOOP COUNTER = {0} ", loopCounter );
                
#endif
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
                        if ( loopCounter < MAX_LOOP_COUNTER )
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
            Console.WriteLine ( " Parameters  : {0}", ExecuteParameter.ToString ( ) );
            Console.WriteLine ( " Find what   : {0}", MyCommons.MyViewModel.FindWhatTextBoxText );
            Console.WriteLine ( " Replace with: {0}", MyCommons.MyViewModel.ReplaceWithTextBoxText );
            Console.WriteLine ( );

            if ( !( ExecuteDoc.Parameters == ExecuteParameter.ToString ( ) ) )
            {
                Console.WriteLine ( " CHANGE NEEDED " );
                Console.WriteLine ( " Found matching parameters: {2} Find: {0} {2} Repl: {1} ",
                                    ExecuteDoc.Parameters,
                                    ExecuteParameter.ToString ( ),
                                    Environment.NewLine
                                    );

            }
#endif

            // Show detailed output if the user wants it.
            if ( Settings.Default.ShowDetailedOutput  )
            {
                // Update DetailsTextBoxText.
                MyCommons.MyViewModel.DetailsTextBoxText =
                    MyCommons.LogProcess.Append (
                    string.Format (
                            CultureInfo.InvariantCulture,
                            ExecuteDoc.Parameters == ExecuteParameter.ToString ( ) ? MyResources.Strings_NoChanges : MyResources.Strings_ParamsChanged,
                            ExecuteDoc.Parameters,                    
                            Environment.NewLine,
                            ExecuteParameter.ToString ( ) ) )
                    .ToString ( );
            }
            
            // Update Omicron Execute Module parameters.
            ExecuteDoc.Parameters = ExecuteParameter.ToString ( );
                        
            // Exit the MyTestModule.
            ExecuteApp.Quit ( );

            // Disconnect from the Execute Module.
            MyTestModule.Disconnect ( );

#if DEBUG
            Console.WriteLine ( " Quitting Test Module." );
            Console.WriteLine ( " Disconnecting Test Module." );
            Console.WriteLine ( " -------------End of Exceute Module --------------" );
#endif

            try
            {

                // Terminate Execute Module in memory.
                KillOmicronProcesses ( MyTestModule.ProgID ); // ProgId.Execute );

            }

            catch ( AggregateException ae )
            {
                foreach ( Exception ex in ae.InnerExceptions )
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log ( ex, CurrentFileName );
                }

                // Terminate Execute Module in memory.
                KillOmicronProcesses ( MyTestModule.ProgID );  // ProgId.Execute );

                return string.Empty;
            }

            return ExecuteParameter.ToString ( );

        }

        #endregion

    }
}
