using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using EditProfiles.Behaviors;
using EditProfiles.Data;
using EditProfiles.Properties;
using System.Diagnostics;

namespace EditProfiles.Operations
{
    /// <summary>
    /// File operations for the Omicron Control Center files.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private string FilePath { get; set; }

        private string FilePassword { get; set; }

        #endregion

        #region IStartProcessInterface Members


        /// <summary>
        /// Opens the Omicron Control Center file.
        /// </summary>
        /// <param name="path">Location of the OCC file.</param>
        /// <returns>Returns unlocked the Omicron Control Center file.</returns>
        public OMICRON.OCCenter.IAutoDoc OpenDocument(string path)
        {
            try
            {
                // Not all files have a password.
                // Do not check if the password is empty.
                if (string.IsNullOrWhiteSpace(path))
                {
                    throw new ArgumentNullException("path");
                }

                this.FilePath = path;

                return OpenDocuments();
            }
            catch (ArgumentNullException ae)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ae, this.CurrentFileName);

                return null;
            }
        }

        #endregion

        #region Private Methods

        private OMICRON.OCCenter.IAutoDoc OpenDocuments()
        {
            try
            {
                this.OmicronApplication = new OMICRON.OCCenter.Application();
                OMICRON.OCCenter.IAutoConst occConstants = this.OmicronApplication.Constants;  // occApp.Constants;

                try
                {
                    Debug.WriteLine(string.Format("{1}\tOpening file {0} ---", this.FilePath, DateTime.Now));
                    Debug.WriteLine(string.Format("OPENDOCUMENT thread: {0}", Thread.CurrentThread.GetHashCode()));

                    // Open the document.
                    this.OmicronDocument = this.OmicronApplication.Documents.Open(this.FilePath);

                    // Store Omicron.Application.Protection level to save each new file with same Protection Level 
                    // as the original file.
                    this.ProtectionLevel = this.OmicronDocument.Protection;

                    //// TODO: Add logic to perform if ClearAll needed.
                    //// Clear All Test Modules.

                    //Debug.WriteLine("--- ClearAll command is processing ---");
                    //this.OmicronDocument.ClearAll();

                    // Strip down any type of file protection if a file is protected with the user specified password.
                    // If the password does not unlock the file report it and move to the next file in the list.
                    if (!(this.OmicronDocument.Protection == occConstants.cProtectionNoProtection))
                    {

                        // Setting new password to nothing will unlock the file for editing.
                        // For more info: http://briannoyesblog.azurewebsites.net/2015/03/04/wpf-using-passwordbox-in-mvvm/
                        ISecurePasswordToString secureToString = new SecurePasswordToString();
                        string insecurePassword = secureToString.ConvertToInsecureString(this.ViewModel.Password);

                        bool protectionChanged = this.OmicronDocument.ChangeProtection(occConstants.cProtectionNoProtection, insecurePassword, string.Empty);

                        // Clear insecurePassword.
                        insecurePassword = string.Empty;

                        // Verify protection removed.
                        if (!protectionChanged)
                        {
                            MessageBox.Show(MyResources.Strings_ErrorProtectionMessage,
                                              MyResources.Strings_ErrorProtectionCaption,
                                              MessageBoxButton.OK,
                                              MessageBoxImage.Information);

                            // Update DetailsTextBoxText.
                            MyCommons.MyViewModel.DetailsTextBoxText =
                                MyCommons.LogProcess.Append(
                                string.Format(
                                CultureInfo.InvariantCulture,
                                MyResources.Strings_ErrorProtectionLevel,
                                Environment.NewLine,
                                this.CurrentFileName,
                                Repeat.StringDuplicate(Settings.Default.RepeatChar, Settings.Default.RepeatNumber)))
                                .ToString();

                            // Bad password or unknown problem the user interaction necessary.
                            // try to fail gracefully.
                            // TODO: Change default behavior to move to the next file in the line.
                            // Currently stops processing of the files.
                            throw new ArgumentException("password",
                                string.Format(CultureInfo.InvariantCulture,
                                                MyResources.Strings_ErrorProtectionLevel,
                                                Environment.NewLine,
                                                this.CurrentFileName,
                                                Repeat.StringDuplicate(Settings.Default.RepeatChar, Settings.Default.RepeatNumber)));
                        }
                    }
                }
                catch (ArgumentException ae)
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log(ae, this.CurrentFileName);

                    // Trying to shut down processing of the file.
                    // cannot change protection of a file.
                    MyCommons.TokenSource.Cancel();

                    return null;
                }

                // Returns the Omicron Control Center file.
                return this.OmicronDocument;

            }
            catch (ArgumentException ae)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ae, this.CurrentFileName);
                return null;
            }
            catch (NullReferenceException nre)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(nre, this.CurrentFileName);
                return null;
            }
            catch (COMException ce)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ce, this.CurrentFileName);
                return null;
            }
        }

        #endregion

    }
}
