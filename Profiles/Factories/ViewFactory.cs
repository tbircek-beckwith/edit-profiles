using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditProfiles;

// FxCop warning: CA1014 : Microsoft.Design : Mark 'EditProfiles.exe' with CLSCompliant(true) because it exposes externally visible types.
[assembly:CLSCompliant(true)]
namespace EditProfiles
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewFactory
    {
        private EraseDelegateCommand eraseDelegateCommand;

        private SaveDelegateCommand saveDelegateCommand;

        private FindReplaceDelegateCommand findReplaceCommand;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ViewInfrastructure Create ( )
        {
            // Initialize the commands.
            this.eraseDelegateCommand = new EraseDelegateCommand ( );
            this.saveDelegateCommand = new SaveDelegateCommand ( );
            this.findReplaceCommand = new FindReplaceDelegateCommand ( );

            // Initilaize new Model.
            Model model = new Model ( );

            // Initialize new ViewModel.
            ViewModel viewModel = new ViewModel ( model, this.saveDelegateCommand.Command, this.eraseDelegateCommand.Command, this.findReplaceCommand.Command );

            // Ini
            EditProfiles.MainWindow view = new EditProfiles.MainWindow ( );

            // Here setting Commands ViewModel so the commands can access to the viewModel,
            // otherwise ViewModel would be null and act weirdly.
            eraseDelegateCommand.ViewModel = viewModel;
            saveDelegateCommand.ViewModel = viewModel;
            findReplaceCommand.ViewModel = viewModel;
            
            return new ViewInfrastructure ( view, viewModel, model );
        }
    }
}
