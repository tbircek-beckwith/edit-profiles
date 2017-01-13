using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        /// Holds user entry to removeModule command
        /// </summary>
        private static String ModuleRemovalPattern
        {
            get
            {
                return @"^(?:removeModule=)";
            }
        }

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
                if (Regex.IsMatch(item, ModuleRemovalPattern))
                {
                    requirement = true;
                    break;
                }

            }
            return requirement;
        }

        private IList<string> ListOfTestModulesToDelete(IList<string> userEntry)
        {
            IList<string> result = new List<string>() { };
            if (IsRemovalRequired(userEntry))
            {
                foreach (var item in userEntry)
                {
                    if (Regex.IsMatch(item, ModuleRemovalPattern))
                    {
                        result.Add(Regex.Split(item, ModuleRemovalPattern).GetValue(1).ToString());
                    }
                }
            }

            return result;
        }

        #endregion


    }
}
