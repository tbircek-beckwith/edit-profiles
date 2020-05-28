using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditProfiles.Data;
using System.Windows;

namespace EditProfiles.Commands
{
    /// <summary>
    /// Responsible to Stop this program.
    /// </summary>
    public class StopCommand
    {
        /// <summary>
        /// Default command.
        /// </summary>
        public DefaultCommand Command { get; private set; }

        /// <summary>
        /// Provides Stop Button command.
        /// </summary>
        public StopCommand ( )
        {
            this.Command = new DefaultCommand ( this.ExecuteStop, this.CanExecute );
        }

        /// <summary>
        /// Actual command to run.
        /// </summary>
        /// <param name="unused"></param>
        public void ExecuteStop ( object unused )
        {
            // Cancels the Cancellation Token.
            MyCommons.TokenSource.Cancel ( );

            MyCommons.TokenSource.Dispose ( );

            // Create a new Token in case the user wants to run the program again.
            MyCommons.TokenSource = new System.Threading.CancellationTokenSource ( );
            MyCommons.CancellationToken = new System.Threading.CancellationToken ( false );

        }

        /// <summary>
        /// Decides if this command can run.
        /// </summary>
        /// <param name="unused"></param>
        /// <returns></returns>
        public bool CanExecute ( object unused )
        {
            return MyCommons.CancellationToken.CanBeCanceled;
        }
    }
}
