
using System.Collections.ObjectModel;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Holds a group of the register that belongs to same profile
    /// </summary>
    public class Profile
    {
        #region Public Properties

        /// <summary>
        /// The profile number
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of <see cref="Registers"/>
        /// </summary>
        public ObservableCollection<Register> Registers { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Profile()
        {

        }

        #endregion
    }
}
