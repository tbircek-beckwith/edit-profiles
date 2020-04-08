using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using EditProfiles.Properties;
using System.Diagnostics;
using System.Linq;

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

        private string ProcessFileName()
        {
            // (?<powerScheme>\b(([Ff]orward\s)?|(([Ss]mart\s)?([Ss]rc\s)?([Rr]everse\s))?)([Pp]ower)\b\w*)|(?<product>\b([Mm]-6200[Aa])\b\w*)|(?<revision>\b[Rr]ev(\d)\b\w*)|(?<notUsed>\b([Tt]est*)|([Ss]hort\s[Vv]ersion)\b\w*)|(?<frequency>\b\d{2}[Hh][Zz]\b\w*)|(?<profile>\b[Pp](\d)\b\w*)

            // (?<powerScheme>\b(([Ff]orward\s)?|(([Ss]mart\s)?([Ss]rc\s)?([Rr]everse\s))?)([Pp]ower)\b\w*)

            // (?<product>\b([Mm]-6200[Aa])\b\w*)

            // (?<revision>\b[Rr]ev(\d)\b\w*)

            // (?<frequency>\b\d{2}[Hh][Zz]\b\w*)

            // (?<profile>\b[Pp](\d)\b\w*)

            // (?<notUsed>\b([Tt]est*)|([Ss]hort\s[Vv]ersion)\b\w*)

            string pattern = @"(?<powerScheme>\b(([Ff]orward\s)?|(([Ss]mart\s)?([Ss]rc\s)?([Rr]everse\s))?)([Pp]ower)\b\w*)|(?<product>\b([Mm]-6200[Aa])\b\w*)|(?<revision>\b[Rr]ev(\d)\b\w*)|(?<notUsed>\b([Tt]est*)|([Ss]hort\s[Vv]ersion)\b\w*)|(?<frequency>\b\d{2}[Hh][Zz]\b\w*)|(?<profile>\b[Pp](\d)\b\w*)";

            Regex r = new Regex(pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));

            string testName = r.Replace(Path.GetFileNameWithoutExtension(FileNameWithPath), string.Empty).Trim() + "_";

            //if (m.Success)
            //    Console.WriteLine(m.Result("${product}_${powerScheme}"));

            string output = string.Empty;

            foreach (Match match in Regex.Matches(Path.GetFileNameWithoutExtension(FileNameWithPath), pattern))
            {
                Console.WriteLine("Match: {0}", match.Value);
                //    //for (int groupCtr = 0; groupCtr < match.Groups.Count; groupCtr++)
                //    //{
                //    //    Group group = match.Groups[groupCtr];
                //    //    Console.WriteLine("   Group {0}: {1}", groupCtr, group.Value);
                //    //    for (int captureCtr = 0; captureCtr < group.Captures.Count; captureCtr++)
                //    //        Console.WriteLine("      Capture {0}: {1}", captureCtr,
                //    //                          group.Captures[captureCtr].Value);
                //    //}

                //    GroupCollection groups = match.Groups;
                output += ReplaceFileNameWords(match.Value);
            }

            int productNameLength = output.IndexOf('_') + 1;
            string newFileName = Path.Combine(Path.GetDirectoryName(FileNameWithPath), output.Substring(0, productNameLength) + testName + "Reg 1_" + output.Substring(productNameLength, output.Length - productNameLength))+ Path.GetExtension(FileNameWithPath);
            return newFileName;
        }

        private string ReplaceFileNameWords(string fileNameWithoutExtension)
        {
            //Regex rx = new Regex(@"\b[Rr]ev(\d)*\b\w*",
            //                RegexOptions.Compiled |
            //                RegexOptions.IgnoreCase |
            //                RegexOptions.CultureInvariant);

            //var searchRegex = new Dictionary<Regex, string>
            //{
            //    { new Regex(@"\b[Rr]ev(\d)*\b\w*"), string.Empty },        // rev## like string
            //    { new Regex(@"\b[Pp](\d)*\b\w*", string.Empty},            // p1 likestring
            //    { new Regex(@"\b[Ff]orward*\b\w*"), "Fwd " },              // forward like string
            //    { new Regex(@"\b[Rr]everse*\b\w*"), "Rev " },              // reverse like string in the file name
            //    { new Regex(@"\b[Pp]ower*\b\w*"), "Pwr " },                // power like string in the file name
            //    { new Regex(@"\b[Tt]est*\b\w*"), string.Empty },           // test like string in the file name
            //    { new Regex(@"\b[Ss]hort(\d)*\b\w*"), string.Empty },      // short like string in the file name
            //    { new Regex(@"\b\d{2}[Hh][Zz]*\b\w*"), string.Empty },     // 60Hz like string in the file name
            //    { new Regex(@"\b[Ss]rc*\b\w*"), string.Empty },            // src like string in the file name
            //    { new Regex(@"\b[Ss]mart*\b\w*"), string.Empty },          // smart like string in the file name

            //};


            var searchTerms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"m-6200a", "M-6200B_"},
                {"forward", "Fwd "},
                {"reverse", "Rev "},
                {"power", "Pwr_"},
                {"test", string.Empty},
                {"short", string.Empty},
            };

            string pattern = "(.)*[^<>]list|" + GetKeyList(searchTerms);
            Regex match = new Regex(pattern, RegexOptions.IgnoreCase);


            string output = "";

            output = match.Replace(fileNameWithoutExtension, replace =>
            {
                Console.WriteLine(" - " + replace.Value);

                return searchTerms.ContainsKey(replace.Value) ? searchTerms[replace.Value] : replace.Value;
            });

            if (!Settings.Default.AppendFileName)
            {
                Regex rx = new Regex(@"\b[Rr]ev(\d)*\b\w*",
                                 RegexOptions.Compiled |
                                 RegexOptions.IgnoreCase |
                                 RegexOptions.CultureInvariant);
                output = rx.Replace(output, string.Empty);
            }

            Regex profile = new Regex(@"\b[Pp](\d)*\b\w*");
            output = profile.Replace(output, output + "_");

            Console.WriteLine(output);

            return output; // Path.Combine(Path.GetDirectoryName(FileNameWithPath),output);
        }

        private static string GetKeyList(Dictionary<string, string> list)
        {
            return string.Join("|", new List<string>(list.Keys).ToArray());
        }

        private string GenerateTheFileName()
        {

            try
            {
                // following pattern picks Src, Smart, Forward, Reverse and Power entries in the file name.
                // new Regex(@"\b([Ss]rc|[Ss]mart)|([Ff]orward|[Rr]everse)|[Pp]ower*\b\w*")

                // following pattern picks Src, Smart, Forward, Reverse, Power, Rev#, Test, ##Hz entries in the file name.
                //Regex searchTerms = new Regex(@"\b(?<type>[Ss]rc|[Ss]mart)|(?<direction>[Ff]orward|[Rr]everse)|[Pp]ower*\b\w*|\b[Rr]ev(\d)*\b\w*|\b[Tt]est*\b\w*|\b[Pp](\d)*\b\w*|\b(?<frequency>\d{2}[Hh][Zz]*)\b\w*",
                //                            RegexOptions.Compiled |
                //                            RegexOptions.IgnoreCase |
                //                            RegexOptions.CultureInvariant);

                // IEnumerable<string> fileNameItem = ReplaceFileNameWords(Path.GetFileNameWithoutExtension(FileNameWithPath)).Split(' ');

                // NewFileName.Append(ReplaceFileNameWords(Path.GetFileNameWithoutExtension(FileNameWithPath))).Append(".occ");
                NewFileName.Append(ProcessFileName());

                //string searchTerms = @"\b(?<type>[Ss]rc|[Ss]mart)|(?<direction>[Ff]orward|[Rr]everse)|[Pp]ower*\b\w*|\b[Rr]ev(\d)*\b\w*|\b[Tt]est*\b\w*|\b[Pp](\d)*\b\w*|\b(?<frequency>\d{2}[Hh][Zz]*)\b\w*";

                //foreach (Match match in Regex.Matches(Path.GetFileNameWithoutExtension(FileNameWithPath), searchTerms))
                //{
                //    Console.WriteLine(match.Value);
                //}
                // Match m = searchTerms.Match(Path.GetFileNameWithoutExtension(FileNameWithPath));

                //if (m.Success)
                //    Console.WriteLine(m.Result("${type}_${direction}_${frequency}"));

                //var queryMatchingItems = from items in fileNameItem
                //                         let matches = searchTerms.Matches(items)
                //                         where matches.Count > 0
                //                         select new
                //                         {
                //                             matchedValue = from Match match in matches
                //                                            select match.Value,

                //                         };


                //foreach (var item in queryMatchingItems)
                //{

                //    // For this file, write out all the matching strings  
                //    foreach (var value in item.matchedValue)
                //    {
                //        Console.WriteLine(value);

                //        //string product = Regex.Replace(value, @"\b[Mm]-6200A*\b\w*", "M-6200B");
                //        //string profile = Regex.Replace(value, @"\b[Pp](\d)*\b\w*", "Fwd");
                //        //string fwdPower = Regex.Replace(value, @"\b[Ff]orward*\b\w*", "Fwd");
                //        //string revPower = Regex.Replace(value, @"\b[Rr]everse*\b\w*", "Rev");
                //        //string revision = Regex.Replace(value, @"\b[Rr]ev(\d)*\b\w*", string.Empty);
                //        //string frequency = Regex.Replace(value, @"\b\d{2}[Hh][Zz]*\b\w*", value);

                //        //Console.WriteLine($"{product}_rest of the file name_{fwdPower}{revPower}_{profile}_reg1_{frequency}");
                //    }
                //}

                //// Search pattern for "rev#" and "rev##" like file name entries.
                //// used http://regexr.com to generate following expression.
                ////
                //Regex rx = new Regex(@"\b[Rr]ev(\d)*\b\w*",
                //                RegexOptions.Compiled |
                //                RegexOptions.IgnoreCase |
                //                RegexOptions.CultureInvariant);

                //// Search pattern for "p#" or "P#" like in file name entries.
                //Regex profile = new Regex(@"\b[Pp](\d) *\b\w*",
                //                    RegexOptions.Compiled |
                //                    RegexOptions.IgnoreCase |
                //                    RegexOptions.CultureInvariant);

                // some actual file names with extensions
                // M-6200A P1 Forward Power Bandwidth Test 60Hz Rev1.occ  -> M-6200B_Bandwidth_Fwd Pwr_Reg 1_P1_60Hz .occ
                // M-6200A P1 Forward Power Bandcenter Test 50Hz Rev2.occ -> M-6200B_Bandcenter Test_Fwd Pwr_P1_Reg1_50Hz.occ
                // M-6200A P2 Forward Power Definite Time Delay Test 60Hz Rev1.occ -> M-6200B_Definite Time Delay_Fwd Pwr_P2_Reg1_60Hz.occ

                // example of the changes:
                // 0 -> M-6200A -> M-6200B 
                // 3 -> P1 -> P1, P2 -> P2 ->                    change position
                // 2 -> Forward -> Fwd, Reverse -> Rev 
                // 2 -> Power -> Pwr
                // 5 -> 60Hz -> 60Hz, 50Hz -> 50Hz ->            change position
                // Revxx -> replace with string.Empty
                // Test -> replace with string.Empty
                // 1 -> remaining string. hopefully this is the actual test name.
                // 7 -> Regx -> x = 1,2, or 3 -> adds new value

                //// split up file name without extension
                //OriginalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileNameWithPath).Split(' ');



                //// append new file name with directory
                //NewFileName.Append($"{Path.GetDirectoryName(FileNameWithPath)}\\M-6200B_");

                //// loop thru original file name starting index of "Power" keyword + 1 until "Test" keyword 
                //for (int i = OriginalFileNameWithoutExtension.IndexOf("Power") + 1; i < OriginalFileNameWithoutExtension.Count - 3; i++)
                //{
                //    // add original file name keywords
                //    NewFileName.Append($"{OriginalFileNameWithoutExtension[i]} ");
                //}

                //// replace the last space character with "_".
                //NewFileName.Remove(NewFileName.Length - 1, 1);
                //NewFileName.Append("_");

                //// process power scheme. eg: Forward Power -> Fwd Pwr
                //for (int t = 2; t < OriginalFileNameWithoutExtension.IndexOf("Power") + 1; t++)
                //{
                //    switch (OriginalFileNameWithoutExtension[t].ToLowerInvariant())
                //    {
                //        case "src":
                //            OriginalFileNameWithoutExtension[t] = "Src ";
                //            break;
                //        case "forward":
                //            OriginalFileNameWithoutExtension[t] = "Fwd ";
                //            break;
                //        case "reverse":
                //            OriginalFileNameWithoutExtension[t] = "Rev ";
                //            break;
                //        case "power":
                //            if (OriginalFileNameWithoutExtension[t - 1] == "Georgia")
                //            {
                //                OriginalFileNameWithoutExtension[t] = "Power_";
                //            }
                //            else
                //            {
                //                OriginalFileNameWithoutExtension[t] = "Pwr_";
                //            }
                //            break;
                //        default:
                //            break;
                //    }

                //    // add modified power scheme keywords.
                //    NewFileName.Append($"{OriginalFileNameWithoutExtension[t]}");
                //}

                //// append "Profile" number, frequency and extension
                //NewFileName.Append($"{OriginalFileNameWithoutExtension[1]}_{OriginalFileNameWithoutExtension[OriginalFileNameWithoutExtension.Count - 2]}{Path.GetExtension(FileNameWithPath)}");

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

        //private string GenerateTheFileName_Old()
        //{
        //    try
        //    {
        //        //
        //        // Search pattern for "rev#" and "rev##" like file name entries.
        //        // used http://regexr.com to generate following expression.
        //        //
        //        Regex rx = new Regex(@"\brev(\d)*\b\w*",
        //                        RegexOptions.Compiled |
        //                        RegexOptions.IgnoreCase |
        //                        RegexOptions.CultureInvariant);

        //        OriginalFileNameWithPath = Path.GetFullPath(FileNameWithPath)
        //                                           .Replace(".occ", "")
        //                                           .Split(' ');

        //        foreach (string word in OriginalFileNameWithPath)
        //        {
        //            if (rx.IsMatch(word))
        //            {
        //                //// If the file name contains "rev" followed by a number or a space increment the rev number by one.
        //                // NewFileName.Append(("Rev") +
        //                //    (int.Parse(word.Trim().ToUpperInvariant().Replace("REV", ""),
        //                //    CultureInfo.InvariantCulture) + 1).ToString(
        //                //    CultureInfo.InvariantCulture));

        //                // Since Ritchie has version control file names do not require Rev# suffix.
        //                // the following line adds an extra space to the fileName.
        //                //  NewFileName.Append(" ");
        //            }
        //            else if (!string.IsNullOrWhiteSpace(word))
        //            {
        //                NewFileName.Append(word.Trim() + " ");
        //            }
        //        }

        //        // If the file name does not contain "rev" followed by a number or a space add 
        //        // "Rev1" string to the file name.
        //        if (Settings.Default.AppendFileName)
        //        {
        //            if (!NewFileName.ToString().ToUpperInvariant().Contains("REV"))
        //            {
        //                NewFileName.Append("Rev1");
        //            }
        //        }

        //        // Add Omicron Control Center extension to the file name.
        //        NewFileName.Append(".occ");

        //        // Create new folder to store modified files.
        //        if (!(Directory.Exists(Path.Combine(
        //                                    Path.GetDirectoryName(
        //                                     NewFileName.ToString()),
        //                                    MyResources.Strings_ModifedFolderName))))
        //        {
        //            Directory.CreateDirectory(Path.Combine(
        //                                        Path.GetDirectoryName(
        //                                         NewFileName.ToString()),
        //                                        MyResources.Strings_ModifedFolderName));
        //        }

        //        // Replace old folder path with the new location.
        //        NewFileName.Replace(Path.GetDirectoryName(
        //                                           NewFileName.ToString()),
        //                                           Path.Combine(Path.GetDirectoryName(
        //                                            NewFileName.ToString()),
        //                                           MyResources.Strings_ModifedFolderName));

        //        Debug.WriteLine("New file name is {0}", NewFileName.ToString());
        //        return NewFileName.ToString();
        //    }
        //    catch (FormatException fe)
        //    {
        //        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
        //        ErrorHandler.Log(fe, CurrentFileName);
        //        return string.Empty;
        //    }
        //    catch (OverflowException oe)
        //    {
        //        // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
        //        ErrorHandler.Log(oe, CurrentFileName);
        //        return string.Empty;
        //    }
        //}

        #endregion

    }
}
