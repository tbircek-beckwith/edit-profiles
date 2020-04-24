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

        /// <summary>
        /// This is maximum number of the regulators M-6200B supports.
        /// </summary>
        public int MaximumRegulatorNumber = 3;

        /// <summary>
        /// Holds the current regulator value.
        /// </summary>
        public int CurrentRegulatorValue = 0;

        /// <summary>
        /// Holds protection level of the omicron test file.
        /// </summary>
        private short ProtectionLevel { get; set; }

        /// <summary>
        /// Holds current file name without folder path.
        /// </summary>
        private string CurrentFileName { get; set; }

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
        /// <param name="viewModel">The copy of the current ViewModel for the business logic.</param>
        public void StartProcessing(IList<string> fileNames, ViewModel viewModel)
        {
            FileNames = fileNames;
            ViewModel = viewModel;

            // Following items are not allowed to be modified after "Find & Replace" button clicked.
            ItemsToFind = new List<string>(ViewModel.FindWhatTextBoxText.Split('|'));
            ItemsToReplace = new List<string>(ViewModel.ReplaceWithTextBoxText.Split('|'));

            ItemsToRemove = new Dictionary<string, string>(ListOfTestModulesToDelete(ViewModel.FindWhatTextBoxText.Split('|')));

            ItemsToRename = new Dictionary<string, string>(ListOfTestModulesToRename(ItemsToFind));

            StartProcessingFiles();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Start processing the user selected Omicron Test Files.
        /// </summary>
        private void StartProcessingFiles()
        {
            // Reset File counter.
            MyCommons.CurrentFileNumber = 0;

            // Update FileProcessBar;
            MyCommons.TotalFileNumber = FileNames.Count;
            MyCommons.MyViewModel.FileProgressBarMax = FileNames.Count;

            // Refresh Process bars.
            MyCommons.MyViewModel.UpdateCommand.Execute(null);

            try
            {

                for (CurrentRegulatorValue = 0; CurrentRegulatorValue < MaximumRegulatorNumber; CurrentRegulatorValue++)
                {
                    Parallel.ForEach(FileNames, MyCommons.ParallelingOptions, (currentFile) =>
                            {
                        // Increment current file number;
                        MyCommons.CurrentFileNumber++;
                                MyCommons.CurrentModuleNumber = 0;
                                MyCommons.MyViewModel.UpdateCommand.Execute(null);

                                CurrentFileName = currentFile;
                                MyCommons.FileName = Path.GetFileName(CurrentFileName);

                                try
                                {
                            // opens the document
                            Task.Factory.StartNew(() =>
                                        {
                                            try
                                            {
                                        // Polling CancellationToken's status.
                                        // If cancellation requested throw error and exit loop.
                                        if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                                {
                                                    MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                                }

                                        // Update DetailsTextBoxText.
                                        MyCommons.MyViewModel.DetailsTextBoxText =
                                                    MyCommons.LogProcess.Append(
                                                        string.Format(
                                                                CultureInfo.InvariantCulture,
                                                                MyResources.Strings_CurrentFileName,
                                                                Environment.NewLine,
                                                                Repeat.StringDuplicate(Settings.Default.RepeatChar, Settings.Default.RepeatNumber),
                                                                Path.GetFileName(CurrentFileName),
                                                                string.Format(CultureInfo.InvariantCulture,
                                                                        MyResources.Strings_TestStart,
                                                                        DateTime.Now)))
                                                    .ToString();

                                        // Open Omicron Document.
                                        // this.OmicronDocument = OpenDocument ( this.CurrentFileName, "" ); 
                                        OmicronDocument = OpenDocument(CurrentFileName);

                                            }
                                            catch (ArgumentException ae)
                                            {
                                        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
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
                                        , MyCommons.CancellationToken)
                                        .ContinueWith(scanThread =>
                                    {

                                // scans the document
                                try
                                        {
                                    // Polling CancellationToken's status.
                                    // If cancellation requested throw error and exit loop.
                                    if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                            {
                                                MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                            }

                                    // Scan the Test Document for the Test Modules.
                                    Scan();

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
                                    , MyCommons.CancellationToken)
                                    .Wait();

                            // Save the new file with a different name.
                            Task.Factory.StartNew(() =>
                                        {
                                            try
                                            {

                                        // Polling CancellationToken's status.
                                        // If cancellation requested throw error and exit loop.
                                        if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                                {
                                                    MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                                }

                                        // Save the new file.
                                        SaveOmicronFiles(OmicronDocument.FullName, true);

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
                                        , MyCommons.CancellationToken)
                                        .ContinueWith(closeThread =>
                                    {

                                // close the document
                                try
                                        {

                                    // Polling CancellationToken's status.
                                    // If cancellation requested throw error and exit loop.
                                    if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                            {
                                                MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                            }

                                    // Close Omicron Control Center without saving any changes to the original file.
                                    OmicronDocument.Close(false);

                                        }
                                        catch (COMException ae)
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
                                    , MyCommons.CancellationToken)
                                    .ContinueWith(threadCloseApp =>
                                    {

                                // quit the application
                                try
                                        {
                                    // Polling CancellationToken's status.
                                    // If cancellation requested throw error and exit loop.
                                    if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                            {
                                                MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                            }

                                    // Close Omicron Control Center Application
                                    OmicronApplication.Quit();

                                        }
                                        catch (COMException ae)
                                        {
                                            ErrorHandler.Log(ae, CurrentFileName);
                                        }
                                        catch (ObjectDisposedException ae)
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
                                    , MyCommons.CancellationToken)
                                    .Wait();

                            // terminate omicron processes.
                            Task.Factory.StartNew(() =>
                                        {
                                            try
                                            {
                                        // Garbage Collection.
                                        OmicronApplication = null;
                                                OmicronDocument = null;

                                        // Polling CancellationToken's status.
                                        // If cancellation requested throw error and exit loop.
                                        if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                                {
                                                    MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                                }

                                        // Terminate Omicron Processes.
                                        KillOmicronProcesses();
                                            }
                                            catch (COMException ae)
                                            {
                                                ErrorHandler.Log(ae, CurrentFileName);
                                            }
                                            catch (ObjectDisposedException ae)
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
                                        , MyCommons.CancellationToken)
                                        .Wait();

                                    MyCommons.MyViewModel.FileSideCoverText = MyResources.Strings_FormEndTest;

                            // Refresh Process bars.
                            MyCommons.MyViewModel.UpdateCommand.Execute(null);

                                }
                                catch (COMException ae)
                                {
                                    ErrorHandler.Log(ae, CurrentFileName);
                                }
                                catch (ObjectDisposedException ae)
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
                            }); 
                }

            }
            catch (OperationCanceledException oe)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(oe, CurrentFileName);
                return;
            }
            catch (COMException ce)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ce, CurrentFileName);
                return;
            }
        }

        #endregion

    }
}
