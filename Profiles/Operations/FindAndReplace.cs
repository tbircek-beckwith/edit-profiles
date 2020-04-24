using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

        private List<string> ItemsToFind { get; set; }

        private List<string> ItemsToReplace { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Finds and replaces the Execute Test Module Parameters.
        /// </summary>
        /// <param name="findParameters">The parameters currently in the Exceute Module.</param>
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
            // initialize Dictionaries
            Dictionary<int, int> oldExecuteParameters = new Dictionary<int, int>();
            Dictionary<int, int> newExecuteParameters = new Dictionary<int, int>();

            // convert execute module entry to Dictionary
            for (int i = 0; i < FindParam.Split(',').Length; i++)
            {
                // add key, value pairs
                oldExecuteParameters.Add(int.Parse(FindParam.Split(',')[i]), int.Parse(FindParam.Split(',')[i + 1]));
                // skip values as they included in previous step
                i++;
            }


            // only one repeating item with one index
            List<int> duplicateEntries = ItemsToFind.GroupBy(x => x)
                                                    .Where(g => g.Count() > 2)
                                                    .Select(y => int.Parse(y.Key)) // new { Element = y.Key, Index = ItemsToFind.IndexOf(y.Key) })
                                                    .ToList();

            //// finds at least g.Count() > 2 in ItemsToFind all items with all indexes
            //var duplicates = ItemsToFind.Select((item, index) => new { Item = item, Index = index })
            //                            .GroupBy(g => g.Item)
            //                            .Where(g => g.Count() > 2);


            // scan whole entry
            for (int i = 0; i < ItemsToFind.Count; i++)
            {
                // ignore values cannot be numbers.
                // modbus registers are numbers.
                if (int.TryParse(ItemsToFind.ElementAt(i), out int oldKey))
                {

                    // is the modbus register in the entry?
                    if (oldExecuteParameters.ContainsKey(oldKey))
                    {

                        // take value
                        oldExecuteParameters.TryGetValue(oldKey, out int value);

                        // if the register has multiple entry for each regulator,
                        // use the one appropriate to the current regulator 
                        // eg: second for regulator 2
                        // otherwise use corresponding the new register value 
                        // from the excel file.

                        int newKey = default;
                        if (duplicateEntries.Contains(oldKey))
                        {
                            int.TryParse(ItemsToReplace.ElementAt(GetDuplicateIndex(i)), out newKey);
                            
                        }
                        else
                        {
                            int.TryParse(ItemsToReplace.ElementAt(i), out newKey);
                        }
                        //string newValue = ItemsToReplace.ElementAt(i);
                        //if (duplicateEntries.Contains(ItemsToFind.ElementAt(i)))
                        //{
                        //    foreach (var item in duplicates)
                        //    {
                        //        if (Equals(item.Key, ItemsToFind.ElementAt(i)))
                        //        {
                        //            // ItemsToReplace.ElementAt(item.ElementAt(CurrentRegulatorValue).Index)
                        //            newValue = ItemsToReplace.ElementAt(item.ElementAt(CurrentRegulatorValue).Index);
                        //            Debug.WriteLine($"something found: {item.Key}");

                        //            // canRegisterAdded = false;
                        //            break;
                        //        }
                        //    }
                        //}

                        // take new key - for each regulator value add 10000 to replacement value
                        //int.TryParse(ItemsToReplace.ElementAt(i), out int newKey);
                        // int.TryParse(newValue, out int newKey);

                        // add new key with old value.
                        if (!newExecuteParameters.ContainsKey(newKey))
                            newExecuteParameters.Add(newKey > 39999 && newKey < UInt16.MaxValue ? newKey : newKey + (CurrentRegulatorValue * 10000), value);
                    }
                }
            }

            NewParameterString = new StringBuilder("/select AutoTestIP," + string.Join(",", newExecuteParameters.Select(x => x.Key + "," + x.Value)));

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
                    // ItemsToReplace.ElementAt(item.ElementAt(CurrentRegulatorValue).Index)
                    result = item.ElementAt(CurrentRegulatorValue).Index;
                    Debug.WriteLine($"something found: {item.Key}");

                    // canRegisterAdded = false;
                    break;
                }
            }
            return result;
        }

        //private StringBuilder FindAndReplaceParameter_old()
        //{
        //    // Decide if any string matches to the user "Find what".
        //    int numberOfFindings = 0;

        //    foreach (string item in ItemsToFind)
        //    {
        //        // if item is blank don't match to anything.
        //        if (!string.IsNullOrWhiteSpace(item))
        //        {

        //            // one of the actual execute input
        //            // 4716,0,4719,1,4736,0,4738,0,4777,0
        //            // 4712,0,4713,0,4714,0,4715,1,4959,0,4828,0,5224,0
        //            // ignores the case of the inputs.
        //            if (FindParam.IndexOf(item, StringComparison.OrdinalIgnoreCase) > -1)
        //            {
        //                // this variable > 0, if there is a match.
        //                numberOfFindings++;
        //            }
        //        }
        //    }

        //    // We find at least 1 item matched the user input.
        //    if (numberOfFindings > 0)
        //    {
        //        NewParameterString = new StringBuilder(FindParam);

        //        int position = 0;

        //        foreach (string item in ItemsToFind)
        //        {
        //            // if the item is in the rename or remove list ignore it. 
        //            // since they are handle by their respective functions.
        //            // if (!(ItemsToRemove.ContainsKey(item.Split('=')[1].ToString()) || ItemsToRename.ContainsKey(item.Split('=')[1].ToString())))
        //            // there is no '='. it means ExeCute parameters.
        //            //if (item.Split('=').Count() == 1)
        //            //{
        //            //    // Replace the Execute Parameter 
        //            //    NewParameterString.Replace(item, ItemsToReplace.ElementAt(position));

        //            //}

        //            //position++;
        //            //    }
        //            //}
        //            // if item is blank don't match to anything.
        //            if (!string.IsNullOrWhiteSpace(item))
        //            {
        //                if (NewParameterString.ToString().Contains(item))
        //                {
        //                    NewParameterString.Replace(item, ItemsToReplace.ElementAt(position));

        //                }
        //                position++;
        //            }
        //        }

        //        return NewParameterString;
        //    }
        //    else
        //    {
        //        return new StringBuilder(FindParam);
        //    }
        //}

        #endregion

    }
}
