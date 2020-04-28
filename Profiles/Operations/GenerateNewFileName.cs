using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using EditProfiles.Properties;

namespace EditProfiles.Operations
{
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Private Variables

        // holds replacement words with ignoring casing.
        // must add or remove words here to provide replacements.
        Dictionary<string, string> FileNameTerms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // {"original", "replacement"},
                {"m-6200a", "M-6200B_"},
                {"forward power", "Fwd Pwr_"},
                {"fp", "Fwd Pwr_"},
                {"reverse power", "Rev Pwr_"},
                {"rp", "Rev Pwr_"},
                {"smart reverse power", "AD_" },
                {"src reverse power", "Src Rev Pwr_" },
                {"src reverse", "Src Rev Pwr_" },
                {"src", "Src Rev Pwr_" },
                {"smart src reverse power", "ADM_" },
                {"adm", "ADM_" },
                {"ad", "AD_" },
                {"dg", "DG_" },
                {"test", string.Empty},
                {"short version", string.Empty},
            };

        // holds replacement words with ignoring casing.
        // must add or remove words here to provide replacements.
        Dictionary<string, string> FolderNameTerms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // {"original", "replacement"},
                {"alarm", "alarms"},
                {"bandcenter", "bandcenters"},
                {"bandwidth", "bandwidths"},
                {"definite", "definite-time-delays"},
                {"distributed", "distributed-generations"},
                {"georgia", "georgia-power"},
                {"intertap", "intertap-time-delays"},
                {"interse", "inverse-time-delays"},
                {"ldc", "ldc-settings"},
                {"line limit", "line-limits"},
                {"pulse width", "output-pulse-widths"},
                {"varbias", "varbias"},
                {"smart vr", "voltage-reductions"},
                {"voltage reduction", "voltage-reductions"},
                {"correction", "vt-corrections"},
            };

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

            // used http://regexr.com to generate following expressions.
            //
            // combination "Regular Expressions" pattern with group names.
            // (?<powerKeyword>\b(([Ff]orward\s)|(([Ss]mart\s)?([Ss]rc\s)?([Rr]everse\s)))([Pp]ower)\b\w*)|(?<product>\b([Mm]-6200[Aa])\b\w*)|(?<revision>\b[Rr]ev\d\b\w*)|(?<notUsed>\b([Tt]est*)|([Ss]hort\s[Vv]ersion)\b\w*)|(?<frequency>\b\d{2}[Hh][Zz]\b\w*)|(?<profile>\b[Pp](\d)\b\w*)|(?<determination>\b[Aa][Dd]([Mm])?\b\w*)|(?<distrubuted>\b[Dd][Gg]\b\w*)|(?<powerInitials>\b([Ff][Pp]|([Rr][Pp]))\b\w*)|(?<srcReverse>\b([Ss]rc\s)([Rr]everse)\b\w*)|(?<src>\b([Ss]rc)\b\w*)
            //
            // power scheme "Regular Expressions" pattern group.
            // captures (Forward || (Smart || Src || Reverse)) Power like string in the file name.
            // (?< powerKeyword >\b(([Ff]orward\s) | (([Ss]mart\s)?([Ss]rc\s)?([Rr]everse\s)))([Pp]ower)\b\w*)
            //
            // determination "Regular Expressions" pattern group.
            // captures (AD || ADM) like string in the file name.
            // (?<determination>\b[Aa][Dd]([Mm])?\b\w*)
            //
            // distributed "Regular Expressions" pattern group.
            // captures DG like string in the file name.
            // (?<distrubuted>\b[Dd][Gg]\b\w*)
            //
            // product "Regular Expressions" pattern group.
            // captures M-6200A like string in the file name.
            // (?<product>\b([Mm]-6200[Aa])\b\w*)
            //
            // revision "Regular Expressions" pattern group.
            // captures Rev# like string in the file name.
            // (?<revision>\b[Rr]ev(\d)\b\w*)
            //
            // frequency "Regular Expressions" pattern group.
            // captures 60Hz like string in the file name.
            // (?<frequency>\b\d{2}[Hh][Zz]\b\w*)
            //
            // profile "Regular Expressions" pattern group.
            // captures P1 like string in the file name.
            // (?<profile>\b[Pp](\d)\b\w*)
            //
            // powerInitials "Regular Expressions" pattern group.
            // captures FP or RP like strings in the file name.
            // (?<powerInitials>\b([Ff][Pp]|([Rr][Pp]))\b\w*)
            //
            // srcReverse "Regular Expressions" pattern group.
            // captures Src Reverse like strings in the file name.
            // (?<srcReverse>\b([Ss]rc\s)([Rr]everse)\b\w*)
            //
            // src "Regular Expressions" pattern group.
            // captures Src like strings in the file name.
            // (?<src>\b([Ss]rc)\b\w*)
            //
            // not used "Regular Expressions" pattern group.
            // captures (Test || (Short Version)) like string in the file name.
            // (?<notUsed>\b([Tt]est*)|([Ss]hort\s[Vv]ersion)\b\w*)

            // combination Regex pattern with group names
            string testFileNamePatterns = @"(?<powerKeyword>\b(([Ff]orward\s)|(([Ss]mart\s)?([Ss]rc\s)?([Rr]everse\s)))([Pp]ower)\b\w*)|(?<product>\b([Mm]-6200[Aa])\b\w*)|(?<revision>\b[Rr]ev\d\b\w*)|(?<notUsed>\b([Tt]est*)|([Ss]hort\s[Vv]ersion)\b\w*)|(?<frequency>\b\d{2}[Hh][Zz]\b\w*)|(?<profile>\b[Pp](\d)\b\w*)|(?<determination>\b[Aa][Dd]([Mm])?\b\w*)|(?<distrubuted>\b[Dd][Gg]\b\w*)|(?<powerInitials>\b([Ff][Pp]|([Rr][Pp]))\b\w*)|(?<srcReverse>\b([Ss]rc\s)([Rr]everse)\b\w*)|(?<src>\b([Ss]rc)\b\w*)";

            // Regex to capture combination pattern with 100 milliseconds timeout.
            Regex fileName = new Regex(testFileNamePatterns, RegexOptions.None, TimeSpan.FromMilliseconds(100));

            // Remove every matching pattern to exposed root test name like "Bandwidth"
            string testName = fileName.Replace(Path.GetFileNameWithoutExtension(FileNameWithPath), string.Empty).Trim();

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
            string output = string.Empty;

            // stroll through matching patterns to get replacements.
            foreach (Match match in Regex.Matches(fileNameWithoutRevision, testFileNamePatterns))
            {
                Debug.WriteLine($"Match: {match.Value}");
                // stitch replacement words together.
                output += ReplaceFileNameWords(match.Value, FileNameTerms); // DictionaryOptions.fileNameTerms); // ReplaceFileNameWords_Old(match.Value); // 
            }

            // temp storage to keep replacement words.
            string testSubFolderName = string.Empty;

            // combination Regex pattern with group names
            string testFolderNamePatterns = @"(?<alarms>\b((?=Alarm)\b\w*))|(?<bandcenters>\b((?=Bandcenter)\b\w*))|(?<bandwidth>\b((?=Bandwidth)\b\w*))|(?<dtDelay>\b((?=Definite)\b\w*))|(?<dg>\b((?=Distributed)\b\w*))|(?<georgia>\b((?=Georgia)\b\w*))|(?<intertap>\b((?=Inter[Tt]ap)\b\w*))|(?<interse>\b((?=Inverse)\b\w*))|(?<ldc>\b((?=[Ll][Dd][Cc][\s])\b\w*))|(?<limit>\b((?=Line Limit)\b\w*))|(?<pulse>\b((?=Pulse Width)\b\w*))|(?<varbias>\b((?=VAr[Bb]ias)\b\w*))|(?<smartVR>\b(Smart VR)\b\w*)|(?<vr>\b(Voltage Reduction (?!Alarm))\b\w*)|(?<vt>\b(Correction)\b\w*)";

            // stroll through matching patterns to get replacements.
            foreach (Match match in Regex.Matches(testName, testFolderNamePatterns))
            {
                Debug.WriteLine($"Match: {match.Value}");
                // stitch replacement words together.
                testSubFolderName += ReplaceFileNameWords(match.Value, FolderNameTerms); // DictionaryOptions.folderNameTerms);
            }
            
            // index of first '_' is right after product number
            int productNameLength = output.IndexOf('_') + 1;

            // split replacement words to insert test file name and new short name for "Regulator #" in the file name, also append file extension.
            // CurrentRegulatorValue is 0 based.
            string modifiedFolderName = Path.Combine(MyResources.Strings_ModifedFolderName, testSubFolderName);
            string regulatorFolderName = $"regulator {CurrentRegulatorValue + 1}";
            string profileFolderName = $"profile {output.Substring(productNameLength + 1, 1)}";
            string testFolderName = Path.Combine(modifiedFolderName, Path.Combine(regulatorFolderName, new Regex(@"(?<profile>\b[Pp](\d)\b\w*)", RegexOptions.None, TimeSpan.FromMilliseconds(100)).IsMatch(fileNameWithoutRevision) ? profileFolderName : string.Empty)).ToLower();
            string newFileName = Path.Combine(Path.Combine(Path.GetDirectoryName(FileNameWithPath), testFolderName), $"{output.Substring(0, productNameLength)}{testName}_Reg {CurrentRegulatorValue + 1}_{output.Substring(productNameLength, output.Length - productNameLength)}{Path.GetExtension(FileNameWithPath)}");

            return newFileName;
        }

        /// <summary>
        /// Replaces words per the dictionary provided.
        /// </summary>
        /// <param name="wordToReplace">this is the word to look for a replacement in the dictionary</param>
        /// <param name="searchTerms">select dictionary to use in replacements</param>
        /// <returns>Returns a replacement word if the word provided exists in the dictionary</returns>
        private string ReplaceFileNameWords(string wordToReplace, Dictionary<string, string> searchTerms) // DictionaryOptions options)
        {

            // holds dictionary entires as a "Regular Expression" search pattern.
            string pattern = "(.)*[^<>]list|" + GetKeyList(searchTerms);
            Regex match = new Regex(pattern, RegexOptions.IgnoreCase);

            // temp replacement holder.
            string output = "";

            // returns replacement word.
            output = match.Replace(wordToReplace, replace =>
            {
                // if dictionary doesn't have contains a replacement returns  original value, otherwise replacement value.
                return searchTerms.ContainsKey(replace.Value) ? searchTerms[replace.Value] : replace.Value;
            });

            // instead of replacing "P#" just appending '_' character.
            Regex profile = new Regex(@"\b[Pp](\d)*\b\w*");
            output = profile.Replace(output, output + "_");

            // return a word.
            return output;
        }

        /// <summary>
        /// collects and generates a dictionary key pattern that "Regular Expression" can use a search pattern.
        /// </summary>
        /// <param name="list">the Dictionary to collect the keys from.</param>
        /// <returns>Returns a pattern that "Regular Expression" can use a search pattern.</returns>
        private static string GetKeyList(Dictionary<string, string> list)
        {
            return string.Join("|", new List<string>(list.Keys).ToArray());
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
                if(!Directory.Exists(Path.GetDirectoryName(NewFileName.ToString())))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(NewFileName.ToString()));
                }

                Debug.WriteLine($"New file name is {NewFileName.ToString()}");

                // return new file name.
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
