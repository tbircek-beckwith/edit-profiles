
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using EditProfiles.Data;

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
        public Regulator() { }

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
        /// Scans through <see cref="Regulator"/>
        /// <para>Returns a string contains all available <see cref="Register"/> for each specified <see cref="Profile"/> and <see cref="Regulator"/>.</para>
        /// </summary>
        /// <param name="regulator"></param>
        /// <param name="profile"></param>
        /// <param name="property"></param>
        /// <returns>Returns a string contains all available <see cref="Register"/> for each specified <see cref="Profile"/> and <see cref="Regulator"/>.</returns>
        public string GetValues(int regulator, int profile, Column property)
        {
            StringBuilder values = new StringBuilder();
            ObservableCollection<Regulator> regulators = new ObservableCollection<Regulator>(MyCommons.Regulators) { };

            // filter register per profile #
            IEnumerable<Profile> filter = from x in regulators
                                           .Where(x => x.Id == regulator)
                                           .SelectMany(x => new[] { x.Profiles[0], x.Profiles[profile] })
                                          select x;

            foreach (Profile item in filter)
            {
                foreach (Register register in item.Registers)
                {
                    // gets value of "Column" property out of the collection of properties for current Register.
                    values.Append($"{TypeDescriptor.GetProperties(register)[property.ToString()].GetValue(register)}|");
                }
            }

            // remove last "|" from the string.
            return values.Remove(values.Length - 1, 1).ToString();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Gets a <see cref="Register"/> collection per specified <paramref name="filter"/> and <paramref name="regulatorValue"/>
        /// <para>Returns a collection of <see cref="Register"/></para>
        /// </summary>
        /// <param name="filter">Query to filter <see cref="Register"/>s</param>
        /// <param name="regulatorValue">the current <see cref="Regulator"/> value </param>
        /// <returns>Returns a collection of <see cref="Register"/></returns>
        private ObservableCollection<Register> GetProfile(IEnumerable<Register> filter, int regulatorValue)
        {

            ObservableCollection<Register> profileRegisters = new ObservableCollection<Register>() { };

            // generate temp collection of register per filter
            foreach (Register item in filter)
            {
                // Debug.Write($"profile: {item.Profile} origSetValue: {item.OriginalSettingValue} origTestValue: {item.OriginalTestValue} oldValue: {item.ReplacementValue}");

                // don't modify IsRegulatorCommon == true
                // because these points will not change for either Profile or Regulator.
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
