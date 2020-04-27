using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Windows.Input;
using EditProfiles.Data;
using EditProfiles.MainModel;
using EditProfiles.Properties;

namespace EditProfiles
{
    /// <summary>
    /// The ViewModel has to be view agnostic,
    /// so no View concepts/types should appear on it.
    /// Handles all the commands and validations.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Private Variables

        private readonly Model model;

        private const double OPACITY_MAX = 100.0;

        private const double OPACITY_MIN = 0.0;

        private const double PROGRESSBAR_MIN = 0.0;

        private readonly object lockObject = new object();

        #endregion

        #region Program

        /// <summary>
        /// Sets and gets ProgramTitle.
        /// </summary>
        public string ProgramTitle
        {
            get => string.Format(CultureInfo.InvariantCulture,
                                     MyResources.Strings_FormTitle,
                                     typeof(MainWindow)
                                        .Assembly
                                        .GetName()
                                        .Version.ToString(3));
            set
            {
                if (model.ProgramTitle != value)
                {
                    model.ProgramTitle = value;

                    OnPropertyChanged("ProgramTitle");
                }
            }
        }

        /// <summary>
        /// Prevents the user interaction while test program is running.
        /// </summary>
        public bool Editable
        {
            get => model.Editable;
            set
            {
                if (model.Editable != value)
                {
                    model.Editable = value;

                    OnPropertyChanged("Editable");
                }
            }
        }

        #endregion

        #region FindWhat Label & Textbox

        /// <summary>
        /// Sets and gets FindWhatLabelText.
        /// </summary>
        public string FindWhatLabelText
        {
            get => MyResources.Strings_LabelFind;
            set
            {
                if (model.FindWhatLabelText != value)
                {
                    model.FindWhatLabelText = value;

                    OnPropertyChanged("FindWhatLabelText");
                }
            }
        }

