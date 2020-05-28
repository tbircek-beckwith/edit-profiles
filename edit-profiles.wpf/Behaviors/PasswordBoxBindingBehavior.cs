using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace EditProfiles.Behaviors
{
    /// <summary>
    /// Implements Password Box Binding Behavior.
    /// </summary>
    public class PasswordBoxBindingBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// Override OnAttached().
        /// </summary>
        protected override void OnAttached ( )
        {
            AssociatedObject.PasswordChanged += OnPasswordBoxValueChanged;
        }

        /// <summary>
        /// Access to SecureString of the Passwordbox
        /// </summary>
        public SecureString Password
        {
            get { return ( SecureString ) GetValue ( PasswordProperty ); }
            set { SetValue ( PasswordProperty, value ); }
        }

        /// <summary>
        /// Register new Password dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register ( "Password",
                                          typeof ( SecureString ),
                                          typeof ( PasswordBoxBindingBehavior ),
                                          new PropertyMetadata ( null ) );

        private void OnPasswordBoxValueChanged ( object sender, RoutedEventArgs e )
        {
            var binding = BindingOperations.GetBindingExpression ( this, PasswordProperty );

            if ( binding != null )
            {
                PropertyInfo property = binding.DataItem.GetType ( ).GetProperty ( binding.ParentBinding.Path.Path );
                
                if ( property != null )
                {
                    property.SetValue ( binding.DataItem, AssociatedObject.SecurePassword, null );
                }
            }
        }
    }
}
