namespace EditProfiles.MainModel
{
    /// <summary>
    /// Contains data, not behavior.
    /// This class only contains properties.
    /// </summary>
    public class Model
    {

        #region Program

        /// <summary>
        /// ProgramTitle
        /// </summary>
        public string ProgramTitle { get; set; }

        /// <summary>
        /// Prevents the user interaction while test program is running.
        /// </summary>
        public bool Editable { get; set; }

        #endregion

        #region FindWhat Label & TextBox

        /// <summary>
        /// FindWhatLabelText
        /// </summary>
        public string FindWhatLabelText { get; set; }

        /// <summary>
        /// FindWhatTextBoxText
        /// </summary>
        public string FindWhatTextBoxText { get; set; }

        /// <summary>
        /// Holds alternate register values to find.
        /// <para>This is special case where original test file has only
        /// Profile 1 values and the application is generating other profiles.</para>
        /// </summary>
        public string FindOriginalTestValues { get; set; }
        #endregion

        #region ReplaceWith Label & Text

        /// <summary>
        /// ReplaceWithLabelText
        /// </summary>
        public string ReplaceWithLabelText { get; set; }

        /// <summary>
        /// ReplaceWithTextBoxText
        /// </summary>
        public string ReplaceWithTextBoxText { get; set; }

        #endregion

        #region Password

        /// <summary>
        /// PasswordLabelText
        /// </summary>
        public string PasswordLabelText { get; set; }

        /// <summary>
        /// Holds actual password.
        /// </summary>
        public System.Security.SecureString Password { get; set; }

        #endregion

        #region FindReplace ToggleButton

        /// <summary>
        /// FindReplaceButtonText
        /// </summary>
        public string FindReplaceButtonText { get; set; }

        /// <summary>
        /// StopTestButtonText
        /// </summary>
        public string StopTestButtonText { get; set; }

        /// <summary>
        /// Status of the toggle button.
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// availability of the toggle button 
        /// </summary>
        public bool IsEnabled { get; set; } 

        #endregion

        #region Details Textbox

        /// <summary>
        /// DetailsRichTextBoxText
        /// </summary>
        public string DetailsTextBoxText { get; set; }

        #endregion

        #region File ProgressBar

        /// <summary>
        /// OverviewRichTextBoxText
        /// </summary>
        public string FileProgressBar { get; set; }

        /// <summary>
        /// File ProgressBar Value
        /// </summary>
        public int FileProgressBarValue { get; set; }

        /// <summary>
        /// File ProgressBar Max Value
        /// </summary>
        public int FileProgressBarMax { get; set; }

        /// <summary>
        /// Set opacity of FileProgressBar.
        /// </summary>
        public double FileProgressBarOpacity { get; set; }

        /// <summary>
        /// Set opacity of textbox cover of the FileProgressBarOpacity.
        /// </summary>
        public double FileSideTextOpacity { get; set; }

        /// <summary>
        /// Modify test to show start and end messages.
        /// </summary>
        public string FileSideCoverText { get; set; }

        #endregion

        #region Module ProgressBar

        /// <summary>
        /// Sets and gets ModuleProcessBar.
        /// </summary>
        public string ModuleProgressBar { get; set; }

        /// <summary>
        /// Module ProgressBar Value
        /// </summary>
        public long ModuleProgressBarValue { get; set; }

        /// <summary>
        /// Module ProgressBar Max Value
        /// </summary>
        public int ModuleProgressBarMax { get; set; }

        /// <summary>
        /// Set opacity of ModuleProgressBar.
        /// </summary>
        public double ModuleProgressBarOpacity { get; set; }

        /// <summary>
        /// Set opacity of textbox cover of the ModuleProgressBarOpacity. 
        /// </summary> 
        public double ModuleSideTextOpacity { get; set; }

        /// <summary>
        /// Modify test to show start and end messages.
        /// </summary> 
        public string ModuleSideCoverText { get; set; }

        #endregion 

    }
}
