using EditProfiles.Operations;
using System.Collections.Generic;

namespace EditProfiles.Data
{
    /// <summary>
    /// Omicron Control Center Module names
    /// </summary>
    internal sealed class TestModuleName : ProcessFiles
    {

        /// <summary>
        /// Holds supported Omicron Module list
        /// </summary>
        internal static IList<string> OmicronModuleList
        {
            get
            {
                return new List<string>
                {
                    "OCCenter",
                    "OMExec",
                    "OMPlsRmp",
                    "OMRamp",
                    "OMSeq",
                    "QuickCmc"
                };
            }
        }

    }
}