        /// <summary>
        /// Sets and gets FindWhatTextBoxText.
        /// </summary>
        public string FindWhatTextBoxText
        {

            // Return empty string if the user not specified any values  
            get => model.FindWhatTextBoxText ?? string.Empty;
            set
            {
                if (model.FindWhatTextBoxText != value)
                {
                    model.FindWhatTextBoxText = value;

                    OnPropertyChanged("FindWhatTextBoxText");
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        #endregion

        #region ReplaceWith Label & Text

        /// <summary>
        /// Sets and gets ReplaceWithLabelText.
        /// </summary>
        public string ReplaceWithLabelText
        {
            get => MyResources.Strings_LabelReplace;
            set
            {
                if (model.ReplaceWithLabelText != value)
                {
                    model.ReplaceWithLabelText = value;

                    OnPropertyChanged("ReplaceWithLabelText");
                }
            }
        }

        /// <summary>
        /// Sets and gets ReplaceWithTextBoxText.
        /// </summary>
        public string ReplaceWithTextBoxText
        {

            // Return empty string if the user not specified any values   
            get => model.ReplaceWithTextBoxText ?? string.Empty;
            set
            {
                if (model.ReplaceWithTextBoxText != value)
                {
                    model.ReplaceWithTextBoxText = value;

                    OnPropertyChanged("ReplaceWithTextBoxText");
                }
            }
        }

        #endregion

        #region Password

        /// <summary>
        /// Sets and gets PasswordLabelText.
        /// </summary>
        public string PasswordLabelText
        {
            get => MyResources.Strings_LabelPassword;
            set
            {
                if (model.PasswordLabelText != value)
                {
                    model.PasswordLabelText = value;

                    OnPropertyChanged("PasswordLabelText");
                }
            }
        }

        private SecureString _password;

        /// <summary>
        /// Password box string.
        /// </summary>
        public SecureString Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        #endregion

        #region FindReplace ToggleButton

        /// <summary>
        /// Sets and gets FindReplaceButtonText.
        /// </summary>
        public string FindReplaceButtonText
        {
            get => MyResources.Strings_ButtonFindReplace;
            set
            {
                if (model.FindReplaceButtonText != value)
                {
                    model.FindReplaceButtonText = value;

                    OnPropertyChanged("FindReplaceButtonText");
                }
            }
        }

        /// <summary>
        /// Sets and gets FindReplaceButtonText.
        /// </summary>
        public string StopTestButtonText
        {
            get => MyResources.Strings_ButtonStopTest;
            set
            {
                if (model.StopTestButtonText != value)
                {
                    model.StopTestButtonText = value;

                    OnPropertyChanged("StopTestButtonText");
                }
            }
        }

        /// <summary>
        /// Sets or gets ToggleButton Checked status.
        /// </summary>
        public bool IsChecked
        {
            get => model.IsChecked;
            set
            {
                // will prevent if the toggle button is not enabled.
                // without this if statement there is nothing would stop toggling this button.
                //if (this.model.IsEnabled)
                //{
                if (model.IsChecked != value)
                {
                    model.IsChecked = value;

                    OnPropertyChanged("IsChecked");
                }
                //}
            }
        }

        /// <summary>
        /// Sets or gets ToggleButton Enabled status.
        /// </summary>
        public bool IsEnabled
        {
            // it is enabled while testing
            get => true;
            //{
            //    return this.model.IsEnabled = !(String.IsNullOrWhiteSpace(MyCommons.MyViewModel.FindWhatTextBoxText));
            //}
            set
            {
                //if (this.model.IsEnabled != value)
                //{
                model.IsEnabled = value;

                OnPropertyChanged("IsEnabled");
                //}
            }
        }
        #endregion

        #region Details Textbox

        /// <summary>
        /// Sets and gets DetailsRichTextBoxText.
        /// </summary>
        public string DetailsTextBoxText
        {
            get
            {
                lock (lockObject)
                {
                    return MyCommons.LogProcess.ToString();
                }
            }
            set
            {
                if (MyCommons.LogProcess.ToString() == value)
                {

                    // This text set while MyCommons.LogProcess updated.
                    OnPropertyChanged("DetailsTextBoxText");
                }
            }
        }

        #endregion

        #region File ProgressBar

        /// <summary>
        /// Sets and gets FileProgressBar.
        /// </summary>
        public string FileProgressBar
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                                       MyResources.Strings_FileProcessBar,
                                       MyCommons.CurrentFileNumber,
                                       MyCommons.TotalFileNumber);
            }
            set
            {
                if (this.model.FileProgressBar != value)
                {
                    this.model.FileProgressBar = value;

                    this.OnPropertyChanged("FileProgressBar");
                }
            }
        }

        /// <summary>
        /// Sets and get FileProgressBarValue
        /// </summary>
        public int FileProgressBarValue
        {
            get
            {
                return this.model.FileProgressBarValue;
            }
            set
            {
                if (this.model.FileProgressBarValue != value)
                {
                    this.model.FileProgressBarValue = value;

                    this.OnPropertyChanged("FileProgressBarValue");
                }
            }
        }

        /// <summary>
        /// File ProgressBar Max Value
        /// </summary>
        public int FileProgressBarMax
        {
            get
            {
                return this.model.FileProgressBarMax;
            }
            set
            {
                if (this.model.FileProgressBarMax != value)
                {
                    if (value == PROGRESSBAR_MIN)
                    {
                        this.FileSideTextOpacity = OPACITY_MAX;
                        this.FileProgressBarOpacity = OPACITY_MIN;
                    }
                    else
                    {
                        this.FileSideTextOpacity = OPACITY_MIN;
                        this.FileProgressBarOpacity = OPACITY_MAX;
                    }

                    this.model.FileProgressBarMax = value;
                    this.OnPropertyChanged("FileProgressBarMax");
                }
            }
        }

