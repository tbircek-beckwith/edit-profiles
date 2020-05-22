using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            using (var reader = new StreamReader(dlg.FileName))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // initialize holders for find and replace items
                    StringBuilder finds = new StringBuilder();
                    StringBuilder replaces = new StringBuilder();

                    // the .csv file doesn't have any header.
                    csv.Configuration.HasHeaderRecord = false;

                    // initialize a new Register collection
                    ObservableCollection<Register> registers = new ObservableCollection<Register>() { };

                    // scan through the file
                    while (csv.Read())
                    {
                        // ignore the blank entries.
                        if (!string.IsNullOrWhiteSpace(csv.GetField((int)Column.OriginalSettingValue)))
                        {
                            // append new field to appropriate groups with "|" 
                            finds.Append(csv.GetField((int)Column.OriginalSettingValue)).Append("|");
                            replaces.Append(csv.GetField((int)Column.ReplacementValue)).Append("|");

                            // Profile 2,3, and 4's "Original Value" must exchange to Profile 1 "Original Value".
                            // since .cvs file follows profile order we can assume first two profiles are configuration
                            // and profile 1.
                            int profileCount = registers.Select(a => a.Profile).Distinct().Count();

                            // Set points name must include a "setpoints[#]" in their name and Column R != "NULL"
                            bool isSetPoint = Regex.Split(csv.GetField((int)Column.Location), new AnalyzeValues().SetPointPatterns, RegexOptions.None, TimeSpan.FromSeconds(2)).Length > 1 && !string.Equals(csv.GetField((int)Column.AltName), "NULL");

                            bool hasRegulatorInfo = Regex.Split(csv.GetField((int)Column.Location), new AnalyzeValues().RegulatorPatterns, RegexOptions.None, TimeSpan.FromSeconds(2)).Length > 1;

                            // if a value don't include ".set[points[#]." then it belongs to other variations of the modbus register.
                            // referring them as profile 0
                            string profile = isSetPoint ? $"{Convert.ToInt32(new AnalyzeValues().Match(csv.GetField((int)Column.Location), new AnalyzeValues().SetPointPatterns).Split('[').GetValue(1).ToString().Split(']').GetValue(0)) + 1}" : "0";

                            // if a value don't include "[REG_IDX_1]" then it unattached.
                            // referring them as regulator 0
                            string regulator = hasRegulatorInfo ? $"{new AnalyzeValues().Match(csv.GetField((int)Column.Location), new AnalyzeValues().RegulatorPatterns).Split('_').GetValue(2)}" : "0";

                            // retrieve the row.
                            Register register = new Register
                            {
                                Index = csv.Context.Row - 1,
                                Row = csv.Context.Row,
                                ReplacementValue = csv.GetField((int)Column.ReplacementValue),
                                OriginalSettingValue = csv.GetField((int)Column.OriginalSettingValue),
                                OriginalTestValue = (isSetPoint) && (2 < profileCount) && (!string.Equals(profile, registers[0].Profile)) ? registers.Where(y => y.Location.Contains(Regex.Split(csv.GetField((int)Column.Location), new AnalyzeValues().SetPointPatterns).Last())).ElementAt(0).OriginalSettingValue : csv.GetField((int)Column.OriginalSettingValue),
                                Location = csv.GetField((int)Column.Location),
                                MinimumValue = csv.GetField((int)Column.MinimumValue),
                                MaximumValue = csv.GetField((int)Column.MaximumValue),
                                Increment = csv.GetField((int)Column.Increment),
                                OptionalName = csv.GetField((int)Column.OptionalName),
                                AltName = csv.GetField((int)Column.AltName),
                                Profile = profile,
                                IsRegulatorCommon = Convert.ToInt32(csv.GetField((int)Column.ReplacementValue)) >= 40000,
                                DataType = csv.GetField((int)Column.DataType),
                                MBFunction = csv.GetField((int)Column.MBFunction),
                                Permission = csv.GetField((int)Column.Permission),
                                ProtectionLevel = csv.GetField((int)Column.ProtectionLevel),
                                RegisterPermission = csv.GetField((int)Column.RegisterPermissions),
                            };

                            //  Debug.WriteLine($"record #:{registers.Count}\t index:{register.Index}\t row: {register.Row}\t repl: {register.ReplacementValue}\t ori: {register.OriginalSettingValue}\t profile: {register.Profile}\t opt: {register.OptionalName}\t loc: {register.Location}");

                            // add new register to the collection
                            registers.Add(register);

                            // Debug.WriteLine($"row: {register.Row} -- reg value: {register.OriginalSettingValue},{csv.GetField((int)Column.OriginalSettingValue)} :excel value, ---> profile: {register.Profile}");
                        }
                    }
                    
                    // generate all regulators with all profiles and registers values using the .csv file.
                    MyCommons.Regulators = new Regulator().GetRegulators(registers);
                }
            }

            // unhook the dialog event
            dlg.FileOk -= CsvDialog_FileOk;
        }

        #endregion
    }
}
