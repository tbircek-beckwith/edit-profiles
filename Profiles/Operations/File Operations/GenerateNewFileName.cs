using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EditProfiles.Properties;

namespace EditProfiles.Operations
{
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Private Variables

        #endregion

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

        /// <summary>
        /// Generates a new file name from original file name.
        /// </summary>
        /// <returns>Returns new file name with a different order of words per team meeting requests.</returns>
        private string ProcessFileName()
        {

            // Remove every matching pattern to exposed root test name like "Bandwidth"
            string testName = new AnalyzeValues().Extract(input: Path.GetFileNameWithoutExtension(FileNameWithPath), pattern: new AnalyzeValues().TestFileNamePatterns, keywords: new AnalyzeValues().FileNameKeywords);

            string fileNameWithoutRevision = Path.GetFileNameWithoutExtension(FileNameWithPath);

            // if the user desires to keep "Rev#" otherwise just delete it.
            if (!Settings.Default.AppendFileName)
            {
                // Ritchie has version control file names do not require Rev# suffix. However,
                // since "Rev#" contains a number providing a dictionary solution would be difficult so this service
                // provided by the following code.
                Regex rx = new Regex(@"\b[Rr]ev\d\b\w*");
                // replace "Rev#" with empty string.
                fileNameWithoutRevision = rx.Replace(fileNameWithoutRevision, string.Empty);
            }

            // temp storage to keep replacement words.
            string keywords = new AnalyzeValues().Replace(input: fileNameWithoutRevision, pattern: new AnalyzeValues().TestFileNamePatterns, keywords: new AnalyzeValues().FileNameKeywords);
            // words[0]: ProductName, words[1]: P1, words[2]: PowerScheme, words[3]: Frequency
            List<string> words = new List<string>(keywords.Split('_').ToList());

            // temp storage to keep replacement words.
            string testSubFolderName = new AnalyzeValues().Replace(input: testName, pattern: new AnalyzeValues().TestFolderNamePatterns, keywords: new AnalyzeValues().FolderNameKeywords);

            // index of first '_' is right after product number
            int productNameLength = keywords.IndexOf('_') + 1;

            // split replacement words to insert test file name and new short name for "Regulator #" in the file name, also append file extension.
            // regulator is 1 based.
            string modifiedFolderName = Path.Combine(MyResources.Strings_ModifedFolderName, testSubFolderName);

            // generate new values 
            // TODO: Add a logic check to verify the source of the changes if .csv file generate new file names per profile/regulator
            string regulatorFolderName = $"regulator {regulator}";
            string profileFolderName = activeProfile > 0 ? $"profile {activeProfile}" : string.Empty;
            words[1] = $"P{activeProfile}";

            // string testFolderName = Path.Combine(modifiedFolderName, Path.Combine(regulatorFolderName, new Regex(@"(?<profile>\b[Pp](\d)\b\w*)", RegexOptions.None, TimeSpan.FromMilliseconds(100)).IsMatch(fileNameWithoutRevision) ? profileFolderName : string.Empty)).ToLower();
            string testFolderName = Path.Combine(path1: modifiedFolderName,
                                                 path2: regulatorFolderName,
                                                 path3: profileFolderName).ToLower();

            //string newFileName = Path.Combine(path1: Path.Combine(path1: Path.GetDirectoryName(FileNameWithPath), 
            //                                                      path2: testFolderName), 
            //                                  path2: $"{keywords.Substring(0, productNameLength)}{testName}_Reg {regulator}_{keywords.Substring(productNameLength, keywords.Length - productNameLength)}{Path.GetExtension(FileNameWithPath)}");

            string fileNameString = $"{words[0]}_{testName}_Reg {regulator}_{words[1]}_{words[2]}_{words[3]}{Path.GetExtension(FileNameWithPath)}";

            string newFileName = Path.Combine(path1: Path.GetDirectoryName(FileNameWithPath),
                                              path2: testFolderName,
                                              path3: fileNameString);



            // let's limit file name length > MaxFileNameLength omicron test universe fails to save modified files.
            // just add a sub folder named "modified files" with original file name
            // do not modify anything else
            if (newFileName.Length > Settings.Default.MaxFileNameLength)
            {
                // mode full filename
                newFileName = Path.Combine(Path.GetDirectoryName(FileNameWithPath), Path.Combine(MyResources.Strings_ModifedFolderName, Path.GetFileName(FileNameWithPath)));

                // if this new file name is still > MaxFileNameLength 
                // let put this file under user document
                if (newFileName.Length > Settings.Default.MaxFileNameLength)
                {
                    newFileName = Path.Combine(path1: Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                               path2: typeof(MainWindow).Assembly.GetName().Name,
                                               path3: $"{MyResources.Strings_ModifedFolderName}",
                                               path4: Path.GetFileName(FileNameWithPath));
                }
            }

            return newFileName;
        }

        /// <summary>
        /// Generates a new file name based on the original file name.
        /// </summary>
        /// <returns>Returns a new file name.</returns>
        private string GenerateTheFileName()
        {

            try
            {

                // Generate a new file name.
                NewFileName.Append(ProcessFileName());

                // If the file name does not contain "rev" followed by a number or a space add 
                // "Rev1" string to the file name.
                if (Settings.Default.AppendFileName)
                {
                    if (!NewFileName.ToString().ToUpperInvariant().Contains("REV"))
                    {
                        NewFileName.Append("Rev1");
                    }
                }

                // Create new folder to store modified files.
                if (!Directory.Exists(Path.GetDirectoryName(NewFileName.ToString())))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(NewFileName.ToString()));
                }

                Debug.WriteLine($"New file name is {NewFileName.ToString()}");

                // return new file name.
                return NewFileName.ToString();
            }
            catch (PathTooLongException ex)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ex, CurrentFileName);
                return Path.Combine(Path.GetDirectoryName(FileNameWithPath), Path.Combine(MyResources.Strings_ModifedFolderName, Path.GetFileName(FileNameWithPath)));
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
