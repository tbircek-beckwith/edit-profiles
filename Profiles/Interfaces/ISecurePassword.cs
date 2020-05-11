using System.Security;

namespace EditProfiles.Behaviors
{
    /// <summary>
    /// Handles conversion of the SecureString to string.
    /// For more info: http://briannoyesblog.azurewebsites.net/2015/03/04/wpf-using-passwordbox-in-mvvm/
    /// </summary>
    public interface ISecurePasswordToString
    {
        /// <summary>
        /// Coverts SecureString to an unsecure string.
        /// </summary>
        /// <param name="secureString">the content of PasswordBox</param>
        /// <returns></returns>
        string ConvertToInsecureString ( SecureString secureString );
    }
}
