using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EditProfiles.Properties;
using System.Windows.Input;
using System.Globalization;
using System.Diagnostics;

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
                return this.model.FindWhatTextBoxText;
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
                return this.model.ReplaceWithTextBoxText;
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
        /// Sets and gets OverviewRichTextBoxText.
        /// </summary>
        public string OverviewTextBoxText
        {
            get
            {
                return this.model.OverviewTextBoxText;
            }
            set
            {
                if ( this.model.OverviewTextBoxText != value )
                {
                    this.model.OverviewTextBoxText = value;

                    this.OnPropertyChanged ( "OverviewTextBoxText" );
                }
            }
        }

        /// <summary>
        /// Sets and gets ProcessFiles.
        /// </summary>
        public string ProcessFiles
        {
            get
            {
                return MyResources.Strings_ProcessFiles;
            }
            set
            {
                if ( this.model.ProcessFiles != value )
                {
                    this.model.ProcessFiles = value;

                    this.OnPropertyChanged ( "ProcessFiles" );
                }
            }
        }

        /// <summary>
        /// Sets and gets FindReplaceButtonEnabled.
        /// </summary>
        public bool FindReplaceButtonEnabled
        {
            get
            {
                return this.model.FindReplaceButtonEnabled;
            }
            set
            {
                if ( this.model.FindReplaceButtonEnabled != value )
                {
                    this.model.FindReplaceButtonEnabled = value;

                    this.OnPropertyChanged ( "FindReplaceButtonEnabled" );
                }
            }
        }

        #endregion

        #region Public Commands

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
        public ViewModel ( Model model, ICommand saveCommand, ICommand eraseCommand, ICommand findReplaceCommand )
        {
            this.model = model;
            this.SaveCommand = saveCommand;
            this.EraseCommand = eraseCommand;
            this.FindReplaceCommand = findReplaceCommand;
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
                Debug.WriteLine ( "Invalid property" );
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

                //error = ( model as IDataErrorInfo )[ propertyName ];

                this.FindReplaceButtonEnabled = !( string.IsNullOrWhiteSpace ( FindWhatTextBoxText ) );

                // CommandManager.InvalidateRequerySuggested ( );

                return error;
            }
        }

        #endregion
    }
}
