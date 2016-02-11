using System;
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
        private readonly Model model;

        #region Public Properties

        /// <summary>
        /// Sets and gets ProgramTitle.
        /// </summary>
        public string ProgramTitle
        {
            get
            {
                return string.Format ( CultureInfo.InvariantCulture,
                                     MyResources.Strings_FormTitle,
                                     typeof ( EditProfiles.MainWindow )
                                        .Assembly
                                        .GetName ( )
                                        .Version );
            }
            set
            {
                if ( this.model.ProgramTitle != value )
                {
                    this.model.ProgramTitle = value;

                    this.OnPropertyChanged ( "ProgramTitle" );
                }
            }
        }

        /// <summary>
        /// Sets and gets FindWhatLabelText.
        /// </summary>
        public string FindWhatLabelText
        {
            get
            {
                return MyResources.Strings_LabelFind;
            }
            set
            {
                if ( this.model.FindWhatLabelText != value )
                {
                    this.model.FindWhatLabelText = value;

                    this.OnPropertyChanged ( "FindWhatLabelText" );
                }
            }
        }

        /// <summary>
        /// Sets and gets FindWhatTextBoxText.
        /// </summary>
        public string FindWhatTextBoxText
        {
            get
            {
#if DEBUG
                return MyResources.Strings_Debug_TextBoxFind;
#else
                // Return empty string if the user not specified any values  
                return this.model.FindWhatTextBoxText ?? string.Empty;  // MyResources.Strings_DefaultTextBoxValues;
#endif
            }
            set
            {
                if ( this.model.FindWhatTextBoxText != value )
                {
                    this.model.FindWhatTextBoxText = value;

                    this.OnPropertyChanged ( "FindWhatTextBoxText" );
                }
            }
        }

        /// <summary>
        /// Sets and gets ReplaceWithLabelText.
        /// </summary>
        public string ReplaceWithLabelText
        {
            get
            {
                return MyResources.Strings_LabelReplace;
            }
            set
            {
                if ( this.model.ReplaceWithLabelText != value )
                {
                    this.model.ReplaceWithLabelText = value;

                    this.OnPropertyChanged ( "ReplaceWithLabelText" );
                }
            }
        }

        /// <summary>
        /// Sets and gets ReplaceWithTextBoxText.
        /// </summary>
        public string ReplaceWithTextBoxText
        {
            get
            {
#if DEBUG
                return MyResources.Strings_Debug_TextBoxReplace;
#else
                // Return empty string if the user not specified any values                
                return this.model.ReplaceWithTextBoxText ?? string.Empty;  // MyResources.Strings_DefaultTextBoxValues;
#endif
            }
            set
            {
                if ( this.model.ReplaceWithTextBoxText != value )
                {
                    this.model.ReplaceWithTextBoxText = value;

                    this.OnPropertyChanged ( "ReplaceWithTextBoxText" );
                }
            }
        }

        /// <summary>
        /// Sets and gets PasswordLabelText.
        /// </summary>
        public string PasswordLabelText
        {
            get
            {
                return MyResources.Strings_LabelPassword;
            }
            set
            {
                if ( this.model.PasswordLabelText != value )
                {
                    this.model.PasswordLabelText = value;

                    this.OnPropertyChanged ( "PasswordLabelText" );
                }
            }
        }

        /// <summary>
        /// Sets and gets PasswordTextBoxText.
        /// </summary>
        public string PasswordTextBoxText
        {
            get
            {
                return this.model.PasswordTextBoxText;
            }
            set
            {
                if ( this.model.PasswordTextBoxText != value )
                {
                    this.model.PasswordTextBoxText = value;

                    this.OnPropertyChanged ( "PasswordTextBoxText" );
                }
            }
        }

        /// <summary>
        /// Sets and gets FindReplaceButtonText.
        /// </summary>
        public string FindReplaceButtonText
        {
            get
            {
                return MyResources.Strings_ButtonFindReplace;
            }
            set
            {
                if ( this.model.FindReplaceButtonText != value )
                {
                    this.model.FindReplaceButtonText = value;

                    this.OnPropertyChanged ( "FindReplaceButtonText" );
                }
            }
        }

        /// <summary>
        /// Sets and gets DetailsRichTextBoxText.
        /// </summary>
        public string DetailsTextBoxText
        {
            get
            {
                return this.model.DetailsTextBoxText;
            }
            set
            {
                if ( this.model.DetailsTextBoxText != value )
                {
                    MyCommons.LogProcess.Append ( value );

                    this.model.DetailsTextBoxText += value;
                                       
                    this.OnPropertyChanged ( "DetailsTextBoxText" );
                }
            }
        }
        
        /// <summary>
        /// Sets and gets FileProgressBar.
        /// </summary>
        public string FileProgressBar
        {
            get
            {
                return string.Format ( CultureInfo.InvariantCulture,
                                       MyResources.Strings_FileProcessBar,
                                       MyCommons.CurrentFileNumber,
                                       MyCommons.TotalFileNumber );
            }
            set
            {
                if ( this.model.FileProgressBar != value )
                {
                    this.model.FileProgressBar = value;

                    this.OnPropertyChanged ( "FileProgressBar" );
                }
            }
        }

        /// <summary>
        /// Sets and gets ModuleProgressBar.
        /// </summary>
        public string ModuleProgressBar
        {
            get
            {
                return string.Format ( CultureInfo.InvariantCulture,
                                       MyResources.Strings_ModuleProcessBar,
                                       MyCommons.CurrentModuleNumber,
                                       MyCommons.TotalModuleNumber );
            }
            set
            {
                if ( this.model.ModuleProgressBar != value )
                {
                    this.model.ModuleProgressBar = value;

                    this.OnPropertyChanged ( "ModuleProgressBar" );
                }
            }
        }

        /// <summary>
        /// Sets and get ModuleProgressBarValue
        /// </summary>
        public int ModuleProgressBarValue
        {
            get
            {
                return this.model.ModuleProgressBarValue;
            }
            set
            {
                if ( this.model.ModuleProgressBarValue != value )
                {
                    this.model.ModuleProgressBarValue = value;

                    this.OnPropertyChanged ( "ModuleProgressBarValue" );
                }
            }
        }

        /// <summary>
        /// File ProgressBar Max Value
        /// </summary>
        public int ModuleProgressBarMax
        {
            get
            {
                return this.model.ModuleProgressBarMax;
            }
            set
            {
                if ( this.model.ModuleProgressBarMax != value )
                {
                    this.model.ModuleProgressBarMax = value;
                    this.OnPropertyChanged ( "ModuleProgressBarMax" );
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
                if ( this.model.FileProgressBarValue != value )
                {
                    this.model.FileProgressBarValue = value;

                    this.OnPropertyChanged ( "FileProgressBarValue" );
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
                if ( this.model.FileProgressBarMax != value )
                {
                    this.model.FileProgressBarMax = value;
                    this.OnPropertyChanged ( "FileProgressBarMax" );
                }
            }
        }

        private SecureString _password;

        /// <summary>
        /// Password box string.
        /// </summary>
        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                if ( _password != value )
                {
                    _password = value;
                    this.OnPropertyChanged ( "Password" );
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
        /// <param name="updateCommand">This command handles updating process bars and their texts</param>
        public ViewModel ( Model model,
                           ICommand saveCommand,
                           ICommand eraseCommand,
                           ICommand findReplaceCommand,
                           ICommand updateCommand )
        {
            this.model = model;
            this.SaveCommand = saveCommand;
            this.EraseCommand = eraseCommand;
            this.FindReplaceCommand = findReplaceCommand;
            this.UpdateCommand = updateCommand;
        }

        /// <summary>
        /// PropertyChange event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles the property changes.
        /// </summary>
        /// <param name="propertyName">Property name changed.</param>
        protected virtual void OnPropertyChanged ( string propertyName )
        {

#if DEBUG
            Console.WriteLine ( "OnPropertyChanged ( propertyName: {0} ) just processed.", propertyName );
#endif

            this.VerifyPropertyName ( propertyName );

            var handler = PropertyChanged;

            if ( handler != null )
            {
                handler ( this, new PropertyChangedEventArgs ( propertyName ) );
            }
        }

        #endregion

        #region Debugging Aides

        /// <summary>
        /// Throws a 'InvalidProperty' exception.
        /// It will not exists in 'Release' version.
        /// Allows me see if a property named incorrectly.
        /// </summary>
        /// <param name="propertyName">Property name to verify.</param>
        [Conditional ( "DEBUG" )]
        [DebuggerStepThrough]
        public void VerifyPropertyName ( string propertyName )
        {
            if ( TypeDescriptor.GetProperties ( this )[ propertyName ] == null )
            {
                Debug.WriteLine ( string.Format ( CultureInfo.InvariantCulture, "Invalid property: {0}", propertyName ) );
            }
        }

        #endregion

        #region IDataErrorInfo Members

        /// <summary>
        /// provides an Error string.
        /// </summary>
        public string Error
        {
            get { return ( model as IDataErrorInfo ).Error; }
        }

        /// <summary>
        /// Provides actual error messages. 
        /// NOT IN USE at the moment.
        /// </summary>
        /// <param name="columnName">Property name. FxCop very particular to keep this name here.</param>
        /// <returns>Returns a custom string message for property changes.</returns>
        public string this[ string columnName ]
        {
            get
            {
                string error = string.Empty;

                error = ( model as IDataErrorInfo )[ columnName ];

                // this.FindReplaceButtonEnabled = !( string.IsNullOrWhiteSpace ( FindWhatTextBoxText ) );

                CommandManager.InvalidateRequerySuggested ( );

                return error;
            }
        }

        #endregion
    }
}
