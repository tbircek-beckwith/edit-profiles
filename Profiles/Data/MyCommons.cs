using System;
using System.IO;
using System.Text;
using System.Threading;
using EditProfiles.Properties;

namespace EditProfiles.Data
{
    /// <summary>
    /// Holds project wide variables.
    /// </summary>
    public static class MyCommons // : ProcessFiles
    {
        /// <summary>
        /// 
        /// </summary>
        public static ViewModel MyViewModel { get; set; }

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
        /// Total file number selected by the user.
        /// </summary>
        public static int TotalFileNumber { get; set; }

        /// <summary>
        /// Currently processing file number.
        /// </summary>
        public static int CurrentFileNumber { get; set; }

        /// <summary>
        /// Total module number in current file.
        /// </summary>
        public static int TotalModuleNumber { get; set; }

        /// <summary>
        /// Current processing module number.
        /// </summary>
        public static int CurrentModuleNumber { get; set; }

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
