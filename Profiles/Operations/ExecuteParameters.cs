﻿using System;
using System.Globalization;
using System.Text;
using System.Threading;
using EditProfiles.Data;
using EditProfiles.Properties;
using OMICRON.OMExec;
using System.Diagnostics;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Extracts Omicron Execute MyTestModule parameters.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties
        // Omicron Execute Application.
        private IAutoApp application { get; set; }

        // Omicron Execute Document.
        private IAutoDoc document { get; set; }

        // Omicron MyTestModule
        private OMICRON.OCCenter.IAutoTM testModule { get; set; }

        // Extracted Execute MyTestModule parameter values.
        private StringBuilder testModuleParameters { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Main Method to extract values form Execute MyTestModule.
        /// </summary>
        /// <param name="testModuleToModify">Execute MyTestModule to process.</param>
        /// <returns>Returns parameter values of Execute MyTestModule parameters.</returns>
        public string Retrieve(OMICRON.OCCenter.IAutoTM testModuleToModify)
        {
            try
            {
                if (testModuleToModify == null)
                {
                    throw new ArgumentNullException("testModuleToModify");
                }

                // Assing new test module.
                this.testModule = testModuleToModify;
                // Clear the module so values can be changed.
                testModule.Clear();

                // Load Omicron Execute Application.
                this.application = testModule.Specific; // this.MyTestModule.Specific;

                // Load Omicron Execute Document.
                this.document = application.Document;

                // Sets visibility of the Test Module while the program running.
                application.Visible = Settings.Default.TestModulesVisibility;

                return this.RetrieveParameters();

            }
            catch (ArgumentNullException ae)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ae, this.CurrentFileName);

                // Terminate Execute Module.
                KillOmicronProcesses(testModule.ProgID); // ProgId.Execute );
                return string.Empty;
            }
        }

        #endregion

        #region Private Methods

        private string RetrieveParameters()
        {

            Debug.WriteLine("\t--- Start of Execute Module ---");
            Debug.WriteLine("\t\tExecute Test Module thread: " + Thread.CurrentThread.GetHashCode());
            Debug.WriteLine("\t\tConnecting Test Module.");

            // Connect to the Test Module.
            testModule.Connect();

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
                    this.testModuleParameters = FindAndReplaceParameters(document.Parameters);
                    Debug.WriteLine("\t\t\tPath value: " + document.Path);
                    Debug.WriteLine("\t\t\tExecution options: " + document.Option);
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    if (retry = e.ShouldRetry())
                    {
                        // Execute Module is busy. Wait ...
                        Thread.Sleep(WaitTime / 10);

                        // Find the user specified texts in the parameters.
                        // Replace the user specified texts.
                        testModuleParameters = FindAndReplaceParameters(document.Parameters);


                        Debug.WriteLine(" LOOP COUNTER = " + loopCounter);

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
                        if (loopCounter < MAX_LOOP_COUNTER)
                        {

                            Debug.WriteLine(" LOOP COUNTER = " + loopCounter);


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
            } while (retry);


            Debug.WriteLine(" Parameters  : " + testModuleParameters.ToString());
            Debug.WriteLine(" Find what   : " + MyCommons.MyViewModel.FindWhatTextBoxText);
            Debug.WriteLine(" Replace with: " + MyCommons.MyViewModel.ReplaceWithTextBoxText);
            Debug.WriteLine("");

            if (!(document.Parameters == testModuleParameters.ToString()))
            {
                Debug.WriteLine(" CHANGE NEEDED ");
                Debug.WriteLine(string.Format(" Found matching parameters: {2} Find: {0} {2} Repl: {1} ",
                                    document.Parameters,
                                    testModuleParameters.ToString(),
                                    Environment.NewLine)
                                    );

            }


            // Show detailed output if the user wants it.
            if (Settings.Default.ShowDetailedOutput)
            {
                // Update DetailsTextBoxText.
                MyCommons.MyViewModel.DetailsTextBoxText =
                    MyCommons.LogProcess.Append(
                    string.Format(
                            CultureInfo.InvariantCulture,
                            document.Parameters == testModuleParameters.ToString() ? MyResources.Strings_NoChanges : MyResources.Strings_ParamsChanged,
                            document.Parameters,
                            Environment.NewLine,
                            testModuleParameters.ToString()))
                    .ToString();
            }

            // Update Omicron Execute Module parameters.
            document.Parameters = testModuleParameters.ToString();

            // Exit the MyTestModule.
            application.Quit();

            // Disconnect from the Execute Module.
            testModule.Disconnect();

            Debug.WriteLine(" Quitting Test Module.");
            Debug.WriteLine(" Disconnecting Test Module.");
            Debug.WriteLine(" --- End of ExeCute Module ---");

            try
            {

                // Terminate Execute Module in memory.
                KillOmicronProcesses(testModule.ProgID); // ProgId.Execute );

            }

            catch (AggregateException ae)
            {
                foreach (Exception ex in ae.InnerExceptions)
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log(ex, CurrentFileName);
                }

                // Terminate Execute Module in memory.
                KillOmicronProcesses(testModule.ProgID);  // ProgId.Execute );

                return string.Empty;
            }

            return testModuleParameters.ToString();

        }

        #endregion

    }
}
