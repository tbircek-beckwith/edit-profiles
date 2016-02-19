using System;
using System.IO;
using System.Text;
using System.Threading;
using EditProfiles.Properties;
using System.Threading.Tasks;

namespace EditProfiles.Data
{
    /// <summary>
    /// Holds project wide variables.
    /// </summary>
    internal static class MyCommons // : ProcessFiles
    {
        /// <summary>
        /// The current view
        /// </summary>
        internal static ViewModel MyViewModel { get; set; }

        /// <summary>
        /// Increment it for faster executions but it is not reliable over value of 1;
        /// </summary>
        private const int MaxDegreeOfParallelism = 1;

        /// <summary>
        /// Paralleling options
        /// </summary>
        internal static ParallelOptions ParallelingOptions
        {
            get
            {
                ParallelOptions parallelingOptions = new ParallelOptions ( );
                parallelingOptions.MaxDegreeOfParallelism = MyCommons.MaxDegreeOfParallelism;
                parallelingOptions.CancellationToken = MyCommons.CancellationToken;

                return parallelingOptions;            
            }
        }

        /// <summary>
        /// Holds log files folder location.
        /// Folder Location: C:\Users\yourName\AppData\Local\ProfileChanger
        /// </summary>
        internal static string FileOutputFolder
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
        internal static int TotalFileNumber { get; set; }

        /// <summary>
        /// Currently processing file number.
        /// </summary>
        internal static int CurrentFileNumber { get; set; }

        /// <summary>
        /// Total module number in current file.
        /// </summary>
        internal static int TotalModuleNumber { get; set; }

        /// <summary>
        /// Current processing module number.
        /// </summary>
        internal static int CurrentModuleNumber { get; set; }

        /// <summary>
        /// Storage for the process of the program.
        /// </summary>
        internal static StringBuilder LogProcess { get; set; }

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        internal static CancellationTokenSource TokenSource { get; set; }

        /// <summary>
        /// Cancellation token.
        /// </summary>
        internal static CancellationToken CancellationToken { get; set; }
    }
}
