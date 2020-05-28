using System;
using System.Windows.Input;

namespace EditProfiles.Commands
{
    /// <summary>
    /// Default template for every other command created by this program.
    /// </summary>
    public class DefaultCommand : ICommand
    {

        #region Attributes

        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        #endregion // Attributes

        #region Public Methods

        /// <summary>
        /// Establishes Default structure for every command.
        /// </summary>
        /// <param name="execute">Action to execute.</param>
        /// <param name="canExecute">This is where blocking of the command occurs.</param>
        public DefaultCommand ( Action<object> execute, Predicate<object> canExecute )
        {
            this.execute = execute ?? throw new ArgumentNullException ( "execute" );
            this.canExecute = canExecute ?? throw new ArgumentNullException ( "canExecute" );
        }

        /// <summary>
        /// Establishes Default structure for every command.
        /// </summary>
        /// <param name="execute">Action to execute.</param>
        public DefaultCommand ( Action<object> execute )
        {
            this.execute = execute ?? throw new ArgumentNullException ( "execute" );
        }

        #endregion // Public Methods

        #region ICommand Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute ( object parameter )
        {
            if ( this.canExecute == null )
            {
                return true;
            }
            else
            {
                return this.canExecute ( parameter );
            }
        }

        /// <summary>
        /// CanExecuteChanged eventhandler.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Establishes Default structure for every command.
        /// </summary>
        /// <param name="parameter">This is where blocking of the command occurs.</param>
        public void Execute ( object parameter )
        {
            this.execute ( parameter );
        }

        /// <summary>
        /// Calls OnCanExecuteChanged
        /// </summary>
        public void RaiseCanExecuteChanged ( )
        {
            this.OnCanExecuteChanged ( );
        }

        /// <summary>
        /// Handles if execute changes
        /// </summary>
        protected virtual void OnCanExecuteChanged ( )
        {
            EventHandler handler = this.CanExecuteChanged;
            if ( handler != null )
            {
                handler.Invoke ( this, EventArgs.Empty );
            }
        }

        #endregion
    }
}
