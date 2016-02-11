using System;
using System.Collections.Generic;
using OMICRON.OCCenter;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Provides interface to Start Processing of Scanning Omicron Test Files.
    /// </summary>
    partial interface IStartProcessInterface
    {
        /// <summary>
        /// Start processing the user selected Omicron Test Files.
        /// </summary>
        /// <param name="fileNames">The file names to process.</param>
        /// <param name="password">The password to open files in case there is a protection.</param>
        /// <param name="viewModel">The copy of the current ViewModel for the business logic.</param>
        void StartProcessing ( IList<string> fileNames, string password, ViewModel viewModel );

        /// <summary>
        /// Start processing the user selected Omicron Test Files.
        /// </summary>
        /// <param name="fileNames">The file names to process.</param>
        /// <param name="password">The password to open files in case there is a protection.</param>
        [Obsolete ( "This module will not support WPF functionalities." )]
        void StartProcessing ( IList<string> fileNames, string password );

        /// <summary>
        /// Scans specified Omicron Test Document.
        /// </summary>
        void Scan ( );

         /// <summary>
        /// Main Method to extract values form Execute TestModule.
        /// </summary>
        /// <param name="testModule">Execute TestModule to process.</param>
        /// <returns>Returns parameter values of Execute TestModule parameters.</returns>
        string Retrieve ( IAutoTM testModule );

        /// <summary>
        /// Search for Omicron processes by the file names
        /// and terminate all of them.
        /// </summary>
        bool KillOmicronProcesses ( );
                
        /// <summary>
        /// Search for Omicron processes by the progID (Omicron reference)
        /// and terminate all of them.
        /// </summary>
        /// <param name="omicronProgId">Omicron ProgID value</param>
        bool KillOmicronProcesses ( string omicronProgId );
        
        /// <summary>
        /// Opens the Omicron Control Center file.
        /// </summary>
        /// <param name="path">Location of the OCC file.</param>
        /// <param name="password">The password used to protect the file.</param>
        /// <returns>Returns unlocked the Omicron Control Center file.</returns>
        IAutoDoc OpenDocument ( string path, string password );
    }
}
