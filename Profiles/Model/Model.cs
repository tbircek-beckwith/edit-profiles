using System.Windows.Controls;

namespace EditProfiles.MainModel
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
        public string FileProgressBar { get; set; }
        
        /// <summary>
        /// Sets and gets ModuleProcessBar.
        /// </summary>
        public string ModuleProgressBar { get; set; }

        /// <summary>
        /// File ProgressBar Value
        /// </summary>
        public int FileProgressBarValue { get; set; }

        /// <summary>
        /// Module ProgressBar Value
        /// </summary>
        public int ModuleProgressBarValue { get; set; }

        /// <summary>
        /// Module ProgressBar Max Value
        /// </summary>
        public int ModuleProgressBarMax { get; set; }

        /// <summary>
        /// File ProgressBar Max Value
        /// </summary>
        public int FileProgressBarMax { get; set; }

        /// <summary>
        /// Textbox to show process
        /// </summary>
        public TextBox TextBoxResults { get; set; }
    }
}
