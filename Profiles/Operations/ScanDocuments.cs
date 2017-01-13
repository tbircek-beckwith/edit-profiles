using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EditProfiles.Data;
using EditProfiles.Properties;
using OMICRON.OCCenter;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Scans Omicron test documents to locate the user specified keywords.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private string OmicronProgramName { get; set; }

        private string OmicronProgramId { get; set; }

        private IAutoDoc OmicronDocument { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Scans specified Omicron Test Document.
        /// </summary>
        public void Scan()
        {

            this.ScanDocument();
        }

        #endregion

        #region Private Methods

        private void ScanDocument()
        {
            try
            {
                // Omicron Test Module index is between 1 and TestModules.Count
                int currentPosition = 1;

                // Local thread variable to hold total module number after deletion.
                int moduleCounter = 0;

                // Update total module number.
                MyCommons.TotalModuleNumber = this.OmicronDocument.TestModules.Count;
                MyCommons.MyViewModel.ModuleProgressBarMax = MyCommons.TotalModuleNumber;
                MyCommons.MyViewModel.UpdateCommand.Execute(null);

#if DEBUG
                Console.WriteLine("SCANNING thread: {0}", Thread.CurrentThread.GetHashCode());
                Console.WriteLine("Total TestModule: {0} ", MyCommons.TotalModuleNumber);
#endif

                // All Test Modules in the Omicron test file.
                TestModules testModules = this.OmicronDocument.TestModules;

                // Parallel.For (fromInclusive Int32, toExclusive Int32, parallelOptions, body)
                // totalModelNumber IS EXCLUSIVE SO MUST ADD 1 TO IT.
                // Otherwise the last Test Module will never be processed.
                Parallel.For(currentPosition, MyCommons.TotalModuleNumber + 1, MyCommons.ParallelingOptions, testModule =>
                {
                    // increment counter to open next test module.
                    Interlocked.Add(ref moduleCounter, 1);

                    // update current module number.
                    // MyCommons.CurrentModuleNumber = testModule;
                    MyCommons.CurrentModuleNumber = moduleCounter;
                    MyCommons.MyViewModel.UpdateCommand.Execute(null);

                    TestModule currentTestModule = testModules.Item[moduleCounter];

                    this.OmicronProgramName = currentTestModule.Name;

                    this.OmicronProgramId = currentTestModule.ProgID;

#if DEBUG
                    Console.WriteLine("{0}", new String(Settings.Default.RepeatChar, Settings.Default.RepeatNumber));
                    Console.WriteLine("SCAN PARALLEL thread: {0}", Thread.CurrentThread.GetHashCode());
                    Console.WriteLine("Test module text...: {0}", OmicronProgramName);
                    Console.WriteLine("Test module type...: {0}", OmicronProgramId);
                    Console.WriteLine("{0}", new String(Settings.Default.RepeatChar, Settings.Default.RepeatNumber));
                    Console.WriteLine("Making a decision if the user wants to delete this test module.....");
                    Console.WriteLine("Test module name...: {0}", OmicronProgramName);
#endif
                    if (TestModuleNamesToRemove.Contains(OmicronProgramName))
                    {
#if DEBUG
                        Console.WriteLine(" ... TEST MODULE MARK FOR DELETION ....");
#endif
                        currentTestModule.Delete();
                        // just deleted a test module. Omicron updates total test module counter.
                        // this is the reason for the decrement.
                        Interlocked.Decrement(ref moduleCounter);

                        // Show detailed output if the user wants it.
                        if (Settings.Default.ShowDetailedOutput)
                        {
                            // Update DetailsTextBoxText.
                            MyCommons.MyViewModel.DetailsTextBoxText =
                                MyCommons.LogProcess.Append(
                                        string.Format(
                                                CultureInfo.InvariantCulture,
                                                MyResources.Strings_RemoveTM,
                                                OmicronProgramName,
                                                OmicronProgramId,
                                                Environment.NewLine))
                                        .ToString();
                        }

                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine(" .... NO DELETION REQUIRED ....");
#endif
                        switch (this.OmicronProgramId)
                        {
                            case ProgId.Execute:

                                try
                                {

                                    // Polling CancellationToken's status.
                                    // If cancellation requested throw error and exit loop.
                                    if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                    {
                                        MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                    }

                                    // Retrieve parameters and save.
                                    Retrieve(currentTestModule);
                                }
                                catch (AggregateException ae)
                                {
                                    foreach (Exception ex in ae.InnerExceptions)
                                    {
                                        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                        ErrorHandler.Log(ex, this.CurrentFileName);
                                    }
                                    return;
                                }

                                break;

                            default:
                                // Not supported test module. move on to the next module.
                                break;
                        }
                    }
                });
            }
            catch (System.NullReferenceException ae)
            {
                ErrorHandler.Log(ae, this.CurrentFileName);
                return;
            }
            catch (System.OperationCanceledException ae)
            {
                ErrorHandler.Log(ae, this.CurrentFileName);
                return;
            }
            catch (AggregateException ae)
            {
                foreach (Exception ex in ae.InnerExceptions)
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log(ex, this.CurrentFileName);
                }
                return;
            }
        }

        #endregion

    }
}
