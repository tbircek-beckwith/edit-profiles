using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using EditProfiles.Data;
using Microsoft.Win32;

namespace EditProfiles.Operations
{
    /// <summary>
    /// File operations for the Omicron Control Center files.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        #endregion

        #region IStartProcessInterface Members


        /// <summary>
        /// Shows file dialog to open .csv files
        /// </summary>
        public void OpenCsvFile()
        {
            try
            {
                // set up OpenFileDialog
                OpenFileDialog CsvDialog = new OpenFileDialog
                {
                    Title = "Select modbus CSV file",
                    Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*",
                    DefaultExt = "csv",
                    Multiselect = false,
                    FileName = "",
                    AddExtension = true,
                };

                // hook up to the dialog event
                CsvDialog.FileOk += CsvDialog_FileOk;

                // Show save file dialog box
                CsvDialog.ShowDialog();
            }
            catch (ArgumentNullException ae)
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log(ae, CurrentFileName);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// generates find and replace parameters based on the user selected .csv file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CsvDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

            // Sender is a FileDialog
            var dlg = sender as FileDialog;

            //ItemsToFind = new List<string>();
            //ItemsToReplace = new List<string>();

            using (var reader = new StreamReader(dlg.FileName))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // initialize holders for find and replace items
                    StringBuilder finds = new StringBuilder();
                    /// List<string> finds = new List<string>();
                    StringBuilder replaces = new StringBuilder();
                    // List<string> replaces = new List<string>();

                    // the .csv file doesn't have any header.
                    csv.Configuration.HasHeaderRecord = false;
                   
                    while (csv.Read())
                    {
                        // ignore the blank entries.
                        if (!string.IsNullOrWhiteSpace(csv.GetField(3)))
                        {
                            // append new field to appropriate groups with "|" 
                            finds.Append(csv.GetField(3)).Append("|");
                            //ItemsToFind.Add(csv.GetField(3));
                            replaces.Append(csv.GetField(1)).Append("|");
                            //ItemsToReplace.Add(csv.GetField(1));
                        }
                    }

                    // assign values to the text boxes.
                    //ItemsToReplace = new List<string>(replaces);
                    //ItemsToFind = new List<string>(finds);

                    MyCommons.MyViewModel.FindWhatTextBoxText = finds.ToString();
                    MyCommons.MyViewModel.ReplaceWithTextBoxText = replaces.ToString();
                }
            }

            // unhook the dialog event
            dlg.FileOk -= CsvDialog_FileOk;
        }

        #endregion
    }
}
