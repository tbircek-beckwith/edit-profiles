using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using EditProfiles.Data;
using EditProfiles.Properties;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Omicron Control Center will not close/terminate the Test Module after opening.
    /// Following functions aim to rectify this issue.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        // Given OCCenter a chance to close/terminate opened the Test Module(s).        
        private const int WAIT_TIME_MIN = 5000;

        private const int WAIT_TIME_MAX = 10000;

        private const int WAIT_TIME_NOM = 5000;

        private int WaitTime { get; set; }

        private string OmicronProgId { get; set; }

        // Verify this value between limits
        private int WaitTimeToKillProcess
        {
            get
            {
                this.WaitTime = Settings.Default.WaitTimeToKillProcess;

                if ( ( this.WaitTime > WAIT_TIME_MIN ) && ( this.WaitTime < WAIT_TIME_MAX ) )
                {
                    return this.WaitTime;
                }
                else
                {
                    return WAIT_TIME_NOM;
                }
            }
        }

        private IList<string> OmicronProgIDs
        {
            get
            {
                return new List<string>
                 {
                    ProgId.Dummy, 
                    ProgId.Execute, 
                    ProgId.OMSeq, 
                    ProgId.OMRamp, 
                    ProgId.OMPulse 
                 };
            }
        }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Search for Omicron processes by the file names
        /// and terminate all of them.
        /// </summary>
        [SecurityCritical]
        public bool KillOmicronProcesses()
        {

            //// Save debug and trace info before terminating the program.
            //Debug.Flush ( );
            MyCommons.EditProfileTraceSource.Flush();

            return this.KillOmicronFiles ( );
        }

        /// <summary>
        /// Search for Omicron processes by the progID (Omicron reference)
        /// and terminate all of them.
        /// </summary>
        /// <param name="omicronProgId">Omicron ProgID value</param>
        [SecurityCritical]
        public bool KillOmicronProcesses(string omicronProgId)
        {
            try
            {

                if ( string.IsNullOrWhiteSpace ( omicronProgId ) )
                {
                    throw new ArgumentNullException ( "omicronProgId" );
                }

                this.OmicronProgId = omicronProgId;
                return this.KillOmicronFiles ( this.OmicronProgId );
            }
            catch ( ArgumentNullException ae )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ae, this.CurrentFileName );
                return false;
            }
        }

        #endregion

        #region Private Methods

        // LinkDemands are deprecated in the level 2 security rule set.
        // https://msdn.microsoft.com/en-us/library/dd997569.aspx
        //[SecurityPermission ( SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
        //[SecurityPermission ( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
        [SecurityCritical]
        private bool KillOmicronFiles ( )
        {
            try
            {

                foreach ( string moduleName in TestModuleName.OmicronModuleList )
                {
                    foreach ( var process in Process.GetProcessesByName ( moduleName ) )
                    {

                        if ( !process.HasExited )
                        {
                            Debug.WriteLine ( "KillOmicronFiles ( ) thread: {0}", Thread.CurrentThread.GetHashCode ( ) );
                            process.Kill ( );
                        }
                    }
                }

                return true;
            }
            catch ( AggregateException ae )
            {
                foreach ( Exception ex in ae.InnerExceptions )
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log ( ex, this.CurrentFileName );
                }
                return false;
            }
        }

        // LinkDemands are deprecated in the level 2 security rule set.
        // https://msdn.microsoft.com/en-us/library/dd997569.aspx
        //[SecurityPermission ( SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
        //[SecurityPermission ( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
        [SecurityCritical]
        private bool KillOmicronFiles ( string omicronProgId )
        {
            try
            {
                // Identify the module that still running.
                string omicronModuleName = TestModuleName.OmicronModuleList.ElementAt ( 
                                                                this.OmicronProgIDs.IndexOf ( omicronProgId ) ).
                                                                ToString ( );

                foreach ( var process in Process.GetProcessesByName ( omicronModuleName ) )
                {

                    if ( !process.HasExited )
                    {
                        Debug.WriteLine ( "KillOmicronFiles ( {1} ) thread: {0}", Thread.CurrentThread.GetHashCode ( ), omicronProgId );
                        process.Kill ( );

                    }
                }

                return true;
            }
            catch ( AggregateException ae )
            {
                foreach ( Exception ex in ae.InnerExceptions )
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log ( ex, this.CurrentFileName );
                }
                return false;
            }
        }

        #endregion

    }
}
