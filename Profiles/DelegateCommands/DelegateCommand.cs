using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace EditProfiles
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateCommand : ICommand
    {

        #region Attributes

        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        #endregion // Attributes

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public DelegateCommand ( Action<object> execute, Predicate<object> canExecute )
        {

            if ( execute == null )
            {
                throw new ArgumentNullException ( "execute" );
            }

            if ( canExecute == null )
            {
                throw new ArgumentNullException ( "canExecute" );
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        public DelegateCommand ( Action<object> execute )
        {
            if ( execute == null )
            {
                throw new ArgumentNullException ( "execute" );
            }

            this.execute = execute;
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
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute ( object parameter )
        {
            this.execute ( parameter );
        }

        /// <summary>
        /// 
        /// </summary>
        public void RaiseCanExecuteChanged ( )
        {
            this.OnCanExecuteChanged ( );
        }

        /// <summary>
        /// 
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
