using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditProfiles
{
    /// <summary>
    /// 
    /// </summary>
    public class EraseDelegateCommand
    {
        /// <summary>
        /// 
        /// </summary>
        public DelegateCommand Command { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ViewModel ViewModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EraseDelegateCommand ( )
        {
            this.Command = new DelegateCommand ( this.ExecuteErase, this.CanErase );
        }

        /// <summary>
        /// Clear all previous the user inputs.
        /// </summary>
        /// <param name="unused"></param>
        public void ExecuteErase ( object unused )
        {
            this.ViewModel.FindWhatTextBoxText = string.Empty;
            this.ViewModel.ReplaceWithTextBoxText = string.Empty;
            this.ViewModel.PasswordTextBoxText = string.Empty;
            this.ViewModel.DetailsTextBoxText = string.Empty;
            this.ViewModel.OverviewTextBoxText = string.Empty;
            this.ViewModel.FindReplaceButtonEnabled = false;
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
