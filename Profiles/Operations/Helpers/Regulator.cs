
using System.Collections.ObjectModel;

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
        public ObservableCollection<Profile> Profiles { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Regulator()
        {

        }

        #endregion
    }
}
