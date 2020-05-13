
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
        /// The register row number in selected .csv file
        /// remember row is 1 based while collection is 0 based
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// The new modbus register address. 
        /// Column B : Field 1
        /// </summary>
        public string ReplacementValue { get; set; }

        /// <summary>
        /// The old modbus register address
        /// Column D : Field 3
        /// </summary>
        public string OriginalValue { get; set; }

        /// <summary>
        /// This is C code memory location
        /// Column G : Field 6
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The register minimum value
        /// Column I : Field 8
        /// </summary>
        public string MinimumValue { get; set; }

        /// <summary>
        /// The register maximum value
        /// Column J : Field 9
        /// </summary>
        public string MaximumValue { get; set; }

        /// <summary>
        /// The increment step size
        /// Column K : Field 10
        /// </summary>
        public string Increment { get; set; }

        /// <summary>
        /// The register optional name. Could be nothing.
        /// Column R : Field 17 or Column W : 22 (if Column R == "NULL")
        /// </summary>
        public string OptionalName { get; set; }

        /// <summary>
        /// Hold information about <see cref="Register"/> profile
        /// </summary>
        public string Profile { get; set; }

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
