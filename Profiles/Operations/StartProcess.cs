using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int regulator = 0;

        /// <summary>
        /// Holds the active <see cref="Profile.Id"/> value.
        /// </summary>
        public int activeProfile = 1;

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

            // TODO: this codes needs to handle manually entered user inputs
            SetSearchItems();

            StartProcessingFiles();
        }

        #endregion

        #region Private Methods

        private void SetSearchItems()
        {

            // Following items are not allowed to be modified after "Find & Replace" button clicked.
            ItemsToFind = new List<string>(ViewModel.FindWhatTextBoxText.Split('|'));
            ItemsToReplace = new List<string>(ViewModel.ReplaceWithTextBoxText.Split('|'));            
            ItemsToRemove = new Dictionary<string, string>(ListOfTestModulesToDelete(ViewModel.FindWhatTextBoxText.Split('|')));
            ItemsToRename = new Dictionary<string, string>(ListOfTestModulesToRename(ItemsToFind));

        }

        /// <summary>
        /// Start processing the user selected Omicron Test Files.
        /// </summary>
        private void StartProcessingFiles()
        {
            // Reset File counter.
            MyCommons.CurrentFileNumber = 0;

            // Update FileProcessBar;
            ViewModel.FileProgressBarMax = MyCommons.TotalFileNumber = FileNames.Count * MaximumRegulatorNumber * 4;
            
            // Refresh Process bars.
            ViewModel.UpdateCommand.Execute(null);

            // get Change Active Profile value.
            ViewModel.ChangeActiveProfile = MyCommons.ChangeActiveProfileValue;

            try
            {
                // make regulator files.
                for (regulator = 1; regulator <= MaximumRegulatorNumber; regulator++)
                {
                    // since original files are always profile 1 need to update this only when regulator changes.
                    ViewModel.FindWhatTextBoxText = new Regulator().GetValues(regulator, 0, Column.OriginalSettingValue) ?? string.Empty; // would have every profiles
                    ViewModel.ReplaceWithTextBoxText = new Regulator().GetValues(regulator, 0, Column.ReplacementValue) ?? string.Empty;  // would have every profiles

                    // since original files are always profile 1. Change 1 -> whatever is the file profile is.
                    MyCommons.FindProfile = new Regulator().GetValues(regulator, 1, Column.OriginalTestValue) ?? string.Empty;
                    // MyCommons.FindProfile = new Regulator().GetValues(MyCommons.Regulators, regulator, 1, Column.OriginalTestValue);

                    Parallel.ForEach(FileNames, MyCommons.ParallelingOptions, (currentFile) =>
                    {
                        // for (int profile = 1; profile <= 4; profile++)
                        Parallel.For(1, 5, MyCommons.ParallelingOptions, (profile) =>
                         {
                             // would have Profile x only
                             MyCommons.ReplaceProfile = new Regulator().GetValues(regulator, profile, Column.ReplacementValue) ?? string.Empty;
                             //MyCommons.ReplaceProfile = new Regulator().GetValues(MyCommons.Regulators, regulator, profile, Column.ReplacementValue);

                             activeProfile = profile;

                             SetSearchItems();

                             // Increment current file number;
                             MyCommons.CurrentFileNumber++;
                             MyCommons.CurrentModuleNumber = 0;
                             ViewModel.UpdateCommand.Execute(null);

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
                                         ViewModel.DetailsTextBoxText =
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

                                 ViewModel.FileSideCoverText = MyResources.Strings_FormEndTest;

                                 // Refresh Process bars.
                                 ViewModel.UpdateCommand.Execute(null);

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
