using System;
using System.IO;
using EditProfiles.Data;
using EditProfiles.Properties;

namespace EditProfiles.Operations
{
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private bool SaveAs { get; set; }

        private string OldFileName { get; set; }

        private short ProtectionLevel { get; set; }

        #endregion
        // TODO: ProtectionLevel must be preserved file saving new file also with same password.
        #region IStartProcessInterface Members

        /// <summary>
        /// Sets specified protection level and saves modified Omicron Test File.
        /// </summary>
        /// <param name="oldFileName">Original file name.</param>
        /// <param name="saveAs">If it is true the file "SaveAs", false the file "Save".</param>
        /// <param name="protectionLevel">provide a protection level to protect the file.</param>
        public void SaveOmicronFiles ( string oldFileName, bool saveAs, short protectionLevel )
        {
            try
            {
                this.ProtectionLevel = protectionLevel;

                if ( this.ProtectionLevel != this.OmicronDocument.Protection )
                {

                    throw new ArgumentException ( "protectionLevel", MyResources.Strings_ErrorProtectionLevel );

                }

                this.SaveOmicronFiles ( oldFileName, saveAs );
            }
            catch ( ArgumentException ae )
            {

                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ae, this.OldFileName );
                return;
            }

        }

        /// <summary>
        /// Saves modified Omicron Test File.
        /// </summary>
        /// <param name="oldFileName">Original file name.</param>
        /// <param name="saveAs">If it is true the file "saveAs", false the file "save".</param>
        public void SaveOmicronFiles ( string oldFileName, bool saveAs )
        {
            try
            {
                if ( string.IsNullOrWhiteSpace ( oldFileName ) )
                {
                    throw new ArgumentNullException ( "oldFileName" );
                }

                this.OldFileName = GenerateNewFileName ( oldFileName );
                this.SaveAs = saveAs;

                this.SaveOmicronFile ( );

            }
            catch ( ArgumentNullException ae )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ae, this.OldFileName );
                return;
            }
        }

        #endregion

        #region Private Methods

        private void SaveOmicronFile ( )
        {
            try
            {

                // Polling CancellationToken's status.
                // If cancellation requested throw error and exit loop.
                if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                {
                    MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                }

                bool protectionChanged = this.OmicronDocument.ChangeProtection ( this.ProtectionLevel, string.Empty, this.FilePassword );

                if ( !protectionChanged )
                {
                    // Bad password or unknown problem the user interaction necessary.
                    // try to fail gracefully.
                    throw new ArgumentException ( "protectionChanged" );
                }
                else
                {
                    // Depending file exists and saveAs value,
                    // Save the files.
                    if ( File.Exists ( this.OldFileName ) )
                    {
                        if ( this.SaveAs )
                        {
                            this.OmicronDocument.SaveAs ( this.OldFileName );
                        }
                        else
                        {
                            this.OmicronDocument.Save ( );
                        }
                    }
                    else
                    {
                        this.OmicronDocument.SaveAs ( this.OldFileName );
                    }
                }
            }
            catch ( ArgumentException ae )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ae, this.OldFileName );
                return;
            }
            catch ( NullReferenceException ne )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ne, this.OldFileName );
                return;
            }
            catch ( AggregateException ae )
            {
                foreach ( Exception ex in ae.InnerExceptions )
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log ( ex, this.OldFileName );
                }
                return;
            }
        }

        #endregion

    }
}
