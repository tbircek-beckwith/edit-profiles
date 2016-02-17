using System;
using EditProfiles.Commands;
using EditProfiles.Data;
using EditProfiles.MainModel;
using System.Threading;
using System.Text;

// FxCop warning: CA1014 : Microsoft.Design : Mark 'EditProfiles.exe' with CLSCompliant(true) because it exposes externally visible types.
[assembly: CLSCompliant ( true )]
namespace EditProfiles.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewFactory
    {
        private EraseDelegateCommand eraseDelegateCommand;

        private SaveDelegateCommand saveDelegateCommand;

        private FindReplaceCommand findReplaceCommand;

        private UpdateCommand updateCommand;

        private StopCommand stopCommand;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ViewInfrastructure Create ( )
        {
            // Initialize the commands.
            this.eraseDelegateCommand = new EraseDelegateCommand ( );
            this.saveDelegateCommand = new SaveDelegateCommand ( );
            this.findReplaceCommand = new FindReplaceCommand ( );
            this.updateCommand = new UpdateCommand ( );
            this.stopCommand = new StopCommand ( );

            // Initialize Common Properties.
            MyCommons.LogProcess = new StringBuilder ( );

            // Initilaize new Model.
            Model model = new Model ( );

            // Initialize new ViewModel.
            ViewModel viewModel = new ViewModel ( model,
                                                  this.saveDelegateCommand.Command,
                                                  this.eraseDelegateCommand.Command,
                                                  this.findReplaceCommand.Command,
                                                  this.updateCommand.Command,
                                                  this.stopCommand.Command );

            // Initialize mainview.
            EditProfiles.MainWindow view = new EditProfiles.MainWindow ( );

            // Here setting Commands ViewModel so the commands can access to the viewModel,
            // otherwise ViewModel would be null and act weirdly.
            // Will use one viewModel and share for all commands.
            MyCommons.MyViewModel = viewModel;

            return new ViewInfrastructure ( view, viewModel, model );
        }
    }
}
