using System.Globalization;
using EditProfiles.Data;
using EditProfiles.Properties;
using System;
using System.Diagnostics;

namespace EditProfiles.Commands
{
    /// <summary>
    /// Updates certain controls text and values.
    /// </summary>
    public class UpdateCommand
    {
        /// <summary>
        /// Default command
        /// </summary>
        public DefaultCommand Command { get; set; }

        /// <summary>
        /// Update progress bar and strings attached to them.
        /// </summary>
        public UpdateCommand ( )
        {
            this.Command = new DefaultCommand ( this.ExecuteUpdate, this.CanUpdate );
        }

        /// <summary>
        /// Update progress bar and strings attached to them.
        /// </summary>
        /// <param name="unused"></param>
        public void ExecuteUpdate ( object unused )
        {

            MyCommons.MyViewModel.FileProgressBar =
                                                    string.Format ( CultureInfo.InvariantCulture,
                                                    MyResources.Strings_FileProcessBar,
                                                    MyCommons.CurrentFileNumber,
                                                    MyCommons.TotalFileNumber );

            MyCommons.MyViewModel.ModuleProgressBar =
                                                    string.Format ( CultureInfo.InvariantCulture,
                                                    MyResources.Strings_ModuleProcessBar,
                                                    MyCommons.CurrentModuleNumber,
                                                    MyCommons.TotalModuleNumber );

            MyCommons.MyViewModel.FileProgressBarValue = MyCommons.CurrentFileNumber;

            MyCommons.MyViewModel.ModuleProgressBarValue = MyCommons.CurrentModuleNumber;

            Debug.WriteLine (" Update Command ran. ");
        }

        /// <summary>
        /// Provide logic to run this command.
        /// </summary>
        /// <param name="unused"></param>
        /// <returns>Always true at the moment. Future use</returns>
        public bool CanUpdate ( object unused )
        {
            return true;
        }
    }
}