        /// <summary>
        /// Show progress bars and texts.
        /// </summary>
        public double FileProgressBarOpacity
        {
            get
            {
                if (FileProgressBarMax != PROGRESSBAR_MIN)
                {
                    this.FileSideTextOpacity = OPACITY_MIN;
                    return this.model.FileProgressBarOpacity;
                }
                else
                {
                    this.FileSideTextOpacity = OPACITY_MAX;
                    return FileProgressBarMax;
                }
            }
            set
            {
                if (this.model.FileProgressBarOpacity != value)
                {
                    this.model.FileProgressBarOpacity = value;
                    this.OnPropertyChanged("FileProgressBarOpacity");
                }
            }
        }

        /// <summary>
        /// Shows textbox.
        /// </summary>
        public double FileSideTextOpacity
        {
            get
            {
                if (FileProgressBarMax != PROGRESSBAR_MIN)
                {
                    this.FileProgressBarOpacity = OPACITY_MAX;
                    return this.model.FileSideTextOpacity;
                }
                else
                {
                    this.FileProgressBarOpacity = OPACITY_MIN;
                    return OPACITY_MAX;
                }
            }
            set
            {
                if (this.model.FileSideTextOpacity != value)
                {
                    this.model.FileSideTextOpacity = value;
                    this.OnPropertyChanged("FileSideTextOpacity");
                }
            }
        }

        /// <summary>
        /// Modify test to show start and end messages.
        /// </summary>
        public string FileSideCoverText
        {
            get
            {
                return this.model.FileSideCoverText;
            }
            set
            {
                if (this.model.FileSideCoverText != value)
                {
                    this.model.FileSideCoverText = value;
                    this.OnPropertyChanged("FileSideCoverText");
                }
            }
        }

        #endregion

        #region Module ProgressBar

