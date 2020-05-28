
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EditProfiles.Operations
{
    /// <summary>
    ///
    /// </summary>
    public class AnalyzeValues
    {

        #region Public Variables

        //
        // used http://regexr.com to generate following expressions.
        //

        /// <summary>
        /// Holds patterns to retrieve profile set points from a .csv file
        /// </summary>
        public readonly string SetPointPatterns = @"(?<setpointKeyword>(?:setpoints\[\d\]))+";

        /// <summary>
        /// Holds patterns to retrieve regulator number from a .csv file
        /// </summary>
        public readonly string RegulatorPatterns = @"(?<regulatorKeyword>(?:REG_IDX_\d))+";

        /// <summary>
        /// Holds patterns to retrieve Profile 1 or P1 like strings.
        /// </summary>
        public readonly string ProfilePatterns = @"(?<p1Keyword>(?:P\d))+|(?<profileKeyword>(?:Profile \d))+";

        /// <summary>
        /// Holds patterns to retrieve regulator number from a .csv file
        /// </summary>
        public readonly string GroupFolderPatterns = @"(?<groupSetKeyword>(?:Set))+|(?<groupProfileKeyword>(?:Profile \d))+";

        /// <summary>
        /// Holds patterns to generate test module titles
        /// </summary>
        public readonly string TitlePatterns = @"(?<powerKeyword>\b(([Ff]orward\s)|(([Ss]mart\s)?([Ss]rc\s)?([Rr]everse\s)))([Pp]ower)\b\w*)|(?<product>\b([Mm]-6200[A-Za-z]?)\b\w*)|(?<powerInitials>\b([Ff][Pp]|([Rr][Pp]))\b\w*)|(?<srcReverse>\b([Ss]rc\s)([Rr]everse)\b\w*)|(?<src>\b([Ss]rc)\b\w*)";

        /// <summary>        
        /// Holds patterns to generate test file names
        /// </summary>
        public readonly string TestFileNamePatterns = @"(?<powerKeyword>\b(([Ff]orward\s)|(([Ss]mart\s)?([Ss]rc\s)?([Rr]everse\s)))([Pp]ower)\b\w*)|(?<product>\b([Mm]-6200[A-Za-z]?)\b\w*)|(?<revision>\b[Rr]ev\d\b\w*)|(?<notUsed>\b([Tt]est*)|([Ss]hort\s[Vv]ersion)\b\w*)|(?<frequency>\b\d{2}[Hh][Zz]\b\w*)|(?<profile>\b(?=[Pp](\d))\b\w*)|(?<determination>\b[Aa][Dd]([Mm])?\b\w*)|(?<distrubuted>\b[Dd][Gg]\b\w*)|(?<powerInitials>\b([Ff][Pp]|([Rr][Pp]))\b\w*)|(?<srcReverse>\b([Ss]rc\s)([Rr]everse)\b\w*)|(?<src>\b([Ss]rc)\b\w*)";

        /// <summary>
        /// Holds patterns to generate test folder names
        /// </summary>
        public readonly string TestFolderNamePatterns = @"(?<alarms>\b((?=Alarm)\b\w*))|(?<bandcenters>\b((?=Bandcenter)\b\w*))|(?<bandwidth>\b((?=Bandwidth)\b\w*))|(?<dtDelay>\b((?=Definite)\b\w*))|(?<dg>\b((?=Distributed)\b\w*))|(?<georgia>\b((?=Georgia)\b\w*))|(?<intertap>\b((?=Inter[Tt]ap)\b\w*))|(?<inverse>\b((?=Inverse)\b\w*))|(?<ldc>\b((?=[Ll][Dd][Cc][\s])\b\w*))|(?<limit>\b((?=Line Limit)\b\w*))|(?<pulse>\b((?=Pulse Width)\b\w*))|(?<varbias>\b((?=VAr[Bb]ias)\b\w*))|(?<smartVR>\b(Smart [Vv][Rr])\b\w*)|(?<vr>\b(Reduction)\b\w*)|(?<vt>\b(Correction)\b\w*)";

        /// <summary>
        /// holds replacement words with ignoring casing.
        /// <para>must add or remove words here to provide replacements.</para>
        /// </summary>
        public Dictionary<string, string> TitleKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // {"original", "replacement"},
                {"m-6200a", "M-6200B"},
                //{"m-6200", "M-6200B"},
                {"forward power", "Fwd Pwr"},
                {"fp", "Fwd Pwr"},
                {"reverse power", "Rev Pwr"},
                {"rp", "Rev Pwr"},
                {"smart reverse power", "AD" },
                {"src reverse power", "Src Rev Pwr" },
                {"src reverse", "Src Rev Pwr" },
                {"src", "Src Rev Pwr" },
                {"smart src reverse power", "ADM" },
                //{"adm", "ADM" },
                //{"ad", "AD" },
                //{"dg", "DG" },
                //{"test", string.Empty},
                //{"short version", string.Empty},
            };

        /// <summary>
        /// holds replacement words with ignoring casing.
        /// <para>must add or remove words here to provide replacements.</para>
        /// </summary>
        public Dictionary<string, string> FileNameKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // {"original", "replacement"},
                {"m-6200a", "M-6200B_"},
                //{"m-6200b_", "M-6200B_"},   // reformat
                {"m-6200b", "M-6200B_"},    // reformat
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

        /// <summary>
        /// holds replacement words with ignoring casing.
        /// <para>must add or remove words here to provide replacements.</para>
        /// </summary>
        public Dictionary<string, string> FolderNameKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // {"original", "replacement"},
                {"alarm", "alarms"},                            //
                {"bandcenter", "bandcenters"},                  //
                {"bandwidth", "bandwidths"},                    //
                {"definite", "definite-time-delays"},           //
                {"distributed", "distributed-generations"},     //
                {"georgia", "georgia-power"},                   //
                {"intertap", "intertap-time-delays"},           //
                {"intertap_output", "intertap-time-delays"},    //
                {"inverse", "inverse-time-delays"},             //
                {"ldc", "ldc-settings"},                        //
                // {"line limit", "line-limits"},      
                {"line", "line-limits"},                        //
                // {"pulse width", "output-pulse-widths"},
                {"pulse", "output-pulse-widths"},               //
                {"varbias", "varbias"},                         //
                {"smart vr", "voltage-reductions"},             //
                {"voltage reduction", "voltage-reductions"},
                {"reduction test", "voltage-reductions"},       //
                {"reduction", "voltage-reductions"},       //
                {"correction", "vt-corrections"},               //
            };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AnalyzeValues()
        {
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Removes input string non-matching keywords using specified pattern.
        /// </summary>
        /// <param name="input">original string to be modified.</param>
        /// <param name="pattern">pattern to use matching keywords</param>
        /// <returns>Returns only matched, others return as empty string.</returns>
        public string Match(string input, string pattern)
        {

            // temp storage to keep replacement words.
            string output = string.Empty;

            if(IsMatch(input, pattern))
            {
                Match match = Regex.Match(input, pattern);
                output = match.Value;
            }
            else
            {
                output = input;
            }

            return output;
        }

        /// <summary>
        /// Replaces input string with matching keywords using specified pattern.
        /// </summary>
        /// <param name="input">original string to be modified.</param>
        /// <param name="pattern">pattern to use matching keywords</param>
        /// <param name="keywords">keywords to match and replace</param>
        /// <returns>Returns replacement of the values matched, others return as empty string.</returns>
        public string Replace(string input, string pattern, Dictionary<string, string> keywords)
        {

            // temp storage to keep replacement words.
            string output = string.Empty;

            // if there is no match return original input.
            if (Regex.Matches(input, pattern).Count > 0)
            {
                // stroll through matching patterns to get replacements.
                foreach (Match match in Regex.Matches(input, pattern))
                {
                    Debug.WriteLine($"Match: {match.Value}");
                    // stitch replacement words together.
                    output += ReplaceValue(match.Value, keywords);
                }

                // return modified input
                return output;
            }
            else
            {
                // return original input
                return input;
            }
        }

        /// <summary>
        /// Replaces input string with matching keywords using specified pattern.
        /// </summary>
        /// <param name="input">original string to be modified.</param>
        /// <param name="pattern">pattern to use matching keywords</param>
        /// <param name="keywords">keywords to match and replace</param>
        /// <returns>Returns replacement of the values matched, and non-matched original values.</returns>
        public string Change(string input, string pattern, Dictionary<string, string> keywords)
        {

            return ReplaceValue(input, keywords);
        }

        /// <summary>
        /// Replaces input string with matching keywords using specified pattern.
        /// </summary>
        /// <param name="input">original string to be modified.</param>
        /// <param name="pattern">pattern to use matching keywords</param>
        /// <param name="keywords">keywords to match and replace</param>
        /// <returns>Returns value/s that doesn't match the pattern</returns>
        public string Extract(string input, string pattern, Dictionary<string, string> keywords)
        {

            // temp storage to keep replacement words.
            string output = Regex.Replace(input, pattern, string.Empty, RegexOptions.None, TimeSpan.FromMilliseconds(200)).Trim();

            return output;
        }

        /// <summary>
        /// Indicates if <paramref name="input"/> contains matching <paramref name="pattern"/>
        /// </summary>
        /// <param name="input">original string to be modified.</param>
        /// <param name="pattern">pattern to use matching keywords</param>
        /// <returns>Returns true if there is a match</returns>
        public bool IsMatch(string input, string pattern)
        {
            // return false if input is null
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            return Regex.IsMatch(input, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Replaces words per the dictionary provided.
        /// </summary>
        /// <param name="textToReplace">this is the word to look for a replacement in the dictionary</param>
        /// <param name="searchTerms">select dictionary to use in replacements</param>
        /// <returns>Returns a replacement word if the word provided exists in the dictionary</returns>
        private string ReplaceValue(string textToReplace, Dictionary<string, string> searchTerms) // DictionaryOptions options)
        {

            // holds dictionary entires as a "Regular Expression" search pattern.
            string pattern = "(.)*[^<>]list|" + GetKeyList(searchTerms);
            Regex match = new Regex(pattern, RegexOptions.IgnoreCase);

            // temp replacement holder.
            string output = "";

            // returns replacement word.
            output = match.Replace(textToReplace, replace =>
            {
                // if dictionary doesn't have contains a replacement returns  original value, otherwise replacement value.
                return searchTerms.ContainsKey(replace.Value) ? searchTerms[replace.Value] : replace.Value;
            });

            // 
            if (!new Regex(@"(?<setting>\b(?=Set)\b\w*)").IsMatch(textToReplace))
            {
                // instead of replacing "P#" just appending '_' character.
                Regex profile = new Regex(@"(?<profile>\b(?=[Pp](\d))\b\w*)");
                output = profile.Replace(output, output + "_");
            }

            //// the most likely file name and/or folder name wouldn't have "Set" and "P#" in their name.
            //if (!new Regex(@"(?<profile>\b(?=[Pp](\d))\b\w*)|(?<setting>\b(?=Set)\b\w*)").IsMatch(textToReplace))
            //{
            //    // instead of replacing "P#" just appending '_' character.
            //    Regex profile = new Regex(@"(?<profile>\b(?=[Pp](\d))\b\w*)");
            //    output = profile.Replace(output, output + "_");
            //}

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

        #endregion
    }
}
