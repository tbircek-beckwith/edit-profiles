using EditProfiles.Data;

namespace EditProfiles.Commands
{
    /// <summary>
    /// NOT IN USE.
    /// </summary>
    public class EraseDelegateCommand
    {
        /// <summary>
        /// Default command.
        /// </summary>
        public DefaultCommand Command { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EraseDelegateCommand ( )
        {
            this.Command = new DefaultCommand ( this.ExecuteErase, this.CanErase );
        }

        /// <summary>
        /// Clear all previous the user inputs.
        /// </summary>
        /// <param name="unused"></param>
        public void ExecuteErase ( object unused )
        {
            MyCommons.MyViewModel.FindWhatTextBoxText = string.Empty;
            MyCommons.MyViewModel.ReplaceWithTextBoxText = string.Empty;
            MyCommons.MyViewModel.PasswordTextBoxText = string.Empty;
            MyCommons.MyViewModel.DetailsTextBoxText = string.Empty;
           // MyCommons.MyViewModel.OverviewTextBoxText = string.Empty;
            // MyCommons.MyViewModel.FindReplaceButtonEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unused"></param>
        /// <returns></returns>
        public bool CanErase ( object unused )
        {
            return true;
        }
    }
}
