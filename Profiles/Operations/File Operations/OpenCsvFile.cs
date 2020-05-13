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

            //ItemsToFind = new List<string>();
            //ItemsToReplace = new List<string>();

            using (var reader = new StreamReader(dlg.FileName))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // initialize holders for find and replace items
                    StringBuilder finds = new StringBuilder();
                    // List<string> finds = new List<string>();
                    StringBuilder replaces = new StringBuilder();
                    // List<string> replaces = new List<string>();
                    //int counter = 0;
                    // the .csv file doesn't have any header.
                    csv.Configuration.HasHeaderRecord = false;

                    // initialize 
                    ObservableCollection<Register> registers = new ObservableCollection<Register>();


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

                            //var test = registers.Where(y => y.Location.Contains(Regex.Split(csv.GetField(6), new AnalyzeValues().SetPointPatterns).Last())).ElementAt(0).OriginalValue;

                            if (new AnalyzeValues().IsMatch(csv.GetField(6), new AnalyzeValues().SetPointPatterns))
                            {

                                // var test = registers.Select(a => a.Profile).Distinct().Count() < 2 ? csv.GetField(3) : registers.Where(y => y.Location.Contains(Regex.Split(csv.GetField(6), new AnalyzeValues().SetPointPatterns).Last())).ElementAt(0).OriginalValue;
                                int profileCount = registers.Select(a => a.Profile).Distinct().Count();
                                string profile = $"{Convert.ToInt32(new AnalyzeValues().Match(csv.GetField(6), new AnalyzeValues().SetPointPatterns).Split('[').GetValue(1).ToString().Split(']').GetValue(0)) + 1}";

                                // read a row of .csv file
                                Register register = new Register
                                {
                                    Index = csv.Context.Row - 1,
                                    RowNumber = csv.Context.Row,
                                    ReplacementValue = csv.GetField(1),
                                    // assumption made: original file(s) are/is always Profile 1 
                                    // so always change "Find" value to Profile 1 register number
                                    OriginalValue = (1 <= profileCount) && (!string.Equals(profile,registers[0].Profile)) ? registers.Where(y => y.Location.Contains(Regex.Split(csv.GetField(6), new AnalyzeValues().SetPointPatterns).Last())).ElementAt(0).OriginalValue : csv.GetField(3), // csv.GetField(3),
                                    Location = csv.GetField(6),
                                    MinimumValue = csv.GetField(8),
                                    MaximumValue = csv.GetField(9),
                                    Increment = csv.GetField(10),
                                    OptionalName = string.Equals(csv.GetField(17), "NULL") ? csv.GetField(22) : csv.GetField(17),
                                    Profile = profile,
                                };

                                //  Debug.WriteLine($"record #:{registers.Count}\t index:{register.Index}\t row: {register.RowNumber}\t repl: {register.ReplacementValue}\t ori: {register.OriginalValue}\t profile: {register.Profile}\t opt: {register.OptionalName}\t loc: {register.Location}");

                                registers.Add(register);

                                // var test = registers.Where(y => y.Location.Contains(Regex.Split(csv.GetField(6), new AnalyzeValues().SetPointPatterns).Last())).ElementAt(0).OriginalValue;
                                Debug.WriteLine($"row: {register.RowNumber} -- reg value: {register.OriginalValue},{csv.GetField(3)} :excel value, ---> profile: {register.Profile}");
                            }
                        }
                    }

                    // store registers
                    MyCommons.Registers = registers;

                    // initialize
                    ObservableCollection<Profile> profiles = new ObservableCollection<Profile>();

                    //// get profile information of 2 and up
                    //// assumption made: original file(s) are/is always Profile 1 
                    //// assign Profile 1
                    //ObservableCollection<Register> profile1Registers = new ObservableCollection<Register>(registers.Where(x => x.Profile == $"{1}"));
                    //Profile profile = new Profile()
                    //{
                    //    Id = 1,
                    //    Name = $"Profile {1}",
                    //    Registers = profile1Registers,
                    //};

                    //profiles.Add(profile);

                    // assign other profile values
                    for (int i = 1; i <= registers.Select(a => a.Profile).Distinct().Count(); i++)
                    {

                        ObservableCollection<Register> profileRegisters = new ObservableCollection<Register>(registers.Where(d => d.Profile == $"{i}"));

                        //// initialize 
                        //ObservableCollection<Register> newRegisters = new ObservableCollection<Register>();

                        //foreach (var item in profileRegisters)
                        //{

                        //    Debug.WriteLine($"loc: {item.Location}, ori: {item.OriginalValue}, rep: {item.ReplacementValue}");

                        //    Register register = new Register
                        //    {
                        //        Index = item.Index,
                        //        RowNumber = item.RowNumber,
                        //        ReplacementValue = item.ReplacementValue,
                        //        OriginalValue = profile1Registers.Where(y => y.Location.Contains(Regex.Split(item.Location, new AnalyzeValues().SetPointPatterns).Last())).ElementAt(0).OriginalValue,
                        //        Location = item.Location,
                        //        MinimumValue = item.MinimumValue,
                        //        MaximumValue = item.MaximumValue,
                        //        Increment = item.Increment,
                        //        OptionalName = item.OptionalName,
                        //        Profile = item.Profile,
                        //    };

                        //    Debug.WriteLine($"loc: {register.Location}, ori: {register.OriginalValue}, rep: {register.ReplacementValue}");

                        //    newRegisters.Add(register);
                        //}

                        Profile profile = new Profile()
                        {
                            Id = i,
                            Name = $"Profile {i}",
                            Registers = profileRegisters,
                        };

                        profiles.Add(profile);
                    }

                    // assign values to the text boxes.
                    MyCommons.MyViewModel.FindWhatTextBoxText = finds.ToString();
                    MyCommons.MyViewModel.ReplaceWithTextBoxText = replaces.ToString();

                    MyCommons.Profiles = profiles;
                }
            }

            // unhook the dialog event
            dlg.FileOk -= CsvDialog_FileOk;
        }

        #endregion
    }
}
