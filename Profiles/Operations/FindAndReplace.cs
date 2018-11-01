using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EditProfiles.Data;

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

                this.FindParam = findParameters;

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

        private StringBuilder FindAndReplaceParameter()
        {
            // Decide if any string matches to the user "Find what".
            int numberOfFindings = 0;

            foreach (string item in this.ItemsToFind)
            {
                // if item is blank don't match to anything.
                if (!string.IsNullOrWhiteSpace(item))
                {
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

                    if (NewParameterString.ToString().Contains(item))
                    {
                        NewParameterString.Replace(item, ItemsToReplace.ElementAt(position));
                        
                    }
                    position++;
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
