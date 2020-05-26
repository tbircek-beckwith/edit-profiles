using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EditProfiles.Data;
using EditProfiles.Properties;
using OMICRON.OCCenter;

namespace EditProfiles.Operations
{
    ///// <summary>
    ///// Scans Omicron test documents to locate the user specified keywords.
    ///// </summary>
    /// <inheritdoc/>
    partial class ProcessFiles : IStartProcessInterface
    {
        #region Public Properties

        /// <summary>
        /// sets default behavior for processing of execute parameter replacements.
        /// <para></para>
        /// <list type="bullet">
        /// <item>
        /// <term><see langword="true"/>: </term>
        /// <description>Old parameters will replace with new parameters.</description>
        /// </item>
        /// <item>
        /// <term><see langword="false"/>: </term>
        /// <description>Old parameters (which are expected to be Profile 1) replace with new Profile values.</description>
        /// </item>
        /// </list>
        /// </summary>
        private bool? UseDefaultSettingBehavior { get; set; } = null;

        #endregion

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

            ScanDocument();
        }

        #endregion

        #region Private Methods

        private void ScanDocument()
        {
            try
            {
                // Omicron Test Module index is between 1 and TestModules.Count
                int startPosition = 1;

                // Local thread variable to hold total module number after deletion.
                int moduleCounter = 0;

                // Update total module number.
                // MyCommons.TotalModuleNumber = this.OmicronDocument.TestModules.Count;
                MyCommons.TotalModuleNumber = OmicronDocument.OLEObjects.Count;
                MyCommons.MyViewModel.ModuleProgressBarMax = MyCommons.TotalModuleNumber;
                MyCommons.MyViewModel.UpdateCommand.Execute(null);

                Debug.WriteLine("SCANNING thread: {0}", Thread.CurrentThread.GetHashCode());
                Debug.WriteLine("Total TestModule: {0} ", MyCommons.TotalModuleNumber);

                // All Test Modules in the Omicron test file.
                // TestModules testModules = this.OmicronDocument.TestModules;

                // Parallel.For (fromInclusive Int32, toExclusive Int32, parallelOptions, body)
                // TotalModelNumber IS EXCLUSIVE SO MUST ADD 1 TO IT.
                // Otherwise the last Test Module will never be processed.
                Parallel.For(startPosition, MyCommons.TotalModuleNumber + 1, MyCommons.ParallelingOptions, testModule =>
                {
                    // increment counter to open next test module.
                    Interlocked.Add(ref moduleCounter, 1);

                    OmicronProgramId = OmicronDocument.OLEObjects.get_Item(moduleCounter).ProgID;
                    OmicronProgramName = OmicronDocument.OLEObjects.get_Item(moduleCounter).Name;
                    // update current module number.
                    // MyCommons.CurrentModuleNumber = testModule;
                    MyCommons.CurrentModuleNumber = moduleCounter;
                    MyCommons.MyViewModel.UpdateCommand.Execute(null);


                    Debug.WriteLine(string.Format("{0}", new String(Settings.Default.RepeatChar, Settings.Default.RepeatNumber)));
                    Debug.WriteLine(string.Format("SCAN PARALLEL thread: {0}", Thread.CurrentThread.GetHashCode()));
                    Debug.WriteLine(string.Format("Test module name...: {0}", OmicronProgramName));
                    Debug.WriteLine(string.Format("Test module type...: {0}", OmicronProgramId));
                    Debug.WriteLine(string.Format("{0}", new String(Settings.Default.RepeatChar, Settings.Default.RepeatNumber)));
                    Debug.WriteLine("Making a decision if the user wants to delete this test module.....");

                    // REMOVE named Module(s)
                    if (ItemsToRemove.TryGetValue(OmicronProgramName, out string tempValue))
                    {
                        if (tempValue == OmicronProgramId)
                        {
                            Debug.WriteLine(string.Format("Deleting ProgID {0}\tand Name: {1}", OmicronProgramId, OmicronProgramName));
                            Debug.WriteLine(" ... TEST MODULE MARK FOR DELETION ....");
                            OmicronDocument.OLEObjects.get_Item(moduleCounter).Delete();
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
                    }

                    // store old title
                    string title = OmicronDocument.OLEObjects.get_Item(moduleCounter).Name;
                    Debug.WriteLine($"--------------------------> old title: {title}");
                    // update title.
                    OmicronDocument.OLEObjects.get_Item(moduleCounter).Name = new AnalyzeValues().Change(input: title, pattern: new AnalyzeValues().TitlePatterns, keywords: new AnalyzeValues().TitleKeywords);


                    // temp storage to keep replacement words.
                    if (new AnalyzeValues().IsMatch(title, new AnalyzeValues().ProfilePatterns))
                    {
                        string keywords = new AnalyzeValues().Match(input: title, pattern: new AnalyzeValues().ProfilePatterns);
                        string replacementTitle = System.Text.RegularExpressions.Regex.Split(input: keywords,
                                                                                      pattern: "(?<=\\D)(?=\\d)|(?<=\\d)(?=\\D)",
                                                                                      options: System.Text.RegularExpressions.RegexOptions.None,
                                                                                      matchTimeout: TimeSpan.FromMilliseconds(100))[0];

                        Dictionary<string,string> what = new Dictionary<string, string>() { { keywords, $"{replacementTitle}{activeProfile}"} };

                        string newTitle = new AnalyzeValues().Change(input: title, pattern: new AnalyzeValues().ProfilePatterns, keywords: what); // System.Text.RegularExpressions.Regex.Replace(input: title, pattern: "(?<=\\D)(?=\\d)|(?<=\\d)(?=\\D)", replacement: replacementTitle); 
                        Debug.WriteLine($"--------------------------> keywords: {keywords}, replacementTitle: {replacementTitle}, newTitle: {newTitle}");

                        OmicronDocument.OLEObjects.get_Item(moduleCounter).Name = newTitle;
                    }

                    Debug.WriteLine($"--------------------------> new title: {OmicronDocument.OLEObjects.get_Item(moduleCounter).Name} ");
                    Debug.WriteLine($"--------------------------> group mod: {(Convert.ToBoolean(UseDefaultSettingBehavior) ? "Using Settings" : "Using Tests")}");

                    // Categorize Omicron Modules
                    if (OmicronDocument.OLEObjects.get_Item(moduleCounter).IsTestModule)
                    {
                        TestModule currentModule = OmicronDocument.OLEObjects.get_Item(moduleCounter).TestModule; //testModules.Item[moduleCounter];

                        //// update current test module title if needed.
                        // currentModule.Name = new AnalyzeValues().Change(input: currentModule.Name, pattern: new AnalyzeValues().TitlePatterns, keywords: new AnalyzeValues().TitleKeywords);

                        Debug.WriteLine(" .... NO DELETION REQUIRED ....");

                        switch (OmicronProgramId)
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
                                    Debug.WriteLine("--- Add option to modify 'Title' --- DONE");
                                    Debug.WriteLine("--- Add option to modify 'Path' --- DONE");
                                    Debug.WriteLine("--- Add option to modify 'Execution Options' ---");
                                    Debug.WriteLine("--- as of 10/30/2018 ---");
                                    Debug.WriteLine($"--------------------------> group mod: {(Convert.ToBoolean(UseDefaultSettingBehavior) ? "Using Settings" : "Using Tests")}");

                                    // Retrieve parameters and save.
                                    Retrieve(currentModule);
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

                                break;
                            case ProgId.OMRamp:
                                try
                                {
                                    // Polling CancellationToken's status.
                                    // If cancellation requested throw error and exit loop.
                                    if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                    {
                                        MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                    }

                                    Debug.WriteLine("--- Future use object 'Ramping Test Module' ---");
                                    Debug.WriteLine("--- Add options to link each item to 'XRio Block' ---");
                                    Debug.WriteLine("--- Add option to modify 'Title' ---");
                                    Debug.WriteLine("--- as of 10/30/2018 ---");

                                    //// update current test module title if needed.
                                    //currentModule.Name = new AnalyzeValues().Change(input: currentModule.Name, pattern: new AnalyzeValues().TitlePatterns, keywords: new AnalyzeValues().TitleKeywords);

                                    // Retrieve parameters and save.
                                    // Retrieve(currentTestModule);
                                    currentModule.Clear();
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
                                break;
                            case ProgId.OMSeq:
                                try
                                {
                                    // Polling CancellationToken's status.
                                    // If cancellation requested throw error and exit loop.
                                    if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                    {
                                        MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                    }

                                    Debug.WriteLine("--- Future use object 'State Sequencer Test Module' ---");
                                    Debug.WriteLine("--- Add options to link each item to 'XRio Block' ---");
                                    Debug.WriteLine("--- Add option to modify 'Title' ---");
                                    Debug.WriteLine("--- as of 10/30/2018 ---");

                                    // Retrieve parameters and save.
                                    // Retrieve(currentTestModule);
                                    currentModule.Clear();
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
                                break;
                            default:
                                // Not supported test module. move on to the next module.
                                break;
                        }
                    }
                    else
                    {

                        Debug.WriteLine("--- NonTest object ---");
                        Debug.WriteLine(string.Format("Test module name...: {0}", OmicronProgramName));
                        Debug.WriteLine(string.Format("Test module type...: {0}", OmicronProgramId));

                        // it is not a Test Module.
                        switch (OmicronProgramId)
                        {
                            case ProgId.XRio:

                                // Polling CancellationToken's status.
                                // If cancellation requested throw error and exit loop.
                                if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                {
                                    MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                }
                                Debug.WriteLine("--- Future use object 'XRio Block' ---");
                                Debug.WriteLine("--- Add option to modify 'Title' ---");
                                Debug.WriteLine("--- as of 10/30/2018 ---");

                                // future use.
                                var xrio = OmicronDocument.OLEObjects.get_Item(moduleCounter);

                                break;
                            case ProgId.Hardware:
                                // Polling CancellationToken's status.
                                // If cancellation requested throw error and exit loop.
                                if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                {
                                    MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                }
                                Debug.WriteLine("--- Future use object 'Hardware Configuration' ---");
                                Debug.WriteLine("--- Add option to modify 'Title' ---");
                                Debug.WriteLine("--- as of 10/30/2018 ---");
                                break;
                            case ProgId.Group:
                                // Polling CancellationToken's status.
                                // If cancellation requested throw error and exit loop.
                                if (MyCommons.CancellationToken.IsCancellationRequested == true)
                                {
                                    MyCommons.CancellationToken.ThrowIfCancellationRequested();
                                }

                                Debug.WriteLine("--- Future use object 'Groups' ---");
                                Debug.WriteLine("--- Add option to modify 'Title' ---");
                                Debug.WriteLine("--- as of 10/30/2018 ---");

                                Group group = OmicronDocument.OLEObjects.get_Item(moduleCounter).Group;

                                UseDefaultSettingBehavior = new AnalyzeValues().IsMatch(group.Name, new AnalyzeValues().GroupFolderPatterns);

                                // decide which behavior to use based on the folder name.
                                if (Convert.ToBoolean(UseDefaultSettingBehavior))
                                {
                                    // true: Old method where point read and replaced per Execute parameters.
                                    ItemsToFind = new List<string>(ViewModel.FindWhatTextBoxText.Split('|'));
                                    ItemsToReplace = new List<string>(ViewModel.ReplaceWithTextBoxText.Split('|'));
                                }
                                else
                                {
                                    // false: New method where point read is always Profile 1 and replaced per active Profile generating process
                                    ItemsToFind = new List<string>(MyCommons.FindProfile.Split('|'));
                                    ItemsToReplace = new List<string>(MyCommons.ReplaceProfile.Split('|'));
                                }

                                Debug.WriteLine($"ItemsToFind: {ItemsToFind[5]}-{ItemsToFind[10]}-{ItemsToFind[15]}, ItemsToReplace: {ItemsToReplace[5]}-{ItemsToReplace[10]}-{ItemsToReplace[15]}");

                                break;
                            default:
                                break;
                        }

                    }
                });
            }
            catch (NullReferenceException ae)
            {
                ErrorHandler.Log(ae, CurrentFileName);
                return;
            }
            catch (OperationCanceledException ae)
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

        #endregion

    }
}
