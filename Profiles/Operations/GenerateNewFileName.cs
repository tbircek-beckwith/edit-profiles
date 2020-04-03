using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using EditProfiles.Properties;
using System.Diagnostics;

namespace EditProfiles.Operations
{
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private StringBuilder NewFileName { get; set; }

        private string FileNameWithPath { get; set; }

        private IList<string> OriginalFileNameWithPath { get; set; }

        /// <summary>
        /// holds file name only without extension
        /// </summary>
        private IList<string> OriginalFileNameWithoutExtension { get; set; }

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

                FileNameWithPath = fileNameWithPath;

                NewFileName = new StringBuilder();

                return GenerateTheFileName();
            }
            catch (ArgumentNullException ae)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ae, CurrentFileName);

                return string.Empty;
            }
        }

        #endregion

        #region Private Methods

        private string GenerateTheFileName()
        {
            // Search pattern for "rev#" and "rev##" like file name entries.
            // used http://regexr.com to generate following expression.
            //
            Regex rx = new Regex(@"\brev(\d)*\b\w*",
                            RegexOptions.Compiled |
                            RegexOptions.IgnoreCase |
                            RegexOptions.CultureInvariant);

            // Path.GetDirectoryName(FileNameWithPath) ->            "C:\Users\TBircek\Desktop\Test Profile Test\actual test files\m6200a"
            // Path.GetFileNameWithoutExtension(FileNameWithPath) -> "M-6200A P2 Forward Power Definite Time Delay Test 60Hz Rev1"

            // OriginalFileNameWithPath = Path.GetFileNameWithoutExtension(FileNameWithPath).ToString().Split();

            // actual file name with extension
            // M-6200A P1 Forward Power Bandwidth Test 60Hz Rev1.occ  -> M-6200B_Bandwidth_Fwd Pwr_Reg 1_P1_60Hz .occ
            // M-6200A P1 Forward Power Bandcenter Test 50Hz Rev2.occ -> M-6200B_Bandcenter Test_Fwd Pwr_P1_Reg1_50Hz.occ

            // split up file name without extension
            OriginalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileNameWithPath).Split(' ');

            // process file name
            foreach (string item in OriginalFileNameWithoutExtension)
            {
                // remove "Rev xx" texts.
                if (!rx.IsMatch(item))
                {
                    // ignore white spaces or null values.
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        string value = string.Join("_", item);

                        // generate the file name without extension.
                        NewFileName.Append(value);
                    }
                }
            }

            return NewFileName.ToString();
        }

        private string GenerateTheFileName_Old()
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

                OriginalFileNameWithPath = Path.GetFullPath(FileNameWithPath)
                                                   .Replace(".occ", "")
                                                   .Split(' ');

                foreach (string word in OriginalFileNameWithPath)
                {
                    if (rx.IsMatch(word))
                    {
                        //// If the file name contains "rev" followed by a number or a space increment the rev number by one.
                        // NewFileName.Append(("Rev") +
                        //    (int.Parse(word.Trim().ToUpperInvariant().Replace("REV", ""),
                        //    CultureInfo.InvariantCulture) + 1).ToString(
                        //    CultureInfo.InvariantCulture));

                        // Since Ritchie has version control file names do not require Rev# suffix.
                        // the following line adds an extra space to the fileName.
                        //  NewFileName.Append(" ");
                    }
                    else if (!string.IsNullOrWhiteSpace(word))
                    {
                        NewFileName.Append(word.Trim() + " ");
                    }
                }

                // If the file name does not contain "rev" followed by a number or a space add 
                // "Rev1" string to the file name.
                if (Settings.Default.AppendFileName)
                {
                    if (!NewFileName.ToString().ToUpperInvariant().Contains("REV"))
                    {
                        NewFileName.Append("Rev1");
                    }
                }

                // Add Omicron Control Center extension to the file name.
                NewFileName.Append(".occ");

                // Create new folder to store modified files.
                if (!(Directory.Exists(Path.Combine(
                                            Path.GetDirectoryName(
                                             NewFileName.ToString()),
                                            MyResources.Strings_ModifedFolderName))))
                {
                    Directory.CreateDirectory(Path.Combine(
                                                Path.GetDirectoryName(
                                                 NewFileName.ToString()),
                                                MyResources.Strings_ModifedFolderName));
                }

                // Replace old folder path with the new location.
                NewFileName.Replace(Path.GetDirectoryName(
                                                   NewFileName.ToString()),
                                                   Path.Combine(Path.GetDirectoryName(
                                                    NewFileName.ToString()),
                                                   MyResources.Strings_ModifedFolderName));

                Debug.WriteLine("New file name is {0}", NewFileName.ToString());
                return NewFileName.ToString();
            }
            catch (FormatException fe)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(fe, CurrentFileName);
                return string.Empty;
            }
            catch (OverflowException oe)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(oe, CurrentFileName);
                return string.Empty;
            }
        }

        #endregion

    }
}
