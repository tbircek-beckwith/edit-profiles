using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EditProfiles.Behaviors
{
    /// <summary>
    /// The code copied from http://stackoverflow.com/questions/10097417/how-do-i-create-an-autoscrolling-textbox
    /// Usage in xaml follows: 
    /// src:TextBoxBehavior.ScrollOnTextChanged="True"
    /// </summary>
    public static class TextBoxBehavior
    {
        /// <summary>
        /// Holds Textbox information to capture.
        /// </summary>
        static readonly Dictionary<TextBox, Capture> _associations = new Dictionary<TextBox, Capture> ( );

        /// <summary>
        /// Registers the "ScrollOnTextChanged property.
        /// </summary>
        public static readonly DependencyProperty ScrollOnTextChangedProperty =
            DependencyProperty.RegisterAttached ( "ScrollOnTextChangedProperty",
            typeof ( bool ),
            typeof ( TextBoxBehavior ),
            new UIPropertyMetadata ( false, OnScrollOnTextChanged ) );

        /// <summary>
        /// Returns ScrollOnTextChangedProperty value as boolean.
        /// </summary>
        /// <param name="dependencyObject">Object to value return</param>
        public static bool GetScrollOnTextChanged ( DependencyObject dependencyObject )
        {
            return ( bool ) dependencyObject.GetValue ( ScrollOnTextChangedProperty );
        }

        /// <summary>
        /// Set whether Object scrolls or not.
        /// </summary>
        /// <param name="dependencyObject">Object to investigate.</param>
        /// <param name="value">true = scroll, false = not scroll.</param>
        public static void SetScrollOnTextChanged ( DependencyObject dependencyObject, bool value )
        {
            dependencyObject.SetValue ( ScrollOnTextChangedProperty, value );
        }

        /// <summary>
        /// Attach Load and Unload events to the TextBox.
        /// </summary>
        /// <param name="dependencyObject">Object to work with.</param>
        /// <param name="e">arguments.</param>
        static void OnScrollOnTextChanged ( DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e )
        {
            var textBox = dependencyObject as TextBox;

            if ( textBox == null )
            {
                return;
            }

            bool oldValue = ( bool ) e.OldValue;
            bool newValue = ( bool ) e.NewValue;

            if ( newValue == oldValue )
            {
                return;
            }

            if ( newValue )
            {
                textBox.Loaded += new RoutedEventHandler ( textBox_Loaded );
                textBox.Unloaded += new RoutedEventHandler ( textBox_Unloaded );
            }
            else
            {
                textBox.Loaded -= textBox_Loaded;
                textBox.Unloaded -= textBox_Unloaded;

                if ( _associations.ContainsKey ( textBox ) )
                {
                    _associations[ textBox ].Dispose ( );
                }
            }
        }

        static void textBox_Loaded ( object sender, RoutedEventArgs e )
        {
            var textBox = ( TextBox ) sender;
            textBox.Loaded -= textBox_Loaded;
            _associations[ textBox ] = new Capture ( textBox );
        }

        static void textBox_Unloaded ( object sender, RoutedEventArgs e )
        {
            var textBox = ( TextBox ) sender;
            _associations[ textBox ].Dispose ( );
            textBox.Unloaded -= textBox_Unloaded;
        }
    }

    class Capture : IDisposable
    {
        private TextBox TextBox { get; set; }

        /// <summary>
        /// Capture text changed events.
        /// </summary>
        /// <param name="textBox"></param>
        public Capture ( TextBox textBox )
        {
            this.TextBox = textBox;
            this.TextBox.TextChanged += new TextChangedEventHandler ( TextBox_TextChanged );
        }

        private void TextBox_TextChanged ( object sender, TextChangedEventArgs e )
        {
            this.TextBox.ScrollToEnd ( );
        }

        #region IDisposable Members

        /// <summary>
        /// Remove text changed event.
        /// </summary>
        public void Dispose ( )
        {
            this.TextBox.TextChanged -= TextBox_TextChanged;
        }

        #endregion
    }


}
