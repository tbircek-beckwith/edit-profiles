using EditProfiles.Operations;

namespace EditProfiles.Data
{
    /// <summary>
    /// Omicron Control Center ProgIDs.
    /// </summary>
    public sealed class ProgId : ProcessFiles
    {
        /// <summary>
        /// "DummyProgID".
        /// </summary>
        public const string Dummy = "DummyProgID";

        /// <summary>
        /// "ExeCute.Document".
        /// </summary>
        public const string Execute = "ExeCute.Document";

        /// <summary>
        /// "OMPlsRamp.Document".
        /// </summary>
        public const string OMPulse = "OMPlsRmp.Document";

        /// <summary>
        /// "OMRamp.Document".
        /// </summary>
        public const string OMRamp = "OMRamp.Document";

        /// <summary>
        /// "OMSeq.Document".
        /// </summary>
        public const string OMSeq = "OMSeq.Document";
    }
}