        /// <summary>
        /// Sets and gets ModuleProgressBar.
        /// </summary>
        public string ModuleProgressBar
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                                       MyResources.Strings_ModuleProcessBar,
                                       MyCommons.CurrentModuleNumber,
                                       MyCommons.TotalModuleNumber);
            }
            set
            {
                if (this.model.ModuleProgressBar != value)
                {
                    this.model.ModuleProgressBar = value;

                    this.OnPropertyChanged("ModuleProgressBar");
                }
            }
        }

        /// <summary>
        /// Sets and get ModuleProgressBarValue
        /// </summary>
        public long ModuleProgressBarValue
        {
            get
            {
                return this.model.ModuleProgressBarValue;
            }
            set
            {
                if (this.model.ModuleProgressBarValue != value)
                {
                    this.model.ModuleProgressBarValue = value;

                    this.OnPropertyChanged("ModuleProgressBarValue");
                }
            }
        }

        /// <summary>
        /// Module ProgressBar Max Value
        /// </summary>
        public int ModuleProgressBarMax
        {
            get
            {
                return this.model.ModuleProgressBarMax;
            }
            set
            {
                if (this.model.ModuleProgressBarMax != value)
                {
                    if (value == PROGRESSBAR_MIN)
                    {
                        this.ModuleSideTextOpacity = OPACITY_MAX;
                        this.ModuleProgressBarOpacity = OPACITY_MIN;
                    }
                    else
                    {
                        this.ModuleSideTextOpacity = OPACITY_MIN;
                        this.ModuleProgressBarOpacity = OPACITY_MAX;
                    }
                    this.model.ModuleProgressBarMax = value;
                    this.OnPropertyChanged("ModuleProgressBarMax");
                }
            }
        }

        /// <summary>
        /// Show progress bars and texts.
        /// </summary>
        public double ModuleProgressBarOpacity
        {
            get
            {
                if (ModuleProgressBarMax != PROGRESSBAR_MIN)
                {
                    this.ModuleSideTextOpacity = OPACITY_MIN;
                    return this.model.ModuleProgressBarOpacity;
                }
                else
                {
                    this.ModuleSideTextOpacity = OPACITY_MAX;
                    return ModuleProgressBarMax;
                }
            }
            set
            {
                if (this.model.ModuleProgressBarOpacity != value)
                {
                    this.model.ModuleProgressBarOpacity = value;
                    this.OnPropertyChanged("ModuleProgressBarOpacity");
                }
            }
        }

        /// <summary>
        /// Shows textbox.
        /// </summary>
        public double ModuleSideTextOpacity
        {
            get
            {
                if (ModuleProgressBarMax != PROGRESSBAR_MIN)
                {
                    this.ModuleProgressBarOpacity = OPACITY_MAX;
                    return this.model.ModuleSideTextOpacity;
                }
                else
                {
                    this.ModuleProgressBarOpacity = OPACITY_MIN;
                    return OPACITY_MAX;
                }
            }
            set
            {
                if (this.model.ModuleSideTextOpacity != value)
                {
                    this.model.ModuleSideTextOpacity = value;
                    this.OnPropertyChanged("ModuleSideTextOpacity");
                }
            }
        }

        /// <summary>
        /// Modify test to show start and end messages.
        /// </summary>
        public string ModuleSideCoverText
        {
            get
            {
                return this.model.ModuleSideCoverText;
            }
            set
            {
                if (this.model.ModuleSideCoverText != value)
                {
                    this.model.ModuleSideCoverText = value;
                    this.OnPropertyChanged("ModuleSideCoverText");
                }
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// Handles FindReplace button clicks.
        /// </summary>
        public ICommand UpdateCommand { get; private set; }

        /// <summary>
        /// Handles FindReplace button clicks.
        /// </summary>
        public ICommand FindReplaceCommand { get; private set; }

        /// <summary>
        /// Handles Stop button clicks.
        /// </summary>
        public ICommand StopCommand { get; private set; }

        /// <summary>
        /// NOT IN USE at the moment.
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// NOT IN USE at the moment.
        /// </summary>
        public ICommand EraseCommand { get; private set; }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Provides behind the code logic.
        /// This is where everything actual run.
        /// </summary>
        /// <param name="model">The current model like to manipulate.</param>
        /// <param name="saveCommand">NOT IN USE at the moment.</param>
        /// <param name="eraseCommand">NOT IN USE at the moment.</param>
        /// <param name="findReplaceCommand">This command handles FindReplace Button Clicks.</param>
        /// <param name="updateCommand">This command handles updating process bars and their texts.</param>
        /// <param name="stopCommand">This command handles stopping the program.</param>
        public ViewModel(Model model,
                           ICommand saveCommand,
                           ICommand eraseCommand,
                           ICommand findReplaceCommand,
                           ICommand updateCommand,
                           ICommand stopCommand)
        {
            this.model = model;
            SaveCommand = saveCommand;
            EraseCommand = eraseCommand;
            FindReplaceCommand = findReplaceCommand;
            UpdateCommand = updateCommand;
            StopCommand = stopCommand;
        }

        /// <summary>
        /// PropertyChange event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles the property changes.
        /// </summary>
        /// <param name="propertyName">Property name changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {

            Debug.WriteLine(string.Format("OnPropertyChanged ( propertyName: {0} ) just processed.", propertyName));

            VerifyPropertyName(propertyName);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Debugging Aides

        /// <summary>
        /// Throws a 'InvalidProperty' exception.
        /// It will not exists in 'Release' version.
        /// Allows me see if a property named incorrectly.
        /// </summary>
        /// <param name="propertyName">Property name to verify.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Invalid property: {0}", propertyName));
            }
        }

        #endregion

        #region IDataErrorInfo Members

        /// <summary>
        /// provides an Error string.
        /// </summary>
        public string Error => (model as IDataErrorInfo).Error;

        /// <summary>
        /// Provides actual error messages. 
        /// NOT IN USE at the moment.
        /// </summary>
        /// <param name="columnName">Property name. FxCop very particular to keep this name here.</param>
        /// <returns>Returns a custom string message for property changes.</returns>
        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                error = (model as IDataErrorInfo)[columnName];

                // this.FindReplaceButtonEnabled = !( string.IsNullOrWhiteSpace ( FindWhatTextBoxText ) );

                CommandManager.InvalidateRequerySuggested();

                return error;
            }
        }

        #endregion

    }
}
