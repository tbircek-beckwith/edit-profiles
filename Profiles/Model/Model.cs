using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditProfiles
{
    /// <summary>
    /// Contains data, not behavior.
    /// This class only contains properties.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// ProgramTitle
        /// </summary>
        public string ProgramTitle { get; set; }

        /// <summary>
        /// FindWhatLabelText
        /// </summary>
        public string FindWhatLabelText { get; set; }

        /// <summary>
        /// FindWhatTextBoxText
        /// </summary>
        public string FindWhatTextBoxText { get; set; }

        /// <summary>
        /// ReplaceWithLabelText
        /// </summary>
        public string ReplaceWithLabelText { get; set; }

        /// <summary>
        /// ReplaceWithTextBoxText
        /// </summary>
        public string ReplaceWithTextBoxText { get; set; }

        /// <summary>
        /// PasswordLabelText
        /// </summary>
        public string PasswordLabelText { get; set; }

        /// <summary>
        /// PasswordTextBoxText
        /// </summary>
        public string PasswordTextBoxText { get; set; }

        /// <summary>
        /// FindReplaceButtonText
        /// </summary>
        public string FindReplaceButtonText { get; set; }

        /// <summary>
        /// DetailsRichTextBoxText
        /// </summary>
        public string DetailsTextBoxText { get; set; }

        /// <summary>
        /// OverviewRichTextBoxText
        /// </summary>
        public string OverviewTextBoxText { get; set; }

        /// <summary>
        /// FindReplaceButtonEnabled
        /// </summary>
        public bool FindReplaceButtonEnabled { get; set; }

        /// <summary>
        /// OverviewRichTextBoxText
        /// </summary>
        public string ProcessFiles { get; set; }

    }
}
