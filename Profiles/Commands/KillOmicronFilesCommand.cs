using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditProfiles.Data;
using EditProfiles.Operations;
using System.Security;

namespace EditProfiles.Commands
{
    /// <summary>
    /// Terminates Omicron files.
    /// </summary>
    public class KillOmicronFilesCommand
    {
        // Module type names Omicron internally refers.
        private IList<string> OmicronModuleList = new List<string> { 
                                TestModuleName.OCCenter, 
                                TestModuleName.OMExec,
                                TestModuleName.OMSeq, 
                                TestModuleName.OMRamp, 
                                TestModuleName.OMPulse,
                                TestModuleName.QuickCmc
        };

        // Omicron test module's names internally.
        private IList<string> OmicronProgIDs = new List<string> { 
                                ProgId.Dummy, 
                                ProgId.Execute, 
                                ProgId.OMSeq, 
                                ProgId.OMRamp, 
                                ProgId.OMPulse 
        };

        private string OmicronProgId { get; set; }
        /// <summary>
        /// Default Command.
        /// </summary>
        public DefaultCommand Command { get; private set; }

        /// <summary>
        /// Terminates Omicron files.
        /// </summary>
        public KillOmicronFilesCommand ( )
        {
            this.Command = new DefaultCommand ( this.KillOmicronFiles, this.CanExecute );
        }


        // LinkDemands are deprecated in the level 2 security rule set.
        // https://msdn.microsoft.com/en-us/library/dd997569.aspx
        //[SecurityPermission ( SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
        //[SecurityPermission ( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode )]
        [SecurityCritical]
        public void KillOmicronFiles ( object omicronProgId )
        {
            try
            {

                
            }
            catch ( ArgumentNullException ae )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ae );                
            }
        }

        public bool CanExecute ( object unused )
        {
            return false;
        }
    }
}
