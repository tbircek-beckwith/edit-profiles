using System.Collections.Generic;
using System.Text.RegularExpressions;
using EditProfiles.Data;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Searches user entered removeModule command and deletes specified test module.
    /// Search done by name of the test module.
    /// </summary>
    partial class ProcessFiles
    {
        #region Properties

        /// <summary>
        /// Holds user entry to remove commands
        /// </summary>
        private static Dictionary<string, string> ModuleRemovalPatterns = new Dictionary<string, string>()
        {
            {@"^(?:removeExecute=)", ProgId.Execute},
            {@"^(?:removeSequencer=)", ProgId.OMSeq},
            {@"^(?:removeRamp=)", ProgId.OMRamp},
            {@"^(?:removePulse=)", ProgId.OMPulse},
            {@"^(?:removeGroup=)", ProgId.Group},
            {@"^(?:removeXRio=)", ProgId.XRio},
            {@"^(?:removeHardware=)", ProgId.Hardware}
        };

        #endregion

        #region Private Methods

        /// <summary> 
        ///  
        /// </summary>
        /// <param name="userEntry"></param>
        /// <returns>Returns true if the user requires removal any test module</returns>
        private bool IsRemovalRequired(IList<string> userEntry)
        {
            bool requirement = false;

            foreach (var item in userEntry)
            {
                foreach (var value in ModuleRemovalPatterns)
                {
                    if (Regex.IsMatch(item, value.Key))
                    {
                        requirement = true;
                        break;
                    }
                    if (requirement)
                    {
                        break;
                    }
                }
                if (requirement)
                {
                    break;
                }
            }
            return requirement;
        }

        private IDictionary<string, string> ListOfTestModulesToDelete(IList<string> userEntry)
        {
            bool removed = false;
            IDictionary<string, string> result = new Dictionary<string, string>();
            if (IsRemovalRequired(userEntry))
            {
                this.ItemsToFind = new List<string>();

                foreach (var item in userEntry)
                {
                    foreach (var value in ModuleRemovalPatterns)
                    {
                        if (Regex.IsMatch(item, value.Key))
                        {
                            if (!result.ContainsKey(Regex.Split(item, value.Key).GetValue(1).ToString()))
                            {
                                result.Add(Regex.Split(item, value.Key).GetValue(1).ToString(), value.Value);
                            }                             

                            // already been added to the Dictionary.
                            removed = true;
                            break;
                        }                                           
                    }
                    if (!(removed) && this.ItemsToFind.IndexOf(item) < 0)
                    {
                        this.ItemsToFind.Add(item);
                    }
                    removed = false;
                }
            }

            return result;
        }

        #endregion


    }
}
