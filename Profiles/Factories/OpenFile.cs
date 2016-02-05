using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EditProfiles.Values;
using EditProfiles;

namespace EditProfiles.Operations
{
    /// <summary>
    /// File operations for the Omicron Control Center files.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private string FilePath { get; set; }

        private string FilePassword { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Opens the Omicron Control Center file.
        /// </summary>
        /// <param name="path">Location of the OCC file.</param>
        /// <param name="password">The password used to protect the file.</param>
        /// <returns>Returns unlocked the Omicron Control Center file.</returns>
        public OMICRON.OCCenter.IAutoDoc OpenDocument ( string path, string password )
        {
            try
            {
                // Not all files have a password.
                // Do not check if the password is empty.
                if ( string.IsNullOrWhiteSpace ( path ) )
                {
                    throw new ArgumentNullException ( "path" );
                }

                this.FilePath = path;

                this.FilePassword = password;

                return OpenDocuments ( );
            }
            catch ( ArgumentNullException ae )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ae, this.CurrentFileName );

                return null;
            }
        }

        #endregion

        #region Private Methods

        private OMICRON.OCCenter.IAutoDoc OpenDocuments ( )
        {
            try
            {
                this.OmicronApplication = new OMICRON.OCCenter.Application ( );
                OMICRON.OCCenter.IAutoConst occConstants = this.OmicronApplication.Constants;  // occApp.Constants;

                Task.Factory.StartNew ( ( ) =>
                    {
                        try
                        {

#if DEBUG
                            Console.WriteLine ( "OPENDOCUMENT thread: {0}", Thread.CurrentThread.GetHashCode ( ) );
#endif

                            // Polling CancellationToken's status.
                            // If cancellation requested throw error and exit loop.
                            if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                            {
                                MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                            }
                            
                            // Open the document.
                            this.OmicronDocument = this.OmicronApplication.Documents.Open ( this.FilePath );
                        }
                        catch ( AggregateException ae )
                        {
                            foreach ( Exception ex in ae.InnerExceptions )
                            {
                                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                ErrorHandler.Log ( ex, this.CurrentFileName );
                            }
                            return;
                        }
                    }
                    , MyCommons.CancellationToken )
                    .ContinueWith ( threadProtection =>
                        {

                            // Polling CancellationToken's status.
                            // If cancellation requested throw error and exit loop.
                            if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                            {
                                MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                            }

                            // Strip down any type of file protection if a file is protected.
                            if ( !( this.OmicronDocument.Protection == occConstants.cProtectionNoProtection ) )
                            {
                                // Setting new password to nothing will unlock the file for editing.
                                this.OmicronDocument.ChangeProtection ( occConstants.cProtectionNoProtection, this.FilePassword, string.Empty );
                            }

                        }
                        , MyCommons.CancellationToken )
                        .Wait ( );

                // Returns the Omicron Control Center file.
                return this.OmicronDocument;

            }
            catch ( NullReferenceException nre )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( nre, this.CurrentFileName );
                return null;
            }
            catch ( COMException ce )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ce, this.CurrentFileName );
                return null;
            }
        }

        #endregion

    }
}
