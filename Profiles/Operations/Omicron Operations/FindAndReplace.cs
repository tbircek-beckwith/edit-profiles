using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EditProfiles.Properties;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Scans Omicron Test Module Parameters and replaces with the user specified keywords.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private string FindParam { get; set; }

        private StringBuilder NewParameterString { get; set; }

        private IDictionary<string, string> ItemsToRemove = new Dictionary<string, string>();

        private IDictionary<string, string> ItemsToRename = new Dictionary<string, string>();

        /// <summary>
        /// Holds information about items to be find in <see cref="OMICRON.OCCenter.IAutoTM"/>
        /// </summary>
        private List<string> ItemsToFind { get; set; }

        /// <summary>
        /// Holds information about items to be replaced in <see cref="OMICRON.OCCenter.IAutoTM"/>
        /// </summary>
        private List<string> ItemsToReplace { get; set; }

        /// <summary>
        /// Holds information about items to be replaced in <see cref="OMICRON.OCCenter.IAutoTM"/>
        /// <para>This is special case list to generate <see cref="Profile"/> given test file
        /// where every value is belong to <see cref="Profile"/> 1.</para>
        /// </summary>
        private List<string> ProfileItemsToReplace { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Finds and replaces the Execute Test Module Parameters.
        /// </summary>
        /// <param name="findParameters">The parameters currently in the Execute Module.</param>
        /// <returns>Returns a modified string.</returns>
        public StringBuilder FindAndReplaceParameters(string findParameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(findParameters))
                {
                    throw new ArgumentNullException("findParameters");
                }

                FindParam = findParameters;

                return FindAndReplaceParameter();
            }
            catch (ArgumentNullException ae)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ae, CurrentFileName);
                return new StringBuilder();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// find and replace execute module parameters.
        /// </summary>
        /// <returns>a StringBuilder that contains a new execute module parameters.</returns>
        private StringBuilder FindAndReplaceParameter()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            // initialize Dictionaries
            Dictionary<int, int> oldExecuteParameters = new Dictionary<int, int>();
            Dictionary<int, int> newExecuteParameters = new Dictionary<int, int>();

            // convert execute module entry to Dictionary
            for (int i = 0; i < FindParam.Split(',').Length; i++)
            {
                // verify items are numbers
                if (int.TryParse(FindParam.Split(',')[i], out int key))
                {
                    if (int.TryParse(FindParam.Split(',')[i + 1], out int value))
                    {
                        // add key, value pairs
                        oldExecuteParameters.Add(key, value);

                        // skip values as they included in previous step
                        i++;
                    }
                }

            }

            // is there any matching parameters?
            if (!oldExecuteParameters.Keys.Any(x => ItemsToFind.Contains(x.ToString())))
            {
                // NO. return old parameters. 
                NewParameterString = new StringBuilder(MyResources.Strings_DrIPNet_prepend + string.Join(",", FindParam)); //new StringBuilder(FindParam); //
            }
            else
            {
                // only one repeating item with one index
                List<int> duplicateEntries = ItemsToFind.GroupBy(x => x)
                                                        .Where(g => g.Count() > 2)
                                                        .Select(y => int.Parse(y.Key)) // new { Element = y.Key, Index = ItemsToFind.IndexOf(y.Key) })
                                                        .ToList();

                // scan whole Execute parameter entry
                foreach (var currentKey in oldExecuteParameters.Keys)
                {

                    // scan whole .csv file for the current key
                    for (int i = 0; i < ItemsToFind.Count; i++)
                    {
                        // string matching would match strings of "4567" and "4567x"
                        // so match them as numbers
                        if (int.TryParse(ItemsToFind.ElementAt(i), out int oldKey))
                        {
                            // verify both keys are matched.
                            if (currentKey == oldKey)
                            {


                                // initialize the replacement key
                                int replacementKey = default;

                                // if the register has multiple entry for each regulator,
                                if (duplicateEntries.Contains(oldKey))
                                {
                                    // use the one appropriate to the current regulator 
                                    // eg: second for regulator 2
                                    int.TryParse(ItemsToReplace.ElementAt(GetDuplicateIndex(i)), out replacementKey);

                                }
                                else
                                {
                                    // otherwise use corresponding the new register value 
                                    // from the excel file.
                                    int.TryParse(ItemsToReplace.ElementAt(i), out replacementKey);
                                }

                                // retrieve original value
                                oldExecuteParameters.TryGetValue(currentKey, out int originalValue);

                                // modify MB_ActiveProfile instead of using original value
                                // TODO: This should be aware of if the user entry or a .csv file in use.
                                newExecuteParameters.Add(replacementKey, ViewModel.ChangeActiveProfile == currentKey ? activeProfile - 1 : originalValue);

                                // stop scanning and move on to next entry.
                                break;
                            }
                        }
                    }

                }

                // return new parameters.
                NewParameterString = new StringBuilder(MyResources.Strings_DrIPNet_prepend + string.Join(",", newExecuteParameters.Select(x => x.Key + "," + x.Value)));

            }

            Debug.WriteLine($"find: {FindParam}\r\nrep: {NewParameterString}");
            Debug.WriteLine($"-----------------------------------------------------> total time: {stopwatch.ElapsedMilliseconds}");
            
            // return new StringBuilder
            return NewParameterString;
        }

        private int GetDuplicateIndex(int currentIndex)
        {
            int result = default;

            // finds at least g.Count() > 2 in ItemsToFind all items with all indexes
            var duplicates = ItemsToFind.Select((item, index) => new { Item = item, Index = index })
                                        .GroupBy(g => g.Item)
                                        .Where(g => g.Count() > 2);

            foreach (var item in duplicates)
            {
                if (Equals(item.Key, ItemsToFind.ElementAt(currentIndex)))
                {
                    result = item.ElementAt(regulator - 1).Index;
                    Debug.WriteLine($"something found: {item.Key}");

                    // canRegisterAdded = false;
                    break;
                }
            }
            return result;
        }

        #endregion

    }
}
