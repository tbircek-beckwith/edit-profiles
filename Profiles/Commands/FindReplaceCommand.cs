using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EditProfiles.Data;
using EditProfiles.Operations;
using EditProfiles.Properties;
using Microsoft.Win32;
// using Microsoft.WindowsAPICodePack.Dialogs;

namespace EditProfiles.Commands
{
    /// <summary>
    /// Opens FileDialog UI interface to provide
    /// entry point to the this program.
    /// </summary>
    public class FindReplaceCommand
    {
        private Stopwatch stopwatch;

        /// <summary>
        /// Default command
        /// </summary>
        public DefaultCommand Command { get; private set; }

        /// <summary>
        /// Provides FindReplace Button command.
        /// </summary>
        public FindReplaceCommand()
        {
            this.Command = new DefaultCommand(this.ExecuteFindReplace, this.CanExecute);
        }

        /// <summary>
        /// Actual command that does the work.
        /// </summary>
        /// <param name="unused"></param>
        public void ExecuteFindReplace(object unused)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            // CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            // CommonFileDialogFilter filter = new CommonFileDialogFilter(MyResources.Strings_FileDialogFilter, "*.occ");


            // Set OpenFileDialog defaults.
            dlg.Title = MyResources.Strings_OpenFileDialog;
            dlg.Filter = MyResources.Strings_FileDialogFilter;
            dlg.DefaultExt = MyResources.Strings_FileDialogDefault;
            dlg.Multiselect = true;
            // dlg.IsFolderPicker = true;


#if DEBUG
            dlg.InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), MyResources.Strings_Debug_InitialDirectory);
#else
            dlg.InitialDirectory = Environment.GetFolderPath ( Environment.SpecialFolder.DesktopDirectory );
#endif

            // if (dlg.ShowDialog()== CommonFileDialogResult.Ok)
            if (dlg.ShowDialog() == true)
            {
                // initialize stopwatch 
                stopwatch = new Stopwatch();
                stopwatch.Start();

                if (MyCommons.TokenSource == null)
                {
                    // Initialize Common Properties.
                    MyCommons.TokenSource = new CancellationTokenSource();
                    MyCommons.CancellationToken = MyCommons.TokenSource.Token;
                }

                // Start Processing the user specified files.
                IStartProcessInterface spi = new ProcessFiles();

                Task.Factory.StartNew(() =>
                    {

                        try
                        {
                            // Polling Cancellation Token Status.
                            // If cancellation requested throw an error and exit loop.
                            if (MyCommons.CancellationToken.IsCancellationRequested == true)
                            {
                                MyCommons.CancellationToken.ThrowIfCancellationRequested();
                            }

                            // Prevents the user to modify texts of FindWhat, ReplaceWith and Password 
                            // while test running.
                            this.Enable(false);

                            // Start process.
                            // spi.StartProcessing ( dlg.FileNames, MyCommons.MyViewModel.Password, MyCommons.MyViewModel );
                            spi.StartProcessing(dlg.FileNames, MyCommons.MyViewModel);

                        }
                        catch (AggregateException ae)
                        {
                            ErrorHandler.Log(ae);
                        }

                    }
                    , MyCommons.CancellationToken)
                    .ContinueWith(value =>
                        {
                            // Reset and Activate controls.
                            this.ResetControls();
                        });
            }
            else
            {
                // Reverts back 'Start', since the user 'Cancelled' to select file
                MyCommons.MyViewModel.IsChecked = false;

                MyCommons.MyViewModel.IsEnabled = false;

                // Prevents the user to modify texts of FindWhat, ReplaceWith and Password 
                // while test running.
                this.Enable(true);

                // Reset all controls
                MyCommons.MyViewModel.EraseCommand.Execute(null);
            }
        }

        /// <summary>
        /// Disable the controls to prevent the user modifying text inputs.
        /// </summary>
        /// <param name="value">True to allow the user modification of the text inputs.</param>
        private void Enable(bool value)
        {
            MyCommons.MyViewModel.Editable = value;
        }

        /// <summary>
        /// Reset Controls.
        /// </summary>
        private void ResetControls()
        {
            stopwatch.Stop();

            // Update DetailsTextBoxText.
            MyCommons.MyViewModel.DetailsTextBoxText =
                MyCommons.LogProcess.Append(
                (string.Format(
                CultureInfo.InvariantCulture,
                MyResources.Strings_WholeTestEnd,
                stopwatch.Elapsed,
                DateTime.Now,
                Repeat.StringDuplicate(Settings.Default.RepeatChar, Settings.Default.RepeatNumber),
                Environment.NewLine)))
                .ToString();

            this.Enable(true);

            // Prepare Toggle Button to restest. 'Start'
            MyCommons.MyViewModel.IsChecked = false;
        }

        /// <summary>
        /// Insert any validation requirement for the 
        /// FindReplaceCommand here.
        /// </summary>
        /// <param name="unused"></param>
        /// <returns></returns>
        public bool CanExecute(object unused)
        {
            return !(String.IsNullOrWhiteSpace(MyCommons.MyViewModel.FindWhatTextBoxText));
        }
    }
}
