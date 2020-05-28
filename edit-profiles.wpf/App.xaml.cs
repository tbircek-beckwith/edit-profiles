using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EditProfiles.Behaviors;
using EditProfiles.Data;
using EditProfiles.Operations;
using EditProfiles.Properties;
using Squirrel;

namespace EditProfiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private Methods

        /// <summary>
        /// Checks for the updates if there is one updates application for next start up
        /// </summary>
        /// <returns>AsyncStateMachine of Squirrel</returns>
        private async Task CheckForUpdates()
        {
            // for future use 
            // change update location to github
            // using (var updateManager = UpdateManager.GitHubUpdateManager("https://api.github.com/repos/tbircek/metering/releases"))

            // specify the location of update
            using (UpdateManager updateManager = new UpdateManager(@"\\volta\Eng_Lab\Software Updates\EditProfiles"))
            {
                try
                {
                    // check if there is an update
                    UpdateInfo updateInfo = await updateManager.CheckForUpdate();

                    // prevent error in development computer
                    if (updateInfo.CurrentlyInstalledVersion != null)
                        // log the current installed version of the application
                        Debug.WriteLine($"Current version: {updateInfo.CurrentlyInstalledVersion.Version}", "Informative");

                    // if update location contains update for this application
                    if (updateInfo.ReleasesToApply.Count > 0)
                    {
                        // log the current installed version of the application
                        Debug.WriteLine($"Update version: {updateInfo.FutureReleaseEntry.Version}", "Informative");

                        // update this application
                        await updateManager.UpdateApp();

                    }
                    // no update available
                    else
                    {
                        // log application update message
                        Debug.WriteLine($"No updates: Update version: {updateInfo.FutureReleaseEntry.Version}", "Informative");
                    }
                }
                catch (Exception ex)
                {
                    // log application update message
                    Debug.WriteLine($"Error: {ex.Message}", "Error");
                    // throw;
                }
            }
        }

        #endregion

        #region Startup

        /// <summary>
        /// Start up 
        /// </summary>
        /// <param name="e">arguments</param>
        protected override async void OnStartup(StartupEventArgs e)
        {

            #region Catch All Unhandled Exceptions

            // Following code found on stackoverflow.com
            // http://stackoverflow.com/questions/10202987/in-c-sharp-how-to-collect-stack-trace-of-program-crash

            AppDomain currentDomain = default;
            currentDomain = AppDomain.CurrentDomain;

            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;

            // Handler for exceptions in thread behind forms.
            Current.DispatcherUnhandledException += GlobalThreadExceptionHandler;

            #endregion

            #region Create LocalApplication Folders.

            if (!Directory.Exists(MyCommons.LogFileFolderPath))
            {
                Directory.CreateDirectory(MyCommons.LogFileFolderPath);
            }

            if (!Directory.Exists(MyCommons.CrashFileFolderPath))
            {
                Directory.CreateDirectory(MyCommons.CrashFileFolderPath);
            }

            #endregion

            #region Error Logging.

            //// Trace is empty.
            //MyCommons.isTraceEmpty = true;

            //// Add textwriter for console.
            //TextWriterTraceListener consoleOut =
            //    new TextWriterTraceListener(System.Console.Out);

            //Debug.Listeners.Add(consoleOut);

            //TextWriterTraceListener fileOut =
            //    new TextWriterTraceListener(MyCommons.CrashFileNameWithPath);//File.CreateText(MyCommons.CrashFileNameWithPath));
            //Debug.Listeners.Add(fileOut);


            // Set the Trace to track errors.
            // moving all tracing to the ErrorHandler module.
            // ErrorHandler.Tracer();

            #endregion

            #region Application Starts Here.

            base.OnStartup(e);

            //// Verify none of the Omicron modules running,
            //// without it unexpected behaviors would occur.
            DispatchService.Invoke(() =>
                {

                    IStartProcessInterface spi = new ProcessFiles();
                    spi.KillOmicronProcesses();
                });

            ViewFactory factory = new ViewFactory();

            ViewInfrastructure infrastructure = factory.Create();

            infrastructure.View.DataContext = infrastructure.ViewModel;

#if DEBUG
            MyCommons.MyViewModel.ReplaceWithTextBoxText = MyResources.Strings_Debug_TextBoxReplace;
            MyCommons.MyViewModel.FindWhatTextBoxText = MyResources.Strings_Debug_TextBoxFind;

#endif

            MyCommons.MyViewModel.FileSideCoverText = MyResources.Strings_FormStartTest;
            MyCommons.MyViewModel.ModuleSideCoverText = MyResources.Strings_FormStartModuleTest;

            MyCommons.MyViewModel.Editable = true;

            // check for the updates
            await Task.Run(async () =>
            {
                // log application update message
                Debug.WriteLine("Checking for updates", "Informative");

                // await for application update
                await CheckForUpdates();
            });

            infrastructure.View.Show();

            #endregion

        }

        #endregion

        #region Exit

        //  private void Application_Exit ( object sender, ExitEventArgs e )
        /// <summary>
        /// Exit 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            try
            {
                // terminate any running Omicron related process.
                IStartProcessInterface spi = new ProcessFiles();
                do
                {
                    if (MyCommons.CancellationToken.IsCancellationRequested == true)
                    {
                        break;
                    }
                } while (!(Task.Factory.StartNew(() => spi.KillOmicronProcesses())).Result);


                // Log the operations completed so far.
                if (MyCommons.LogProcess.Length > 0)
                {

                    // String logFilePath = Path.Combine(MyCommons.FileOutputFolder, MyResources.Strings_LogFolder);

                    // write log to the file.
                    File.WriteAllText(MyCommons.LogFileNameWithPath, MyCommons.LogProcess.ToString());

                    // open saved log file for the user.
                    Process.Start(MyCommons.LogFileNameWithPath);
                }
            }
            catch (NullReferenceException nre)
            {
                // save to the fileOutputFolder and print to Debug window 
                // if the project build is in DEBUG.
                ErrorHandler.Log(nre);
                //Debug.Flush();
                MyCommons.EditProfileTraceSource.Flush();
            }
            finally
            {
                // save debug and trace info before terminating the program.
                //Debug.Flush();
                MyCommons.EditProfileTraceSource.Flush();
                

                if (MyCommons.CancellationToken.CanBeCanceled)
                {
                    MyCommons.TokenSource.Dispose();
                }
            }
        }

        #endregion

        #region Catch All Unhandled Exceptions

        // Following code found on stackoverflow.com
        // http://stackoverflow.com/questions/10202987/in-c-sharp-how-to-collect-stack-trace-of-program-crash
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = default;
            ex = (Exception)e.ExceptionObject;

            // Save to the fileOutputFolder and print to Debug window if the project build is in DEBUG.
            ErrorHandler.Log(ex);
            //Debug.Flush();
            MyCommons.EditProfileTraceSource.Flush();

        }

        // Following code found on stackoverflow.com
        // http://stackoverflow.com/questions/10202987/in-c-sharp-how-to-collect-stack-trace-of-program-crash
        private static void GlobalThreadExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = default;
            ex = e.Exception;

            // Save to the fileOutputFolder and print to Debug window if the project build is in DEBUG.
            ErrorHandler.Log(ex);
            //Debug.Flush();
            MyCommons.EditProfileTraceSource.Flush();
        }

        #endregion

    }
}
