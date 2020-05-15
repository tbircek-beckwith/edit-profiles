
namespace EditProfiles.Operations
{
    /// <summary>
    /// Holds a row information from the .csv file.
    /// </summary>
    public class Register
    {
        #region Public Properties

        /// <summary>
        /// The register index number in selected .csv file
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The register row number in selected .csv file.
        /// <para>RowNumber starts at 1.</para>
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// The new modbus register address. 
        /// <para>Column B : Field 1</para>
        /// <para>"T:N"</para>
        /// </summary>
        public string ReplacementValue { get; set; }

        /// <summary>
        /// The old modbus register address
        /// <para>Assumption made: original file(s) are/is always Profile 1
        /// so always change "Find" value to Profile 1 register number</para>
        /// <para>Column D : Field 3</para>
        /// </summary>
        public string OriginalValue { get; set; }
        
        /// <summary>
        /// This is register's read/write permissions 
        /// <para>Column G : Field 5</para>
        /// </summary>
        public string RegisterPermissions { get; set; }

        /// <summary>
        /// This is C code variable name
        /// <para>Column G : Field 6</para>
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// This is C data type of the variable
        /// <para>Column H : Field 7</para>
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// The register minimum value
        /// <para>Column I : Field 8</para>
        /// </summary>
        public string MinimumValue { get; set; }

        /// <summary>
        /// The register maximum value
        /// <para>Column K : Field 9</para>
        /// </summary>
        public string MaximumValue { get; set; }

        /// <summary>
        /// The increment step size
        /// <para>Column L : Field 10</para>
        /// </summary>
        public string Increment { get; set; }

        /// <summary>
        /// The increment step size
        /// <para>Column O : Field 14</para>
        /// </summary>
        public string MBFunction { get; set; }

        /// <summary>
        /// The increment step size
        /// <para>Column P : Field 15</para>
        /// </summary>
        public string ProtectionLevel { get; set; }

        /// <summary>
        /// The increment step size
        /// <para>Column Q : Field 16</para>
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// The register optional name. Could be nothing.
        /// <para>Column S : Field 18 or Column W : 23 (if Column S == "NULL")</para>
        /// </summary>
        public string OptionalName { get; set; }

        /// <summary>
        /// Hold information about <see cref="Register"/> profile
        /// <para>all <see cref="Profile"/> must modify <see cref="OriginalValue"/> to match Profile 1.
        /// Except Profile 0 and 1</para>
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// Hold information whether this <see cref="Register"/> is common to all regulators. 
        /// <para><see cref="Register"/> value >= 40000</para>
        /// </summary>
        public bool IsRegulatorCommon { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Register()
        {

        }

        #endregion
    }
}
