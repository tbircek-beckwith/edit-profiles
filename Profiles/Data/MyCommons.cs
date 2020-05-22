using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EditProfiles.Operations;
using EditProfiles.Properties;

namespace EditProfiles.Data
{
    /// <summary>
    /// Holds project wide variables.
    /// </summary>
    internal static class MyCommons // : ProcessFiles
    {
        /// <summary>
        /// Trace source.
        /// </summary>
        internal static TraceSource EditProfileTraceSource = new TraceSource(MyResources.Strings_TracerName);

        /// <summary>
        /// holds file name without folder path.
        /// </summary>
        internal static string FileName { get; set; }

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
                ParallelOptions parallelingOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = MyCommons.MaxDegreeOfParallelism,
                    CancellationToken = MyCommons.CancellationToken
                };

                return parallelingOptions;
            }
        }

        /// <summary>
        /// generate a folder to hold log files.
        /// path..: C:\Users\yourName\AppData\Local\EditProfiles\versionNumber\Logs\
        /// </summary>
        internal static string LogFileFolderPath
        {
            get
            {
                return Path.Combine(FileOutputFolder, MyResources.Strings_LogFolder);
            }
        }

        /// <summary>
        /// To add crash log file.
        /// path..: C:\Users\yourName\AppData\Local\EditProfiles\versionNumber\CrashReports\
        /// </summary>
        internal static string CrashFileFolderPath
        {
            get
            {
                return Path.Combine(FileOutputFolder, MyResources.Strings_ErrorFolder);
            }
        }

        /// <summary>
        /// To add log file
        /// path..: C:\Users\yourName\AppData\Local\EditProfiles\Logs\crash_09_43_34_01_04_2017.log
        /// </summary>
        internal static string CrashFileNameWithPath
        {
            get
            {
                return Path.Combine(CrashFileFolderPath,
                            String.Format(MyResources.Strings_Error_FileName,
                                          DateTime.Now.ToString(MyResources.Strings_DateTimeFormat)  // DateTime.Now.ToString("_HH_mm_ss_MM_dd_yyyy")
                                          )
                                    );
            }
        }

        /// <summary>
        /// To add log file
        /// path..: C:\Users\yourName\AppData\Local\EditProfiles\Logs\editprofiles_09_43_34_01_04_2017.log
        /// </summary>
        internal static string LogFileNameWithPath
        {
            get
            {
                return Path.Combine(LogFileFolderPath,
                            String.Format(MyResources.Strings_LogFileName,
                                          DateTime.Now.ToString(MyResources.Strings_DateTimeFormat)  // DateTime.Now.ToString("_HH_mm_ss_MM_dd_yyyy")
                                          )
                                    );
            }
        }

        /// <summary>
        /// Holds appdata folder location for this app.
        /// path..: C:\Users\yourName\AppData\Local\ProfileChanger\
        /// </summary>
        private static string FileOutputFolder
        {
            get
            {
                return Path.Combine(path1: Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                    path2: MyResources.Strings_FolderName);
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
        internal static long CurrentModuleNumber { get; set; }

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

        /// <summary>
        /// Holds OleObject type.
        /// </summary>
        internal static OMICRON.OCCenter.OLEObject OccOleObject { get; set; }

        /// <summary>
        /// Holds <see cref="Regulator"/>s
        /// </summary>
        internal static ObservableCollection<Regulator> Regulators { get; set; } = new ObservableCollection<Regulator>() { };

        /// <summary>
        /// Sets and gets <see cref="Profile.Registers"/>.Items(<see cref="Register.ReplacementValue"/>)
        /// </summary>
        internal static string ReplaceProfile { get; set; } = string.Empty;

        /// <summary>
        /// Sets and gets <see cref="Profile.Registers"/>.Items(<see cref="Register.OriginalSettingValue"/>) 
        /// or <see cref="Profile.Registers"/>.Items(<see cref="Register.OriginalTestValue"/>)
        /// </summary>
        internal static string FindProfile { get; set; } = string.Empty;

    }
}
