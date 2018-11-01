using System;
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
        /// Holds user entry to rename commands
        /// </summary>
        private static Dictionary<string, string> ModuleRenamePatterns = new Dictionary<string, string>()
        {
            {@"^(?:renameExecute=)", ProgId.Execute},
            {@"^(?:renameSequencer=)", ProgId.OMSeq},
            {@"^(?:renameRamp=)", ProgId.OMRamp},
            {@"^(?:renamePulse=)", ProgId.OMPulse},
            {@"^(?:renameGroup=)", ProgId.Group},
            {@"^(?:renameXRio=)", ProgId.XRio},
            {@"^(?:renameHardware=)", ProgId.Hardware}
        };

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userEntry"></param>
        /// <returns>Returns true if the user requires removal any test module</returns>
        private bool IsRenameRequired(IList<string> userEntry)
        {
            bool requirement = false;

            foreach (var item in userEntry)
            {
                foreach (var value in ModuleRenamePatterns)
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

        private IDictionary<string, string> ListOfTestModulesToRename(IList<string> userEntry)
        {
            //IList<string> result = new List<string>() { };
            IDictionary<string, string> result = new Dictionary<string, string>();
            if (IsRenameRequired(userEntry))
            {
                foreach (var item in userEntry)
                {
                    foreach (var value in ModuleRenamePatterns)
                    {
                        if (Regex.IsMatch(item, value.Key))
                        {
                            // result.Add(value.Value, Regex.Split(item, value.Key).GetValue(1).ToString());
                            result.Add(Regex.Split(item, value.Key).GetValue(1).ToString(),value.Value);
                            // System.Diagnostics.Debug.WriteLine("Matched");
                            // this.ItemsToFind.Remove(item);
                        }
                    }
                }
            }

            return result;
        }

        #endregion


    }
}
