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
        /// Used to re-index ProcessKille.OmicronProgIDs list to 1 instead of 0.
        /// </summary>
        internal const string Dummy = "DummyProgID";

        /// <summary>
        /// Omicron ExeCute Test Module.
        /// </summary>
        internal const string Execute = "ExeCute.Document";

        /// <summary>
        /// Omicron Pulse Ramping Test Module.
        /// </summary>
        internal const string OMPulse = "OMPlsRmp.Document";

        /// <summary>
        /// Omicron Ramping Test Module.
        /// </summary>
        internal const string OMRamp = "OMRamp.Document";

        /// <summary>
        /// Omicron State Sequencer Test Module.
        /// </summary>
        internal const string OMSeq = "OMSeq.Document";

        /// <summary>
        /// Omicron XRio Test Object.
        /// </summary>
        internal const string XRio = "OMICRON.RioCtrl";

        /// <summary>
        /// Omicron Hardware Configuration Object.
        /// </summary>
        internal const string Hardware = "OMICRON.HCCCtrl";

        /// <summary>
        /// Omicron Group Object.
        /// </summary>
        internal const string Group = "WinCMCGroupCtrl.1";
    }
}