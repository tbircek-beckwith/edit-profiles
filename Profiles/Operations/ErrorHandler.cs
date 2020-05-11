﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using EditProfiles.Data;
using EditProfiles.Properties;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Handles all error messages.
    /// </summary>
    internal static class ErrorHandler
    {

        #region Private Variables

        #endregion

        #region Constants

        // System.Runtime.InteropServices.COMException (0x8001010A):
        // The message filter indicated that the application is busy.
        // (Exception from HRESULT: 0x8001010A ( RPC_E_SERVERCALL_RETRYLATER ))
        internal const UInt32 RPC_E_SERVERCALL_RETRYLATER = 0x8001010A;

        // System.Runtime.InteropServices.COMException (0x80010105): 
        // The server threw an exception.
        // (Exception from HRESULT: 0x80010105 ( RPC_E_SERVERFAULT ))
        internal const UInt32 RPC_E_SERVERFAULT = 0x80010105;

        // System.Runtime.InteropServices.COMException (0x800706BE):
        // The remote procedure call failed. 
        // (Exception from HRESULT: 0x800706BE ( RPC_E_SERVERFAILED ))
        internal const UInt32 RPC_E_SERVERFAILED = 0x800706BE;

        // System.Runtime.InteropServices.COMException (0x80040154):
        // The class not registered. (No Omicron Test Suite installed.)
        // (Exception from HRESULT: 0x80040154 ( REGDB_E_CLASSNOTREG ))
        internal const UInt32 REGDB_E_CLASSNOTREG = 0x80040154;

        #endregion

        #region Methods

        /// <summary>
        /// Logs the errors.
        /// </summary>
        /// <param name="ex">Exception generated.</param>
        /// <param name="fileName">Currently processed Omicron Control Center file name.</param>
        /// <returns>Exception.</returns>
        internal static Exception Log(this Exception ex, string fileName = "Not specified")
        {
            string path = MyCommons.CrashFileNameWithPath;

            // Print Filename to the Debug window.
            Debug.WriteLine(fileName);

            // Print to "Debug output"
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} ", ex), "Errors ");


            if (!File.Exists(path))
            {
                File.CreateText(path);
            }

            //long length = new System.IO.FileInfo(path).Length;
            //Debug.WriteLine("File size is {0}", length);

            //if (length > Settings.Default.CrashLogFileSize)
            //{
            //    File.Create(path);
            //}

            // activate tracer here to prevent unnecessary crash file generation.
            // initialize a new tracer if there is only default.
            if (MyCommons.EditProfileTraceSource.Name == "Default")
            {
                Tracer();
            }            
 
            // Save to the fileOutputFolder
            MyCommons.EditProfileTraceSource.TraceEvent(TraceEventType.Error,
                                                 ex.HResult,
                                                 string.Format(
                                                                 CultureInfo.InvariantCulture,
                                                                 MyResources.Strings_ErrorTraceErrors,
                                                                 DateTime.Now,
                                                                 ex,
                                                                 Environment.NewLine,
                                                                 fileName
                                 ));

            return ex;
        }

        // Following code to handle System.Runtime.InteropServices.COMException (0x8001010A) error.
        // Generated by Omicron Engine while closing and starting a new Omicron Application File.
        // unchecked ( ) is necessary to ignore overflow exceptions.
        // http://codereview.stackexchange.com/questions/582/handling-com-exceptions-busy-codes                
        internal static bool ShouldRetry(this COMException e)
        {

            Debug.WriteLine(string.Format(" ERROR HANDLING: 0x{0:X8} , will return {1} ", e.ErrorCode, e.ErrorCode == unchecked((int)RPC_E_SERVERCALL_RETRYLATER)));

            return (e.ErrorCode == unchecked((int)RPC_E_SERVERCALL_RETRYLATER));

        }

        /// <summary>
        /// Provides tracing through a trace source without using a configuration file.
        /// Although this is not a recommended practice, could not find any other method to 
        /// customize file location in app.config file.
        /// Further info...: https://msdn.microsoft.com/en-us/library/ms228984.aspx
        /// </summary>
        internal static void Tracer()
        {

            MyCommons.EditProfileTraceSource.Switch = new SourceSwitch("sourceSwitch", "Error");

            // without this line there will be a Default switch.
            MyCommons.EditProfileTraceSource.Listeners.Remove("Default");

            //XmlWriterTraceListener xmlListener =
            //    new XmlWriterTraceListener(MyCommons.CrashFileNameWithPath);

            TextWriterTraceListener textListener =
                new TextWriterTraceListener(MyCommons.CrashFileNameWithPath)
                {
                    Filter = new EventTypeFilter(SourceLevels.Error)
                };

            //xmlListener.Filter =
            //    new EventTypeFilter(SourceLevels.Error);

            MyCommons.EditProfileTraceSource.Listeners.Add(textListener);
            //MyCommons.EditProfileTraceSource.Listeners.Add(xmlListener);

            // Allow the trace source to send messages to 
            // listeners for all event types. Currently only 
            // error messages or higher go to the listeners.
            // Messages must get past the source switch to 
            // get to the listeners, regardless of the settings 
            // for the listeners.
            MyCommons.EditProfileTraceSource.Switch.Level = SourceLevels.All;

            return;
        }
        #endregion

        ///// <summary>
        ///// Displays errors as a MessageBox for immediate attention.
        ///// </summary>
        ///// <param name="ex">Exception generated.</param>
        ///// <param name="msg">Default message to display if there is no error message from the exception.</param>
        ///// <param name="icon">Default to Error icon.</param>
        ///// <returns>Exception.</returns>
        //[Obsolete ( "For future use." )]
        //internal static Exception Display ( this Exception ex, string msg = null, MessageBoxIcon icon = MessageBoxIcon.Error )
        //{

        //    MessageBox.Show ( msg ?? ex.Message, "", MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly );
        //    return ex;
        //}

    }
}
