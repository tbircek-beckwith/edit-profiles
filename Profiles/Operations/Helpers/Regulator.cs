
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace EditProfiles.Operations
{

    /// <summary>
    /// Holds every regulator's set points
    /// </summary>
    public class Regulator
    {
        #region Public Properties

        /// <summary>
        /// Holds the current regulator number
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the current regulator name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Holds every available <see cref="Profiles"/>
        /// </summary>
        public ObservableCollection<Profile> Profiles { get; set; } = new ObservableCollection<Profile>() { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Regulator()
        {
            // Profiles.Clear();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Generates "3" <see cref="Regulator"/>s with all available <see cref="Profile"/>s and <see cref="Register"/>s information.
        /// </summary>
        /// <param name="registers"> available <see cref="Register"/> information from a .csv file.</param>
        /// <returns>Returns 3 <see cref="Regulator"/>.</returns>
        public ObservableCollection<Regulator> GetRegulators(ObservableCollection<Register> registers)
        {
            ObservableCollection<Regulator> regulators = new ObservableCollection<Regulator>();

            // generate regulators
            for (int i = 1; i <= 3; i++)
            {

                ObservableCollection<Profile> profiles = new ObservableCollection<Profile>();

                //split register into Profiles
                for (int c = 0; c <= registers.Select(a => a.Profile).Distinct().Count() - 1; c++)
                {

                    // filter register per profile #
                    IEnumerable<Register> filter = from x in registers
                                                   where !string.IsNullOrWhiteSpace(x.Location)
                                                   where !string.Equals(x.Location, "&dummy")
                                                   where !string.Equals(x.OriginalSettingValue, "0")
                                                   where x.Profile == $"{c}"
                                                   select x;

                    // retrieve filtered Registers
                    ObservableCollection<Register> FilteredRegisters = new ObservableCollection<Register>(GetProfile(filter, i));

                    // generated a new profile out of filtered Registers
                    profiles.Add(new Profile
                    {
                        Id = c,
                        RegulatorId = i,
                        Name = $"Profile {c}",
                        Registers = FilteredRegisters,
                    });
                }

                // generate a regulator
                regulators.Add(new Regulator
                {
                    Id = i,
                    Name = $"Regulator {i}",
                    Profiles = profiles,
                });

              
            }

            return regulators;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetOriginalSettingValues()
        {
            string values = string.Empty;

            foreach (Profile item in Profiles)
            {
                foreach (Register register in item.Registers)
                {
                    values += register.OriginalSettingValue + "|";
                }
            }


            return values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetOriginalTestValues()
        {
            string values = string.Empty;

            foreach (Profile item in Profiles)
            {
                foreach (Register register in item.Registers)
                {
                    values += register.OriginalTestValue + "|";
                }
            }


            return values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetReplacementValues()
        {
            string values = string.Empty;

            foreach (Profile item in Profiles)
            {
                foreach (Register register in item.Registers)
                {
                    values += register.ReplacementValue + "|";
                }
            }


            return values;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="regulatorValue"></param>
        /// <returns></returns>
        ObservableCollection<Register> GetProfile(IEnumerable<Register> filter, int regulatorValue)
        {

            ObservableCollection<Register> profileRegisters = new ObservableCollection<Register>() { };

            // generate temp collection of register per filter
            foreach (Register item in filter)
            {
                // Debug.Write($"profile: {item.Profile} origSetValue: {item.OriginalSettingValue} origTestValue: {item.OriginalTestValue} oldValue: {item.ReplacementValue}");

                if (!item.IsRegulatorCommon)
                {
                    item.ReplacementValue = $"{Convert.ToInt32(item.ReplacementValue) + ((regulatorValue >> 1) * 10000)}";
                }
                // Debug.WriteLine($" newValue: {item.ReplacementValue}");

                profileRegisters.Add(new Register
                {
                    Index = item.Index,
                    Row = item.Row,
                    ReplacementValue = item.ReplacementValue,
                    OriginalSettingValue = item.OriginalSettingValue,
                    OriginalTestValue = item.OriginalTestValue,
                    Location = item.Location,
                    MinimumValue = item.MinimumValue,
                    MaximumValue = item.MaximumValue,
                    Increment = item.Increment,
                    OptionalName = item.OptionalName,
                    AltName = item.AltName,
                    Profile = item.Profile,
                    IsRegulatorCommon = item.IsRegulatorCommon,
                    DataType = item.DataType,
                    MBFunction = item.MBFunction,
                    Permission = item.Permission,
                    ProtectionLevel = item.ProtectionLevel,
                    RegisterPermission = item.RegisterPermission,
                });
            }

            return profileRegisters;
        }
        #endregion
    }
}
