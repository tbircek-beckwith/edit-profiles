using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using EditProfiles.Properties;

namespace EditProfiles.Operations
{
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private StringBuilder NewFileName { get; set; }

        private string FileNameWithPath { get; set; }

        private IList<string> OriginalFileNameWithPath { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Generates a new file name (including the path) based on the original
        /// file name and path.
        /// </summary>
        /// <param name="fileNameWithPath">Original file name and the path.</param>
        /// <returns>Returns a new file name and a path.</returns>
        public string GenerateNewFileName(string fileNameWithPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileNameWithPath))
                {
                    throw new ArgumentNullException("fileNameWithPath");
                }

                this.FileNameWithPath = fileNameWithPath;

                this.NewFileName = new StringBuilder();

                return this.GenerateTheFileName();
            }
            catch (ArgumentNullException ae)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ae, this.CurrentFileName);

                return string.Empty;
            }
        }

        #endregion

        #region Private Methods

        private string GenerateTheFileName()
        {
            try
            {
                //
                // Search pattern for "rev#" and "rev##" like file name entries.
                // used http://regexr.com to generate following expression.
                //
                Regex rx = new Regex(@"\brev(\d)*\b\w*",
                                RegexOptions.Compiled |
                                RegexOptions.IgnoreCase |
                                RegexOptions.CultureInvariant);

                this.OriginalFileNameWithPath = Path.GetFullPath(this.FileNameWithPath)
                                                    .Replace(".occ", "")
                                                    .Split(' ');

                foreach (string word in this.OriginalFileNameWithPath)
                {
                    if (rx.IsMatch(word))
                    {
                        //// If the file name contains "rev" followed by a number or a space increment the rev number by one.
                        //this.NewFileName.Append(("Rev") +
                        //    (int.Parse(word.Trim().ToUpperInvariant().Replace("REV", ""),
                        //    CultureInfo.InvariantCulture) + 1).ToString(
                        //    CultureInfo.InvariantCulture));

                        // Since Ritchie has version control file names do not require Rev# suffix.
                        this.NewFileName.Append(" ");
                    }
                    else if (!string.IsNullOrWhiteSpace(word))
                    {
                        this.NewFileName.Append(word.Trim() + " ");
                    }
                }

                // If the file name does not contain "rev" followed by a number or a space add 
                // "Rev1" string to the file name.
                if (Settings.Default.AppendFileName)
                {
                    if (!this.NewFileName.ToString().ToUpperInvariant().Contains("REV"))
                    {
                        this.NewFileName.Append("Rev1");
                    }
                }

                // Add Omicron Control Center extension to the file name.
                this.NewFileName.Append(".occ");

                // Create new folder to store modified files.
                if (!(Directory.Exists(Path.Combine(
                                            Path.GetDirectoryName(
                                            this.NewFileName.ToString()),
                                            MyResources.Strings_ModifedFolderName))))
                {
                    Directory.CreateDirectory(Path.Combine(
                                                Path.GetDirectoryName(
                                                this.NewFileName.ToString()),
                                                MyResources.Strings_ModifedFolderName));
                }

                // Replace old folder path with the new location.
                this.NewFileName.Replace(Path.GetDirectoryName(
                                                   this.NewFileName.ToString()),
                                                    Path.Combine(Path.GetDirectoryName(
                                                    this.NewFileName.ToString()),
                                                    MyResources.Strings_ModifedFolderName));

#if DEBUG
                Console.WriteLine("New file name is {0}", this.NewFileName.ToString());
#endif
                return this.NewFileName.ToString();
            }
            catch (FormatException fe)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(fe, this.CurrentFileName);
                return string.Empty;
            }
            catch (OverflowException oe)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(oe, this.CurrentFileName);
                return string.Empty;
            }
        }

        #endregion

    }
}
