using System.Text;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Provides interface to Start Processing of Scanning Omicron Test Files.
    /// </summary>
    public partial interface IStartProcessInterface
    {

        /// <summary>
        /// Saves modified Omicron Test File.
        /// </summary>
        /// <param name="oldFileName">Original file name.</param>
        /// <param name="saveAs">If it is true the file "SaveAs", false the file "Save".</param>
        void SaveOmicronFiles ( string oldFileName, bool saveAs );

        /// <summary>
        /// Sets specified protection level and saves modified Omicron Test File.
        /// </summary>
        /// <param name="oldFileName">Original file name.</param>
        /// <param name="saveAs">If it is true the file "SaveAs", false the file "Save".</param>
        /// <param name="protectionLevel">provide a protection level to protect the file.</param>
        void SaveOmicronFiles ( string oldFileName, bool saveAs, short protectionLevel );

        /// <summary>
        /// Generates a new file name based on the original file name.
        /// </summary>
        /// <param name="fileNameWithPath">A string that contains original file's fullname.</param>
        /// <returns>Returns a full path of the new file including path.</returns>
        string GenerateNewFileName ( string fileNameWithPath );

        /// <summary>
        /// Finds and replaces the Execute Test Module Parameters.
        /// </summary>
        /// <param name="findParameters">The parameters currently in the Exceute Module.</param>
        /// <returns>Returns a modified string.</returns>
        StringBuilder FindAndReplaceParameters ( string findParameters );
    }
}
