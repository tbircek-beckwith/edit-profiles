using System;
using System.Collections.Generic;
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

        private IList<string> ItemsToFind { get; set; }

        private IList<string> ItemsToReplace { get; set; }

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

            //  foreach (string item in ItemsToFind)
            //  {
            //      if (int.TryParse(item, out int oldKey))
            //      {
            //          if (executeParameters.ContainsKey(oldKey))
            //          {
            //              // take value
            //              executeParameters.TryGetValue(oldKey, out int value);
            //              // take new key
            //              int.TryParse(ItemsToReplace.ElementAt(ItemsToFind.IndexOf(item)), out int newKey);
            //              // add new key with old value
            //              executeParameters.Add(newKey, value);

            //             // remove old key pair
            //             executeParameters.Remove(oldKey);
            //          }
            //      }
            //  }

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
                        // take new key
                        int.TryParse(ItemsToReplace.ElementAt(i), out int newKey);
                        // add new key with old value
                        newExecuteParameters.Add(newKey, value);
                    }
                }
            }

            NewParameterString = new StringBuilder("/select AutoTestIP," + string.Join(",", newExecuteParameters.Select(x => x.Key + "," + x.Value)));

            // return new StringBuilder
            return NewParameterString;
        }

        private StringBuilder FindAndReplaceParameter_old()
        {
            // Decide if any string matches to the user "Find what".
            int numberOfFindings = 0;

            foreach (string item in ItemsToFind)
            {
                // if item is blank don't match to anything.
                if (!string.IsNullOrWhiteSpace(item))
                {

                    // one of the actual execute input
                    // 4716,0,4719,1,4736,0,4738,0,4777,0
                    // 4712,0,4713,0,4714,0,4715,1,4959,0,4828,0,5224,0
                    // ignores the case of the inputs.
                    if (FindParam.IndexOf(item, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        // this variable > 0, if there is a match.
                        numberOfFindings++;
                    }
                }
            }

            // We find at least 1 item matched the user input.
            if (numberOfFindings > 0)
            {
                NewParameterString = new StringBuilder(FindParam);

                int position = 0;

                foreach (string item in ItemsToFind)
                {
                    // if the item is in the rename or remove list ignore it. 
                    // since they are handle by their respective functions.
                    // if (!(ItemsToRemove.ContainsKey(item.Split('=')[1].ToString()) || ItemsToRename.ContainsKey(item.Split('=')[1].ToString())))
                    // there is no '='. it means ExeCute parameters.
                    //if (item.Split('=').Count() == 1)
                    //{
                    //    // Replace the Execute Parameter 
                    //    NewParameterString.Replace(item, ItemsToReplace.ElementAt(position));

                    //}

                    //position++;
                    //    }
                    //}
                    // if item is blank don't match to anything.
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        if (NewParameterString.ToString().Contains(item))
                        {
                            NewParameterString.Replace(item, ItemsToReplace.ElementAt(position));

                        }
                        position++;
                    }
                }

                return NewParameterString;
            }
            else
            {
                return new StringBuilder(FindParam);
            }
        }

        #endregion

    }
}
