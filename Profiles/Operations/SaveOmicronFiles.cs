using System;
using System.Globalization;
using System.IO;
using EditProfiles.Behaviors;
using EditProfiles.Data;
using EditProfiles.Properties;

namespace EditProfiles.Operations
{
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private bool SaveAs { get; set; }

        private string OldFileName { get; set; }

        // private short ProtectionLevel { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Sets specified protection level and saves modified Omicron Test File.
        /// </summary>
        /// <param name="oldFileName">Original file name.</param>
        /// <param name="saveAs">If it is true the file "SaveAs", false the file "Save".</param>
        /// <param name="protectionLevel">provide a protection level to protect the file.</param>
        [Obsolete(" Protection Level decided by the original file.", true)]
        public void SaveOmicronFiles(string oldFileName, bool saveAs, short protectionLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves modified Omicron Test File.
        /// </summary>
        /// <param name="oldFileName">Original file name.</param>
        /// <param name="saveAs">If it is true the file "saveAs", false the file "save".</param>
        public void SaveOmicronFiles(string oldFileName, bool saveAs)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(oldFileName))
                {
                    throw new ArgumentNullException("oldFileName");
                }

                this.OldFileName = GenerateNewFileName(oldFileName);
                this.SaveAs = saveAs;

                this.SaveOmicronFile();

            }
            catch (ArgumentNullException ae)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ae, this.OldFileName);
                return;
            }
        }

        #endregion

        #region Private Methods

        private void SaveOmicronFile()
        {
            try
            {

                // Polling CancellationToken's status.
                // If cancellation requested throw error and exit loop.
                if (MyCommons.CancellationToken.IsCancellationRequested == true)
                {
                    MyCommons.CancellationToken.ThrowIfCancellationRequested();
                }

                // Setting new password to original password so the user can still have same level protection in the modified files.
                // For more info: http://briannoyesblog.azurewebsites.net/2015/03/04/wpf-using-passwordbox-in-mvvm/
                ISecurePasswordToString secureToString = new SecurePasswordToString();
                string insecurePassword = secureToString.ConvertToInsecureString(this.ViewModel.Password);

                bool protectionChanged = this.OmicronDocument.ChangeProtection(this.ProtectionLevel, string.Empty, insecurePassword);

                // Clear insecurePassword.
                insecurePassword = string.Empty;

                if (!protectionChanged)
                {
                    // Bad password or unknown problem the user interaction necessary.
                    // try to fail gracefully.
                    throw new ArgumentException("protectionChanged");
                }
                else
                {
                    // Depending file exists and saveAs value,
                    // Save the files.
                    if (File.Exists(this.OldFileName))
                    {
                        if (this.SaveAs)
                        {
                            this.OmicronDocument.SaveAs(this.OldFileName);
                        }
                        else
                        {
                            this.OmicronDocument.Save();
                        }
                    }
                    else
                    {
                        this.OmicronDocument.SaveAs(this.OldFileName);
                    }

                    // Update DetailsTextBoxText.
                    MyCommons.MyViewModel.DetailsTextBoxText =
                        MyCommons.LogProcess.Append(
                            (string.Format(
                                CultureInfo.InvariantCulture,
                                MyResources.Strings_TestEnd,
                                DateTime.Now,
                                Environment.NewLine,
                                Repeat.StringDuplicate(Settings.Default.RepeatChar, Settings.Default.RepeatNumber),
                                MyCommons.FileName)
                             ))
                        .ToString();
                }

            }
            catch (System.Runtime.InteropServices.COMException ae)
            {
                ErrorHandler.Log(ae, this.OldFileName);
                return;
            }
            catch (ArgumentException ae)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ae, this.OldFileName);
                return;
            }
            catch (NullReferenceException ne)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ne, this.OldFileName);
                return;
            }
            catch (AggregateException ae)
            {
                foreach (Exception ex in ae.InnerExceptions)
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log(ex, this.OldFileName);
                }
                return;
            }
        }

        #endregion

    }
}
