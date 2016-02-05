using System;
using System.IO;
using System.Text;
using System.Threading;
using EditProfiles.Properties;
using EditProfiles.Operations;

namespace EditProfiles
{
    /// <summary>
    /// Holds project wide variables.
    /// </summary>
    public static class MyCommons // : ProcessFiles
    {

        /// <summary>
        /// Holds log files folder location.
        /// Folder Location: C:\Users\yourName\AppData\Local\ProfileChanger
        /// </summary>
        public static string FileOutputFolder
        {
            get
            {
                return Path.Combine ( Environment.GetFolderPath (
                                      Environment.SpecialFolder.LocalApplicationData ),
                                      MyResources.Strings_FolderName );
            }
        }

        /// <summary>
        /// Provides limited interfaces to MainForm to update texts.
        /// </summary>
        [Obsolete ("This module will not support WPF functionalities." )]
        public static IRestrictedMainFormInterface MainFormRestrictedAccessTo
        {
            get
            {
                return null;
                // return ( MainForm ) Application.OpenForms[ 0 ];
            }
        }

        /// <summary>
        /// Storage for the process of the program.
        /// </summary>
        public static StringBuilder LogProcess { get; set; }

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        public static CancellationTokenSource TokenSource { get; set; }

        /// <summary>
        /// Cancellation token.
        /// </summary>
        public static CancellationToken CancellationToken { get; set; }
    }
}
