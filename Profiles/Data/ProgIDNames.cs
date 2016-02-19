using EditProfiles.Operations;

namespace EditProfiles.Data
{
    /// <summary>
    /// Omicron Control Center ProgIDs.
    /// </summary>
    internal sealed class ProgId : ProcessFiles
    {
        /// <summary>
        /// "DummyProgID".
        /// </summary>
        internal const string Dummy = "DummyProgID";

        /// <summary>
        /// "ExeCute.Document".
        /// </summary>
        internal const string Execute = "ExeCute.Document";

        /// <summary>
        /// "OMPlsRamp.Document".
        /// </summary>
        internal const string OMPulse = "OMPlsRmp.Document";

        /// <summary>
        /// "OMRamp.Document".
        /// </summary>
        internal const string OMRamp = "OMRamp.Document";

        /// <summary>
        /// "OMSeq.Document".
        /// </summary>
        internal const string OMSeq = "OMSeq.Document";
    }
}