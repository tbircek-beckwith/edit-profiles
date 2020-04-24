using System;
using System.Runtime.InteropServices;
using System.Security;
using EditProfiles.Behaviors;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Handles conversion of the SecureString to string.
    /// For more info: http://briannoyesblog.azurewebsites.net/2015/03/04/wpf-using-passwordbox-in-mvvm/
    /// </summary>
    internal class SecurePasswordToString :ISecurePasswordToString
    {
        #region ISecurePasswordToString Members

        /// <summary>
        /// Coverts SecureString to an insecure string.
        /// </summary>
        /// <param name="secureString">the content of PasswordBox</param>
        /// <returns>Returns a plain string of the secureString.
        /// The plain string MUST be DESTROYED after the usage.
        /// Otherwise, it can be seen in the memory scans.</returns>
        public string ConvertToInsecureString ( SecureString secureString )
        {
            IntPtr passwordBSTR = default;
            string insecurePassword = string.Empty;

            try
            {
                if ( secureString != null )
                {
                    passwordBSTR = Marshal.SecureStringToBSTR ( secureString );
                    insecurePassword = Marshal.PtrToStringBSTR ( passwordBSTR );
                }
                else
                {
                    insecurePassword = string.Empty;
                }
                
            }
            catch ( ArgumentNullException ae )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ae );
                insecurePassword = string.Empty;
            }
            catch ( NotSupportedException  nse )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( nse );
                insecurePassword = string.Empty;
            }
            catch ( OutOfMemoryException ome )
            {
                // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                ErrorHandler.Log ( ome );
                insecurePassword = string.Empty;
            }

            return insecurePassword;
        }

        #endregion
    }
}
