using System.Collections.Generic;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Interface between MainForm and the Other Classes.
    /// </summary>
    public interface IRestrictedMainFormInterface
    {
        /// <summary>
        /// Passes information to store and show.
        /// </summary>
        string LogUpdater { get; set; }

        /// <summary>
        /// Provide interface to the user selected FileNames.
        /// </summary>
        IList<string> FileNames { get; }

        /// <summary>
        /// Provide interface to the user specified Password.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Provides thread safe interface to "Find what" items as a list.
        /// </summary>
        IList<string> ItemsToFind { get; }

        /// <summary>
        /// Provides thread safe interface to the user specified "Replace" items as a list.
        /// </summary>
        IList<string> ItemsToReplace { get; }

        /// <summary>
        /// Provides thread safe interface to the user specified "Add" items
        /// which must be marked by a "+" sign
        /// </summary>
        IList<string> ItemsToAdd { get; }

        /// <summary>
        /// Provides thread safe interface to the user specified "Delete" items
        /// which must be marked by a "-" sign
        /// </summary>
        IList<string> ItemsToDelete { get; }
        
#if DEBUG
        /// <summary>
        /// Provides thread safe interface to "Find what" items as a string.
        /// </summary>
        string ItemsToFindString { get; }

        /// <summary>
        /// Provides thread safe interface to the user specified "Replace" items as a string.
        /// </summary>
        string ItemsToReplaceString { get; }
#endif
       
    }
}
