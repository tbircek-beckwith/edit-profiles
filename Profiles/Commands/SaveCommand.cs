using System.Windows;

namespace EditProfiles.Commands
{
    /// <summary>
    /// Save Command.
    /// </summary>
    public class SaveDelegateCommand 
    {

        private ViewModel viewModel;

        /// <summary>
        /// Generic command that can be used as any command.
        /// </summary>
        public DefaultCommand Command { get; private set; }

        /// <summary>
        /// Returns a viewmodel and 
        /// creates a handle for Property Changed events.
        /// </summary>
        public ViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                if ( viewModel != value )
                {
                    if ( viewModel != null )
                    {
                        viewModel.PropertyChanged -= this.OnViewModelPropertyChanged;
                    }
                    viewModel = value;
                    viewModel.PropertyChanged += this.OnViewModelPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Creates a new save command to save log to the file.
        /// </summary>
        public SaveDelegateCommand ( )
        {
            this.Command = new DefaultCommand ( this.ExecuteSave, this.CanSave );
        }

        /// <summary>
        /// This will change for now this code ok.
        /// </summary>
        /// <param name="unused"></param>
        public void ExecuteSave ( object unused )
        {
            MessageBox.Show ( "Save Done. Please modify this code to write log file to the host computer." );
        }

        /// <summary>
        /// Verify we actually modify files by checking DetailsRichTextBoxText.
        /// </summary>
        /// <param name="unused">not used.</param>
        /// <returns>If there is something to save True otherwise False.</returns>
        public bool CanSave ( object unused )
        {
            return string.IsNullOrEmpty ( this.ViewModel.DetailsTextBoxText );
        }

        private void OnViewModelPropertyChanged ( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            this.Command.RaiseCanExecuteChanged ( );
        }
    }
}
